
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Tags.EntityWithTag;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using UserModel = Sparkle.Services.Networks.Models.UserModel;

    public class TagsService : ServiceBase, ITagsService
    {
        private static readonly NetworkAccessLevel[] adminRoles = new NetworkAccessLevel[] { NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.ContentManager, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff, };

        private readonly IDictionary<string, ValidateEntity> entityValidators = new Dictionary<string, ValidateEntity>();
        private readonly IDictionary<string, ValidateEntityWithTag> entityWithTagValidators = new Dictionary<string, ValidateEntityWithTag>();
        private readonly IDictionary<string, Tuple<EntityWithTagRepositoryType, string, string>> entityWithTagRepositories = new Dictionary<string, Tuple<EntityWithTagRepositoryType, string, string>>();
        private bool disableV1;

        [DebuggerStepThrough]
        internal TagsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
            PeopleService.RegisterTags(this);
            CompanyService.RegisterTags(this);
            GroupsService.RegisterTags(this);
            AdsService.RegisterTags(this);
            this.disableV1 = serviceFactory.AppConfiguration.Tree.Features.Tags.DisableV1;
        }

        public void Initialize()
        {
            this.InitializeTagCategories();
        }

        internal void RegisterEntityValidator(string key, ValidateEntity value)
        {
            this.entityValidators.Add(key, value);
        }

        internal void RegisterTagValidator(string key, ValidateEntityWithTag value)
        {
            this.entityWithTagValidators.Add(key, value);
        }

        internal void RegisterTagRepository(string key, EntityWithTagRepositoryType type, string entityTagTableName, string entityForeignKeyToTable)
        {
            this.entityWithTagRepositories.Add(key, new Tuple<EntityWithTagRepositoryType, string, string>(type, entityTagTableName, entityForeignKeyToTable));
        }

        public bool DisableV1
        {
            get { return this.disableV1; }
        }

        public IList<TagCategoryModel> GetCategories()
        {
            return this.Repo.TagCategories.GetAll(this.Services.NetworkId)
                .Select(c => new TagCategoryModel(c))
                .ToList();
        }

        public IList<Tag2Model> GetAll()
        {
            var newItems = this.Repo.TagDefinitions.GetAll(this.Services.NetworkId)
                .Select(c => new Tag2Model(c))
                .ToList();

            if (!disableV1)
            {
                var oldItems1 = this.Repo.Skills.GetAll()
                    .Select(c => new Tag2Model(c.Value))
                    .ToList();
                var oldItems2 = this.Repo.Interests.GetAll()
                    .Select(c => new Tag2Model(c.Value))
                    .ToList();
                var oldItems3 = this.Repo.Recreations.GetAll()
                    .Select(c => new Tag2Model(c.Value))
                    .ToList();
                newItems.AddRange(oldItems1);
                newItems.AddRange(oldItems2);
                newItems.AddRange(oldItems3);
            }

            return newItems;
        }

        public IList<Tag2Model> GetTagsByType(TagType type)
        {
            return this.Repo
                .TagDefinitions
                .GetByType(this.Services.NetworkId, type)
                .Select(o => new Tag2Model(o))
                .ToList();
        }

        public IList<CityTagModel> GetCityTags()
        {
            return this.Repo
                .TagDefinitions
                .GetByType(this.Services.NetworkId, TagType.City)
                .Select(o => new CityTagModel(o))
                .ToList();
        }

        public IList<Tag2Model> SearchForCities(string search)
        {
            return this.Repo
                .TagDefinitions
                .SearchFor(this.Services.NetworkId, search, TagType.City)
                .Select(o => new Tag2Model(o))
                .ToList();
        }

        public IList<Tag2Model> SearchFor(string search, params TagType[] type)
        {
            return this.Repo
                .TagDefinitions
                .SearchFor(this.Services.NetworkId, search, type)
                .Select(o => new Tag2Model(o))
                .ToList();
        }

        public IList<Tag2Model> GetTagsFromPartnerResourceId(int partnerId)
        {
            var partnerResourceTagIds = this.Repo
                .PartnerResourceTags
                .GetByPartnerResourceId(partnerId)
                .Select(o => o.TagId)
                .ToArray();

            return this.Repo
                .TagDefinitions
                .GetByIds(this.Services.NetworkId, partnerResourceTagIds)
                .Select(o => new Tag2Model(o))
                .ToList();
        }

        ////public TagPictureModel GetTagPicture(string alias)
        ////{
        ////    if (alias == null)
        ////        throw new ArgumentNullException("alias");

        ////    var basePath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
        ////    var tagsPath = Path.Combine(basePath, "Networks", this.Services.Network.Name, "Tags", alias);

        ////    var model = new TagPictureModel();


        ////    return null;
        ////}

        public string GetSOTagFromTags(IList<Tag2Model> list)
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                if (sb.Length > 0)
                    sb.Append(";");
                sb.Append(item.Id.ToString());
                sb.Append('_');
                sb.Append(item.Name);
            }

            return sb.ToString();
        }

        public IList<Tag2Model> GetTagsFromSOTagString(string sotag)
        {
            var split = sotag.Split(new char[] { ';', }, StringSplitOptions.RemoveEmptyEntries);
            var ids = new List<int>();
            var names = new List<string>();

            foreach (var item in split)
            {
                if (item.Contains('_'))
                {
                    var splits = item.Split(new char[] { '_', }, 2, StringSplitOptions.None);
                    int id;
                    if (int.TryParse(splits[0], out id))
                    {
                        ids.Add(id);
                    }
                    else if (!string.IsNullOrEmpty(splits[1]))
                    {
                        names.Add(splits[1]);
                    }
                }
            }

            var models = new List<Tag2Model>(ids.Count + names.Count);
            models.AddRange(this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, ids.ToArray()).Select(x => new Tag2Model(x)));
            models.AddRange(this.Repo.TagDefinitions.GetByAlias(this.Services.NetworkId, names.ToArray()).Select(x => new Tag2Model(x)));

            return models;
        }

        public Tag2Model GetByAlias(string alias)
        {
            var item = this.Repo
                .TagDefinitions
                .GetByAlias(this.Services.NetworkId, alias);

            return item != null ? new Tag2Model(item) : null;
        }

        public TagCategoryModel GetCategoryByAlias(string alias)
        {
            var item = this.Repo.TagCategories.GetByAlias(this.Services.NetworkId, alias);
            if (item == null)
                return null;

            return new TagCategoryModel(item);
        }

        public IList<Tag2Model> GetTagsByCategoryId(int categoryId)
        {
            return this.Repo.TagDefinitions.GetByCategoryId(this.Services.NetworkId, categoryId).Select(o => new Tag2Model(o)).ToList();
        }

        public IList<Tag2Model> GetTagsByCompanyIdAndCategoryId(int companyId, int categoryId)
        {
            var companyTags = this.Repo.CompanyTags.GetByCompanyId(companyId).Where(o => !o.DateDeletedUtc.HasValue).ToList();
            var tags = this.Repo.TagDefinitions.GetByIdsAndCategoryId(this.Services.NetworkId, companyTags.Select(o => o.TagId).ToArray(), categoryId);

            return tags.Select(o => new Tag2Model(o)).ToList();
        }

        public IList<TagCategoryModel> GetCategoriesApplyingToCompanies()
        {
            return this.Repo.TagCategories
                .GetByNonNullRules(this.Services.NetworkId)
                .Select(o => new TagCategoryModel(o))
                .Where(o => o.ApplyToCompanies)
                .ToList();
        }

        public IList<TagCategoryModel> GetCategoriesApplyingToUsers()
        {
            return this.Repo.TagCategories
                .GetByNonNullRules(this.Services.NetworkId)
                .Select(o => new TagCategoryModel(o))
                .Where(o => o.ApplyToUsers)
                .ToList();
        }

        public IList<TagCategoryModel> GetCategoriesApplyingToGroups()
        {
            return this.Repo.TagCategories
                .GetByNonNullRules(this.Services.NetworkId)
                .Select(o => new TagCategoryModel(o))
                .Where(o => o.ApplyToGroups)
                .ToList();
        }

        public IDictionary<TagCategoryModel, IList<Tag2Model>> GetTagsByCompanyIdAndCategories(int companyId, IList<TagCategoryModel> categories)
        {
            var companyTags = this.Repo.CompanyTags.GetByCompanyId(companyId).Where(o => !o.DateDeletedUtc.HasValue).ToList();
            var tagsIds = companyTags.Select(o => o.TagId).ToArray();
            var tags = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, tagsIds);

            return categories
                .ToDictionary(o => o, o => (IList<Tag2Model>)tags.Where(t => t.CategoryId == o.Id).Select(t => new Tag2Model(t)).ToList());
        }

        public IDictionary<TagCategoryModel, IList<Tag2Model>> GetTagsByUserIdAndCategories(int userId, IList<TagCategoryModel> categories)
        {
            var userTags = this.Repo.UserTags.GetByUserId(userId).Where(o => !o.DateDeletedUtc.HasValue).ToList();
            var tagsIds = userTags.Select(o => o.TagId).ToArray();
            var tags = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, tagsIds);

            return categories
                .ToDictionary(o => o, o => (IList<Tag2Model>)tags.Where(t => t.CategoryId == o.Id).Select(t => new Tag2Model(t)).ToList());
        }

        public IDictionary<TagCategoryModel, IList<Tag2Model>> GetTagsByGroupIdAndCategories(int groupId, IList<TagCategoryModel> categories)
        {
            var groupTags = this.Repo.GroupTags.GetByGroupId(groupId).Where(o => !o.DateDeletedUtc.HasValue).ToList();
            var tagsIds = groupTags.Select(o => o.TagId).ToArray();
            var tags = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, tagsIds);

            return categories
                .ToDictionary(o => o, o => (IList<Tag2Model>)tags.Where(t => t.CategoryId == o.Id).Select(t => new Tag2Model(t)).ToList());
        }

        public IList<Tag2Model> GetCompanyTagsInApplyRequest(Guid applyKey)
        {
            var apply = this.Services.People.GetApplyRequest(applyKey);
            if (apply == null)
                return null;

            return apply.CompanyDataModel.CompanyTags;
        }

        public IList<Tag2Model> GetCompanyTagsInApplyRequest(Guid applyKey, int categoryId)
        {
            return this.GetCompanyTagsInApplyRequest(applyKey).Where(o => o.CategoryId == categoryId).ToList();
        }

        public AddOrRemoveTagResult Create(AddOrRemoveTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("The value cannot be empty.", "request");

            const string logPath = "TagsService.AddOrRemoveTag";
            var result = new AddOrRemoveTagResult(request);

            if (string.IsNullOrEmpty(request.TagId))
            {
                result.Errors.Add(AddOrRemoveTagError.MustSpecifyTagId, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var transaction = this.Services.NewTransaction();
            using (transaction.BeginTransaction())
            {
                // Check General rules

                // TagCategory exists for the network
                var tagCategory = transaction.Repositories.TagCategories.GetByAlias(this.Services.NetworkId, request.CategoryAlias);
                if (tagCategory == null || (tagCategory.NetworkId.HasValue && tagCategory.NetworkId.Value != this.Services.NetworkId))
                {
                    result.Errors.Add(AddOrRemoveTagError.NoSuchTagCategory, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                UserModel actingUser = null;
                if (request.ActingUserId != null)
                {
                    var user=transaction.Repositories.People.GetActiveById(request.ActingUserId.Value, PersonOptions.Company);
                    actingUser = new UserModel(user);
                    if (actingUser == null || !actingUser.IsActive)
                    {
                        result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                        return this.LogResult(result, logPath);
                    }

                    if (actingUser.NetworkAccessLevel.Value.HasAnyFlag(adminRoles) && actingUser.NetworkId == this.Services.NetworkId)
                    { }
                    else if (actingUser.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.SparkleStaff))
                    { }
                    else
                    {
                        result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                        return this.LogResult(result, logPath);
                    }
                }
                else
                {
                    result.Errors.Add(AddOrRemoveTagError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                // this domain method is reserved for admins
                // so we will not check the categoryModel.IsAppliedTo(request.EntityTypeName)
                var categoryModel = new TagCategoryModel(tagCategory);

                // Check Tag exists or CanUserCreate

                var existing = transaction.Repositories.TagDefinitions.GetByNameAndCategoryId(this.Services.NetworkId, request.TagId, categoryModel.Id);
                if (existing != null)
                {
                    result.Errors.Add(AddOrRemoveTagError.TagAlreadyExists, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                var now = DateTime.UtcNow;
                var tag = new TagDefinition();
                tag.Alias = this.MakeTagAlias(transaction.Repositories, request.TagId);
                tag.CategoryId = categoryModel.Id;
                tag.CreatedByUserId = actingUser.Id;
                tag.CreatedDateUtc = now;
                tag.Name = request.TagId;
                tag.NetworkId = this.Services.NetworkId;
                transaction.Repositories.TagDefinitions.Insert(tag);

                transaction.CompleteTransaction();
            }

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public AddOrRemoveTagResult AddOrRemoveTag(AddOrRemoveTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("The value cannot be empty.", "request");

            const string logPath = "TagsService.AddOrRemoveTag";
            var result = new AddOrRemoveTagResult(request);

            if (string.IsNullOrEmpty(request.TagId))
            {
                result.Errors.Add(AddOrRemoveTagError.MustSpecifyTagId, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var transaction = this.Services.NewTransaction();
            using (transaction.BeginTransaction())
            {
                // Check General rules

                // TagCategory exists for the network
                var tagCategory = transaction.Repositories.TagCategories.GetByAlias(this.Services.NetworkId, request.CategoryAlias);
                if (tagCategory == null || (tagCategory.NetworkId.HasValue && tagCategory.NetworkId.Value != this.Services.NetworkId))
                {
                    result.Errors.Add(AddOrRemoveTagError.NoSuchTagCategory, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                var categoryRuleName = request.EntityTypeName;
                // SPECIAL CASE FOR APPLYREQUEST
                if (categoryRuleName.IndexOf("ApplyRequest") != -1)
                {
                    // This way we can retrieve the category rule name inside the EntityTypeName (ex: ApplyRequestCompany)
                    categoryRuleName = categoryRuleName
                        .Substring("ApplyRequest".Length, categoryRuleName.Length - "ApplyRequest".Length);
                }

                // This TagCategory can be used for the desired entity
                var categoryModel = new TagCategoryModel(tagCategory);
                if (!categoryModel.IsAppliedTo(categoryRuleName))
                {
                    result.Errors.Add(AddOrRemoveTagError.CategoryNotAvailableForEntity, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                // Check Tag exists or CanUserCreate
                TagDefinition tag = null;
                {
                    int tagDefinitionId;
                    // TagId is an int, tag seems to exists
                    if (int.TryParse(request.TagId, out tagDefinitionId))
                    {
                        tag = transaction.Repositories.TagDefinitions.GetById(tagDefinitionId);
                        // a tag id is given but the tag does not exist, cannot create it from the name
                        if (request.AddTag && tag == null)
                        {
                            result.Errors.Add(AddOrRemoveTagError.NoSuchTag, NetworksEnumMessages.ResourceManager);
                            return this.LogResult(result, logPath);
                        }
                    }
                    // TagId is a string, tag does not exists
                    else
                    {
                        tag = transaction.Repositories.TagDefinitions.GetByAlias(this.Services.NetworkId, request.TagId);
                        tag = tag != null && tag.CategoryId == categoryModel.Id ? tag : null;

                        if (request.AddTag && tag == null && !categoryModel.CanUsersCreate)
                        {
                            result.Errors.AddDetail(AddOrRemoveTagError.CannotAddTagForCategory, "EntityType:" + request.EntityTypeName + " EntityId:" + request.EntityIdentifier + " TagCategoryId:" + categoryModel.Id + " (" + categoryModel.Alias + ") TagId:" + request.TagId, NetworksEnumMessages.ResourceManager);
                            return this.LogResult(result, logPath);
                        }
                    }
                }

                // Check Custom rules
                if (!this.entityWithTagValidators.ContainsKey(request.EntityTypeName))
                    throw new InvalidOperationException(logPath + ": No key '" + request.EntityTypeName + "' found in the custom validators.");
                if (!this.entityWithTagValidators[request.EntityTypeName](transaction.Services, request.EntityIdentifier, request.TagId, request.ActingUserId, categoryModel, result))
                    return this.LogResult(result, logPath);

                result.Entity.TagId = tag != null ? tag.Id : default(int?);
                result.Entity.TagName = tag != null ? tag.Name : request.TagId;
                result.Entity.TagCategoryId = categoryModel.Id;
                result.Entity.ActingUserId = request.ActingUserId;
                result.Entity.NetworkId = this.Services.NetworkId;

                // Do database operations
                var factory = this.GetFactory(request.EntityTypeName);
                if (request.AddTag)
                {
                    factory.Add(transaction.Services, result.Entity);
                    tag = transaction.Repositories.TagDefinitions.GetById(result.Entity.TagId.Value);
                }
                else
                {
                    factory.Remove(transaction.Services, result.Entity);
                }

                transaction.CompleteTransaction();

                result.Succeed = true;
                return this.LogResult(result, logPath, (request.AddTag ? "Create" : "Delete") + " EntityType:" + request.EntityTypeName + " EntityId:" + request.EntityIdentifier + " TagCategoryId:" + categoryModel.Id + " (" + categoryModel.Alias + ") TagId:" + tag.Id + " (" + tag.Alias + ").");
            }
        }

        public AddOrRemoveTagResult GetTagsListAccordingToEntityType(AddOrRemoveTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("The value cannot be empty.", "request");

            const string logPath = "TagsService.GetTagsListAccordingToEntityType";
            var result = new AddOrRemoveTagResult(request);

            var category = this.Repo.TagCategories.GetByAlias(this.Services.NetworkId, request.CategoryAlias);
            if (category == null || (category.NetworkId.HasValue && category.NetworkId.Value != this.Services.NetworkId))
            {
                result.Errors.Add(AddOrRemoveTagError.NoSuchTagCategory, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            // Check existence of Entity
            int entityId;
            if (!this.entityValidators.ContainsKey(request.EntityTypeName))
                throw new InvalidOperationException(logPath + ": No key '" + request.EntityTypeName + "' found in the custom validators.");
            if (!this.entityValidators[request.EntityTypeName](this.Services, request.EntityIdentifier, result, out entityId))
                return this.LogResult(result, logPath);

            var factory = this.GetFactory(request.EntityTypeName);
            result.TagsList = factory.GetAllTagsForCategory(this.Services, category.Id, entityId);

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public AddOrRemoveTagResult GetEntityTagsAccordingToEntityType(AddOrRemoveTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("The value cannot be empty.", "request");

            const string logPath = "TagsService.GetEntityTagsAccordingToEntityType";
            var result = new AddOrRemoveTagResult(request);

            // Check existence of TagCategory
            var category = this.Repo.TagCategories.GetByAlias(this.Services.NetworkId, request.CategoryAlias);
            if (category == null || (category.NetworkId.HasValue && category.NetworkId.Value != this.Services.NetworkId))
            {
                result.Errors.Add(AddOrRemoveTagError.NoSuchTagCategory, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            // Check existence of Entity
            int entityId;
            if (!this.entityValidators.ContainsKey(request.EntityTypeName))
                throw new InvalidOperationException(logPath + ": No key '" + request.EntityTypeName + "' found in the custom validators.");
            if (!this.entityValidators[request.EntityTypeName](this.Services, request.EntityIdentifier, result, out entityId))
                return this.LogResult(result, logPath);

            var factory = this.GetFactory(request.EntityTypeName);
            result.TagsList = factory.GetEntityTagsForCategory(this.Services, category.Id, entityId);

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public Tag2Model GetTagsById(int tagId)
        {
            var item = this.Repo.TagDefinitions.GetById(tagId);
            if (item == null)
                return null;

            return new Tag2Model(item);
        }

        public TagCategoryModel GetCategoryById(int categoryId)
        {
            var item = this.Repo.TagCategories.GetById(categoryId);
            if (item == null)
                return null;

            return new TagCategoryModel(item);
        }

        public string MakeAlias(string name)
        {
            return this.MakeTagAlias(this.Repo, name);
        }

        public IDictionary<TagCategoryModel, IList<Tag2Model>> GetUsedEntityTags(string entityTypeName)
        {
            var factory = this.GetFactory(entityTypeName);
            var usedTags = factory.GetUsedEntityTags(this.Services);
            var categoriesIds = usedTags.Select(o => o.CategoryId).Distinct().ToArray();
            var usedCategories = this.Repo.TagCategories.GetByIds(categoriesIds).Select(o => new TagCategoryModel(o)).ToList();

            foreach (var cat in usedCategories)
            {
                cat.Name = this.Services.Lang.T("Tag_Title_" + cat.Name);
            }

            return usedCategories
                .OrderBy(o => o.Id)
                .Where(o => o.IsAppliedTo(entityTypeName))
                .ToDictionary(o => o, o => (IList<Tag2Model>)usedTags.Where(t => t.CategoryId == o.Id).ToList());
        }

        public IList<Tag2Model> GetTagsById(int[] ids)
        {
            var items = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, ids)
                .Select(o => new Tag2Model(o))
                .ToList();
            return items;
        }

        public int[] GetEntitiesIdsWhoUsesTagsIds(string entityTypeName, int[] tagIds, bool accrued)
        {
            var factory = this.GetFactory(entityTypeName);
            var entityIdByTagId = factory.GetEntitiesIdsByUsedTagsIds(this.Services, tagIds);
            if (accrued)
            {
                return entityIdByTagId.Where(o => o.Value.Length == tagIds.Length).Select(o => o.Key).ToArray();
            }

            return entityIdByTagId.Select(o => o.Key).ToArray();
        }

        public int CountByCategory(int categoryId)
        {
            var count = this.Repo.TagDefinitions.CountByCategory(categoryId);
            return count;
        }

        public BasicResult OneShotPopulateIndustries(int categoryId, int userId)
        {
            var result = new BasicResult();

            var count = this.Repo.TagDefinitions.CountByCategory(categoryId);
            if (count > 0)
                result.Errors.Add(new BasicResultError("OneShotPopulateIndustries cannot run because the category already has " + count + " items."));

            var category = this.Repo.TagCategories.GetById(categoryId);
            if (category == null)
                result.Errors.Add(new BasicResultError("OneShotPopulateIndustries cannot run because the category " + categoryId + " does not exist."));

            var user = this.Repo.People.GetById(userId);
            if (user == null)
                result.Errors.Add(new BasicResultError("OneShotPopulateIndustries cannot run because the user " + userId + " does not exist."));

            if (result.Errors.Count > 0)
                return result;

            var values = this.Services.ProfileFields.GetAllAvailiableValuesByType(ProfileFieldType.Industry);
            var now = DateTime.UtcNow;
            var culture = new CultureInfo("fr-FR");
            foreach (var value in values)
            {
                var name = this.Services.Lang.T(culture, "Industry: " + value.Value);
                var item = new TagDefinition
                {
                    CategoryId = category.Id,
                    CreatedByUserId = user.Id,
                    CreatedDateUtc = now,
                    Name = name,
                    NetworkId = user.NetworkId,
                };
                item.Alias = this.MakeAlias(item.Name);
                this.Repo.TagDefinitions.Insert(item);
            }

            result.Succeed = true;
            return result;
        }

        public IList<Tag2Model> GetByNameAndCategory(int categoryId, string tagName)
        {
            var items = this.Repo.TagDefinitions.FindByNames(tagName, categoryId);
            return items.Select(o => new Tag2Model(o)).ToList();
        }

        public IList<Tag2Model> GetByNameAndCategory(int[] categoryId, string tagName)
        {
            var items = this.Repo.TagDefinitions.FindByNames(tagName, categoryId);
            return items.Select(o => new Tag2Model(o)).ToList();
        }

        public PagedListModel<Tag2Model> GetTagsByCategoryId(int categoryId, int offset, int count)
        {
            var category = this.GetCategoryById(categoryId);
            var items = this.Repo.TagDefinitions.GetByCategoryId(this.Services.NetworkId, categoryId, offset, count);
            var total = this.Repo.TagDefinitions.CountByCategory(categoryId);
            return new PagedListModel<Tag2Model>
            {
                Count = count,
                Offset = offset,
                Total = total,
                Items = items.Select(x => new Tag2Model(x, category)).ToList(),
            };
        }

        private void InitializeTagCategories()
        {
            const string logPath = "TagsService.InitializeTagCategories";

            var all = this.Repo.TagCategories.GetAll();
            var expected = new List<TagCategory>
            {
                new TagCategory { Id = 1, Name = "Skill",           Alias = "Skill",           CanUsersCreate = true,  Rules = null, },
                new TagCategory { Id = 2, Name = "Interest",        Alias = "Interest",        CanUsersCreate = true,  Rules = null, },
                new TagCategory { Id = 3, Name = "Recreation",      Alias = "Recreation",      CanUsersCreate = true,  Rules = null, },
                new TagCategory { Id = 4, Name = "City",            Alias = "City",            CanUsersCreate = false, Rules = null, },
                new TagCategory { Id = 5, Name = "PartnerResource", Alias = "PartnerResource", CanUsersCreate = true,  Rules = null, },
                new TagCategory { Id = 6, Name = "AdCategories",    Alias = "AdCategory",      CanUsersCreate = false, Rules = null, },
                new TagCategory { Id = 7, Name = "AdTypes",         Alias = "AdType",          CanUsersCreate = false, Rules = null, },
            };

            bool missingCategory = false;
            foreach (var item in expected)
            {
                TagCategory category = all.SingleOrDefault(o => o.Id == item.Id);
                if (category == null || category.Name != item.Name || category.Alias != item.Alias)
                {
                    missingCategory = true;
                    break;
                }
            }

            if (!missingCategory)
            {
                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "All TagCategories OK.");
                return;
            }

            var transaction = this.Repo.NewTransaction();
            using (transaction.BeginTransaction())
            {
                all = transaction.Repositories.TagCategories.GetAll();
                foreach (var item in expected)
                {
                    TagCategory entity = all.SingleOrDefault(o => o.Alias == item.Alias);
                    if (entity == null)
                    {
                        entity = item;

                        transaction.Repositories.TagCategories.Insert(entity);
                        this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created " + entity.ToString() + ".");
                    }
                    else
                    {
                    ////    entity.Name = item.Name;
                    ////    entity.Alias = item.Alias;
                    ////    entity.CanUsersCreate = item.CanUsersCreate;
                    ////    entity.NetworkId = item.NetworkId;
                    ////    entity.Rules = item.Rules;

                    ////    transaction.Repositories.TagCategories.Update(entity);
                    ////    this.Services.Logger.Info(logPath, ErrorLevel.Success, "Updated " + entity.ToString() + ".");
                    }
                }

                transaction.CompleteTransaction();
            }
        }

        private IEntityWithTagRepository GetFactory(string key)
        {
            if (!this.entityWithTagRepositories.ContainsKey(key))
                throw new InvalidOperationException("TagsServices.GetFactory: Key '" + key + "' not found in entityWithTagRepositories.");

            var repo = this.entityWithTagRepositories[key];
            switch (repo.Item1)
            {
                case EntityWithTagRepositoryType.Sql:
                    return new EntityWithTagSqlRepository(repo.Item2, repo.Item3);
                case EntityWithTagRepositoryType.ApplyRequest:
                    return new EntityWithTagApplyRequestRepository(repo.Item2);
                default:
                    throw new InvalidOperationException("TagsServices.GetFactory: Unknown EntityWithTagRepositoryType '" + repo.Item1.ToString() + "'");
            }
        }

        private TagDefinition GetOrCreateTagDefinition(string logPath, bool allowCreate, string idOrName, int categoryId, int? creatingUserId, string description = null)
        {
            TagDefinition tag = null;
            int id;
            if (int.TryParse(idOrName, out id))
            {
                tag = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, new int[] { id }).SingleOrDefault();
            }
            else
            {
                tag = this.Repo.TagDefinitions.GetByNameAndCategoryId(this.Services.NetworkId, idOrName, categoryId);
                if (tag == null && allowCreate && !string.IsNullOrEmpty(idOrName) && creatingUserId.HasValue)
                {
                    tag = new TagDefinition
                    {
                        NetworkId = this.Services.NetworkId,
                        CategoryId = categoryId,
                        Name = idOrName,
                        Alias = this.MakeAlias(idOrName),
                        Description = description,
                        CreatedDateUtc = DateTime.UtcNow,
                        CreatedByUserId = creatingUserId.Value,
                    };

                    tag = this.Repo.TagDefinitions.Insert(tag);
                    this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created " + tag.ToString() + ".");
                }
            }

            return tag;
        }

        private string MakeTagAlias(IRepositoryFactory repositoryFactory, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (repositoryFactory.TagDefinitions.GetByAlias(this.Services.NetworkId, alias) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.TagDefinitions.GetByAlias(this.Services.NetworkId, a) == null);
            }

            return alias;
        }
    }
}
