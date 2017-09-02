
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Ads;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Tags;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ActivityType = Sparkle.Services.Networks.Models.Profile.ActivityType;
    using EntityWithTagRepositoryType = Sparkle.Services.Networks.Tags.EntityWithTag.EntityWithTagRepositoryType;
    using SqlEntityWithTag = Sparkle.Services.Networks.Tags.EntityWithTag.SqlEntityWithTag;
    using UserModel = Sparkle.Services.Networks.Models.UserModel;
    using KnownHints = Sparkle.Services.Networks.Models.KnownHints;

    public class AdsService: ServiceBase, IAdsService
    {
        private static NetworkAccessLevel[] moderateLevels = new NetworkAccessLevel[] { NetworkAccessLevel.ContentManager, NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff, };
        private Dictionary<int, AdCategoryModel> categories;

        [DebuggerStepThrough]
        internal AdsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public IDictionary<int, AdCategoryModel> Categories
        {
            get
            {
                if (this.categories == null)
                {
                    this.categories = this.Repo.AdsCategories.GetAll(this.Services.NetworkId)
                        .ToDictionary(x => x.Id, x => new AdCategoryModel(x));
                }

                return this.categories;
            }
        }

        [Obsolete]
        public int Publish(User me, Ad item)
        {
            item = this.Repo.Ads.Insert(item);
            var id = item.Id;

            this.Services.Wall.Publish(me, null, TimelineItemType.Ad, id.ToString(), TimelineType.Ad, id, scope: TimelineItemScope.Network);

            return id;
        }

        [Obsolete]
        public void Delete(Ad item)
        {
            this.Repo.Ads.Delete(item);
        }

        [Obsolete]
        public Ad Update(Ad item)
        {
            return this.Repo.Ads.Update(item);
        }

        [Obsolete]
        public IList<Ad> SelectAll()
        {
            return this.Repo.Ads.Select()
                .Where(a => a.Owner.NetworkId == this.Services.NetworkId)
                .OrderByDescending(o => o.Date)
                .ToList();
        }

        [Obsolete]
        public Ad SelectById(int adId)
        {
            return this.Repo.Ads.Select()
                .Where(a => a.Owner.NetworkId == this.Services.NetworkId)
                .ById(adId)
                .OrderByDescending(o => o.Date)
                .FirstOrDefault();
        }

        public int Count()
        {
            return this.Repo.Ads.Select()
                .Where(a => a.Owner.NetworkId == this.Services.NetworkId)
                .Count();
        }

        [Obsolete]
        public IDictionary<int, Ad> GetByIdFromAnyNetwork(IList<int> adIds, AdOptions options)
        {
            return this.Repo.Ads.GetById(adIds, options);
        }

        [Obsolete]
        public IDictionary<int, Ad> GetByIdInNetwork(int[] adIds, AdOptions adOptions)
        {
            return this.Repo.Ads.GetById(adIds, this.Services.NetworkId, adOptions);
        }

        public string GetProfileUrl(Ad ad, UriKind uriKind)
        {
            var path = "Ads/Ad/" + ad.Id;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public EditAdRequest GetEditRequest(int? id, EditAdRequest request)
        {
            AdModel item = null;
            if (id != null)
            {
                item = this.GetById(id.Value, AdOptions.None);
            }

            if (request == null)
            {
                request = new EditAdRequest();
                if (item != null)
                {
                    request.Id = item.Id;
                    request.Title = item.Title;
                    request.Content = item.Content;
                    request.CategoryId = item.CategoryId;

                    var adTypeCat = this.Repo.TagCategories.GetByAlias(this.Services.NetworkId, "AdType");
                    var setTags = this.Repo.AdTags.GetByRelationId(item.Id);
                    var setTypes = setTags.Where(x => x.TagDefinition.CategoryId == adTypeCat.Id).ToArray();
                    request.TypeId = setTypes.Length > 0 ? setTypes[0].TagId : 0;
                }
            }

            if (request.AvailableCategories == null)
            {
                request.AvailableCategories = this.Repo.AdsCategories.GetAll(this.Services.NetworkId)
                    .Select(x => new AdCategoryModel(x))
                    .ToList();
            }

            if (request.AvailableTypes == null)
            {
                request.AvailableTypes = this.GetTypes();
            }

            return request;
        }

        public EditAdResult Edit(EditAdRequest request)
        {
            const string path = "AdsService.Edit";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditAdResult(request);

            if (!request.IsValid)
            {
                result.Errors.Add(EditAdError.Invalid, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                Ad item = null;
                bool isEdit = false;
                if (request.Id > 0)
                {
                    isEdit = true;
                    item = tran.Repositories.Ads.GetById(request.Id);
                    if (item == null)
                    {
                        result.Errors.Add(EditAdError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                        return this.LogResult(result, path);
                    }

                    if (item.CloseDateUtc != null)
                    {
                        result.Errors.Add(EditAdError.CannotEditWhenClosed, NetworksEnumMessages.ResourceManager);
                    }
                }

                var user = tran.Repositories.People.GetActiveById(request.ActingUserId, Data.Options.PersonOptions.Company);
                UserModel userModel = null;
                bool isUserAdmin = false, isUserOwner = false;
                if (user != null)
                {
                    userModel = new UserModel(user);
                    if (userModel.IsActive)
                    {
                        if (userModel.NetworkAccessLevel.Value.HasAnyFlag(moderateLevels))
                        {
                            // the admins can edit
                            isUserAdmin = true;
                        }
                        
                        if (item == null)
                        {
                            // anyone can create
                            isUserOwner = true;
                        }
                        
                        if (item != null && item.UserId == user.Id)
                        {
                            // the owner can edit
                            isUserOwner = true;
                        }
                        
                        if (!isUserAdmin && !isUserOwner)
                        {
                            result.Errors.Add(EditAdError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                        }
                    }
                    else
                    {
                        result.Errors.Add(EditAdError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(EditAdError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                }

                var category = tran.Repositories.AdsCategories.GetByIdFromCommonNetwork(request.CategoryId.Value, this.Services.NetworkId);
                ////var categoryTagCategory = tran.Repositories.TagCategories.GetByAlias(this.Services.NetworkId, "AdCategory");
                if (category == null)
                {
                    result.Errors.Add(EditAdError.NoSuchCategory, NetworksEnumMessages.ResourceManager);
                }

                var adTypeCat = tran.Repositories.TagCategories.GetByAlias(this.Services.NetworkId, "AdType");
                var adType = tran.Repositories.TagDefinitions.GetById(request.TypeId.Value);
                if (adType == null || adType.CategoryId != adTypeCat.Id)
                {
                    result.Errors.Add(EditAdError.NoSuchType, NetworksEnumMessages.ResourceManager);
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, path);
                }

                var requiresValidationConfig = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
                var requiresValidation = requiresValidationConfig && !isUserAdmin;
                var title = request.Title.Trim();
                var now = DateTime.UtcNow;
                if (item == null)
                {
                    item = new Ad();
                    item.Date = now;
                    item.Alias = this.MakeAlias(tran.Repositories, title);
                    item.IsValidated = isUserAdmin ? true : default(bool?);
                    item.IsOpen = true;
                    item.NetworkId = this.Services.NetworkId;
                    item.UserId = userModel.Id;
                    item.UpdateDateUtc = now;
                    item.Title = title;
                    item.Message = request.Content;
                    tran.Repositories.Ads.Attach(item);
                }

                item.CategoryId = category.Id;

                if (requiresValidation)
                {
                    if (isEdit)
                    {
                        item.PendingEditDate = now;
                        item.PendingEditMessage = request.Content;
                        item.PendingEditTitle = title;
                        result.IsPendingEdit = true;
                    }
                }
                else
                {
                    item.UpdateDateUtc = now;
                    item.Title = title;
                    item.Message = request.Content;
                }

                tran.ExecuteChanges();

                var setTags = tran.Repositories.AdTags.GetByRelationId(item.Id);

                // update add type
                {
                    var setTypes = setTags.Where(x => x.TagDefinition.CategoryId == adTypeCat.Id).ToArray();
                    bool found = false;
                    if (setTypes.Length > 1)
                    {
                        foreach (var setType in setTypes)
                        {
                            if (setType.TagId == adType.Id)
                            {
                                found = true;
                            }
                            else
                            {
                                tran.Repositories.AdTags.Delete(setType);
                            }
                        }
                    }
                    else if (setTypes.Length == 1)
                    {
                        if (setTypes[0].TagId != adType.Id)
                        {
                            setTypes[0].TagId = adType.Id;
                        }
                    }
                    
                    if (setTypes.Length == 0 || setTypes.Length > 1 && !found)
                    {
                        var tag = new AdTag();
                        tag.CreatedByUserId = user.Id;
                        tag.DateCreatedUtc = now;
                        tag.RelationId = item.Id;
                        tag.TagId = adType.Id;
                        tran.Repositories.AdTags.Attach(tag);
                    }
                }

                // update add category
                {/*
                    var setCats = setTags.Where(x => x.TagDefinition.CategoryId == categoryTagCategory.Id).ToArray();
                    bool create = true;
                    if (setCats.Length > 1)
                    {
                        foreach (var setType in setCats)
                        {
                            if (setType.TagId == category.Id)
                            {
                                create = false;
                            }
                            else
                            {
                                tran.Repositories.AdTags.Delete(setType);
                            }
                        }
                    }

                    if (create)
                    {
                        var tag = new AdTag();
                        tag.CreatedByUserId = user.Id;
                        tag.DateCreatedUtc = now;
                        tag.RelationId = item.Id;
                        tag.TagId = category.Id;
                        tran.Repositories.AdTags.Attach(tag);
                    }
                */}

                tran.ExecuteChanges();

                tran.CompleteTransaction();

                if (requiresValidation)
                {
                    ////var model = this.Services.Email.GetAdminWorkModel(adsValidation: true);
                    ////this.Services.Email.SendAdminWorkEmail(model, true, NetworkAccessLevel.ModerateNetwork | NetworkAccessLevel.ContentManager);
                }

                result.IsPendingValidation = !isEdit && requiresValidation;
                result.Succeed = true;
                result.Item = new AdModel(item, category);
                return this.LogResult(result, path, "Created " + result.Item + ".");
            }
        }

        public AdModel GetById(int id, AdOptions options)
        {
            var item = this.Repo.Ads.GetById(id, this.Services.NetworkId, options);
            if (item != null)
                return new AdModel(item, this.Categories[item.CategoryId]);
            return null;
        }

        public IDictionary<int, AdModel> GetById(int[] ids, AdOptions options)
        {
            var items = this.Repo.Ads.GetById(ids, this.Services.NetworkId, options);
            var models = new Dictionary<int, AdModel>(ids.Length);
            foreach (var id in ids)
            {
                if (items.ContainsKey(id))
                    models.Add(id, new AdModel(items[id], this.Categories[items[id].CategoryId]));
            }

            return models;
        }

        public AdModel GetByAlias(string alias, AdOptions options)
        {
            var item = this.Repo.Ads.GetByAlias(alias, this.Services.NetworkId, options);
            if (item != null)
                return new AdModel(item, this.Categories[item.CategoryId]);
            return null;
        }

        public IList<AdModel> GetList(Ad.Columns sort, bool desc, bool openOnly, int offset, int pageSize, AdOptions options)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            var items = this.Repo.Ads.GetList(sort, desc, this.Services.NetworkId, openOnly, isValidationRequired, offset, pageSize, options, null);
            return items.Select(x => new AdModel(x, this.Categories[x.CategoryId])).ToList();
        }

        public IList<AdModel> GetUserList(Ad.Columns sort, bool desc, bool openOnly, int offset, int pageSize, AdOptions options, int userId)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            var items = this.Repo.Ads.GetList(sort, desc, this.Services.NetworkId, openOnly, isValidationRequired, offset, pageSize, options, userId);
            return items.Select(x => new AdModel(x, this.Categories[x.CategoryId])).ToList();
        }

        public IList<AdModel> GetByDateRange(Ad.Columns sort, bool desc, bool openOnly, int offset, int pageSize, AdOptions options, DateTime from, DateTime to)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            var items = this.Repo.Ads.GetListByDateRange(from, to, sort, desc, this.Services.NetworkId, openOnly, isValidationRequired, offset, pageSize, options);
            return items.Select(x => new AdModel(x, this.Categories[x.CategoryId])).ToList();
        }

        public int CountByDateRange(bool openOnly, DateTime from, DateTime to)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            var items = this.Repo.Ads.CountByDateRange(from, to, this.Services.NetworkId, openOnly, isValidationRequired);
            return items;
        }

        public IList<AdModel> GetPendingList(AdOptions options)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;

            if (isValidationRequired)
            {
                var items = this.Repo.Ads.GetPendingList(this.Services.NetworkId, options);
                return items.Select(x => new AdModel(x, this.Categories[x.CategoryId])).ToList();
            }
            else
            {
                return new List<AdModel>(0);
            }
        }

        public int Count(bool openOnly)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            return this.Repo.Ads.Count(this.Services.NetworkId, openOnly, isValidationRequired, null);
        }

        public int GetPendingCount()
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;

            if (isValidationRequired)
            {
                var items = this.Repo.Ads.GetPendingCount(this.Services.NetworkId);
                return items;
            }
            else
            {
                return 0;
            }
        }

        public bool IsUserAuthorized(int adId, int actingUserId, AdAction action)
        {
            throw new NotImplementedException();
        }

        public IList<Tag2Model> GetTypes()
        {
            var category = this.Services.Tags.GetCategoryByAlias("AdType");
            if (category == null)
                throw new InvalidOperationException("Tag type AdType is not declared.");

            var models = this.Services.Tags.GetTagsByCategoryId(category.Id);
            return models;
        }

        public ValidateAdResult Validate(ValidateAdRequest request)
        {
            const string path = "AdsService.Validate";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ValidateAdResult(request);
            if (!request.IsValid)
            {
                result.Errors.Add(ValidateAdError.Invalid, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, path);
            }

            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                Ad item = null;
                item = tran.Repositories.Ads.GetById(request.Id);
                if (item == null)
                {
                    result.Errors.Add(ValidateAdError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, path);
                }

                if (item.CloseDateUtc != null)
                {
                    result.Errors.Add(ValidateAdError.CannotValidateWhenClosed, NetworksEnumMessages.ResourceManager);
                }

                if (item.IsValidated == null || item.IsValidated == true && !request.Accept || item.PendingEditDate != null)
                {
                }
                else
                {
                    result.Errors.Add(ValidateAdError.AlreadyValidated, NetworksEnumMessages.ResourceManager);
                }

                var owner = tran.Repositories.People.GetActiveById(item.UserId, Data.Options.PersonOptions.Company);
                var actingUser = tran.Repositories.People.GetActiveById(request.ActingUserId, Data.Options.PersonOptions.Company);
                UserModel userModel = null, ownerModel = null;
                if (actingUser != null)
                {
                    userModel = new UserModel(actingUser);
                    if (userModel.IsActive)
                    {
                        if (!userModel.NetworkAccessLevel.Value.HasAnyFlag(moderateLevels))
                        {
                            result.Errors.Add(ValidateAdError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                        }
                    }
                    else
                    {
                        result.Errors.Add(ValidateAdError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(ValidateAdError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                }

                ownerModel = new UserModel(owner);
                if (!ownerModel.IsActive)
                {
                    if (request.Accept)
                    {
                        result.Errors.Add(ValidateAdError.CannotAcceptWhenUserNotActive, NetworksEnumMessages.ResourceManager);
                    }
                }

                if (item.PendingEditDate != null && request.PendingEditDate != item.PendingEditDate.Value)
                {
                    result.Errors.Add(ValidateAdError.ConcurrencyOnPendingEditDate, NetworksEnumMessages.ResourceManager);
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, path);
                }

                var requiresValidationConfig = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
                var now = DateTime.UtcNow;
                Action<IServiceFactory> postCommit = null;

                if (request.Accept)
                {
                    if (item.PendingEditDate != null)
                    {
                        item.Title = item.PendingEditTitle ?? item.Title;
                        item.Message = item.PendingEditMessage ?? item.Message;
                        item.UpdateDateUtc = item.PendingEditDate ?? now;
                        postCommit = s => s.Activities.Create(owner.Id, ActivityType.AdEditValidated, adId: item.Id);
                    }
                    else
                    {
                        item.IsValidated = request.Accept;
                        item.ValidationUserId = actingUser.Id;
                        item.ValidationDateUtc = now;
                        postCommit = s => s.Activities.Create(owner.Id, ActivityType.AdCreateValidated, adId: item.Id);
                    }
                }
                else
                {
                    if (item.PendingEditDate != null)
                    {
                        postCommit = s => s.Activities.Create(owner.Id, ActivityType.AdEditRefused, adId: item.Id);
                    }
                    else
                    {
                        item.IsValidated = request.Accept;
                        item.ValidationUserId = actingUser.Id;
                        item.ValidationDateUtc = now;
                        postCommit = s => s.Activities.Create(owner.Id, ActivityType.AdCreateRefused, adId: item.Id);
                    }
                }

                item.PendingEditDate = null;
                item.PendingEditMessage = null;
                item.PendingEditTitle = null;

                tran.CompleteTransaction();

                if (postCommit != null)
                {
                    postCommit(this.Services);
                }

                result.Succeed = true;
                result.Item = new AdModel(item, this.Categories[item.CategoryId]);
                return this.LogResult(result, path, "User " + actingUser.Id + " " + (request.Accept ? "Accepted" : "Refused") + " " + result.Item);
            }
        }

        public int CountNewAdsForUser(int userId)
        {
            var isValidationRequired = this.Services.AppConfiguration.Tree.Features.Ads.RequireValidation;
            var maxSeenDateHint = this.Services.Hints.GetUserRelation(KnownHints.MaxAdSeenDateKey, userId);
            var maxSeenDate = maxSeenDateHint != null ? maxSeenDateHint.DateDismissedUtc : default(DateTime?);
            int count;
            if (maxSeenDate != null)
            {
                count = this.Repo.Ads.CountActiveOpenAfter(this.Services.NetworkId, maxSeenDate.Value, isValidationRequired);
            }
            else
            {
                count = this.Repo.Ads.CountActiveOpen(this.Services.NetworkId, isValidationRequired);
            }
            
            return count;
        }

        internal static void RegisterTags(TagsService tags)
        {
            tags.RegisterEntityValidator("Ad", AdsService.ValidateEntity);
            tags.RegisterTagValidator("Ad", AdsService.ValidateTag);
            tags.RegisterTagRepository("Ad", EntityWithTagRepositoryType.Sql, "AdTags", "RelationId");
        }

        private static bool ValidateEntity(IServiceFactory services, string entityIdentifier, AddOrRemoveTagResult result, out int entityId)
        {
            entityId = 0;
            var ad = services.Ads.GetByAlias(entityIdentifier, AdOptions.None);
            if (ad == null)
            {
                if (int.TryParse(entityIdentifier, out entityId))
                {
                    ad = services.Ads.GetById(entityId, AdOptions.None);
                }

                if (ad == null)
                {
                    result.Errors.Add(AddOrRemoveTagError.NoSuchAd, NetworksEnumMessages.ResourceManager);
                    return false;
                }
            }

            entityId = ad.Id;
            return true;
        }

        private static bool ValidateTag(IServiceFactory services, string entityIdentifier, string tagId, int? actingUserId, TagCategoryModel tagCategory, AddOrRemoveTagResult result)
        {
            if (!actingUserId.HasValue)
                throw new ArgumentNullException("The value cannot be empty.", "actingUserId");
            if (tagCategory == null || tagCategory.RulesModel == null || !tagCategory.RulesModel.Rules.ContainsKey(RuleType.Group))
                throw new ArgumentNullException("The value cannot be empty.", "tagCategory");

            // Check company exists
            int adId;
            if (!AdsService.ValidateEntity(services, entityIdentifier, result, out adId))
            {
                // when ValidateEntity fails, an error is added to the result
            }

            // Check user rights
            if (actingUserId == null)
            {
                result.Errors.Add(AddOrRemoveTagError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return false;
            }

            var authorized = services.Ads.IsUserAuthorized(adId, actingUserId.Value, AdAction.ChangeTags);
            if (!authorized)
            {
                result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Check TagCategory company rules
            var max = tagCategory.RulesModel.Rules[RuleType.Ad].Maximum;
            if (result.Request.AddTag && services.Repositories.AdTags.CountByRelationAndCategory(adId, tagCategory.Id, false) >= max)
            {
                result.Errors.Add(AddOrRemoveTagError.MaxNumberOfTagForCategory, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Build EntityWithTag into result
            result.Entity = new SqlEntityWithTag
            {
                EntityId = adId,
            };

            return true;
        }

        private string MakeAlias(IRepositoryFactory repositoryFactory, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (repositoryFactory.Ads.GetByAlias(alias, this.Services.NetworkId, AdOptions.None) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.Ads.GetByAlias(a, AdOptions.None) == null);
            }

            return alias;
        }
    }
}
