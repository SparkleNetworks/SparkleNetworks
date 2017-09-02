
namespace Sparkle.Services.Main.Networks
{
    using LinkedInNET.Companies;
    using LinkedInNET.Profiles;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.LinkedInNET;
    using Sparkle.LinkedInNET.OAuth2;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Main.Providers;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Definitions;
    using Sparkle.Services.Networks.EmailModels;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Objects;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Tags.EntityWithTag;
    using Sparkle.Services.Networks.Team;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Objects;
    using Sparkle.UI;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Mail;
    using Neutral = Sparkle.Entities.Networks.Neutral;

    public class PeopleService : ServiceBase, IPeopleService
    {
        private const string UserNotificationKey_Newsletter = "Newsletter";
        private const string UserNotificationKey_DailyNewsletter = "DailyNewsletter";

        #region userPictureOriginalFormat userPictureFormats

        PictureFormat userPictureOriginalFormat = new PictureFormat
        {
            StretchMode = PictureStretchMode.None,
            ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.Unspecified,
            ImageQuality = ImageQuality.Unspecified,
        };

        PictureFormat[] userPictureFormats = new PictureFormat[]
        {
            new PictureFormat
            {
                Name = "Small",
                FileNameFormat = "l",
                Width = 50,
                Height = 50,
                StretchMode = PictureStretchMode.UniformToFill,
                ClipOrigin = PictureClipOrigin.Center,
                ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.JPEG,
                ImageQuality = ImageQuality.Medium,
            },
            new PictureFormat
            {
                Name = "Medium",
                FileNameFormat = "p",
                Width = 170,
                Height = 200,
                StretchMode = PictureStretchMode.UniformToFill,
                ClipOrigin = PictureClipOrigin.Center,
                ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.JPEG,
                ImageQuality = ImageQuality.High,
            },
        };

        #endregion

        #region linkedInFieldSelectors

        private readonly FieldSelector<Person> personFields = FieldSelector.For<LinkedInNET.Profiles.Person>()
                .WithId()
                .WithFirstName()
                .WithLastName()
                .WithDateOfBirth()
                .WithPositions()
                .WithPositionsSummary()
                .WithEmailAddress()
                .WithPhoneNumbers()
                .WithSummary()
                .WithHeadline()
                .WithIndustry()
                .WithLocationCountryCode()
                .WithLocationName()
                .WithSkills()
                .WithInterests()
                .WithPublicProfileUrl()
                .WithProposalComments()
                .WithLanguageId()
                .WithLanguageName()
                .WithLanguageProficiency()
                .WithTwitterAccounts()
                .WithPrimaryTwitterAccount()
                .WithImAccounts()
                .WithEducations()
                .WithFullVolunteer()
                .WithCertifications()
                .WithPatents()
                .WithRecommendationsReceivedWithAdditionalRecommenderInfo();

        private readonly FieldSelector<Sparkle.LinkedInNET.Companies.Company> companyFields = FieldSelector.For<Sparkle.LinkedInNET.Companies.Company>()
                .WithId()
                .WithName()
                .WithUniversalName()
                .WithEmailDomains()
                .WithType()
                .WithTicker()
                .WithWebsiteUrl()
                .WithIndustries()
                .WithStatus()
                .WithLogoUrl()
                .WithSquareLogoUrl()
                .WithBlogRssUrl()
                .WithTwitterId()
                .WithEmployeeCountRange()
                .WithSpecialties()
                .WithLocations()
                .WithDescription()
                .WithStockExchange()
                .WithFoundedYear()
                .WithEndYear()
                .WithNumFollowers();

        #endregion

        private static bool ValidateEntity(IServiceFactory services, string entityIdentifier, AddOrRemoveTagResult result, out int entityId)
        {
            entityId = 0;
            var user = services.People.GetActiveByLogin(entityIdentifier, PersonOptions.None);
            if (user == null)
            {
                result.Errors.Add(AddOrRemoveTagError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return false;
            }

            entityId = user.Id;
            return true;
        }

        private static bool ValidateTag(IServiceFactory services, string entityIdentifier, string tagId, int? actingUserId, TagCategoryModel tagCategory, AddOrRemoveTagResult result)
        {
            if (!actingUserId.HasValue)
                throw new ArgumentNullException("The value cannot be empty.", "actingUserId");
            if (tagCategory == null || tagCategory.RulesModel == null || !tagCategory.RulesModel.Rules.ContainsKey(RuleType.User))
                throw new ArgumentNullException("The value cannot be empty.", "tagCategory");

            // Check user exists
            int userId;
            if (!PeopleService.ValidateEntity(services, entityIdentifier, result, out userId))
            {
                return false;
            }

            // Check user rights
            var user = services.People.GetActiveById(actingUserId.Value, PersonOptions.None);
            if (user == null || !((user.Id == actingUserId.Value) || user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff)))
            {
                result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Check TagCategory company rules
            var max = tagCategory.RulesModel.Rules[RuleType.User].Maximum;
            if (result.Request.AddTag && services.Repositories.UserTags.CountByUserAndCategory(userId, tagCategory.Id, false) >= max)
            {
                result.Errors.Add(AddOrRemoveTagError.MaxNumberOfTagForCategory, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Build EntityWithTag into result
            result.Entity = new SqlEntityWithTag
            {
                EntityId = userId,
            };

            return true;
        }

        private static bool ValidateApplyRequestEntity(IServiceFactory services, string entityIdentifier, AddOrRemoveTagResult result, out int entityId)
        {
            entityId = 0;
            Guid key;
            if (Guid.TryParse(entityIdentifier, out key))
            {
                ApplyRequest item = services.Repositories.ApplyRequests.GetByKey(key, services.NetworkId);
                if (item == null)
                {
                    item = new ApplyRequest
                    {
                        Key = key,
                        NetworkId = services.NetworkId,
                        DateCreatedUtc = DateTime.UtcNow,
                    };
                    item = services.Repositories.ApplyRequests.Insert(item);
                    services.Logger.Info("PeopleService.ValidateEntity", ErrorLevel.Success, "Created " + item.ToString() + ".");
                }

                entityId = item.Id;
                return true;
            }

            result.Errors.Add(AddOrRemoveTagError.NoSuchApplyRequest, NetworksEnumMessages.ResourceManager);
            return false;
        }

        private static bool ValidateApplyRequestCompanyTag(IServiceFactory services, string entityIdentifier, string tagId, int? actingUserId, TagCategoryModel tagCategory, AddOrRemoveTagResult result)
        {
            if (tagCategory == null || tagCategory.RulesModel == null || !tagCategory.RulesModel.Rules.ContainsKey(RuleType.Company))
                throw new ArgumentNullException("The value cannot be empty.", "tagCategory");

            // Check ApplyRequest exists
            int applyRequestId;
            if (!PeopleService.ValidateApplyRequestEntity(services, entityIdentifier, result, out applyRequestId))
            {
                return false;
            }
            var applyModel = services.People.GetApplyRequest(applyRequestId);

            // Check ApplyRequest is not submitted or accepted/refused
            if (applyModel.Status != ApplyRequestStatus.New)
            {
                result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Check TagCategory company rules
            var max = tagCategory.RulesModel.Rules[RuleType.Company].Maximum;
            if (result.Request.AddTag && services.Repositories.CompanyTags.CountByCompanyAndCategory(applyRequestId, tagCategory.Id, false) >= max)
            {
                result.Errors.Add(AddOrRemoveTagError.MaxNumberOfTagForCategory, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Build EntityWithTag into result
            result.Entity = new SqlEntityWithTag
            {
                EntityId = applyRequestId,
            };

            return true;
        }

        ////static PeopleService()
        ////{
        ////    // see RegisterTags
        ////    TagsService.RegisterEntityValidator("User", PeopleService.ValidateEntity);
        ////    TagsService.RegisterTagValidator("User", PeopleService.ValidateTag);
        ////    TagsService.RegisterTagRepository("User", EntityWithTagRepositoryType.Sql, "UserTags", "UserId");

        ////    TagsService.RegisterEntityValidator("ApplyRequestCompany", PeopleService.ValidateApplyRequestEntity);
        ////    TagsService.RegisterTagValidator("ApplyRequestCompany", PeopleService.ValidateApplyRequestCompanyTag);
        ////    TagsService.RegisterTagRepository("ApplyRequestCompany", EntityWithTagRepositoryType.ApplyRequest, "Company", null);
        ////}

        [System.Diagnostics.DebuggerStepThrough]
        internal PeopleService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IPeopleRepository PeopleRepository
        {
            get { return this.Repo.People; }
        }

        public User SelectWithLogin(string login)
        {
            return this.PeopleRepository.Select(OptionsList)
                .SingleOrDefault(u => u.Login == login);
        }

        public User SelectWithLogin(string login, PersonOptions options)
        {
            return this.PeopleRepository.NewQuery(options)
                .SingleOrDefault(u => u.Login == login);
        }

        [Obsolete]
        public IList<User> SelectAll()
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .OrderBy(o => o.LastName)
                .ToList();
        }

        public User GetByGuid(Guid userId)
        {
            return this.PeopleRepository
               .Select(this.OptionsList)
               .WithGuid(userId)
               .SingleOrDefault();
        }

        public User GetForSessionById(Guid id)
        {
            return this.Repo.People.NewQuery(PersonOptions.Company | PersonOptions.Job)
                .Where(u => u.UserId == id)
                .SingleOrDefault();
        }

        public User GetForSessionByLogin(string login)
        {
            return this.Repo.People.NewQuery(PersonOptions.Company | PersonOptions.Job)
                .Where(u => u.Login == login)
                ////.ActiveAccount()
                .SingleOrDefault();
        }

        public User SelectWithId(int userId)
        {
            return this.PeopleRepository
                .Select(this.OptionsList)
                ////.ByNetwork(this.Services.NetworkId)
                .WithId(userId)
                .SingleOrDefault();
        }

        public User SelectWithId(int userId, PersonOptions options)
        {
            return this.PeopleRepository
                .NewQuery(options)
                .WithId(userId)
                .SingleOrDefault();
        }

        public User SelectWithProMail(string email)
        {
            return this.PeopleRepository.GetByEmail(email, this.Services.NetworkId, PersonOptions.None);
        }

        public User SelectWithShortId(int shortId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .WithShortId(shortId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Nombre d'utilisateurs inscrits
        /// </summary>
        /// <returns></returns>
        public int CountAll()
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Count();
        }

        /// <summary>
        /// Selects peoples from company.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        public IList<User> SelectFromCompany(long companyId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .WithCompanyId(companyId)
                .ActiveAccount()
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        /// <summary>
        /// Renvoie le nombre d'items, utilis� sur la page Companies
        /// </summary>
        public int CountCompanyPeoples(long companyId)
        {
            return this.PeopleRepository
                .Select()
                .WithCompanyId(companyId)
                .ActiveAccount()
                .Count();
        }

        /// <summary>
        /// Selects peoples from job.
        /// </summary>
        /// <param name="jobId">The job id.</param>
        /// <returns></returns>
        public IList<User> SelectFromJob(long jobId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithJobId(jobId)
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        public IList<User> Search(string request)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .Contain(request)
                .OrderBy(o => o.FirstName)
                .Take(5)
                .ToList();
        }

        public IList<User> SearchFromCompanyId(string request, int take, int CompanyId)
        {
            return this.PeopleRepository
                .Select(this.OptionsList)
                            .WithCompanyId(CompanyId)
                            .Contain(request)
                            .Take(take)
                            .ToList();
        }

        public IList<User> SearchPeopleWithoutTeam(string request, int take, int CompanyId)
        {
            return this.PeopleRepository
                .Select(this.OptionsList)
                        .WithoutTeam(CompanyId)
                        .Contain(request)
                        .Take(take)
                        .ToList();
        }

        public IList<User> SearchContacts(string search, int Take, int userId)
        {
            return this.PeopleRepository.SelectContacts(search, userId, Take).ToList();
        }

        public IList<User> SelectUnvitedByGroupId(string request, int? GroupId, int Take, int userId)
        {
            return this.PeopleRepository.SelectUnvitedByGroupId(request, GroupId, userId, Take).ToList();
        }

        public IList<User> SelectUnvitedByEventId(string request, int? EventId, int Take, int userId)
        {
            return this.PeopleRepository.SelectUnvitedByEventId(request, EventId, userId, Take).ToList();
        }

        public long Update(User item)
        {
            this.VerifyNetwork(item);

            return PeopleRepository.Update(item).Id;
        }

        public User Insert(User item)
        {
            this.SetNetwork(item);

            return PeopleRepository.Insert(item);
        }

        /// <summary>
        /// Finds peoples by login (firstname.lastname) and firstname and lastname if no results.
        /// </summary>
        /// <param name="firstname">The firstname.</param>
        /// <param name="lastname">The lastname.</param>
        /// <returns></returns>
        public IList<User> FindPeoples(string firstname, string lastname)
        {
            string login = firstname.ToLower() + "." + lastname.ToLower();

            IList<User> results = this.PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithLogin(login)
                .OrderBy(o => o.FirstName)
                .ToList();

            if (results.Count > 0) return results;
            results = this.PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithFirstAndLastName(firstname, lastname)
                .OrderBy(o => o.FirstName)
                .ToList();
            return results;
        }

        public IList<NewsletterSubscriber> SelectForNewsletter()
        {
            var withNotification = this.PeopleRepository.NewQuery(PersonOptions.Notification)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(o => o.Notification != null)
                .Select(o => new NewsletterSubscriber
                {
                    Email = o.Email,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Company = o.Company.Name,
                    Accepted = (o.Notification.Newsletter == true || o.Notification.DailyNewsletter == true) ? true : false
                })
                .ToList();
            var withoutNotification = this.PeopleRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(o => o.Notification == null)
                .Select(o => new NewsletterSubscriber
                {
                    Email = o.Email,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Company = o.Company.Name,
                })
                .ToList();

            bool anyNews = this.DoesPeopleHaveAnyNewsByDefault();

            for (int i = 0; i < withoutNotification.Count; i++)
            {
                withoutNotification[i].Accepted = anyNews;
                withNotification.Add(withoutNotification[i]);
            }

            return withNotification;
        }

        public int CountForNewsletter()
        {
            int count = 0;

            var anyNews = this.DoesPeopleHaveAnyNewsByDefault();
            if (anyNews)
            {
                count += this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification == null)
                    .Count();
            }

            count += this.PeopleRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(u => u.Notification != null && (u.Notification.Newsletter == true || u.Notification.DailyNewsletter == true))
                .Count();

            return count;
        }

        public int CountForDailyNewsletter()
        {
            int count = 0;

            var anyNews = this.DoesPeopleHaveAnyNewsByDefault();
            if (anyNews)
            {
                count += this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification == null)
                    .Count();
            }

            count += this.PeopleRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(u => u.Notification != null && (u.Notification.DailyNewsletter == true))
                .Count();

            return count;
        }

        public int CountForWeeklyNewsletter()
        {
            int count = 0;

            var anyNews = this.DoesPeopleHaveAnyNewsByDefault();
            if (anyNews)
            {
                count += this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification == null)
                    .Count();
            }

            count += this.PeopleRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(u => u.Notification != null && (u.Notification.Newsletter == true))
                .Count();

            return count;
        }

        public int CountForNoNewsletter()
        {
            int count = 0;
            var anyNewsByDefault = this.DoesPeopleHaveAnyNewsByDefault();
            if (anyNewsByDefault)
            {
                // everyone is subscribed by default
                // those who received nothing are only those who opted-out

                count = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification != null && (u.Notification.Newsletter != true && u.Notification.DailyNewsletter != true))
                    .Count();
            }
            else
            {
                // you have to opt-in for any news
                // those who receive nothing are all but those who opted-in
                // count everyone and substract those who opted-in

                count = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Count();

                count -= this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification != null && (u.Notification.Newsletter == true || u.Notification.DailyNewsletter == true))
                    .Count();
            }

            return count;
        }

        public int CountForNoDailyNewsletter()
        {
            int count = 0;
            var anyNewsByDefault = this.DoesPeopleHaveAnyNewsByDefault();
            if (anyNewsByDefault)
            {
                // everyone is subscribed by default
                // those who received nothing are only those who opted-out

                count = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification != null && (u.Notification.DailyNewsletter != true))
                    .Count();
            }
            else
            {
                // you have to opt-in for any news
                // those who receive nothing are all but those who opted-in
                // count everyone and substract those who opted-in

                count = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Count();

                count -= this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification != null && (u.Notification.DailyNewsletter == true))
                    .Count();
            }

            return count;
        }

        public int CountForNoWeeklyNewsletter()
        {
            int count = 0;
            var anyNewsByDefault = this.DoesPeopleHaveAnyNewsByDefault();
            if (anyNewsByDefault)
            {
                // everyone is subscribed by default
                // those who received nothing are only those who opted-out

                count = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification != null && (u.Notification.Newsletter != true))
                    .Count();
            }
            else
            {
                // you have to opt-in for any news
                // those who receive nothing are all but those who opted-in
                // count everyone and substract those who opted-in

                count = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Count();

                count -= this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(u => u.Notification != null && (u.Notification.Newsletter == true))
                    .Count();
            }

            return count;
        }

        public WeeklyMailSubscriber SelectForWeeklyMailByLogin(string login)
        {
            return this.PeopleRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(u => u.Login == login)
                .Select(o => new WeeklyMailSubscriber
                {
                    Email = o.Email,
                    FirstName = o.FirstName,
                    LastName = o.LastName
                })
                .FirstOrDefault();
        }

        public IList<WeeklyMailSubscriber> SelectForWeeklyMail(bool invited, bool registered)
        {
            string defaultNotificationKey = UserNotificationKey_Newsletter;
            Expression<Func<User, bool>> optedInPredicate = p => p.Notification != null && p.Notification.Newsletter == true;
            Expression<Func<User, bool>> notOptedInPredicate = p => p.Notification == null || p.Notification.Newsletter == null;

            IList<WeeklyMailSubscriber> result = this.SelectForNewsletter(
                invited,
                registered,
                defaultNotificationKey,
                optedInPredicate,
                notOptedInPredicate);

            return result;
        }

        public IList<WeeklyMailSubscriber> SelectForDailyMail(bool invited, bool registered)
        {
            string defaultNotificationKey = UserNotificationKey_DailyNewsletter;
            Expression<Func<User, bool>> optedInPredicate = p => p.Notification != null && p.Notification.DailyNewsletter == true;
            Expression<Func<User, bool>> notOptedInPredicate = p => p.Notification == null || p.Notification.DailyNewsletter == null;

            IList<WeeklyMailSubscriber> result = SelectForNewsletter(
                invited,
                registered,
                defaultNotificationKey,
                optedInPredicate,
                notOptedInPredicate);

            return result;
        }

        public int CountByCompany(int companyId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .WithCompanyId(companyId)
                .Count();
        }

        public CompanyAccessLevel GetAccountRight(int userId)
        {
            var person = this.PeopleRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .WithId(userId)
                .Select(p => new
                {
                    Id = p.Id,
                    AccountRight = p.AccountRight,
                    CompanyAccessLevel = p.CompanyAccessLevel,
                })
                .Single();

            return (CompanyAccessLevel)person.CompanyAccessLevel;
        }

        public IList<User> GetActiveByCompany(int companyId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .WithCompanyId(companyId)
                .ActiveAccount()
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        public IList<User> GetAllByCompanyId(int companyId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .WithCompanyId(companyId)
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        public IList<User> GetBySkill(int skillId)
        {
            var peopleSkills = this.Repo.PeoplesSkills.Select().Where(s => s.SkillId == skillId).ToList();
            List<int> peopleId = new List<int>();
            foreach (var item in peopleSkills)
            {
                peopleId.Add(item.UserId);
            }

            return this.Repo.People.Select()
             .ByNetwork(this.Services.NetworkId)
             .Where(i => peopleId.Contains(i.Id))
             .ToList();
        }

        public IList<User> GetByInterest(int interestId)
        {
            var peopleInterests = this.Repo.PeoplesInterests.Select().Where(s => s.InterestId == interestId).ToList();
            List<int> peopleId = new List<int>();
            foreach (var item in peopleInterests)
            {
                peopleId.Add(item.UserId);
            }

            return this.Repo.People.Select()
             .ByNetwork(this.Services.NetworkId)
             .Where(i => peopleId.Contains(i.Id))
             .ToList();
        }

        public IList<User> GetByRecreation(int recreationId)
        {
            var peopleRecreations = this.Repo.PeoplesRecreations.Select().Where(s => s.RecreationId == recreationId).ToList();
            List<int> peopleId = new List<int>();
            foreach (var item in peopleRecreations)
            {
                peopleId.Add(item.UserId);
            }

            return this.Repo.People.Select()
             .ByNetwork(this.Services.NetworkId)
             .Where(i => peopleId.Contains(i.Id))
             .ToList();
        }

        public int CountCompleteProfiles()
        {
            return this.Repo.People.CountCompleteUserProfiles(this.Services.NetworkId);
        }

        public int CountMustBeValidateUsersByCompanyId(int companyId)
        {
            return this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(o => o.CompanyID == companyId && o.AccountClosed.HasValue && o.AccountClosed.Value)
                .Count();
        }

        public IEnumerable<User> QueryActivePeople(PersonOptions options)
        {
            return this.Repo.People.NewQuery(options)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteById(int[] usersIds)
        {
            return this.Repo.People.GetLiteById(usersIds, this.Services.NetworkId);
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteByIdFromAnyNetwork(int[] usersIds)
        {
            return this.Repo.People.GetLiteById(usersIds);
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteByEmail(string[] emails)
        {
            return this.Repo.People.GetLiteByEmail(emails, this.Services.NetworkId);
        }

        public IList<Sparkle.Entities.Networks.Neutral.UserPoco> GetLiteById(int[] usersIds, Func<User, Sparkle.Entities.Networks.Neutral.UserPoco> selector)
        {
            return this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(p => usersIds.Contains(p.Id))
                .Select(selector)
                .ToList();
        }

        public Sparkle.Entities.Networks.Neutral.UserPoco GetLiteById(int userId, Func<User, Sparkle.Entities.Networks.Neutral.UserPoco> selector)
        {
            return this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(p => userId == p.Id)
                .Select(selector)
                .SingleOrDefault();
        }

        public IList<Sparkle.Entities.Networks.Neutral.UserPoco> GetActiveLiteById(int[] usersIds, Func<User, Sparkle.Entities.Networks.Neutral.UserPoco> selector)
        {
            return this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(p => usersIds.Contains(p.Id))
                .Select(selector)
                .ToList();
        }

        /// <summary>
        /// Counts people active in the last 30 days.
        /// </summary>
        /// <returns></returns>
        public int CountActive()
        {
            return PeopleRepository
                .NewQuery(PersonOptions.None)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Count();
        }

        public int Count30DaysConnected()
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Count();
        }

        public bool CanChangeUserCompanyAccess(User targetUser, User actingUser, CompanyAccessLevel levelToSetOnTarget)
        {
            if (targetUser == null)
                throw new ArgumentNullException("targetUser");
            if (actingUser == null)
                throw new ArgumentNullException("actingUser");

            if (actingUser.CompanyAccess == CompanyAccessLevel.Administrator
             && targetUser.CompanyID == actingUser.CompanyID
             || actingUser.NetworkAccess.HasFlag(NetworkAccessLevel.ManageCompany)
             || actingUser.NetworkAccess.HasFlag(NetworkAccessLevel.SparkleStaff)
             || actingUser.NetworkAccess.HasFlag(NetworkAccessLevel.NetworkAdmin))
                return true;

            return false;
        }

        public User GetForCoffeeRoulette(User currentUser)
        {
            User selectedPerson;

            Random rndGenders = new Random();
            int rndGender = rndGenders.Next(0, 2);

            if (rndGender < 2)
            {
                int gender = 0;
                if (currentUser.Gender == 0) gender = 1;

                selectedPerson = PeopleRepository
                    .Select(this.OptionsList)
                    .ActiveAccount()
                    .Where(u => u.Gender == gender)
                    .Where(u => u.Id != currentUser.Id)
                    .FirstOrDefault();
            }
            else
            {
                selectedPerson = PeopleRepository
                    .Select(this.OptionsList)
                    .ActiveAccount()
                    .Where(u => u.Id != currentUser.Id)
                    .FirstOrDefault();
            }

            int max = PeopleRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Count() - 1;


            Random rndNumbers = new Random();
            int rndNumber = rndNumbers.Next(0, max);

            return selectedPerson;
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetByNetworkAccessLevel(NetworkAccessLevel networkAccessLevel)
        {
            var list = new List<Sparkle.Entities.Networks.Neutral.Person>();
            var flags = Enum.GetValues(typeof(NetworkAccessLevel));
            foreach (var flagObject in flags)
            {
                var flag = (NetworkAccessLevel)flagObject;
                int level = (int)flag;

                if (flag == NetworkAccessLevel.Disabled)
                {
                }
                else if (flag == NetworkAccessLevel.SparkleStaff)
                {
                    Sparkle.Entities.Networks.Neutral.Person[] data;
                    data = this.Repo.People
                        .Select()
                        .ActiveAccount()
                        .Where(u => (u.NetworkAccessLevel & level) == level)
                        .Select(u => new Sparkle.Entities.Networks.Neutral.Person
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Username = u.Login,
                            Culture = u.Culture,
                            Timezone = u.Timezone,
                        })
                        .ToArray();

                    foreach (var item in data)
                    {
                        if (!list.Any(u => u.Id == item.Id))
                            list.Add(item);
                    }
                }
                else if (networkAccessLevel.HasFlag(flag))
                {
                    var data = this.Repo.People
                        .Select()
                        .ByNetwork(this.Services.NetworkId)
                        .Where(u => (u.NetworkAccessLevel & level) == level)
                        .Select(u => new Sparkle.Entities.Networks.Neutral.Person
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Username = u.Login,
                            Culture = u.Culture,
                            Timezone = u.Timezone,
                        })
                        .ToArray();
                    foreach (var item in data)
                    {
                        if (!list.Any(u => u.Id == item.Id))
                            list.Add(item);
                    }
                }
            }

            return list;
        }

        public IList<User> SelectCompanyContacts(int companyId)
        {
            return PeopleRepository
                .Select(this.OptionsList)
                .WithCompanyId(companyId)
                .ActiveAccount()
                .Where(p => p.CompanyAccessLevel > 1)
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        public SetProfilePictureResult SetProfilePicture(SetProfilePictureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (request.PictureStream == null)
                throw new ArgumentNullException("request");
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory))
                throw new InvalidOperationException("Configuration entry 'Storage.UserContentsDirectory' must have a valid value");

            var result = new SetProfilePictureResult(request);
            var user = this.Repo.People.GetById(request.UserId);
            IFilesystemProvider provider = new IOFilesystemProvider();
            PictureTransformer transformer = new PictureTransformer();

            // save original file
            string originalPicturePath = provider.EnsureFilePath(
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Original",
                user.Username);
            originalPicturePath = originalPicturePath.GetIncrementedString(path => !provider.FileExists(path + ".jpg"));
            originalPicturePath += ".jpg";

            provider.WriteNewFile(originalPicturePath, request.PictureStream);

            // generate web pictures
            Guid tmp = Guid.NewGuid();
            var profileImageName = tmp.ToString().Replace("-", "").Substring(0, 10);
            user.Picture = profileImageName;

            var formats = userPictureFormats;
            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];

                MemoryStream resized;
                try
                {
                    resized = transformer.FormatPicture(format, request.PictureStream);
                }
                catch (FormatException ex)
                {
                    this.Services.Logger.Error("PeopleServices.SetProfilePicture", ErrorLevel.Business, ex.ToString());
                    result.Errors.Add(SetProfilePictureError.FileIsNotPicture, NetworksEnumMessages.ResourceManager);
                    return result;
                }

                var path = provider.EnsureFilePath(
                    this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                    "Networks",
                    this.Services.Network.Name,
                    "Peoples",
                    user.Login,
                    profileImageName + format.FileNameFormat + ".jpg");
                provider.WriteFile(path, resized);
            }

            if (user.GenderValue != NetworkUserGender.Male && user.GenderValue != NetworkUserGender.Female)
                user.GenderValue = this.Services.AppConfiguration.Tree.Features.Users.DefaultGender == "Male" ? NetworkUserGender.Male
                    : this.Services.AppConfiguration.Tree.Features.Users.DefaultGender == "Female" ? NetworkUserGender.Female
                    : NetworkUserGender.Unspecified;

            this.Services.People.Update(user);

            this.Services.Logger.Info("PeopleServices.SetProfilePicture", ErrorLevel.Success, "Picture for user " + user.Id + " has been updated");
            result.User = user;
            result.Succeed = true;
            return result;
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(int userId, PictureAccessMode mode)
        {
            var user = this.Services.Cache.GetUser(userId);

            return this.GetProfilePicture(user, mode);
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(User user, PictureAccessMode mode)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return this.GetProfilePicture(user.Id, user.Login, user.GenderValue, user.Picture, mode);
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(UserModel user, PictureAccessMode mode)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return this.GetProfilePicture(user.Id, user.Username, user.Gender, user.Picture, mode);
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(int userId, string username, NetworkUserGender gender, string pictureName, PictureAccessMode mode)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("The value cannot be empty", "username");

            var basePath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
            var commonPeoplesPath = Path.Combine(basePath, "Networks", "Common", "Peoples");
            var networkPeoplesPath = Path.Combine(basePath, "Networks", this.Services.Network.Name, "Peoples");

            var model = new ProfilePictureModel();
            var pictures = new Dictionary<string, PictureAccess>(3);
            model.Pictures = pictures;
            model.UserId = userId;
            model.Username = username;

            // impossible to get the path to the original picture...
            // the path is not predictable :'(
            pictures.Add("Original", new PictureAccess
            {
                Format = userPictureOriginalFormat.Clone(),
                FilePath = null,
                Bytes = null,
            });

            if (gender == NetworkUserGender.Unspecified)
            {
                NetworkUserGender tmpGender;
                if (Enum.TryParse<NetworkUserGender>(this.Services.AppConfiguration.Tree.Features.Users.DefaultGender, out tmpGender))
                    gender = tmpGender;
            }

            var formats = userPictureFormats;
            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];
                bool found = false;

                // search user's picture path
                string remark = "user";
                string path = Path.Combine(networkPeoplesPath, username, pictureName + format.FileNameFormat + ".jpg");
                if (!(found = File.Exists(path)))
                {
                    // defaut picture for network
                    remark = "network default";
                    path = Path.Combine(networkPeoplesPath, format.Name + "-" + gender.ToString() + ".jpg");
                    if (!(found = File.Exists(path)))
                    {
                        // defaut picture
                        remark = "default";
                        path = Path.Combine(commonPeoplesPath, format.Name + "-" + gender.ToString() + ".jpg");
                        if (!(found = File.Exists(path)))
                        {
                            path = null;
                        }
                    }
                }

                byte[] bytes = null;
                string mime = null;
                DateTime dateChanged = DateTime.MinValue;
                if (path != null)
                {
                    dateChanged = File.GetLastWriteTimeUtc(path);
                    bytes = mode.HasFlag(PictureAccessMode.Stream) ? File.ReadAllBytes(path) : null;
                    if (bytes != null)
                    {
                        var bytes256 = bytes.Length > 600
                                     ? bytes.Take(600).ToArray()
                                     : bytes;
                        var mimeType = MimeDetective.MimeTypes.GetFileType(bytes256);
                        if (mimeType != null)
                            mime = mimeType.Mime;
                    }
                }

                pictures.Add(format.Name, new PictureAccess
                {
                    Format = format.Clone(),
                    FilePath = mode.HasFlag(PictureAccessMode.FilePath) ? path : null,
                    Bytes = mode.HasFlag(PictureAccessMode.Stream) && path != null ? File.ReadAllBytes(path) : null,
                    MimeType = mime,
                    Remark = remark,
                    DateChangedUtc = dateChanged,
                });
            }

            return model;
        }

        public CreateEmailPassordAccountResult CreateEmailPasswordAccount(CreateEmailPassordAccountRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleService.CreateEmailPasswordAccount";
            var result = new CreateEmailPassordAccountResult(request);

            // this form can be used when companies are disabled
            // or when accepting a register invitation

            var transaction = this.Services.NewTransaction();
            var transactionDisposable = transaction.BeginTransaction();
            try
            {
                // check invitation code
                UserInvitationModel invitation = null;
                RegisterRequestModel registerRequest = null;
                bool isCodeValid = request.FromApplyRequest;
                if (request.InvitationCode != null)
                {
                    var invitationResult = transaction.Services.Invited.ValidateCode(request.InvitationCode.Value);
                    invitation = invitationResult.Invitation;
                    result.CodeValidation = invitationResult;

                    if (invitationResult.IsValid)
                    {
                        isCodeValid = true;

                        if (request.Email != invitation.Email)
                        {
                            result.Errors.Add(CreateEmailPassordAccountError.RegisterRequestEmailMismatch, NetworksEnumMessages.ResourceManager, request.Email);
                            return this.LogResult(result, logPath);
                        }
                        ////registerRequest = invitation.RegisterRequest;
                        ////if (registerRequest != null)
                        ////{
                        ////    if (request.Email != registerRequest.EmailAddress)
                        ////    {
                        ////        result.Errors.Add(CreateEmailPassordAccountError.RegisterRequestEmailMismatch, NetworksEnumMessages.ResourceManager, request.Email);
                        ////        return result;
                        ////    }
                        ////}
                    }
                    else
                    {
                        result.Errors.Add(CreateEmailPassordAccountError.InvalidInvitation, NetworksEnumMessages.ResourceManager);
                        return this.LogResult(result, logPath);
                    }
                }

                // check config for default company
                var defaultCompanyId = transaction.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany;
                bool isJoining = defaultCompanyId != null;
                if (defaultCompanyId == null && !isCodeValid)
                {
                    result.Errors.Add(CreateEmailPassordAccountError.InvitationOnly, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                Sparkle.Entities.Networks.Company company = null;
                if (defaultCompanyId != null && defaultCompanyId.Value != 0)
                {
                    company = transaction.Services.Company.GetById(defaultCompanyId.Value);
                    if (company == null || company.NetworkId != transaction.Services.NetworkId)
                    {
                        result.Errors.Add(CreateEmailPassordAccountError.RegisterInCompanyValueIsInvalid, NetworksEnumMessages.ResourceManager);
                    }
                }
                else if (request.JoinCompanyId.HasValue)
                {
                    company = transaction.Services.Company.GetById(request.JoinCompanyId.Value);
                    if (company == null || company.NetworkId != transaction.Services.NetworkId)
                    {
                        result.Errors.Add(CreateEmailPassordAccountError.JoinCompanyIdIsInvalid, NetworksEnumMessages.ResourceManager);
                    }
                }
                else if (invitation != null)
                {
                    company = transaction.Services.Company.GetById(invitation.CompanyId);
                    if (!transaction.Services.Company.IsActive(company))
                    {
                        result.Errors.Add(CreateEmailPassordAccountError.CompanyIsInactive, NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(CreateEmailPassordAccountError.NoCompanyFound, NetworksEnumMessages.ResourceManager);
                }

                // check email already exists
                var user = transaction.Services.People.SelectWithProMail(request.Email);
                if (user != null)
                {
                    result.Errors.Add(CreateEmailPassordAccountError.UserEmailAlreadyExists, NetworksEnumMessages.ResourceManager);
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, logPath);
                }

                // admin validation
                bool mustBeValidated = isJoining && transaction.Services.AppConfiguration.Tree.Features.Users.RegisterInCompanyRequireValidation;

                // company details
                CompanyAccessLevel companyAccessLevel = CompanyAccessLevel.User;
                if (invitation != null && invitation.CompanyAccessLevel != null)
                {
                    companyAccessLevel = (CompanyAccessLevel)invitation.CompanyAccessLevel.Value;
                }
                else if (!isJoining && transaction.Services.People.CountByCompany(company.ID) == 0)
                {
                    // nobody in company, make first user admin
                    companyAccessLevel = CompanyAccessLevel.Administrator;
                    mustBeValidated = false;
                }
                else if (request.FromApplyRequest)
                {
                    mustBeValidated = false;
                    if (transaction.Services.People.CountByCompany(company.ID) == 0)
                        companyAccessLevel = CompanyAccessLevel.Administrator;
                }

                // username
                var username = transaction.Services.People.MakeUsernameFromName(request.FirstName, request.LastName);

                // membership
                System.Web.Security.MembershipCreateStatus createStatus = System.Web.Security.MembershipCreateStatus.ProviderError;
                var membership = this.Services.MembershipService;

                var mbsUserByUsername = membership.GetUser(username);

                Sparkle.Services.Authentication.MembershipUser mbsUser = membership.GetUserByEmail(request.Email);
                if (mbsUser != null)
                {
                    user = transaction.Services.People.GetByGuid(mbsUser.ProviderUserKey);
                    if (user != null)
                    {
                        result.Errors.Add(CreateEmailPassordAccountError.UserEmailAlreadyExists, NetworksEnumMessages.ResourceManager);
                        return this.LogErrorResult(result, logPath);
                    }
                    else if (!membership.ChangePassword(mbsUser.UserName, request.Password))
                    {
                        result.Errors.Add(CreateEmailPassordAccountError.InternalError1, NetworksEnumMessages.ResourceManager);
                        return this.LogErrorResult(result, logPath);
                    }
                }
                else
                {
                    if (mbsUserByUsername != null)
                    {
                        membership.DeleteUser(mbsUserByUsername.UserName);
                    }

                    createStatus = membership.CreateUser(username, request.Password, request.Email);
                    if (createStatus == System.Web.Security.MembershipCreateStatus.Success)
                    {
                        mbsUser = membership.GetUser(username);
                    }
                    else
                    {
                        switch (createStatus)
                        {
                            case System.Web.Security.MembershipCreateStatus.DuplicateEmail:
                            case System.Web.Security.MembershipCreateStatus.DuplicateUserName:
                                result.Errors.Add(CreateEmailPassordAccountError.InternalError1, NetworksEnumMessages.ResourceManager);
                                break;

                            case System.Web.Security.MembershipCreateStatus.DuplicateProviderUserKey:
                            case System.Web.Security.MembershipCreateStatus.InvalidProviderUserKey:
                            case System.Web.Security.MembershipCreateStatus.InvalidAnswer:
                            case System.Web.Security.MembershipCreateStatus.InvalidQuestion:
                            case System.Web.Security.MembershipCreateStatus.ProviderError:
                            case System.Web.Security.MembershipCreateStatus.UserRejected:
                                result.Errors.Add(CreateEmailPassordAccountError.InternalError2, NetworksEnumMessages.ResourceManager);
                                break;

                            case System.Web.Security.MembershipCreateStatus.InvalidEmail:
                                request.AddValidationError("Email", "Invalid email address");
                                result.Errors.Add(CreateEmailPassordAccountError.InternalError3, NetworksEnumMessages.ResourceManager);
                                break;

                            case System.Web.Security.MembershipCreateStatus.InvalidPassword:
                                request.AddValidationError("Password", "Invalid password");
                                result.Errors.Add(CreateEmailPassordAccountError.InternalError3, NetworksEnumMessages.ResourceManager);
                                break;

                            case System.Web.Security.MembershipCreateStatus.InvalidUserName:
                                request.AddValidationError("", "Invalid username");
                                result.Errors.Add(CreateEmailPassordAccountError.InternalError3, NetworksEnumMessages.ResourceManager);
                                break;
                        }

                        return this.LogResult(result, logPath);
                    }
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, logPath);
                }

                user = new User
                {
                    UserId = mbsUser.ProviderUserKey,
                    Login = username,
                    FirstName = request.FirstName.CapitalizeWords(),
                    LastName = request.LastName.CapitalizeWords(),
                    Email = request.Email,
                    CompanyID = company.ID,
                    GenderValue = request.Gender,
                    JobId = request.JobId,
                    InvitationsLeft = 0,
                    AccountRight = null,
                    AccountClosed = mustBeValidated,
                    CompanyAccessLevel = (int)companyAccessLevel,
                    NetworkAccess = NetworkAccessLevel.User,
                    IsEmailConfirmed = isCodeValid,
                    NetworkId = transaction.Services.NetworkId,
                    CreatedDateUtc = DateTime.UtcNow,
                    // fields to integrate later
                    ////Country = "FR",
                    Culture = null,
                    ////DisplayName = request.Email,
                    ////FailedPasswordAttemptCount = 0,
                    ////FailedPasswordAttemptWindowStart = null,
                    ////FailedPasswordRecoveryAttemptCount = 0,
                    ////FailedPasswordRecoveryAttemptWindowStart = null,
                    ////IsLockedOut = false,
                    ////LastLockoutDate = null,
                    ////LastLoginDate = null,
                    ////Logins = 0,
                    ////PasswordFormat = "s6",
                    ////Password = password,
                    ////PasswordDate = DateTime.UtcNow,
                    Timezone = null,
                };


                user = transaction.Repositories.People.Insert(user);
                user = transaction.Services.People.SelectWithId(user.Id, PersonOptions.Job | PersonOptions.Company);
                var profileFields = transaction.Services.ProfileFields.GetUserProfileFieldsByUserId(user.Id);
                result.User = new UserModel(user, profileFields);

                if (invitation != null)
                {
                    transaction.Services.Invited.MarkAsRegistred(invitation.Code, user.Id);
                }

                if (transaction.Services.People.IsActive(user))
                {
                    transaction.PostSaveActions.Add(t => t.Services.Wall.HasJustJoined(user.Id));
                }
                else if (!user.IsEmailConfirmed)
                {
                    // create email confirmation link
                    var confirmEmailAction = new UserActionKey
                    {
                        Action = UserActionKey.UserEmailConfirmActionKey,
                        DateCreatedUtc = DateTime.UtcNow,
                        DateExpiresUtc = null,
                        MaxUsages = 5,
                        RemainingUsages = 5,
                        Secret = SimpleSecrets.NewMediumRandom,
                        UserId = user.Id,
                    };
                    confirmEmailAction = transaction.Repositories.UserActionKeys.Insert(confirmEmailAction);
                    result.ConfirmEmailActionKey = confirmEmailAction;

                    // send email
                    var emailModel = new NewUserConfirmEmail(request.Email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                    {
                        Recipient = user,
                        User = user,
                        ActionKey = confirmEmailAction,
                        ConfirmUrl = string.Format(
                            "{0}Account/Confirm/{1}?secret={2}",
                            Lang.T("AppDomain"),
                            confirmEmailAction.Id,
                            confirmEmailAction.Secret),
                    };
                    try
                    {
                        transaction.PostSaveActions.Add(t => t.Services.Email.SendNewUserConfirmEmail(emailModel));
                        result.EmailSent = true;
                    }
                    catch
                    {
                        result.EmailSent = false;
                    }
                }

                if (mustBeValidated)
                {
                    try
                    {
                        transaction.PostSaveActions.Add(t => t.Services.People.NotifyAdministratorsOfPendingUserRegistrations(user));
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal())
                            throw;
                        // do nothing because
                        // - we don't care whether the email succeeds or not
                        // - the email.send method does the logging
                    }
                }

                transaction.CompleteTransaction();

                result.Succeed = true;
                return this.LogResult(result, logPath, "Created user " + user);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                transactionDisposable.Dispose();
            }
        }

        public void NotifyAdministratorsOfPendingUserRegistrations(User user)
        {
            var pendingUsers = this.Services.People.GetPendingUserRegistrations(this.Services.NetworkId);
            var recipients = this.Services.People.GetByNetworkAccessLevel(NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.ValidatePendingUsers);

            foreach (var recipient in recipients)
            {
                var recipientContact = new Sparkle.Entities.Networks.Neutral.SimpleContact(recipient);
                var model = new PendingUserRegistrationsModel(recipientContact, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    PendingUsers = pendingUsers,
                    PendingUsersCount = pendingUsers.Count,
                    NewPendingUser = user,
                };
                this.Services.Email.SendPendingUserRegistrations(model);
            }
        }

        public SendActivationEmailResult SendActivationEmail(string email, string message = null)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("The value cannot be empty", "email");

            const string logPath = "PeopleService.SendActivationEmail";
            var result = new SendActivationEmailResult
            {
                Email = email,
            };

            var user = this.Services.People.SelectWithProMail(email);
            result.User = user;
            if (result.User == null)
            {
                result.Errors.Add(SendActivationEmailError.NoSuchUserByEmail, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Email=" + email);
            }

            if (result.User.IsEmailConfirmed)
            {
                result.Errors.Add(SendActivationEmailError.EmailAlreadyConfirmed, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "User=" + user);
            }

            var action = this.Services.UserActionKeys.GetLatestAction(result.User.Id, UserActionKey.UserEmailConfirmActionKey);
            result.ActionKey = action;

            if (action == null)
            {
                result.Errors.Add(SendActivationEmailError.NoEmailActivationAction, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "User=" + user);
            }

            if (action.RemainingUsages <= 0)
            {
                result.Errors.Add(SendActivationEmailError.EmailActionDoneTooManyTimes, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "User=" + user);
            }

            if (action.DateExpiresUtc.HasValue && action.DateExpiresUtc.Value > DateTime.UtcNow)
            {
                result.Errors.Add(SendActivationEmailError.EmailActivationActionExpired, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "User=" + user);
            }

            action.RemainingUsages--;
            this.Services.UserActionKeys.Update(action);

            // send email
            var emailModel = new NewUserConfirmEmail(user.Email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
            {
                Recipient = user,
                User = user,
                ActionKey = action,
                Message = message,
                ConfirmUrl = string.Format( // TODO: use this.Services.GetUrl()
                    "{0}Account/Confirm/{1}?secret={2}",
                    Lang.T("AppDomain"),
                    action.Id,
                    action.Secret),
            };
            try
            {
                this.Services.Email.SendNewUserConfirmEmail(emailModel);
                result.Succeed = true;
            }
            catch
            {
                result.Errors.Add(SendActivationEmailError.EmailProviderError, NetworksEnumMessages.ResourceManager);
            }

            return this.LogResult(result, logPath, "User=" + user);
        }

        public ConfirmEmailResult ConfirmEmail(int actionId, string secret)
        {
            var result = new ConfirmEmailResult
            {
                ActionId = actionId,
                Secret = secret,
            };

            const string logPath = "PeopleService.ConfirmEmail";
            UserActionKey action = result.Action = this.Services.UserActionKeys.GetById(actionId);
            if (action == null)
            {
                result.Errors.Add(ConfirmEmailError.NoSuchActionKey, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ActionId=" + actionId);
            }

            if (action.DateExpiresUtc.HasValue && action.DateExpiresUtc.Value > DateTime.UtcNow)
            {
                result.Errors.Add(ConfirmEmailError.EmailActivationActionExpired, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Action=" + action);
            }

            if (action.Secret != secret)
            {
                result.Errors.Add(ConfirmEmailError.InvalidSecret, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Action=" + action + " GivenSecret='" + secret + "'");
            }

            var user = result.User = this.Repo.People.GetById(action.UserId);

            if (user == null)
            {
                result.Errors.Add(ConfirmEmailError.NoSuchEmail, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Action=" + action);
            }

            if (user.IsEmailConfirmed)
            {
                result.Errors.Add(ConfirmEmailError.EmailAlreadyConfirmed, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Action=" + action);
            }

            if (user.NetworkAccessLevel < 0 || user.CompanyAccessLevel < 0)
            {
                result.Errors.Add(ConfirmEmailError.UserIsDisabled, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Action=" + action);
            }

            if (user.AccountClosed == true)
            {
                result.Errors.Add(ConfirmEmailError.AccountPendingValidation, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "Action=" + action);
            }

            if (user.GenderValue != NetworkUserGender.Male && user.GenderValue != NetworkUserGender.Female)
            {
                user.GenderValue = this.Services.People.GetDefaultGender();
            }

            user.IsEmailConfirmed = true;
            this.Services.People.Update(user);

            this.Services.Wall.HasJustJoined(user.Id);
            this.Services.Notifications.InitializeNotifications(user);

            action.RemainingUsages = 0;
            this.Services.UserActionKeys.Update(action);

            result.Succeed = true;

            return this.LogResult(result, logPath, "Action=" + action + "User=" + user);
        }

        public IList<User> GetPendingUserRegistrations(int networkId)
        {
            return this.Repo.People.Select()
                .ByNetwork(networkId)
                .Where(u => u.AccountClosed == true
                         && u.Company.IsEnabled
                         && u.NetworkAccessLevel > 0)
                .OrderByDescending(u => u.CreatedDateUtc)
                .ToList();
        }

        public Services.Authentication.MembershipUser GetMembershipUser(Guid guid)
        {
            var service = this.Services.MembershipService;
            var user = service.GetUser(guid);
            return user;
        }

        public int CountMustBeValidateUsersByCompany()
        {
            return this.Repo.People.CountMustBeValidateUsersByCompany(this.Services.NetworkId);
        }

        public string MakeUsernameFromName(string firstname, string lastname)
        {
            var baseUsername = firstname.ToLowerInvariant() + "." + lastname.ToLowerInvariant();
            baseUsername = baseUsername.RemoveDiacritics().RemoveSpaces().Replace("@", "").Replace("+", "").Replace("'", "");
            var username = baseUsername;
            var i = 1;
            while (!this.Services.People.IsLoginAvailable(username))
            {
                username = baseUsername + i++;
            }

            return username;
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetAllLite()
        {
            return this.Repo.People.GetAllLite(this.Services.NetworkId);
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetAllActiveLite()
        {
            return this.Repo.People.GetAllActiveLite(this.Services.NetworkId);
        }

        public IList<Services.Networks.Models.UserModel> GetExportableList()
        {
            return this.PeopleRepository.GetExportableListOfUsers(this.Services.NetworkId)
                .Select(u => new UserModel(u))
                .ToList();
        }

        public IList<User> GetLastRegistrants(short max)
        {
            ////var timelineItems = this.Services.Wall.GetLastRegistrants(max);
            ////var ids = timelineItems.Select(t => t.PostedByUserId).ToArray();
            ////var users = this.Repo.People.GetActiveById(ids, this.Services.NetworkId, PersonOptions.Company | PersonOptions.Job);
            ////return timelineItems
            ////    .Where(t => users.ContainsKey(t.PostedByUserId))
            ////    .Select(t => users[t.PostedByUserId])
            ////    .ToList();

            // the createdate is not set on users
            // so use the timelineitems as reference for a few more monthes
            // we should write a script to set the create date of users from membership/timlineitems
            var query = this.PeopleRepository
                .NewQuery(PersonOptions.Company | PersonOptions.Job)
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .OrderByDescending(o => o.CreatedDateUtc)
                .Take(max)
                .ToList();

            if (query.Count > max)
                return (List<User>)query.GetRange(0, max);
            return query;
        }

        public bool IsEmailAddressInUse(string value)
        {
            return this.Repo.People.IsEmailAddressInUse(value);
        }

        public bool IsEmailAddressInUse(string accountPart, string domainPart)
        {
            return this.Repo.People.IsEmailAddressInUse(accountPart, domainPart);
        }

        public IList<UserModel> GetPendingEmailAddressConfirmation()
        {
            return this.Repo.People.GetPendingEmailAddressConfirmation(this.Services.NetworkId)
                .Select(u => new UserModel(u))
                .ToList();
        }

        public IList<UserModel> GetMembershipLockedOutUsers()
        {
            var ids = this.Repo.People.GetMembershipLockedOutUserIds(this.Services.NetworkId);
            var items = this.Repo.People.GetLiteById(ids)
                .Select(u => new UserModel(u)
                {
                    IsLockedOut = true,
                })
                .ToList();

            return items;
        }

        public bool IsActive(User user)
        {
            if (user == null)
                return false;

            return this.IsActive(user.IsEmailConfirmed, user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, user.Company.IsEnabled);
        }

        public bool IsActive(UsersView user)
        {
            if (user == null)
                return false;

            return this.IsActive(user.IsEmailConfirmed, user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, user.Company_IsEnabled);
        }

        public bool IsActive(Sparkle.Entities.Networks.Neutral.UserPoco user)
        {
            if (user == null)
                return false;

            var company = user.Company;
            if (company == null)
            {
                company = this.Repo.Companies.Select()
                    .Where(c => c.ID == user.CompanyID)
                    .Select(c => new Sparkle.Entities.Networks.Neutral.CompanyPoco
                    {
                        ID = c.ID,
                        IsEnabled = c.IsEnabled,
                    })
                    .Single();
            }

            return this.IsActive(user.IsEmailConfirmed, user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, company.IsEnabled);
        }

        private bool IsActive(bool IsEmailConfirmed, bool? AccountClosed, int NetworkAccessLevel, int CompanyAccessLevel, bool isCompanyEnabled)
        {
            return UserModel.IsUserActive(AccountClosed, NetworkAccessLevel, CompanyAccessLevel, isCompanyEnabled, IsEmailConfirmed);
        }

        public bool IsAuthorized(User user)
        {
            if (user == null)
                return false;

            return this.IsAuthorized(user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, user.Company.IsEnabled);
        }

        public bool IsAuthorized(UsersView user)
        {
            if (user == null)
                return false;

            return this.IsAuthorized(user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, user.Company_IsEnabled);
        }

        private bool IsAuthorized(bool? AccountClosed, int NetworkAccessLevel, int CompanyAccessLevel, bool isCompanyEnabled)
        {
            return UserModel.IsUserAuthorized(AccountClosed, NetworkAccessLevel, CompanyAccessLevel, isCompanyEnabled);
        }

        public void LockMembershipAccount(string username)
        {
            this.Repo.Membership.LockMemberhipAccount("/", username, DateTime.UtcNow);
        }

        public SendPasswordRecoveryEmailResult SendPasswordRecoveryEmail(int userId, string message = null, bool sendEmail = true, string subject = null)
        {
            var result = new SendPasswordRecoveryEmailResult();
            var user = this.Repo.People.GetById(userId);
            if (user == null)
            {
                result.Errors.Add(SendPasswordRecoveryEmailError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            return this.SendPasswordRecoveryEmail(result, user, message, sendEmail, subject);
        }

        public SendPasswordRecoveryEmailResult SendPasswordRecoveryEmail(string email, string message = null, bool sendEmail = true, string subject = null)
        {
            var result = new SendPasswordRecoveryEmailResult();
            var user = this.Repo.People.GetByEmail(email, this.Services.NetworkId, PersonOptions.None);
            if (user == null)
            {
                result.Errors.Add(SendPasswordRecoveryEmailError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            return this.SendPasswordRecoveryEmail(result, user, message, sendEmail, subject);
        }

        public SendPasswordRecoveryEmailResult SendPasswordRecoveryEmailOnAutoRecover(string id, string message = null, bool sendEmail = true, string subject = null)
        {
            var result = new SendPasswordRecoveryEmailResult();
            User user = null;
            user = this.Repo.People.GetByEmail(id, this.Services.NetworkId, PersonOptions.None);
            if (user == null)
                user = this.Repo.People.GetByLogin(id, this.Services.NetworkId, PersonOptions.None);

            if (user == null)
            {
                result.Errors.Add(SendPasswordRecoveryEmailError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            return this.SendPasswordRecoveryEmail(result, user, message, sendEmail, subject);
        }

        private SendPasswordRecoveryEmailResult SendPasswordRecoveryEmail(SendPasswordRecoveryEmailResult result, User user, string message, bool sendEmail, string subject)
        {
            const string logPath = "PeopleService.SendPasswordRecoveryEmail";
            if (!this.IsAuthorized(user))
            {
                result.Errors.Add(SendPasswordRecoveryEmailError.UserIsNotAuthorized, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "User=" + user);
            }

            var mbsUser = this.GetMembershipUser(user.UserId);
            var activationLink = this.GetPasswordRecoveryUrl(user, mbsUser);

            if (sendEmail)
            {
                try
                {
                    this.Services.Email.SendRecovery(user, activationLink, message, subject, !mbsUser.IsLockedOut);
                    this.Services.Logger.Info("PeopleService.SendPasswordRecoveryEmail", ErrorLevel.Success, string.Format("Email sent to {0} for login {1}", user.Email, user.Login));
                    result.Succeed = true;
                }
                catch (InvalidOperationException ex)
                {
                    string msg = string.Format("Failed to send email to {0} for login {1} (InvalidOperationException)", user.Email, user.Login) + Environment.NewLine + ex.ToString();
                    this.Services.Logger.Error("PeopleService.SendPasswordRecoveryEmail", ErrorLevel.Internal, msg);
                    result.Errors.Add(SendPasswordRecoveryEmailError.EmailInternalError, NetworksEnumMessages.ResourceManager);
                }
                catch (SmtpException ex)
                {
                    string msg = string.Format("Failed to send email to {0} for login {1} (SmtpException)", user.Email, user.Login) + Environment.NewLine + ex.ToString();
                    this.Services.Logger.Error("PeopleService.SendPasswordRecoveryEmail", ErrorLevel.ThirdParty, msg);
                    result.Errors.Add(SendPasswordRecoveryEmailError.EmailProviderError, NetworksEnumMessages.ResourceManager);
                }
            }
            else
            {
                result.PasswordResetLink = new Uri(activationLink, UriKind.Absolute);
                result.Succeed = true;
                this.Services.Logger.Info("PeopleService.SendPasswordRecoveryEmail", ErrorLevel.Success, string.Format("{0} asked for a password recovery link for user {1}", this.Identity.ToString(), user.Login));
                return this.LogResult(result, logPath, "User=" + user);
            }

            return this.LogResult(result, logPath);
        }

        private string GetPasswordRecoveryUrl(int userId)
        {
            var item = this.Repo.People.GetActiveById(new int[] { userId, }, this.Services.NetworkId, PersonOptions.Company);
            if (item == null || item.Count == 0 || !this.IsActive(item[userId]))
                return null;

            var user = item[userId];
            var mbsUser = this.GetMembershipUser(user.UserId);

            return this.GetPasswordRecoveryUrl(user, mbsUser);
        }

        private string GetPasswordRecoveryUrl(User user, Services.Authentication.MembershipUser mbsUser)
        {
            var key = Sparkle.Services.Authentication.Keys.ComputeForAccount(user.UserId, mbsUser.LastLoginDate);
            return this.Services.GetUrl("Account/Recovery/" + Uri.EscapeDataString(user.Username) + "/" + Uri.EscapeDataString(key));

            //return (this.Services.AppConfiguration.Tree.Site.MainDomainName ?? Lang.T("AppDomain"))
            //    + "Account/Recovery/" + Uri.EscapeDataString(user.Username) + "/" + Uri.EscapeDataString(key);
        }

        public BasicResult<SetNetworkAccessLevelError> SetNetworkAccessLevel(int id, NetworkAccessLevel networkAccessLevel)
        {
            const string logPath = "PeopleService.SetNetworkAccessLevel";
            var result = new BasicResult<SetNetworkAccessLevelError>();

            // TODO: check current user is authorized to perform this action

            var user = this.Repo.People.GetById(id);
            if (user == null)
            {
                result.Errors.Add(SetNetworkAccessLevelError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "UserId=" + id);
            }

            if (!user.Company.IsEnabled)
            {
                result.Errors.Add(SetNetworkAccessLevelError.UserCompanyIsDisabled, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "User=" + user);
            }

            // evaluate user info
            var previous = user.NetworkAccess;
            var wasActive = this.IsActive(user);

            // change user info
            user.NetworkAccess = networkAccessLevel;
            this.Repo.People.Update(user);

            var isNowActive = this.IsActive(user);

            if (wasActive)
            {
                // user was active
                if (!isNowActive)
                {
                    // and is now disabled

                    // reset mailchimp status
                    var notifs = this.Repo.Notifications.GetById(id);
                    if (notifs != null && notifs.MailChimpStatus != null)
                    {
                        notifs.MailChimpStatus = null;
                        notifs.MailChimpStatusDateUtc = null;
                        this.Repo.Notifications.Update(notifs);
                    }
                }
            }
            else
            {
                // user was disabled
                if (isNowActive)
                {
                    // and is now enabled

                    // reset mailchimp status
                    var notifs = this.Repo.Notifications.GetById(id);
                    if (notifs != null && notifs.MailChimpStatus != null)
                    {
                        notifs.MailChimpStatus = notifs.MailChimp ? "resubscribe" : null;
                        notifs.MailChimpStatusDateUtc = null;
                        this.Repo.Notifications.Update(notifs);
                    }
                }
            }

            result.Succeed = true;
            return this.LogResult(result, logPath, "User " + user + " is now '" + networkAccessLevel + "'");
        }

        public IList<User> GetSubscribedToMainTimelineItems()
        {
            var notifs = this.Services.Notifications.GetSubscribedUsers(NotificationType.MainTimelineItems)
                .Select(o => o.Id)
                .ToList();

            var users = this.Repo.People
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithIds(notifs)
                .ToList();

            return users;
        }

        public IList<User> GetSubscribedToMainTimelineComments()
        {
            var notifs = this.Services.Notifications.GetSubscribedUsers(NotificationType.MainTimelineComments)
                .Select(o => o.Id)
                .ToList();

            var users = this.Repo.People
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithIds(notifs)
                .ToList();

            return users;
        }

        public IList<User> GetSubscribedToCompanyNetworkItems(int companyId)
        {
            var notifs = this.Services.Notifications.GetSubscribedUsers(NotificationType.CompanyTimelineItems)
                .Select(o => o.Id)
                .ToList();

            return this.Repo.People
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithCompanyId(companyId)
                .WithIds(notifs)
                .ToList();
        }

        public IList<User> GetSubscribedToCompanyNetworkComments(int companyId)
        {
            var notifs = this.Services.Notifications.GetSubscribedUsers(NotificationType.CompanyTimelineComments)
                .Select(o => o.Id)
                .ToList();

            return this.Repo.People
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .WithCompanyId(companyId)
                .WithIds(notifs)
                .ToList();
        }

        public bool IsLoginAvailable(string username)
        {
            return !this.Repo.People.UsernameExists(username);////&& !this.Repo.People.MembershipUsernameExists(username);
        }

        public CreateEmailPassordAccountRequest GetCreateEmailPassordAccountModel(CreateEmailPassordAccountRequest model, Guid? invitationCode)
        {
            if (model == null)
            {
                model = new CreateEmailPassordAccountRequest
                {
                    Gender = NetworkUserGender.Unspecified,
                };

                NetworkUserGender defaultGender;
                string defaultGenderString = this.Services.AppConfiguration.Tree.Features.Users.DefaultGender;
                if (defaultGenderString != null && Enum.TryParse(defaultGenderString, out defaultGender))
                {
                    model.Gender = defaultGender;
                }
            }
            else
            {
                if (invitationCode == null && model.InvitationCode != null)
                    invitationCode = model.InvitationCode;
            }

            model.AvailableJobs = this.Services.Job.GetAll();

            if (invitationCode != null)
            {
                model.InvitationCode = invitationCode;

                var invitation = this.Services.Invited.GetByInvitationKey(invitationCode.Value);
                model.Invitation = new RegisterInvitationModel(invitation);
                if (model.Invitation != null)
                {
                    model.IsEmailAddressReadOnly = true;
                    model.Email = model.Invitation.Email.OriginalString;
                }
            }

            return model;
        }

        public UserRolesModel GetUserRolesModel()
        {
            var model = new UserRolesModel();
            var usersTotal = this.Repo
                .People
                .GetAllActiveLite(this.Services.NetworkId)
                .Count();
            var users = this.Repo
                .People
                .GetAllUsersForRolesStats(this.Services.NetworkId);

            NetworkAccessLevel[] excluded =
            { 
                NetworkAccessLevel.Disabled, 
                NetworkAccessLevel.User, 
                NetworkAccessLevel.ChangeCompanyFlags, 
                NetworkAccessLevel.SparkleStaff,
                NetworkAccessLevel.All
            };
            var excludedRoles = excluded.ToList();

            if (!this.Services.AppConfiguration.Tree.Features.SparkleTV.IsEnabled)
            {
                excludedRoles.Add(NetworkAccessLevel.ReadDevices);
                excludedRoles.Add(NetworkAccessLevel.ManageDevices);
                excludedRoles.Add(NetworkAccessLevel.ValidatePublications);
            }

            if (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled)
            {
                excludedRoles.Add(NetworkAccessLevel.ManageSubscriptions);
            }

            if (!this.Services.AppConfiguration.Tree.Features.PartnerResources.IsEnabled)
            {
                excludedRoles.Add(NetworkAccessLevel.ManagePartnerResources);
            }

            var roles = new Dictionary<NetworkAccessLevel, UserRoleModel>();
            var visibleRoles = Enum.GetValues(typeof(NetworkAccessLevel)).Cast<NetworkAccessLevel>().Except(excludedRoles);
            foreach (var role in visibleRoles)
            {
                var resourceName = NetworksEnumMessages.ResourceManager.GetString("NetworkAccessLevel_" + role.ToString());
                var resourceDesc = NetworksEnumMessages.ResourceManager.GetString("NetworkAccessLevel_" + role.ToString() + "_Desc");
                var roleVal = Enum.Parse(typeof(NetworkAccessLevel), role.ToString(), false);
                var roleModel = new UserRoleModel(
                    resourceName,
                    resourceDesc,
                    users
                        .Where(o => ((NetworkAccessLevel)o.NetworkAccessLevel & role) == role)
                        .Select(o => new UserModel(o, null))
                        .ToList());

                roles.Add(role, roleModel);
            }

            model.UsersTotal = usersTotal;
            model.Roles = roles;
            return model;
        }

        public UserModel GetActiveById(int userId, PersonOptions options)
        {
            var item = this.Repo.People.GetActiveById(new int[] { userId, }, this.Services.NetworkId, options).Values.SingleOrDefault();
            if (item == null)
                return null;

            IList<UserProfileFieldModel> fields = null;
            if ((options & PersonOptions.ProfileFields) == PersonOptions.ProfileFields)
            {
                this.Services.ProfileFields.GetUserProfileFieldsByUserId(item.Id);
            }

            return new UserModel(item, fields);
        }

        public UserModel GetActiveByLogin(string username, PersonOptions options)
        {
            var item = this.Repo.People.GetByLogin(username, this.Services.NetworkId, options | PersonOptions.Company);
            if (item == null)
                return null;

            UserModel model;
            if ((options & PersonOptions.ProfileFields) == PersonOptions.ProfileFields)
            {
                var fields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(item.Id);
                model = new UserModel(item, fields);
            }
            else
            {
                model = new UserModel(item);
            }

            if (!model.IsActive)
                return null;
            return model;
        }

        public User GetEntityByIdInNetwork(int userId, PersonOptions options)
        {
            return this.PeopleRepository.NewQuery(options)
                .ByNetwork(this.Services.NetworkId)
                .WithId(userId)
                .SingleOrDefault();
        }

        public UserModel GetByIdFromAnyNetwork(int userId, PersonOptions options)
        {
            var item = this.GetEntityByIdFromAnyNetwork(userId, options);
            if (item == null)
                return null;

            if ((options & PersonOptions.ProfileFields) == PersonOptions.ProfileFields)
            {
                var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(item.Id);
                return item != null ? new UserModel(item, profileFields) : null;
            }
            else
            {
                return item != null ? new UserModel(item) : null;
            }
        }

        public UserRoleFormModel GetRolesFormModel(User user, int? currentUserId)
        {
            var model = new UserRoleFormModel();
            model.User = user;
            model.UserId = user.Id;

            var userAccess = user.NetworkAccess;
            if ((userAccess & NetworkAccessLevel.AddCompany) == NetworkAccessLevel.AddCompany)
                model.AddCompany = true;
            if ((userAccess & NetworkAccessLevel.ReadNetworkStatistics) == NetworkAccessLevel.ReadNetworkStatistics)
                model.ReadNetworkStatistics = true;
            if ((userAccess & NetworkAccessLevel.ReadDevices) == NetworkAccessLevel.ReadDevices)
                model.ReadDevices = true;
            if ((userAccess & NetworkAccessLevel.ManageDevices) == NetworkAccessLevel.ManageDevices)
                model.ManageDevices = true;
            if ((userAccess & NetworkAccessLevel.ManageInformationNotes) == NetworkAccessLevel.ManageInformationNotes)
                model.ManageInformationNotes = true;
            if ((userAccess & NetworkAccessLevel.ManageRegisterRequests) == NetworkAccessLevel.ManageRegisterRequests)
                model.ManageRegisterRequests = true;
            if ((userAccess & NetworkAccessLevel.ValidatePublications) == NetworkAccessLevel.ValidatePublications)
                model.ValidatePublications = true;
            if ((userAccess & NetworkAccessLevel.ManageCompany) == NetworkAccessLevel.ManageCompany)
                model.ManageCompany = true;
            if ((userAccess & NetworkAccessLevel.ValidatePendingUsers) == NetworkAccessLevel.ValidatePendingUsers)
                model.ValidatePendingUsers = true;
            if ((userAccess & NetworkAccessLevel.ModerateNetwork) == NetworkAccessLevel.ModerateNetwork)
                model.ModerateNetwork = true;
            if ((userAccess & NetworkAccessLevel.ManageClubs) == NetworkAccessLevel.ManageClubs)
                model.ManageClubs = true;
            if ((userAccess & NetworkAccessLevel.ManageSubscriptions) == NetworkAccessLevel.ManageSubscriptions)
                model.ManageSubscriptions = true;
            if ((userAccess & NetworkAccessLevel.ManagePartnerResources) == NetworkAccessLevel.ManagePartnerResources)
                model.ManagePartnerResources = true;
            if ((userAccess & NetworkAccessLevel.NetworkAdmin) == NetworkAccessLevel.NetworkAdmin)
                model.NetworkAdmin = true;
            if ((userAccess & NetworkAccessLevel.SparkleStaff) == NetworkAccessLevel.SparkleStaff)
                model.SparkleStaff = true;

            if (currentUserId.HasValue)
            {
                var current = this.SelectWithId(currentUserId.Value);
                if ((current.NetworkAccess & NetworkAccessLevel.SparkleStaff) == NetworkAccessLevel.SparkleStaff)
                    model.IsCurrentUserSparkleStaff = true;
            }

            return model;
        }

        public void UpdateUserRolesFromModel(UserRoleFormModel model)
        {
            NetworkAccessLevel newNetworkAccess = 0;

            if (model.AddCompany)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.AddCompany;
            if (model.ReadNetworkStatistics)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ReadNetworkStatistics;
            if (model.ReadDevices)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ReadDevices;
            if (model.ManageDevices)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManageDevices;
            if (model.ManageInformationNotes)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManageInformationNotes;
            if (model.ManageRegisterRequests)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManageRegisterRequests;
            if (model.ValidatePublications)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ValidatePublications;
            if (model.ManageCompany)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManageCompany;
            if (model.ValidatePendingUsers)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ValidatePendingUsers;
            if (model.ModerateNetwork)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ModerateNetwork;
            if (model.ManageClubs)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManageClubs;
            if (model.ManageSubscriptions)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManageSubscriptions;
            if (model.ManagePartnerResources)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.ManagePartnerResources;
            if (model.NetworkAdmin)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.NetworkAdmin;
            if (model.SparkleStaff)
                newNetworkAccess = newNetworkAccess | NetworkAccessLevel.SparkleStaff;

            if (newNetworkAccess == 0)
                newNetworkAccess = NetworkAccessLevel.User;

            model.User.NetworkAccess = newNetworkAccess;
            this.Update(model.User);
        }

        public User GetEntityByIdFromAnyNetwork(int userId, PersonOptions options)
        {
            return this.Repo.People.GetActiveById(userId, options);
        }

        public int[] KeepActiveUserIds(int[] userIds)
        {
            var users = this.Repo.People.GetUsersViewById(userIds, this.Services.NetworkId);
            return users.Values
                .Where(u => this.IsActive(u))
                .Select(u => u.Id)
                .ToArray();
        }

        public IDictionary<int, UserModel> GetModelByIdFromAnyNetwork(int[] ids, PersonOptions options)
        {
            if (options == PersonOptions.None)
            {
                var items = this.Repo.People.GetUsersViewById(ids).Values;
                return items
                    .Select(u =>
                    {
                        var model = new UserModel(u);
                        model.IsActive = this.IsActive(u);
                        return model;
                    })
                    .ToDictionary(u => u.Id, u => u);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string GetProfilePictureUrl(User user, UserProfilePictureSize pictureSize, UriKind uriKind)
        {
            return this.GetProfilePictureUrl(user.Login, user.Picture, pictureSize, uriKind);
        }

        public string GetProfilePictureUrl(UserModel user, UserProfilePictureSize pictureSize, UriKind uriKind)
        {
            return this.GetProfilePictureUrl(user.Login, user.Picture, pictureSize, uriKind);
        }

        public string GetProfilePictureUrl(string login, UserProfilePictureSize pictureSize, UriKind uriKind)
        {
            var user = this.Services.Cache.FindUsers(u => u.Username == login).Values.SingleOrDefault();
            if (user == null)
                return null;

            return this.GetProfilePictureUrl(user.Username, user.Picture, pictureSize, uriKind);
        }

        public string GetProfilePictureUrl(string login, string pictureName, UserProfilePictureSize pictureSize, UriKind uriKind)
        {
            var path = string.Format(
                "Data/PersonPicture/{0}/{1}/{2}",
                Uri.EscapeDataString(login),
                pictureSize.ToString(),
                this.GetProfilePictureLastChangeDate(login, pictureName).ToString(PictureAccess.CacheDateFormat));
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetProfileUrl(User user, UriKind uriKind)
        {
            var path = "Person/" + Uri.EscapeUriString(user.Login);
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetProfileUrl(UserModel user, UriKind uriKind)
        {
            var path = "Person/" + Uri.EscapeUriString(user.Login);
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public CultureInfo GetCulture(UserModel user)
        {
            return this.GetCulture(user != null ? user.Culture : null);
        }

        public CultureInfo GetCulture(User user)
        {
            return this.GetCulture(user != null ? user.Culture : null);
        }

        public CultureInfo GetCulture(User user, CultureInfo[] fallbackCultures)
        {
            return this.GetCulture(user != null ? user.Culture : null, fallbackCultures);
        }

        public CultureInfo GetCulture(Sparkle.Entities.Networks.Neutral.Person user)
        {
            return this.GetCulture(user != null ? user.Culture : null);
        }

        public CultureInfo GetCulture(Sparkle.Entities.Networks.Neutral.Person user, CultureInfo[] fallbackCultures)
        {
            return this.GetCulture(user != null ? user.Culture : null, fallbackCultures);
        }

        public CultureInfo GetCulture(string userCulture)
        {
            return this.GetCulture(userCulture, null);
        }

        public CultureInfo GetCulture(string userCulture, CultureInfo[] fallbackCultures)
        {
            CultureInfo culture = null;
            if (!string.IsNullOrEmpty(userCulture))
            {
                try
                {
                    var tempCulture = new CultureInfo(userCulture);
                    culture = this.GetSupportedCulture(tempCulture, false);
                }
                catch (CultureNotFoundException)
                {
                }
            }

            if (culture == null && fallbackCultures != null)
            {
                for (int i = 0; i < fallbackCultures.Length; i++)
                {
                    culture = this.GetSupportedCulture(fallbackCultures[i], false);
                    if (culture != null)
                        break;
                }
            }

            if (culture == null)
                culture = this.GetSupportedCulture(null, true);

            return culture;
        }

        public CultureInfo GetCulture(int userId)
        {
            var culture = this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(u => u.Id == userId)
                .Select(u => u.Culture)
                .SingleOrDefault();

            return this.GetCulture(culture);
        }

        private CultureInfo GetSupportedCulture(CultureInfo culture, bool fallback)
        {
            var supported = this.Services.SupportedCultures;

            if (culture != null)
            {
                if (supported.Contains(culture))
                    return culture;

                var parent = supported.FirstOrDefault(s => s.Parent != null && s.Parent == culture);
                if (parent != null)
                    return parent;
            }

            if (fallback)
                return supported.First();
            return null;
        }

        public TimeZoneInfo GetTimezone(UserModel user)
        {
            return this.GetTimezone(user != null ? user.Timezone : null);
        }

        public TimeZoneInfo GetTimezone(User user)
        {
            return this.GetTimezone(user != null ? user.Timezone : null);
        }

        public TimeZoneInfo GetTimezone(Sparkle.Entities.Networks.Neutral.Person user)
        {
            return this.GetTimezone(user != null ? user.Timezone : null);
        }

        public TimeZoneInfo GetTimezone(string userTimezone)
        {
            TimeZoneInfo timezone = null;
            if (!string.IsNullOrEmpty(userTimezone))
            {
                try
                {
                    timezone = TimeZoneInfo.FindSystemTimeZoneById(userTimezone);
                }
                catch (TimeZoneNotFoundException)
                {
                }
            }

            if (timezone == null)
                timezone = this.Services.DefaultTimezone;

            return timezone;
        }

        public ChangeUserCultureResult ChangeUserCulture(ChangeUserCultureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ChangeUserCultureResult(request);

            if (!request.UserId.HasValue)
            {
                result.Errors.Add(ChangeUserCultureError.MustBeAuthenticated, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var user = this.SelectWithId(request.UserId.Value);
            if (user == null || !this.IsActive(user))
            {
                result.Errors.Add(ChangeUserCultureError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            user.Culture = request.NewCulture.Name;
            this.Update(user);

            result.Succeed = true;
            return result;
        }

        public TimeZoneInfo GetTimezone(int userId)
        {
            var culture = this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(u => u.Id == userId)
                .Select(u => u.Timezone)
                .SingleOrDefault();

            return this.GetTimezone(culture);
        }

        public void UpdateFieldsFromLinkedInPerson(LinkedInPeopleResult result, Person person, bool saveFields)
        {
            int entityChanges = 0;

            // Birthday
            {
                if (person.DateOfBirth != null)
                {
                    DateTime? birthday = null;
                    var linBirthday = person.DateOfBirth;
                    if (linBirthday.Day.HasValue && linBirthday.Month.HasValue && linBirthday.Year.HasValue)
                    {
                        birthday = new DateTime((int)linBirthday.Year, (int)linBirthday.Month, (int)linBirthday.Day);
                        if (result.UserEntity.Birthday == null || birthday != result.UserEntity.Birthday.Value)
                        {
                            entityChanges++;
                            result.UserEntity.Birthday = birthday;
                        }
                    }
                }
            }

            // Region
            {
                var distantValue = person.Location != null ? person.Location.Name.Trim() : null;
                if (distantValue != null)
                {
                    if (distantValue.Contains(','))
                    {
                        distantValue = distantValue.Split(',')[0].NullIfEmptyOrWhitespace();
                    }

                    this.UpdateSingleField(result, result.UserEntity, distantValue, ProfileFieldType.City, ProfileFieldSource.LinkedIn, saveFields);
                }
            }

            // country
            {
                var distantValue = person.Location != null && person.Location.Country != null ? person.Location.Country.Code.Trim() : null;
                if (distantValue != null)
                {
                    try
                    {
                        var region = new RegionInfo(distantValue);
                        distantValue = region.ThreeLetterISORegionName;
                    }
                    catch (Exception ex)
                    {
                        distantValue = null;
                        this.Services.Logger.Info(
                            "PeopleServices.UpdateFromLinkedIn",
                            ErrorLevel.ThirdParty,
                            "RegionInfo threw an exception: {0}",
                            ex.ToString());
                    }

                    this.UpdateSingleField(result, result.UserEntity, distantValue, ProfileFieldType.Country, ProfileFieldSource.LinkedIn, saveFields);
                }
            }

            // job
            {
                var personPosition = person.Positions != null
                    ? person.Positions.Position != null
                    ? person.Positions.Position.Where(o => o.IsCurrent).FirstOrDefault()
                    : null
                    : null;
                string jobLibelle = null;
                int jobId = 0;
                if (personPosition != null && personPosition.Title != null)
                {
                    var liJobNameAscii = personPosition.Title.RemoveDiacritics().ToLowerInvariant();

#warning Performance issue: using C# instead of SQL to filter in a SQL table
                    // TODO: even if there aren't many jobs, it would be better to ask SQL directly.
                    var allJobs = this.Services.Job.GetAll();
                    var match = allJobs.Where(o => o.Name.ToLowerInvariant().RemoveDiacritics() == liJobNameAscii).SingleOrDefault(); // perfect match !
                    if (match != null)
                    {
                        jobId = match.Id;
                        jobLibelle = match.Name;
                    }
                    else
                    {
                        // try to find a matching job in the database by comparing word-per-word
                        // if we find a job with all the same words (any order), we consider this is a match
                        foreach (var job in allJobs)
                        {
                            var splitJob = job.Name
                                .Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(o => o.Length > 3)
                                .Select(o => o.RemoveDiacritics().ToLowerInvariant())
                                .ToArray();

                            var splitPosition = liJobNameAscii
                                .Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(o => o.Length > 3)
                                .ToArray();

                            int wordMatch = 0;
                            foreach (var jobWord in splitJob)
                            {
                                if (splitPosition.Contains(jobWord))
                                    wordMatch++;
                            }

                            if (wordMatch == splitPosition.Length)
                            {
                                jobId = job.Id;
                                jobLibelle = job.Name;
                                break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(jobLibelle))
                {
                    result.UserEntity.JobId = jobId;
                    entityChanges++;
                }
                else
                {
                    if (personPosition != null)
                    {
                        result.JobTitleToCreate = personPosition.Title;
                    }
                }
            }

            // industry
            if (!string.IsNullOrEmpty(person.Industry))
            {
                string distantValue = null;
                var availableValue = this.Services.ProfileFields.GetProfileFieldsAvailiableValueByValue(ProfileFieldType.Industry, person.Industry);
                if (availableValue != null)
                {
                    distantValue = availableValue.Value;
                }

                this.UpdateSingleField(result, result.UserEntity, distantValue, ProfileFieldType.Industry, ProfileFieldSource.LinkedIn, saveFields);
            }

            // Phone numbers (first)
            if (person.PhoneNumbers != null && person.PhoneNumbers.PhoneNumber != null)
            {
                string distantValue = null;
                var linPhone = person.PhoneNumbers.PhoneNumber.FirstOrDefault();
                if (linPhone != null)
                    distantValue = linPhone.Number;

                this.UpdateSingleField(result, result.UserEntity, distantValue, ProfileFieldType.Phone, ProfileFieldSource.LinkedIn, saveFields);
            }

            // skills
            {
                var linkedInSkills = person.Skills != null && person.Skills.Skill != null && person.Skills.Skill.Count > 0 ?
                    person.Skills.Skill.Where(o => o.SkillName != null).Select(o => o.SkillName.Name).ToArray() : null;
                if (linkedInSkills != null)
                {
                    if (saveFields)
                    {
                        var userSkillsIds = this.Services.PeoplesSkills.GetSkillsIdsByUserId(result.UserEntity.Id);
                        var skills = this.Services.Skills.GetByNames(linkedInSkills);
                        // TODO: there might be a lot of skills, it would be better to ask SQL directly.
                        // TODO: implement tags V2 here
                        foreach (var item in linkedInSkills)
                        {
                            var name = item.RemoveDiacritics().ToLowerInvariant();
                            if (skills.ContainsKey(name))
                            {
                                if (!userSkillsIds.Contains(skills[name]))
                                {
                                    this.Services.PeoplesSkills.Insert(new UserSkill
                                    {
                                        SkillId = skills[name],
                                        Date = DateTime.UtcNow,
                                        UserId = result.UserEntity.Id,
                                    });
                                }
                            }
                            else
                            {
                                if (item.Length <= 50)
                                {
                                    var added = this.Services.Skills.Insert(new Sparkle.Entities.Networks.Skill
                                    {
                                        ParentId = 0,
                                        TagName = item,
                                        Date = DateTime.UtcNow,
                                        CreatedByUserId = result.UserEntity.Id,
                                    });
                                    this.Services.PeoplesSkills.Insert(new UserSkill
                                    {
                                        SkillId = added.Id,
                                        Date = DateTime.UtcNow,
                                        UserId = result.UserEntity.Id,
                                    });
                                }
                                else
                                {
                                    // do nothing
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in linkedInSkills)
                        {
                            result.TagChanges.Add(new TagModel
                            {
                                Name = item,
                                Type = TagModelType.Skill,
                            });
                        }
                    }
                }
            }

            // interests
            {
                var linkedInInterests = !string.IsNullOrEmpty(person.Interests) ? person.Interests.Contains(',') ?
                    person.Interests.Split(new char[] { ',', }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray() : new string[] { person.Interests.Trim(), } : null;
                if (linkedInInterests != null)
                {
                    if (saveFields)
                    {
                        var userInterestsIds = this.Services.PeoplesInterests.GetInterestsIdsByUserId(result.UserEntity.Id);
                        var interests = this.Services.Interests.GetByNames(linkedInInterests);
                        foreach (var item in linkedInInterests)
                        {
                            var name = item.RemoveDiacritics().ToLowerInvariant();
                            if (interests.ContainsKey(name))
                            {
                                if (!userInterestsIds.Contains(interests[name]))
                                {
                                    this.Services.PeoplesInterests.InsertPeoplesInterest(new UserInterest
                                    {
                                        InterestId = interests[name],
                                        Date = DateTime.UtcNow,
                                        UserId = result.UserEntity.Id,
                                    });
                                }
                            }
                            else
                            {
                                if (item.Length <= 50)
                                {
                                    var added = this.Services.Interests.Insert(new Sparkle.Entities.Networks.Interest
                                    {
                                        ParentId = 0,
                                        TagName = item,
                                        Date = DateTime.UtcNow,
                                        CreatedByUserId = result.UserEntity.Id,
                                    });
                                    this.Services.PeoplesInterests.InsertPeoplesInterest(new UserInterest
                                    {
                                        InterestId = added.Id,
                                        Date = DateTime.UtcNow,
                                        UserId = result.UserEntity.Id,
                                    });
                                }
                                else
                                {
                                    // do nothing
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in linkedInInterests)
                        {
                            result.TagChanges.Add(new TagModel
                            {
                                Name = item,
                                Type = TagModelType.Interest,
                            });
                        }
                    }
                }
            }

            // languages => RO PF* Language
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Language);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.Languages != null && person.Languages.Language != null ? person.Languages.Language : Enumerable.Empty<Language>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    LanguageProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.LanguageModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.LanguageModel;
                        modelItem.UpdateFrom(linkedItem);
                        localItem.Model.LanguageModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new LanguageProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new UserProfileFieldModel(ProfileFieldType.Language);
                        model.LanguageModel = modelItem;
                        if (model.LanguageModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Language,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Language)
                    {
                        if (localItem.Model.LanguageModel.LinkedInId == null || !linkedItems.Any(i => i.Id == localItem.Model.LanguageModel.LinkedInId))
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // positions => RO PF* Position
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Position);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.Positions != null && person.Positions.Position != null ? person.Positions.Position : Enumerable.Empty<PersonPosition>();
                if (linkedItems != null && linkedItems.Count() == 69)
                {
                    // w00t
                }

                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    PositionProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.PositionModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.PositionModel;
                        modelItem.UpdateFrom(linkedItem);
                        localItem.Model.PositionModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new PositionProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new UserProfileFieldModel(ProfileFieldType.Position);
                        model.PositionModel = modelItem;
                        if (model.PositionModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Position,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Position)
                    {
                        if (localItem.Model.PositionModel.LinkedInId == null || !linkedItems.Where(i => i.Id == localItem.Model.PositionModel.LinkedInId).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // educations => RO PF* Education
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Education);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.Educations != null && person.Educations.Education != null ? person.Educations.Education : Enumerable.Empty<Education>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    PositionProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.EducationModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.EducationModel;
                        modelItem.UpdateFrom(linkedItem);
                        localItem.Model.EducationModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new PositionProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new UserProfileFieldModel(ProfileFieldType.Education);
                        model.EducationModel = modelItem;
                        if (model.EducationModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Education,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Education)
                    {
                        if (localItem.Model.EducationModel.LinkedInId == null || !linkedItems.Where(i => i.Id == localItem.Model.EducationModel.LinkedInId).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // twitter-accounts => RO PF* Twitter
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Twitter);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => uf);
                var linkedItems = person.TwitterAccounts != null && person.TwitterAccounts.TwitterAccount != null ? person.TwitterAccounts.TwitterAccount : Enumerable.Empty<TwitterAccount>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    var localItem = actualModels.Values.Where(m => m.Value == linkedItem.ProviderAccountName).SingleOrDefault();
                    if (localItem != null)
                    {
                        // OKAY
                    }
                    else
                    {
                        // insert
                        entity = new UserProfileField
                        {
                            DateCreatedUtc = DateTime.UtcNow,
                            ProfileFieldType = ProfileFieldType.Twitter,
                            SourceType = ProfileFieldSource.LinkedIn,
                            UserId = result.UserEntity.Id,
                            Value = linkedItem.ProviderAccountName,
                        };
                        if (saveFields)
                            this.Repo.UserProfileFields.Insert(entity);
                        result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.SourceType == ProfileFieldSource.LinkedIn && localItem.ProfileFieldType == ProfileFieldType.Twitter)
                    {
                        if (!linkedItems.Where(i => i.ProviderAccountName == localItem.Value).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(localItem), ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // im-accounts => RO PF* GTalk|Msn|Skype|Yahoo
            {
                var fieldTypes = new Dictionary<string, ProfileFieldType>
                {
                    {"gtalk",   ProfileFieldType.GTalk},
                    {"msn",     ProfileFieldType.Msn},
                    {"skype",   ProfileFieldType.Skype},
                    {"yahoo",   ProfileFieldType.Yahoo},
                };
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, fieldTypes.Select(o => o.Value).ToArray());
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => uf);
                var linkedItems = person.ImAccounts != null && person.ImAccounts.ImAccount != null ? person.ImAccounts.ImAccount : Enumerable.Empty<ImAccount>();
                foreach (var linkedItem in linkedItems)
                {
                    if (fieldTypes.ContainsKey(linkedItem.ImAccountType))
                    {
                        UserProfileField entity;
                        var localItem = actualModels.Values.Where(m => m.Value == linkedItem.ImAccountName && fieldTypes[linkedItem.ImAccountType] == m.ProfileFieldType).SingleOrDefault();
                        if (localItem != null)
                        {
                            // OKAY
                        }
                        else
                        {
                            // insert
                            entity = new UserProfileField
                            {
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = fieldTypes[linkedItem.ImAccountType],
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = linkedItem.ImAccountName,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.SourceType == ProfileFieldSource.LinkedIn && fieldTypes.Select(o => o.Value).Contains(localItem.ProfileFieldType))
                    {
                        if (!linkedItems.Where(i => i.ImAccountName == localItem.Value && fieldTypes[i.ImAccountType] == localItem.ProfileFieldType).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(localItem), ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // volunteer => RO PF* Volunteer
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Volunteer);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.Volunteer != null && person.Volunteer.VolunteerExperiences != null && person.Volunteer.VolunteerExperiences.VolunteerExperience != null ?
                    person.Volunteer.VolunteerExperiences.VolunteerExperience : Enumerable.Empty<VolunteerExperience>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    PositionProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.VolunteerModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.VolunteerModel;
                        modelItem.UpdateFrom(linkedItem);
                        localItem.Model.VolunteerModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new PositionProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new UserProfileFieldModel(ProfileFieldType.Volunteer);
                        model.VolunteerModel = modelItem;
                        if (model.VolunteerModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Volunteer,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Volunteer)
                    {
                        if (localItem.Model.VolunteerModel.LinkedInId == null || !linkedItems.Where(i => i.Id == localItem.Model.VolunteerModel.LinkedInId).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // certifications => RO PF* Certification
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Certification);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.Certifications != null && person.Certifications.Certification != null ? person.Certifications.Certification : Enumerable.Empty<Certification>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    PositionProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.CertificationModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.CertificationModel;
                        modelItem.UpdateFrom(linkedItem);
                        localItem.Model.CertificationModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new PositionProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new UserProfileFieldModel(ProfileFieldType.Certification);
                        model.CertificationModel = modelItem;
                        if (model.CertificationModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Certification,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Certification)
                    {
                        if (localItem.Model.CertificationModel.LinkedInId == null || !linkedItems.Where(i => i.Id == localItem.Model.CertificationModel.LinkedInId).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // patents => RO PF* Patent
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Patent);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.Patents != null && person.Patents.Patent != null ? person.Patents.Patent : Enumerable.Empty<Patent>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    PatentProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.PatentModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.PatentModel;
                        modelItem.UpdateFrom(linkedItem);
                        localItem.Model.PatentModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new PatentProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new UserProfileFieldModel(ProfileFieldType.Patent);
                        model.PatentModel = modelItem;
                        if (model.PatentModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Patent,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Patent)
                    {
                        if (localItem.Model.PatentModel.LinkedInId == null || !linkedItems.Where(i => i.Id == localItem.Model.PatentModel.LinkedInId).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // recommendations => RO PF* Recommendation
            {
                var actualItems = this.Repo.UserProfileFields.GetManyByUserIdAndFieldType(result.UserEntity.Id, ProfileFieldType.Recommendation);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new UserProfileFieldModel(uf),
                    });
                var linkedItems = person.RecommendationsReceived != null && person.RecommendationsReceived.Recommendation != null ? person.RecommendationsReceived.Recommendation : Enumerable.Empty<Recommendation>();
                foreach (var linkedItem in linkedItems)
                {
                    UserProfileField entity;
                    RecommendationProfileFieldModel modelItem;

                    // get byte[] from linkedItem.Recommender.PictureUrl
                    string picture64 = null;
                    if (linkedItem.Recommender != null && !string.IsNullOrEmpty(linkedItem.Recommender.PictureUrl))
                    {
                        var pictureResult = this.GetPictureFromUrl(new GetPictureFromUrlRequest { PictureUrl = linkedItem.Recommender.PictureUrl, MakeResize = false, });
                        if (pictureResult.Succeed && pictureResult.OriginalPicture != null)
                        {
                            pictureResult.OriginalPicture.Seek(0, SeekOrigin.Begin);
                            picture64 = Convert.ToBase64String(pictureResult.OriginalPicture.ToArray(), Base64FormattingOptions.None);
                        }
                    }

                    var localItem = actualModels.Values.Where(m => m.Model.RecommendationModel.LinkedInId == linkedItem.Id).SingleOrDefault();
                    if (localItem != null)
                    {
                        // update
                        var data = localItem.Entity.Data;
                        modelItem = localItem.Model.RecommendationModel;
                        modelItem.UpdateFrom(linkedItem);
                        if (!string.IsNullOrEmpty(picture64))
                            modelItem.RecommenderPicture = picture64;
                        localItem.Model.RecommendationModel = modelItem;
                        entity = localItem.Entity;
                        if (localItem.Model.Data != data)
                        {
                            // data changed
                            entity.Value = localItem.Model.Value;
                            entity.Data = localItem.Model.Data;
                            entity.DateUpdatedUtc = DateTime.UtcNow;
                            entity.UpdateCount++;
                            if (saveFields)
                                this.Repo.UserProfileFields.Update(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Update));
                        }
                    }
                    else
                    {
                        // insert
                        modelItem = new RecommendationProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        if (!string.IsNullOrEmpty(picture64))
                            modelItem.RecommenderPicture = picture64;
                        var model = new UserProfileFieldModel(ProfileFieldType.Recommendation);
                        model.RecommendationModel = modelItem;
                        if (model.RecommendationModel != null && model.Value != null)
                        {
                            entity = new UserProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Recommendation,
                                SourceType = ProfileFieldSource.LinkedIn,
                                UserId = result.UserEntity.Id,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.UserProfileFields.Insert(entity);
                            result.Changes.Add(new ProfileFieldUpdate(new UserProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Recommendation)
                    {
                        if (localItem.Model.RecommendationModel.LinkedInId == null || !linkedItems.Where(i => i.Id == localItem.Model.RecommendationModel.LinkedInId).Any())
                        {
                            if (saveFields)
                                this.Repo.UserProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new ProfileFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // linkedin unique identifier
            if (!string.IsNullOrEmpty(person.Id) && (!saveFields || result.UserEntity.LinkedInUserId != person.Id) && this.IsLinkedInIdAvailable(person.Id))
            {
                result.UserEntity.LinkedInUserId = person.Id;
                result.UserLinkedInId = true;
                entityChanges++;
            }

            // firstname
            if (!string.IsNullOrEmpty(person.Firstname) && (!saveFields || result.UserEntity.FirstName != person.Firstname))
            {
                result.UserEntity.FirstName = person.Firstname.CapitalizeWords();
                entityChanges++;
            }

            // lastname
            if (!string.IsNullOrEmpty(person.Lastname) && (!saveFields || result.UserEntity.LastName != person.Lastname))
            {
                result.UserEntity.LastName = person.Lastname.CapitalizeWords();
                entityChanges++;
            }

            // personnal email address
            if (!string.IsNullOrEmpty(person.EmailAddress) && (!saveFields || result.UserEntity.PersonalEmail != person.EmailAddress))
            {
                result.UserEntity.PersonalEmail = person.EmailAddress;
                entityChanges++;
            }

            // summary => About
            var summary = person.Summary;
            if (string.IsNullOrEmpty(summary) && person.Positions != null && person.Positions.Position != null && person.Positions.Position.Count > 0)
            {
                var current = person.Positions.Position.Where(o => o.IsCurrent).FirstOrDefault();
                if (current == null)
                    current = person.Positions.Position.FirstOrDefault();

                if (current != null)
                    summary = current.Summary;
            }
            if (!string.IsNullOrEmpty(summary) && (!saveFields || result.UserEntity.UserProfileFields.About() != summary))
            {
                this.UpdateSingleField(result, result.UserEntity, summary.TrimTextRight(4000), ProfileFieldType.About, ProfileFieldSource.LinkedIn, saveFields);
            }

            // headline => CurrentTarget
            if (!string.IsNullOrEmpty(person.Headline) && (!saveFields || result.UserEntity.UserProfileFields.CurrentTarget() != person.Headline))
            {
                this.UpdateSingleField(result, result.UserEntity, person.Headline.TrimTextRight(4000), ProfileFieldType.CurrentTarget, ProfileFieldSource.LinkedIn, saveFields);
            }

            // proposal-comments => ContactGuideline
            if (!string.IsNullOrEmpty(person.ProposalComments) && (!saveFields || result.UserEntity.UserProfileFields.ContactGuideline() != person.ProposalComments))
            {
                this.UpdateSingleField(result, result.UserEntity, person.ProposalComments, ProfileFieldType.ContactGuideline, ProfileFieldSource.LinkedIn, saveFields);
            }

            // linkedin public url
            if (!string.IsNullOrEmpty(person.PublicProfileUrl) && (!saveFields || result.UserEntity.UserProfileFields.LinkedInPublicUrl() != person.PublicProfileUrl))
            {
                this.UpdateSingleField(result, result.UserEntity, person.PublicProfileUrl, ProfileFieldType.LinkedInPublicUrl, ProfileFieldSource.LinkedIn, saveFields);
            }

            result.EntityChanges = entityChanges;

            if (saveFields && entityChanges > 0)
            {
                result.UserEntity.PersonalDataUpdateDateUtc = DateTime.UtcNow;
                this.Services.People.Update(result.UserEntity);
            }
        }

        public bool IsLinkedInIdAvailable(string linkedInId)
        {
            return this.Repo.People.IsLinkedInIdAvailable(linkedInId);
        }

        public LinkedInPeopleResult UpdateFromLinkedIn(LinkedInPeopleRequest request)
        {
            // Check LinkedIn is configured
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new LinkedInPeopleResult(request);

            var user = this.SelectWithId(request.UserId);
            if (user == null || user.NetworkId != this.Services.NetworkId)
            {
                result.Errors.Add(LinkedInPeopleError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (!this.IsAuthorized(user))
            {
                result.Errors.Add(LinkedInPeopleError.UnauthorizedUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (!this.Services.AppConfiguration.Tree.Externals.LinkedIn.AllowProfileUpdate)
            {
                result.Errors.AddDetail(LinkedInPeopleError.LinkedInUpdateIsDisabled, "Configuration does not allow profile update. ", NetworksEnumMessages.ResourceManager);
                return result;
            }

            var linkedInState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.LinkedIn);
            if (linkedInState == null || linkedInState.Entity == null ||
                string.IsNullOrEmpty(linkedInState.Entity.OAuthAccessToken) ||
                string.IsNullOrEmpty(linkedInState.Entity.OAuthAccessSecret))
            {
                result.Errors.Add(LinkedInPeopleError.LinkedInNotConfigured, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var userConnection = this.Services.SocialNetworkConnections.GetByUserIdAndConnectionType(user.Id, SocialNetworkConnectionType.LinkedIn);
            if (userConnection == null || string.IsNullOrEmpty(userConnection.OAuthToken))
            {
                result.Errors.Add(LinkedInPeopleError.LinkedInAccountNotLinked, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // API call
            var apiConfig = new LinkedInApiConfiguration(linkedInState.Entity.OAuthAccessToken, linkedInState.Entity.OAuthAccessSecret);
            var linkedInApi = new LinkedInApi(apiConfig);

            var cultures = new List<CultureInfo>();
            cultures.Add(this.GetCulture(user));
            cultures.AddRange(this.Services.SupportedCultures);

            Person linkedInPerson;
            try
            {
                var userAuth = new UserAuthorization(userConnection.OAuthToken);

                linkedInPerson = linkedInApi.Profiles.GetMyProfile(userAuth, cultures.Select(o => o.Name).ToArray(), personFields);
                if (linkedInPerson.RecommendationsReceived != null && linkedInPerson.RecommendationsReceived.Recommendation != null)
                {
                    foreach (var item in linkedInPerson.RecommendationsReceived.Recommendation)
                    {
                        if (item.Recommender != null)
                        {
                            try
                            {
                                var picture = linkedInApi.Profiles.GetProfilePicture(userAuth, item.Recommender.Id, 100, 100);
                                string firstPicture;
                                if (picture != null && picture.PictureUrl != null && !string.IsNullOrEmpty((firstPicture = picture.PictureUrl.First())))
                                    item.Recommender.PictureUrl = firstPicture;
                            }
                            catch (LinkedInApiException ex)
                            {
                                this.Services.Logger.Error(
                                    "PeopleService.UpdateFromLinkedIn",
                                    ErrorLevel.ThirdParty,
                                    "ProfilePicture Api call failed with the exception: {0}",
                                    ex.ToString());
                            }
                        }
                    }
                }
            }
            catch (LinkedInApiException ex)
            {
                this.Services.Logger.Error(
                    "PeopleServices.UpdateFromLinkedIn",
                    ErrorLevel.ThirdParty,
                    "Api call failed with the exception: {0}",
                    ex.ToString());

                if (ex.Data["ShouldRenewToken"] != null)
                {
                    this.Services.SocialNetworkConnections.ClearToken(request.UserId, SocialNetworkConnectionType.LinkedIn);
                    result.Errors.Add(LinkedInPeopleError.InvalidApiToken, NetworksEnumMessages.ResourceManager);
                    return result;
                }
                else
                {
                    result.Errors.Add(LinkedInPeopleError.ApiCallFailed, NetworksEnumMessages.ResourceManager);
                    return result;
                }
            }

            result.UserEntity = user;
            this.UpdateFieldsFromLinkedInPerson(result, linkedInPerson, true);

            result.Succeed = true;
            return result;
        }

        private LinkedInPeopleResult GetLinkedInResultNoSaving(LinkedInPeopleRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleService.GetLinkedInResultNoSaving";
            var result = new LinkedInPeopleResult(request);

            if (!this.Services.AppConfiguration.Tree.Externals.LinkedIn.AllowProfileUpdate)
            {
                result.Errors.AddDetail(LinkedInPeopleError.LinkedInUpdateIsDisabled, "Configuration does not allow profile update. ", NetworksEnumMessages.ResourceManager);
                return result;
            }

            var linkedInState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.LinkedIn);
            if (linkedInState == null || linkedInState.Entity == null ||
                string.IsNullOrEmpty(linkedInState.Entity.OAuthAccessToken) ||
                string.IsNullOrEmpty(linkedInState.Entity.OAuthAccessSecret))
            {
                result.Errors.Add(LinkedInPeopleError.LinkedInNotConfigured, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "UserId=" + request.UserId);
            }

            // API call
            var apiConfig = new LinkedInApiConfiguration(linkedInState.Entity.OAuthAccessToken, linkedInState.Entity.OAuthAccessSecret);
            var linkedInApi = new LinkedInApi(apiConfig);

            var cultures = new List<CultureInfo>();
            cultures.AddRange(this.Services.SupportedCultures);

            Person linkedInPerson;
            LinkedInNET.Profiles.PictureUrls pictures;
            IList<LinkedInNET.Companies.Company> linkedInCompanies = new List<LinkedInNET.Companies.Company>();
            var userAuth = new UserAuthorization(request.AccessToken);
            try
            {
                linkedInPerson = linkedInApi.Profiles.GetMyProfile(userAuth, cultures.Select(o => o.Name).ToArray(), personFields);
                if (this.Services.AppConfiguration.Tree.Features.EnableCompanies
                    && linkedInPerson.Positions != null
                    && linkedInPerson.Positions.Position != null)
                {
                    foreach (var item in linkedInPerson.Positions.Position.Where(o => o.EndDate == null || (o.EndDate != null && o.EndDate.Year.HasValue && !(o.EndDate.Year.Value < DateTime.UtcNow.Year))))
                    {
                        if (item.Company != null && item.Company.Id != 0)
                        {
                            LinkedInNET.Companies.Company company;
                            try
                            {
                                company = linkedInApi.Companies.GetById(userAuth, item.Company.Id.ToString(), companyFields);
                                linkedInCompanies.Add(company);
                            }
                            catch (LinkedInApiException ex)
                            {
                                this.Services.Logger.Error(
                                    "PeopleService.GetLinkedInResultNoSaving",
                                    ErrorLevel.ThirdParty,
                                    "Company API call failed with the exception: {0}",
                                    ex.ToString());
                            }
                        }
                    }
                }

                if (linkedInPerson.RecommendationsReceived != null && linkedInPerson.RecommendationsReceived.Recommendation != null)
                {
                    foreach (var item in linkedInPerson.RecommendationsReceived.Recommendation)
                    {
                        if (item.Recommender != null)
                        {
                            try
                            {
                                var picture = linkedInApi.Profiles.GetProfilePicture(userAuth, item.Recommender.Id, 100, 100);
                                string firstPicture;
                                if (picture != null && picture.PictureUrl != null && !string.IsNullOrEmpty((firstPicture = picture.PictureUrl.First())))
                                    item.Recommender.PictureUrl = firstPicture;
                            }
                            catch (LinkedInApiException ex)
                            {
                                this.Services.Logger.Error(
                                    "PeopleService.UpdateFromLinkedIn",
                                    ErrorLevel.ThirdParty,
                                    "ProfilePicture API call failed with the exception: {0}",
                                    ex.ToString());
                            }
                        }
                    }
                }

                pictures = linkedInApi.Profiles.GetOriginalProfilePicture(userAuth);
            }
            catch (LinkedInApiException ex)
            {
                this.Services.Logger.Error(
                    "PeopleServices.GetLinkedInResultNoSaving",
                    ErrorLevel.ThirdParty,
                    "API call failed with the exception: {0}",
                    ex.ToString());

                if (ex.Data["ShouldRenewToken"] != null)
                {
                    result.Errors.Add(LinkedInPeopleError.InvalidApiToken, NetworksEnumMessages.ResourceManager);
                    return result;
                }
                else
                {
                    result.Errors.Add(LinkedInPeopleError.ApiCallFailed, NetworksEnumMessages.ResourceManager);
                    return result;
                }
            }

            result.UserEntity = new User();
            this.UpdateFieldsFromLinkedInPerson(result, linkedInPerson, false);

            if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
            {
                foreach (var company in linkedInCompanies)
                {
                    var companyResult = new LinkedInCompanyResult(new LinkedInCompanyRequest());
                    companyResult.CompanyEntity = new Sparkle.Entities.Networks.Company();

                    this.Services.Company.UpdateFieldsFromLinkedInCompany(companyResult, company, false);
                    if (company.EmailDomains != null && company.EmailDomains.EmailDomain != null && company.EmailDomains.EmailDomain.Count > 0)
                    {
                        companyResult.EmailDomains = company.EmailDomains.EmailDomain.ToArray();
                    }

                    result.Companies.Add(companyResult);
                }
            }

            if (pictures != null && pictures.PictureUrl != null && pictures.PictureUrl.Count > 0)
            {
                var pictureUrl = pictures.PictureUrl.FirstOrDefault();
                var pictureResult = this.GetPictureFromUrl(new GetPictureFromUrlRequest { PictureUrl = pictureUrl, });
                if (pictureResult.Succeed)
                {
                    result.PicturesStream = pictureResult;
                }
                else
                {
                    result.Errors.Add(LinkedInPeopleError.CannotRetrieveProfilePicture, NetworksEnumMessages.ResourceManager);
                }
            }

            result.Succeed = true;
            return this.LogResult(result, logPath, "UserId=" + request.UserId);
        }

        private void SavePictureForApplyRequest(ApplyRequestModel model, MemoryStream pictureStream, PictureFormat format = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (pictureStream == null)
                throw new ArgumentNullException("pictureStream");
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory))
                throw new InvalidOperationException("Configuration entry 'Storage.UserContentsDirectory' must have a valid value");

            IFilesystemProvider provider = new IOFilesystemProvider();
            var key = model.Key;
            var picturePath = provider.EnsureFilePath(
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                this.Services.Network.Name,
                "ApplyRequests",
                key.ToString(),
                key.ToString() + (format == null ? "" : format.FileNameFormat) + ".jpg");

            provider.WriteFile(picturePath, pictureStream);
        }

        public GetPictureFromUrlResult GetPictureFromUrl(GetPictureFromUrlRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (string.IsNullOrEmpty(request.PictureUrl))
                throw new ArgumentNullException("request");

            var result = new GetPictureFromUrlResult(request);
            PictureTransformer transformer = new PictureTransformer();

            MemoryStream pictureStream = new MemoryStream();
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(request.PictureUrl);
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                var stream = httpResponse.GetResponseStream();
                stream.CopyTo(pictureStream);
            }
            catch (Exception ex)
            {
                this.Services.Logger.Error(
                    "PeopleServices.GetPictureFromUrl",
                    ErrorLevel.ThirdParty,
                    "Error getting the picture with the given url: " + ex.ToString());

                result.Errors.Add(GetPictureFromUrlError.HttpRequestFailed, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (pictureStream.Length == 0)
            {
                this.Services.Logger.Error(
                    "PeopleServices.GetPictureFromUrl",
                    ErrorLevel.ThirdParty,
                    "The response stream is null for url: " + request.PictureUrl);

                result.Errors.Add(GetPictureFromUrlError.HttpRequestFailed, NetworksEnumMessages.ResourceManager);
                return result;
            }
            result.OriginalPicture = pictureStream;

            if (request.MakeResize)
            {
                var formats = userPictureFormats;
                for (int i = 0; i < formats.Length; i++)
                {
                    var format = formats[i];

                    MemoryStream resized;
                    try
                    {
                        pictureStream.Seek(0, SeekOrigin.Begin);
                        resized = transformer.FormatPicture(format, pictureStream);
                    }
                    catch (FormatException ex)
                    {
                        this.Services.Logger.Error(
                            "PeopleServices.GetPictureFromUrl",
                            ErrorLevel.Business,
                            "Error when formating picture: " + ex.ToString());

                        result.Errors.Add(GetPictureFromUrlError.FileIsNotPicture, NetworksEnumMessages.ResourceManager);
                        return result;
                    }

                    result.ResizedPictures[format] = resized;
                }
            }

            result.Succeed = true;
            return result;
        }

        private void UpdateSingleField(LinkedInPeopleResult result, User user, string distantValue, ProfileFieldType fieldType, ProfileFieldSource source, bool live)
        {
            var localValue = live ? user.UserProfileFields.SingleByType(fieldType) : null;
            if (distantValue != null && (localValue == null || localValue.Value != distantValue))
            {
                UserProfileFieldModel model;
                if (live)
                {
                    model = this.Services.ProfileFields.SetUserProfileField(user.Id, fieldType, distantValue, source);
                }
                else
                {
                    model = new UserProfileFieldModel(user.Id, fieldType, distantValue, source);
                }
                result.Changes.Add(new ProfileFieldUpdate(model, ProfileFieldChange.CreateOrUpdate));
            }
        }

        public ProfileEditRequest GetProfileEditRequest(int? loadUserId, ProfileEditRequest model)
        {
            bool load = model == null;
            model = model ?? new ProfileEditRequest();

            if (loadUserId != null)
            {
                var user = this.Services.People.GetById(loadUserId.Value, PersonOptions.Company | PersonOptions.Job);
                if (user == null)
                    throw new ArgumentException("The given loadUserId does not correspond to a user", "loadUserId");

                model.Picture = this.GetProfilePictureUrl(user, UserProfilePictureSize.Medium, UriKind.Relative);

                var values = this.Services.ProfileFields.GetUserProfileFieldsByUserId(user.Id);

                // User
                model.Id = user.Id;
                model.FirstName = model.FirstName ?? user.FirstName;
                model.LastName = model.LastName ?? user.LastName;
                model.Login = model.Login ?? user.Login;
                model.Birthday = model.Birthday ?? user.Birthday;
                model.ProEmail = model.ProEmail ?? user.EmailValue;
                model.PersonalEmail = model.PersonalEmail ?? user.PersonalEmail;

                // UserProfileFields
                model.ZipCode = model.ZipCode ?? values.GetValue(ProfileFieldType.ZipCode);
                model.ZipCodeSource = values.GetSource(ProfileFieldType.ZipCode);
                model.City = model.City ?? values.GetValue(ProfileFieldType.City);
                model.CitySource = values.GetSource(ProfileFieldType.City);
                model.Phone = model.Phone ?? values.GetValue(ProfileFieldType.Phone);
                model.PhoneSource = values.GetSource(ProfileFieldType.Phone);
                model.About = model.About ?? values.GetValue(ProfileFieldType.About);
                model.AboutSource = values.GetSource(ProfileFieldType.About);
                model.CurrentTarget = model.CurrentTarget ?? values.GetValue(ProfileFieldType.CurrentTarget);
                model.CurrentTargetSource = values.GetSource(ProfileFieldType.CurrentTarget);
                model.FavoriteQuotes = model.FavoriteQuotes ?? values.GetValue(ProfileFieldType.FavoriteQuotes);
                model.FavoriteQuotesSource = values.GetSource(ProfileFieldType.FavoriteQuotes);
                model.Contribution = model.Contribution ?? values.GetValue(ProfileFieldType.Contribution);
                model.ContributionSource = values.GetSource(ProfileFieldType.Contribution);
                model.ContactGuideline = model.ContactGuideline ?? values.GetValue(ProfileFieldType.ContactGuideline);
                model.ContactGuidelineSource = values.GetSource(ProfileFieldType.ContactGuideline);
                model.LinkedInPublicUrl = model.LinkedInPublicUrl ?? values.GetValue(ProfileFieldType.LinkedInPublicUrl);
                model.LinkedInPublicUrlSource = values.GetSource(ProfileFieldType.LinkedInPublicUrl);

                // Industry
                var industryValue = values.SingleByType(ProfileFieldType.Industry);
                model.IndustrySource = values.GetSource(ProfileFieldType.Industry);
                if (model.IndustryId != null)
                {
                    var industry = this.Services.ProfileFields.GetProfileFieldsAvailiableValueByIdAndType(model.IndustryId.Value, ProfileFieldType.Industry);
                    ////var industry = this.Services.ProfileFields.GetProfileFieldsAvailiableValueByValue(ProfileFieldType.Industry, model.IndustryId.Value);
                    if (industry != null)
                    {
                        model.JobLibelle = industry.Value;
                        model.IndustryId = industryValue.Id;
                    }
                }
                else if (industryValue != null)
                {
                    var industry = this.Services.ProfileFields.GetProfileFieldsAvailiableValueByValue(ProfileFieldType.Industry, industryValue.Value);
                    model.JobLibelle = industry.Value;
                    model.IndustryId = industryValue.Id;
                }

                // Job
                if (user.JobId.HasValue)
                {
                    model.JobId = user.JobId.Value;
                    model.JobLibelle = user.JobName;
                }

                // Country
                var countryValue = values.SingleByType(ProfileFieldType.Country);
                model.CountrySource = values.GetSource(ProfileFieldType.Country);
                model.CountryId = model.CountryId ?? (countryValue != null ? countryValue.Value : null);
                if (model.CountryId != null)
                {
                    var country = this.Services.ProfileFields.GetProfileFieldsAvailiableValueByValue(ProfileFieldType.Country, model.CountryId);
                }

                model.AvailableProfileFields = this.Services.ProfileFields.GetUserFields();
                model.ProfileFieldValues = values;
            }

            // check if linkedin is configured
            var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.LinkedIn);
            if (socialState != null && this.Services.AppConfiguration.Tree.Externals.LinkedIn.AllowProfileUpdate)
            {
                if (socialState.NetworkId == this.Services.NetworkId && socialState.Entity != null && socialState.Entity.IsConfigured)
                {
                    model.IsLinkedInConfigured = true;
                }
            }

            this.FillProfileEditList(model);

            return model;
        }

        public ProfileEditResult UpdateUserProfile(ProfileEditRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ProfileEditResult(request);
            var user = this.SelectWithId(request.Id);
            if (user == null)
            {
                result.Errors.Add(ProfileEditError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (!this.IsActive(user))
            {
                result.Errors.Add(ProfileEditError.InactiveUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // TODO: we need a transaction here

            var profileFieldSource = ProfileFieldSource.UserInput;

            // User
            user.FirstName = request.FirstName.CapitalizeWords();
            user.LastName = request.LastName.CapitalizeWords();
            user.Birthday = request.Birthday;
            user.JobId = request.JobId == 0 ? null : new int?(request.JobId);
            user.PersonalDataUpdateDateUtc = DateTime.UtcNow;

            if (user.GenderValue != NetworkUserGender.Male && user.GenderValue != NetworkUserGender.Female)
                user.GenderValue = this.Services.AppConfiguration.Tree.Features.Users.DefaultGender == "Male" ? NetworkUserGender.Male : NetworkUserGender.Female;

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.Email.ToString()))
            {
                string emailValue = request.PersonalEmail.NullIfEmptyOrWhitespace();
                if (emailValue != null)
                {
                    var email = EmailAddress.TryCreate(emailValue);
                    if (email != null)
                    {
                        emailValue = email.Value;
                    }
                    else
                    {
                        emailValue = null;
                    }
                }

                user.PersonalEmail = emailValue;
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.Email, emailValue, profileFieldSource);
            }

            // Industry
            if (request.IndustryId.GetValueOrDefault() != 0)
            {
                ProfileFieldsAvailiableValue industryField = null;
                industryField = this.Services.ProfileFields.GetProfileFieldsAvailiableValueByIdAndType(request.IndustryId.Value, ProfileFieldType.Industry);
                if (industryField == null)
                {
                    result.Errors.Add(ProfileEditError.NoSuchIndustry, NetworksEnumMessages.ResourceManager);
                    return result;
                }

                request.IndustryLibelle = industryField.Value;
            }

            // Country
            if (!string.IsNullOrEmpty(request.CountryId))
            {
                var countries = SrkToolkit.Globalization.CultureInfoHelper.GetCountries();
                if (!countries.Any(o => o.ThreeLetterISORegionName == request.CountryId))
                {
                    result.Errors.Add(ProfileEditError.NoSuchCountry, NetworksEnumMessages.ResourceManager);
                    return result;
                }
            }

            // User profile fields
            this.UpdateProfileFields(request, profileFieldSource);

            this.Update(user);

            result.Succeed = true;
            return result;
        }

        public ApplyRequestRequest GetApplyRequestRequest(Guid? Key, ApplyRequestRequest request, int? userId, string companyCategory)
        {
            ApplyRequest item = null;
            if (Key != null)
            {
                item = this.Repo.ApplyRequests.GetByKey(Key.Value, this.Services.NetworkId);
            }
            else if (request != null && request.Key != Guid.Empty)
            {
                item = this.Repo.ApplyRequests.GetByKey(request.Key, this.Services.NetworkId);
            }

            if (request == null)
            {
                request = new ApplyRequestRequest()
                {
                    Key = (item != null ? item.Key : Key) ?? Key ?? Guid.NewGuid(),
                };
            }

            request.Model = item != null ? new ApplyRequestModel(item) : null;
            if (item != null && !string.IsNullOrEmpty(request.Model.UserDataModel.User.Picture))
            {
                request.ApplyProfilePictureUrl = this.GetApplyProfilePictureUrl(request.Model.Key, UserProfilePictureSize.Small);
            }

            if (request.Model != null)
            {
                this.FillModelFromMetaData(request);
            }

            this.FillApplyRequestList(request);
            if (item == null)
            {
                var defaultGender = this.Services.AppConfiguration.Tree.Features.Users.DefaultGender;
                request.Gender = defaultGender == "Male" ? NetworkUserGender.Male : defaultGender == "Female" ? NetworkUserGender.Female : NetworkUserGender.Unspecified;
            }

            request.CreateCompanyRequest = this.Services.Company.GetCreateRequest(request.CreateCompanyRequest, userId);
            request.CreateCompanyRequest.IsFromApplyRequest = true;

            // init TagPickerModel
            var categories = this.Services.Tags.GetCategoriesApplyingToCompanies().Where(o => o.RulesModel.Rules[RuleType.Company].DisplayInApplyProcess).ToList();
            var tagsPerCategory = categories
                .ToDictionary(o => o, o => (IList<Tag2Model>)(request.Model != null ? request.Model.CompanyDataModel.CompanyTags.Where(t => t.CategoryId == o.Id).ToList() : new List<Tag2Model>()));
            request.CreateCompanyRequest.AjaxTagPicker = new AjaxTagPickerModel(tagsPerCategory, true);

            // set default company category
            var defaultCategory = this.Services.Company.GetDefaultCategory();
            if (defaultCategory != null)
            {
                request.CreateCompanyRequest.CategoryId = defaultCategory.Id;
            }

            if (!string.IsNullOrEmpty(companyCategory))
            {
                request.CompanyCategory = companyCategory;
            }

            CompanyCategoryModel category = null;
            if (!string.IsNullOrEmpty(request.CompanyCategory) && request.CreateCompanyRequest.CategoryId == 0)
            {
                category = this.Services.Company.GetCategoryByName(request.CompanyCategory);
                if (category != null)
                {
                    request.CreateCompanyRequest.CategoryId = (short)category.Id;
                }
            }

            if (request.Model != null && request.Model.ProcessDataModel.CompanyCategoryId.HasValue)
            {
                category = null;
                if ((category = this.Services.Company.GetCategoryById(request.Model.ProcessDataModel.CompanyCategoryId.Value)) != null)
                {
                    request.CreateCompanyRequest.CategoryId = (short)category.Id;
                }
            }

            // check linkedin is configured
            if (this.Services.AppConfiguration.Tree.Externals.LinkedIn.AllowLogon)
            {
                var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.LinkedIn);
                if (socialState != null)
                {
                    if (socialState.NetworkId == this.Services.NetworkId && socialState.Entity != null && socialState.Entity.IsConfigured)
                    {
                        request.IsLinkedInConfigured = true;
                    }
                }
            }

            return request;
        }

        public ApplyRequestModel GetApplyRequest(Guid key)
        {
            var item = this.Repo.ApplyRequests.GetByKey(key, this.Services.NetworkId);
            if (item == null)
                return null;

            return this.GetApplyRequest(item);
        }

        public ApplyRequestModel GetApplyRequest(int id)
        {
            var item = this.Repo.ApplyRequests.GetById(id, this.Services.NetworkId);
            if (item == null)
                return null;

            return this.GetApplyRequest(item);
        }

        private ApplyRequestModel GetApplyRequest(ApplyRequest item)
        {
            var model = new ApplyRequestModel(item);

            if (item.JoinCompanyId != null)
            {
                model.JoinCompany = new CompanyModel(this.Repo.Companies.GetById(item.JoinCompanyId.Value));
            }

            if (item.CreatedUserId.HasValue)
            {
                model.CreatedUser = new UserModel(this.Services.People.SelectWithId(item.CreatedUserId.Value));
            }

            if (item.CreatedCompanyId.HasValue)
            {
                model.CreatedCompany = new CompanyModel(this.Repo.Companies.GetById(item.CreatedCompanyId.Value));
            }

            return model;
        }

        private void FillModelFromMetaData(ApplyRequestRequest model)
        {
            var user = model.Model.UserDataModel.User;
            var fields = model.Model.UserDataModel.UserFields;

            if (user != null)
            {
                model.Firstname = user.FirstName;
                model.Lastname = user.LastName;
                model.JobId = user.JobId ?? 0;
                model.Email = user.Email;
                model.Gender = user.Gender.HasValue ? (NetworkUserGender)user.Gender.Value : NetworkUserGender.Unspecified;
                model.PersonalEmail = user.PersonalEmail;
                model.Timezone = user.Timezone;
                model.Culture = user.Culture;
            }

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    switch (field.ProfileFieldType)
                    {
                        case ProfileFieldType.Phone:
                            model.Phone = field.Value;
                            break;
                        case ProfileFieldType.About:
                            model.About = field.Value;
                            break;
                        case ProfileFieldType.City:
                            model.City = field.Value;
                            break;
                        case ProfileFieldType.Country:
                            model.Country = field.Value;
                            break;
                        case ProfileFieldType.Industry:
                            var industryId = this.Services.ProfileFields.GetAvailableValueIdByName(field.Value);
                            model.Industry = industryId.ToString();
                            break;
                        case ProfileFieldType.LinkedInPublicUrl:
                            model.LinkedInPublicUrl = field.Value;
                            break;
                        case ProfileFieldType.Twitter:
                            model.Twitter = field.Value;
                            break;
                        case ProfileFieldType.Site:
                        case ProfileFieldType.ZipCode:
                        case ProfileFieldType.FavoriteQuotes:
                        case ProfileFieldType.CurrentTarget:
                        case ProfileFieldType.Contribution:
                        case ProfileFieldType.Headline:
                        case ProfileFieldType.ContactGuideline:
                        case ProfileFieldType.Language:
                        case ProfileFieldType.Education:
                        case ProfileFieldType.GTalk:
                        case ProfileFieldType.Msn:
                        case ProfileFieldType.Skype:
                        case ProfileFieldType.Yahoo:
                        case ProfileFieldType.Volunteer:
                        case ProfileFieldType.Certification:
                        case ProfileFieldType.Patent:
                        case ProfileFieldType.Position:
                        default:
                            break;
                    }
                }
            }

        }

        private void FillApplyRequestList(ApplyRequestRequest model)
        {
            // jobs
            IList<Job> jobs = new List<Job> { new Job { Alias = "", Libelle = "", Id = 0 } };
            jobs.AddRange(this.Services.Job.SelectAll());
            model.Jobs = jobs.Select(o => new JobModel(o)).ToList();

            // industries
            IList<IndustryModel> industries = new List<IndustryModel> { new IndustryModel(0, ""), };
            industries.AddRange(this.Services.ProfileFields.GetAllAvailiableValuesByType(ProfileFieldType.Industry).Select(o => new IndustryModel(o)).ToList());
            model.Industries = industries;

            // countries
            model.Countries = this.GetCountriesList(true);

            // cultures
            model.AvailableCultures = new Dictionary<string, string> { { "", "" }, };
            this.Services.SupportedCultures.Where(o => !o.IsNeutralCulture).ToList().ForEach(o => { model.AvailableCultures.Add(o.Name, o.DisplayName); });
        }

        public ApplyRequestResult SaveApplyRequest(ApplyRequestRequest request, bool submit)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string logPath = "PeopleService.SaveApplyRequest(" + (submit ? "submit" : "save") + ")";
            var result = new ApplyRequestResult(request);

            Guid key = request.Key != Guid.Empty ? request.Key : Guid.NewGuid();
            request.Key = key;

            // load or create
            var item = this.Repo.ApplyRequests.GetByKey(key, this.Services.NetworkId);
            if (item == null)
            {
                item = new ApplyRequest
                {
                    Key = key,
                    DateCreatedUtc = DateTime.UtcNow,
                    UserRemoteAddress = request.UserRemoteAddress,
                    NetworkId = this.Services.NetworkId,
                };
            }
            else if (item.UserRemoteAddress == null)
            {
                item.UserRemoteAddress = request.UserRemoteAddress;
            }

            // build model
            if (request.Industries == null)
            {
                this.FillApplyRequestList(request);
            }

            var model = new ApplyRequestModel(item);
            if (model.Status != ApplyRequestStatus.New)
            {
                result.Errors.Add(ApplyRequestError.RequestIsNotNew, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest=" + item);
            }

            model.UpdateFrom(request);
            item.Data = model.Data;
            result.Model = model;

            Trace.Assert(model.UserDataModel != null, logPath + ": 'model.UserDataModel != null' ASSERT FAILED");
            Trace.Assert(model.CompanyDataModel != null, logPath + ": 'model.CompanyDataModel != null' ASSERT FAILED");
            Trace.Assert(model.ProcessDataModel != null, logPath + ": 'model.ProcessDataModel != null' ASSERT FAILED");

            if (model.UserDataModel.User != null && model.UserDataModel.User.Email != null)
            {
                if (this.IsEmailAddressInUse(model.UserDataModel.User.Email))
                {
                    result.Errors.Add(ApplyRequestError.EmailAddressAlreadyInUse, NetworksEnumMessages.ResourceManager);
                }
            }

            // check company id
            Guid linkedInCompanyId;
            int companyId;
            Sparkle.Entities.Networks.Company companyToJoin = null;
            LinkedInCompanyDataModel linkedInCompanyToCreate = null;
            if (!this.Services.AppConfiguration.Tree.Features.EnableCompanies
                && this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.HasValue)
            {
                var company = this.Services.Company.GetById(this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.Value);
                if (company == null)
                {
                    result.Errors.Add(ApplyRequestError.RegisterCompanyMissconfigured, NetworksEnumMessages.ResourceManager);
                }
                else if (this.Services.Company.IsActive(company))
                {
                    companyToJoin = company;
                }
                else
                {
                    result.Errors.Add(ApplyRequestError.CompanyIsDisabled, NetworksEnumMessages.ResourceManager);
                }
            }
            else if (int.TryParse(request.CompanyId, out companyId))
            {
                if (companyId != 0)
                {
                    var company = this.Services.Company.GetById(companyId);
                    if (this.Services.Company.IsActive(company))
                    {
                        companyToJoin = company;
                    }
                    else
                    {
                        result.Errors.Add(ApplyRequestError.CompanyIsDisabled, NetworksEnumMessages.ResourceManager);
                    }
                }
            }
            else if (model.CompanyDataModel.LinkedInCompanies != null && Guid.TryParse(request.CompanyId, out linkedInCompanyId))
            {
                linkedInCompanyToCreate = model.CompanyDataModel.LinkedInCompanies.SingleOrDefault(c => c.Identifier == linkedInCompanyId);
                if (linkedInCompanyToCreate == null)
                    result.Errors.Add(ApplyRequestError.BadCompanyId, NetworksEnumMessages.ResourceManager);

                // try match linkedin company with local
                var linMatchWithLocal = this.Repo.Companies.GetActiveByName(this.Services.NetworkId, linkedInCompanyToCreate.Company.Name).FirstOrDefault();
                if (linMatchWithLocal != null)
                {
                    companyToJoin = linMatchWithLocal;
                }
            }
            else if (submit)
            {
                result.Errors.Add(ApplyRequestError.BadCompanyId, NetworksEnumMessages.ResourceManager);
            }

            // submit?
            if (submit && result.Errors.Count == 0)
            {
                result.Submitted = true;
                model.DateSubmitedUtc = item.DateSubmitedUtc = DateTime.UtcNow;

                if (companyToJoin != null)
                {
                    item.JoinCompanyId = companyToJoin.ID;
                    model.CompanyDataModel.Company = null;
                }
                else if (linkedInCompanyToCreate != null)
                {
                    var linCompany = linkedInCompanyToCreate;
                    var newCompany = new Sparkle.Entities.Networks.Neutral.CompanyPoco();
                    var companyProfileFields = new List<Sparkle.Entities.Networks.Neutral.CompanyProfileFieldPoco>();

                    newCompany.Name = linCompany.Company.Name;

                    var address = EmailAddress.TryCreate(request.Email);
                    if (address != null && linCompany.EmailDomains.Contains(address.DomainPart))
                        newCompany.EmailDomain = address.DomainPart;
                    else
                        newCompany.EmailDomain = linCompany.EmailDomains.First();

                    CompanyCategoryModel category = null;
                    if (model.ProcessDataModel.CompanyCategoryId.HasValue)
                        category = this.Services.Company.GetCategoryById(model.ProcessDataModel.CompanyCategoryId.Value);
                    else if (!string.IsNullOrEmpty(request.CompanyCategory))
                        category = this.Services.Company.GetCategoryByName(request.CompanyCategory);

                    if (category == null)
                        category = this.Services.Company.GetDefaultCategory();

                    Trace.Assert(model.ProcessDataModel != null, logPath + ": 'category == null' ASSERT FAILED");

                    newCompany.CategoryId = (short)category.Id;

                    model.CompanyDataModel.Company = newCompany;
                    model.CompanyDataModel.CompanyFields = linCompany.CompanyFields;
                    ////model.CompanyDataModel.LinkedInCompanies = null;
                }
                else
                {
                    // company to create
                    var newCompany = new Sparkle.Entities.Networks.Neutral.CompanyPoco();
                    newCompany.CategoryId = (short)request.CreateCompanyRequest.CategoryId;
                    newCompany.Name = request.CreateCompanyRequest.Name;
                    newCompany.Baseline = request.CreateCompanyRequest.Baseline;

                    model.CompanyDataModel.CompanyFields.Add(new Neutral.CompanyProfileFieldPoco(request.CreateCompanyRequest.Website, ProfileFieldType.Site, ProfileFieldSource.UserInput));
                    model.CompanyDataModel.CompanyFields.Add(new Neutral.CompanyProfileFieldPoco(request.CreateCompanyRequest.Phone, ProfileFieldType.Phone, ProfileFieldSource.UserInput));
                    model.CompanyDataModel.CompanyFields.Add(new Neutral.CompanyProfileFieldPoco(request.CreateCompanyRequest.About, ProfileFieldType.About, ProfileFieldSource.UserInput));
                    model.CompanyDataModel.CompanyFields.Add(new Neutral.CompanyProfileFieldPoco(request.CreateCompanyRequest.Email, ProfileFieldType.Email, ProfileFieldSource.UserInput));

                    var adminEmails = request.CreateCompanyRequest.AdminEmails;
                    if (!string.IsNullOrEmpty(adminEmails))
                    {
                        var split = Validate.ManyEmailAddresses(adminEmails).ToArray();
                        model.CompanyDataModel.AdminsEmailAddresses = split;
                    }

                    var otherEmails = request.CreateCompanyRequest.OtherEmails;
                    if (!string.IsNullOrEmpty(otherEmails))
                    {
                        var split = Validate.ManyEmailAddresses(otherEmails).ToArray();
                        model.CompanyDataModel.UsersEmailAddresses = split;
                    }

                    model.CompanyDataModel.Company = newCompany;
                    model.CompanyDataModel.LinkedInCompanies = null;
                }

                item.CompanyData = model.CompanyData;
            }

            if (!string.IsNullOrEmpty(request.InviterCode) && this.IsApplyInviterCodeValid(request.InviterCode))
            {
                var inviterItem = Sparkle.Services.Networks.Objects.ApplyInviteCode.FromHex(request.InviterCode);
                item.InvitedByUserId = inviterItem.UserId;
                item.DateInvitedUtc = inviterItem.CreateDateUtc;
            }

            if (item.Id == 0)
            {
                this.Repo.ApplyRequests.Insert(item);
                model.Id = item.Id;
            }
            else
            {
                this.Repo.ApplyRequests.Update(item);
            }

            if (result.Submitted && result.Errors.Count == 0)
            {
                if (model.IsPendingEmailConfirmation)
                {
                    var anonymousUserModel = new UserModel(model.UserDataModel.User);
                    try
                    {
                        this.Services.Email.SendApplyRequestConfirmation(model, anonymousUserModel);
                    }
                    catch (SparkleServicesException ex)
                    {
                        result.EmailErrorMessage = ex.DisplayMessage ?? NetworksLabels.EmailProviderDefaultDisplayError;
                    }
                    catch (InvalidOperationException ex)
                    {
                        result.EmailErrorMessage = NetworksLabels.EmailProviderDefaultDisplayError;
                    }
                }
                else
                {
                    this.NotifyAdminsOfPendingApplyRequest(model);

                    ////// TODO: send email to requester
                    ////var requester = new UserModel(model.UserDataModel.User);
                    ////this.Services.Email.SendApplyRequestConfirmation(model, requester);
                }
            }

            result.Succeed = true;
            return this.LogResult(result, logPath, "ApplyRequest=" + item);
        }

        public ApplyRequestResult AppendSocialNetworkConnection(ApplyRequestRequest request, SocialNetworkConnectionType type, string username, string oAuthToken, string oAuthVerifier, bool isActive, DateTime? oAuthTokenDateUtc = null, int? oAuthTokenDurationMinutes = null)
        {
            const string logPath = "PeopleService.AppendSocialNetworkConnection";
            Guid key = request.Key != Guid.Empty ? request.Key : Guid.NewGuid();

            var item = this.Repo.ApplyRequests.GetByKey(key, this.Services.NetworkId);
            if (item == null)
            {
                item = new ApplyRequest
                {
                    Key = key,
                    DateCreatedUtc = DateTime.UtcNow,
                    UserRemoteAddress = request.UserRemoteAddress,
                    NetworkId = this.Services.NetworkId,
                };
                this.Repo.ApplyRequests.Insert(item);
            }

            var result = new ApplyRequestResult(request);

            var model = new ApplyRequestModel(item);
            if (model.Status != ApplyRequestStatus.New)
            {
                result.Errors.Add(ApplyRequestError.RequestIsNotNew, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "" + item);
            }

            Sparkle.Entities.Networks.Neutral.SocialNetworkConnectionPoco co = null;
            if ((co = model.UserDataModel.UserConnections.Where(o => o.Type == (byte)SocialNetworkConnectionType.LinkedIn).SingleOrDefault()) != null)
            {
                model.UserDataModel.UserConnections.Remove(co);
            }

            model.UserDataModel.UserConnections.Add(new Neutral.SocialNetworkConnectionPoco
            {
                Type = (byte)type,
                Username = username,
                OAuthToken = oAuthToken,
                OAuthVerifier = oAuthVerifier,
                IsActive = isActive,
                OAuthTokenDateUtc = oAuthTokenDateUtc,
                OAuthTokenDurationMinutes = oAuthTokenDurationMinutes,
            });

            item.Data = model.Data;
            this.Repo.ApplyRequests.Update(item);

            result.Model = new ApplyRequestModel(item);
            ////return this.LogResult(result, logPath);
            this.Services.Logger.Info(logPath, ErrorLevel.Success, "Added linkedin connection to " + item);
            return result;
        }

        public LinkedInPeopleResult AppendLinkedInProfileToApplyRequest(ApplyRequestRequest request, string userRemoteAddress)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleService.AppendLinkedInProfileToApplyRequest";
            LinkedInPeopleResult result = new LinkedInPeopleResult(new LinkedInPeopleRequest(0));
            ApplyRequest item;
            item = this.Repo.ApplyRequests.GetByKey(request.Key, this.Services.NetworkId);
            if (item == null)
            {
                result.Errors.Add(LinkedInPeopleError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequestKey=" + request.Key);
            }

            var model = new ApplyRequestModel(item);

            if (model.Status != ApplyRequestStatus.New)
            {
                result.Errors.Add(LinkedInPeopleError.RequestIsNotNew, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest" + item);
            }

            if (model.UserDataModel == null || model.UserDataModel.UserConnections == null)
            {
                this.Services.Logger.Error(
                    "PeopleService.AppendDataModel",
                    ErrorLevel.ThirdParty,
                    "OAuthToken not found for user {0} while trying to use LinkedInApi",
                    userRemoteAddress);
                result.Errors.Add(LinkedInPeopleError.LinkedInAccountNotLinked, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest " + item);
            }

            var userConnection = model.UserDataModel.UserConnections.SingleOrDefault(o => o.Type == (byte)SocialNetworkConnectionType.LinkedIn);
            if (userConnection == null)
            {
                this.Services.Logger.Error(
                    "PeopleService.AppendDataModel",
                    ErrorLevel.ThirdParty,
                    "OAuthToken not found for user {0} while trying to use LinkedInApi",
                    userRemoteAddress);
                result.Errors.Add(LinkedInPeopleError.LinkedInAccountNotLinked, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest " + item);
            }

            // linkedin apply already done
            if (model.UserDataModel.UserFields.Any(o => o.SourceType == ProfileFieldSource.LinkedIn))
            {
                result.Errors.Add(LinkedInPeopleError.LinkedInImportAlreadyDone, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest " + item);
            }

            result = this.GetLinkedInResultNoSaving(new LinkedInPeopleRequest(userConnection.OAuthToken));
            if (result.Succeed)
            {
                model.UserDataModel.UpdateFrom(result);

                if (this.Services.AppConfiguration.Tree.InstanceName == "Spark")
                {
                    model.UserDataModel.User.Email = model.UserDataModel.User.PersonalEmail;
                    model.UserDataModel.User.PersonalEmail = null;
                }

                item.Data = model.Data;
                if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                {
                    model.CompanyDataModel.UpdateFrom(result.Companies);
                    item.CompanyData = model.CompanyData;
                }

                if (result.PicturesStream != null)
                {
                    model.UserDataModel.User.Picture = model.Key.ToString();
                    item.Data = model.Data;
                    var pictures = result.PicturesStream;
                    this.SavePictureForApplyRequest(model, pictures.OriginalPicture);
                    foreach (var resized in pictures.ResizedPictures)
                    {
                        this.SavePictureForApplyRequest(model, resized.Value, resized.Key);
                    }
                }

                this.Repo.ApplyRequests.Update(item);
            }

            return this.LogResult(result, logPath, "ApplyRequest " + item);
        }

        public IList<ApplyRequestModel> GetPendingApplyRequests()
        {
            var items = this.Repo.ApplyRequests.GetPending(this.Services.NetworkId)
                .Select(r => new ApplyRequestModel(r))
                .ToList();
            var companyIds = items.Where(i => i.JoinCompanyId != null).Select(i => i.JoinCompanyId.Value).ToArray();
            var companies = this.Repo.Companies.GetById(companyIds, this.Services.NetworkId);
            foreach (var item in items)
            {
                if (item.JoinCompanyId != null)
                {
                    var company = companies.SingleOrDefault(c => c.ID == item.JoinCompanyId.Value);
                    if (company != null)
                    {
                        item.JoinCompany = new CompanyModel(company);
                    }
                }
            }

            return items;
        }

        public AcceptApplyRequestResult AcceptApplyRequest(AcceptApplyRequestRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleService.AcceptApplyRequest";
            var result = new AcceptApplyRequestResult(request);

            var applyRequest = this.Repo.ApplyRequests.GetByKey(request.ApplyKey, this.Services.NetworkId);
            if (applyRequest == null)
            {
                result.Errors.Add(AcceptApplyRequestError.NoSuchApplyRequest, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequestKey=" + request.ApplyKey);
            }

            // TODO: check current user's permissions to do that!

            var model = new ApplyRequestModel(applyRequest);

            if (model.Status != ApplyRequestStatus.PendingAccept)
            {
                result.Errors.Add(AcceptApplyRequestError.NotInPendingAccept, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest=" + request.ApplyKey + " CurrentUser=?");
            }

            // Forward apply request
            if (request.IsWrongCompany)
            {
                var company = this.Services.Company.GetById(request.JoinCompanyId);
                if (company == null || !this.Services.Company.IsActive(company))
                {
                    result.Errors.Add(AcceptApplyRequestError.OnForwardRequestNoSuchCompany, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                applyRequest.JoinCompanyId = company.ID;
                this.Repo.ApplyRequests.Update(applyRequest);

                model = new ApplyRequestModel(applyRequest);
                this.NotifyAdminsOfPendingApplyRequest(model);

                result.Succeed = true;
                return this.LogResult(result, logPath);
            }

            // Begin transaction
            var transaction = this.Services.NewTransaction();
            using (transaction.BeginTransaction())
            {
                // Create Company
                bool createCompany = false;
                int joinCompany;
                if (!transaction.Services.AppConfiguration.Tree.Features.EnableCompanies
                    && transaction.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.HasValue
                    && transaction.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.Value != 0)
                {
                    joinCompany = transaction.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.Value;
                }
                else if (model.JoinCompanyId.HasValue)
                {
                    joinCompany = model.JoinCompanyId.Value;
                }
                else
                {
                    createCompany = true;
                    var createRequest = new CreateCompanyRequest
                    {
                        IsFromApplyRequest = true,
                    };
                    var poco = model.CompanyDataModel.Company;
                    createRequest.Name = poco.Name.TrimTextRight(100);
                    createRequest.Baseline = poco.Baseline.TrimTextRight(200);
                    createRequest.EmailDomain = poco.EmailDomain;
                    createRequest.CategoryId = poco.CategoryId;
                    createRequest.UserId = request.UserId;

                    if (poco.CategoryId == 0)
                    {
                        var category = transaction.Repositories.CompanyCategories.GetAll(this.Services.NetworkId).First();
                        createRequest.CategoryId = category.Id;
                    }
                    else
                    {
                        var categories = transaction.Repositories.CompanyCategories.GetAll(this.Services.NetworkId);
                        var category = categories.SingleOrDefault(x => x.Id.Equals(poco.CategoryId));
                        if (category == null)
                        {
                            category = categories.First();
                            createRequest.CategoryId = category.Id;
                        }
                    }

                    if (model.CompanyDataModel.AdminsEmailAddresses != null && model.CompanyDataModel.AdminsEmailAddresses.Count() > 0)
                        createRequest.AdminEmails = string.Join(";", model.CompanyDataModel.AdminsEmailAddresses);
                    if (model.CompanyDataModel.UsersEmailAddresses != null && model.CompanyDataModel.UsersEmailAddresses.Count() > 0)
                        createRequest.OtherEmails = string.Join(";", model.CompanyDataModel.UsersEmailAddresses);

                    result.CreateCompanyResult = transaction.Services.Company.ApplyCreateRequest(createRequest, true);
                    if (!result.CreateCompanyResult.Succeed)
                    {
                        var error = result.CreateCompanyResult.Errors.First();
                        transaction.Services.Logger.Error(
                            "PeopleService.AcceptApplyRequest",
                            ErrorLevel.ThirdParty,
                            "CreateCompanyResult contains an error ({0}: {1})",
                            error != null ? error.Code.ToString() : "",
                            error != null ? error.DisplayMessage : "");
                        result.Errors.Add(AcceptApplyRequestError.CreateCompanyRequestFailed, NetworksEnumMessages.ResourceManager);
                        return result;
                    }
                    else
                    {
                        joinCompany = result.CreateCompanyResult.Item.ID;

                        // Create CompanyProfileFields
                        transaction.Services.ProfileFields.InsertManyCompanyProfileFields(model.CompanyDataModel.CompanyFields, joinCompany);

                        // Create CompanyRelationships
                        if (model.ProcessDataModel.CompanyRelationships != null && model.ProcessDataModel.CompanyRelationships.Count > 0)
                        {
                            var relationshipTypes = transaction.Services.CompanyRelationships.GetAllTypes();

                            foreach (var item in model.ProcessDataModel.CompanyRelationships)
                            {
                                CompanyRelationshipTypeModel type = null;
                                if ((type = relationshipTypes.SingleOrDefault(o => o.Id == item.TypeId)) != null && type.KnownType != KnownCompanyRelationshipType.Invited)
                                {
                                    item.SlaveId = result.CreateCompanyResult.Item.ID;
                                    transaction.Services.CompanyRelationships.InsertFromPoco(item);
                                }
                            }
                        }

                        // Create CompanyPlaces
                        var companyLocations = model.CompanyDataModel.CompanyFields.Where(o => o.ProfileFieldType == ProfileFieldType.Location).ToList();
                        foreach (var item in companyLocations)
                        {
                            transaction.Services.Company.AddCompanyPlaceFromCompanyProfileField(logPath, result.CreateCompanyResult.Item, item);
                        }
                    }
                }

                Sparkle.Entities.Networks.Company company = null;
                if ((company = transaction.Services.Company.GetById(joinCompany)) == null
                    || !company.IsEnabled
                    || !company.IsApproved)
                {
                    result.Errors.Add(AcceptApplyRequestError.JoinCompanyIsDisabled, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                // Create User
                var defaultGender = transaction.Services.AppConfiguration.Tree.Features.Users.DefaultGender;
                var userRequest = transaction.Services.People.GetCreateEmailPassordAccountModel(null, null);
                userRequest.FromApplyRequest = true;
                userRequest.Email = model.UserDataModel.User.Email;
                userRequest.Password = Guid.NewGuid().ToString();
                userRequest.FirstName = model.UserDataModel.User.FirstName;
                userRequest.LastName = model.UserDataModel.User.LastName;
                userRequest.Gender = model.UserDataModel.User.Gender.HasValue ? (NetworkUserGender)model.UserDataModel.User.Gender.Value
                    : (defaultGender == "Male" ? NetworkUserGender.Male : defaultGender == "Female" ? NetworkUserGender.Female : NetworkUserGender.Unspecified);
                userRequest.JobId = model.UserDataModel.User.JobId;
                if ((!userRequest.JobId.HasValue || userRequest.JobId.Value == 0) && !string.IsNullOrEmpty(model.UserDataModel.JobTitleToCreate))
                {
                    // Create new job if needed
                    var newJob = new Job
                    {
                        Libelle = model.UserDataModel.JobTitleToCreate.TrimTextRight(255),
                        Alias = transaction.Services.Job.MakeAlias(model.UserDataModel.JobTitleToCreate).TrimTextRight(100),
                    };

                    newJob = transaction.Services.Job.AddJob(newJob);
                    transaction.Services.Logger.Info(logPath, ErrorLevel.Success, "Created " + newJob.ToString() + ".");

                    userRequest.JobId = newJob.Id;
                }

                userRequest.JoinCompanyId = joinCompany;
                result.CreateUserResult = transaction.Services.People.CreateEmailPasswordAccount(userRequest);
                if (!result.CreateUserResult.Succeed)
                {
                    var error = result.CreateUserResult.Errors.First();
                    transaction.Services.Logger.Error(
                        "PeopleService.AcceptApplyRequest",
                        ErrorLevel.ThirdParty,
                        "CreateEmailPasswordAccountResult contains an error ({0}: {1})",
                        error != null ? error.Code.ToString() : "",
                        error != null ? error.DisplayMessage : "");
                    result.Errors.Add(AcceptApplyRequestError.CreateEmailPasswordAccountFailed, NetworksEnumMessages.ResourceManager);
                    return result;
                }

                var user = transaction.Services.People.SelectWithId(result.CreateUserResult.User.Id);
                user.Birthday = model.UserDataModel.User.Birthday;
                user.Culture = model.UserDataModel.User.Culture;
                user.Timezone = model.UserDataModel.User.Timezone;
                user.PersonalEmail = model.UserDataModel.User.PersonalEmail;
                if (model.UserDataModel.User.LinkedInUserId != null && transaction.Services.People.IsLinkedInIdAvailable(model.UserDataModel.User.LinkedInUserId))
                {
                    user.LinkedInUserId = model.UserDataModel.User.LinkedInUserId;
                }

                transaction.Repositories.People.Update(user);

                // Create SocialNetworkConnection
                transaction.Services.SocialNetworkConnections.AddManyWithUserId(model.UserDataModel.UserConnections, user.Id);

                // Create UserTags
                transaction.Services.People.InsertManyTags(model.UserDataModel.UserTags, user.Id);

                // Create UserProfileFields
                transaction.Services.ProfileFields.InsertManyUserProfileFields(model.UserDataModel.UserFields, user.Id);

                // Create profile picture
                if (!string.IsNullOrEmpty(model.UserDataModel.User.Picture))
                {
                    transaction.Services.People.UpdateProfilePictureFromApply(model, user);
                }

                if (createCompany)
                {
                    // Create CompanyTags
                    var tags = model.CompanyDataModel.CompanyTags;
                    foreach (var item in tags.GroupBy(o => o.CategoryId))
                    {
                        var category = transaction.Services.Tags.GetCategoryById(item.Key);
                        if (category.ApplyToCompanies)
                        {
                            foreach (var tag in item.ToList())
                            {
                                var tagRequest = new AddOrRemoveTagRequest
                                {
                                    AddTag = true,
                                    TagId = tag.Id != 0 ? tag.Id.ToString() : tag.Name,
                                    CategoryAlias = category.Alias,
                                    EntityIdentifier = company.Alias,
                                    EntityTypeName = "Company",
                                    ActingUserId = user.Id,
                                };
                                transaction.Services.Tags.AddOrRemoveTag(tagRequest);
                            }
                        }
                    }
                }

                applyRequest.DateAcceptedUtc = DateTime.UtcNow;
                applyRequest.AcceptedByUserId = request.UserId;
                applyRequest.CreatedUserId = user.Id;
                if (createCompany)
                {
                    applyRequest.CreatedCompanyId = joinCompany;
                }

                transaction.Services.Repositories.ApplyRequests.Update(applyRequest);

                model = new ApplyRequestModel(applyRequest);
                user = transaction.Services.People.SelectWithId(user.Id);
                var userModel = new UserModel(user);

                // Send registration accepted email to the created user
                if (request.NotifyNewlyCreatedUser)
                {
                    // add here the post save data
                    transaction.PostSaveActions.Add(t => t.Services.Email.SendApplyRequestAccepted(model, userModel));
                }

                if (model.InvitedByUserId.HasValue)
                {
                    // add here the post save data
                    transaction.PostSaveActions.Add(t => t.Services.Activities.Create(model.InvitedByUserId.Value, ActivityType.InvitedUserHasJoined, profileId: user.Id));

                    // add the invited relationship between the inviting and joining company
                    if (createCompany)
                    {
                        var inviter = transaction.Services.Cache.GetUser(model.InvitedByUserId.Value);
                        var invitedType = transaction.Services.CompanyRelationships.GetTypesByKnownType(KnownCompanyRelationshipType.Invited).First();
                        transaction.Services.CompanyRelationships.Insert(new CompanyRelationship
                        {
                            MasterId = inviter.CompanyId,
                            SlaveId = joinCompany,
                            TypeId = invitedType.Id,
                            DateCreatedUtc = DateTime.UtcNow,
                        });
                    }
                }

                // this is to ensure the user reset the password
                transaction.Services.People.LockMembershipAccount(user.Login);

                transaction.CompleteTransaction();
            }

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public RefuseApplyRequestResult RefuseApplyRequest(RefuseApplyRequestRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleService.RefuseApplyRequest";
            var result = new RefuseApplyRequestResult(request);
            var item = this.Repo.ApplyRequests.GetById(request.Id);
            if (item == null)
            {
                result.Errors.Add(RefuseApplyRequestError.NoSuchApplyRequest, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var model = new ApplyRequestModel(item);
            if (model.IsAccepted)
            {
                result.Errors.Add(RefuseApplyRequestError.AlreadyAccepted, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            if (model.IsRefused)
            {
                result.Errors.Add(RefuseApplyRequestError.AlreadyRefused, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            if (request.ToDelete)
            {
                item.DateDeletedUtc = DateTime.UtcNow;
                item.DeletedByUserId = request.UserId;
            }
            else
            {
                item.RefusedByUserId = request.UserId;
                item.DateRefusedUtc = DateTime.UtcNow;
                item.RefusedRemark = request.Remark;
            }
            this.Repo.ApplyRequests.Update(item);

            if (!request.ToDelete && !request.IsSpam)
            {
                model = new ApplyRequestModel(item);
                this.Services.Email.SendApplyRequestRefused(model, new UserModel(model.UserDataModel.User));
            }

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public void UpdateProfilePictureFromApply(ApplyRequestModel model, User user)
        {
            IFilesystemProvider provider = new IOFilesystemProvider();
            var picturePath = provider.EnsureFilePath(
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                this.Services.Network.Name,
                "ApplyRequests",
                model.UserDataModel.User.Picture,
                model.UserDataModel.User.Picture + ".jpg");

            if (provider.FileExists(picturePath))
            {
                var pictureStream = new MemoryStream();
                using (var fs = File.OpenRead(picturePath))
                {
                    fs.CopyTo(pictureStream);
                }

                var request = new SetProfilePictureRequest
                {
                    PictureStream = pictureStream,
                    UserId = user.Id,
                };
                this.SetProfilePicture(request);
            }
        }

        public string GetApplyProfilePictureUrl(Guid key, UserProfilePictureSize size)
        {
            string sizeFileName = "";
            switch (size)
            {
                case UserProfilePictureSize.Small:
                    sizeFileName = "l";
                    break;
                case UserProfilePictureSize.Medium:
                default:
                    sizeFileName = "p";
                    break;
            }
            return "/Content/Networks/" +
                this.Services.Network.Name + "/" +
                "ApplyRequests/" +
                key.ToString() + "/" +
                key.ToString() + sizeFileName + ".jpg";
        }

        public string GetApplyRequestUrl(ApplyRequestModel applyRequest)
        {
            if (applyRequest == null)
                throw new ArgumentNullException("applyRequest");
            if (applyRequest.Key == Guid.Empty)
                throw new ArgumentException("The value cannot be empty", "applyRequest.Key");

            return this.Services.GetUrl("/Apply/Request/" + applyRequest.Key);
        }

        public string GetApplyRequestConfirmUrl(ApplyRequestModel applyRequest)
        {
            if (applyRequest == null)
                throw new ArgumentNullException("applyRequest");
            if (applyRequest.Key == Guid.Empty)
                throw new ArgumentException("The value cannot be empty", "applyRequest.Key");

            var query = new Dictionary<string, string>();
            query.Add("Secret", applyRequest.GetSecretKey());
            return this.Services.GetUrl("/Apply/Confirm/" + applyRequest.Key, query);
        }

        public string GetApplyRequestJoinUrl(Guid id)
        {
            return this.GetApplyRequestJoinUrl(this.GetApplyRequest(id));
        }

        public string GetApplyRequestJoinUrl(ApplyRequestModel applyRequest)
        {
            if (applyRequest == null)
                throw new ArgumentNullException("applyRequest");
            if (applyRequest.Key == Guid.Empty)
                throw new ArgumentException("The value cannot be empty", "applyRequest.Key");

            var query = new Dictionary<string, string>();
            query.Add("Secret", applyRequest.GetSecretKey());
            return this.Services.GetUrl("/Apply/Join/" + applyRequest.Key, query);
        }

        public ConfirmApplyRequestEmailAddressResult ConfirmApplyRequestEmailAddress(ConfirmApplyRequestEmailAddressRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleService.ConfirmApplyRequestEmailAddress";
            var result = new ConfirmApplyRequestEmailAddressResult(request);

            var item = this.Repo.ApplyRequests.GetByKey(request.Key, this.Services.NetworkId);
            if (item == null)
            {
                result.Errors.Add(ConfirmApplyRequestEmailAddressError.NoSuchApplyRequest, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequestKey=" + request.Key);
            }

            var model = new ApplyRequestModel(item);
            result.Model = model;
            if (!model.CheckSecretKey(request.Secret))
            {
                result.Errors.Add(ConfirmApplyRequestEmailAddressError.InvalidSecret, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, "ApplyRequest=" + item);
            }

            if (model.IsNew)
            {
                result.Errors.Add(ConfirmApplyRequestEmailAddressError.NotSubmitted, NetworksEnumMessages.ResourceManager);
            }
            else if (model.IsPendingEmailConfirmation)
            {
                var now = DateTime.UtcNow;
                item.DateEmailConfirmedUtc = now;
                model.DateEmailConfirmedUtc = now;
                this.Repo.ApplyRequests.Update(item);
                result.Succeed = true;

                EmailAddress address = null;
                Sparkle.Entities.Networks.Company company = null;

                // email domain match
                var isEmailDomainMatch = (address = EmailAddress.TryCreate(model.UserDataModel.User.Email)) != null
                    && model.JoinCompanyId.HasValue
                    && (company = this.Services.Company.GetById(model.JoinCompanyId.Value)) != null
                    && company.EmailDomain == address.DomainPart;
                // admin want to manually validate all apply requests
                var alwaysRequireValidation = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompanyRequireValidation;
                // default company to join
                var hasDefaultRegisterCompany = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.HasValue
                    && (company = this.Services.Company.GetById(this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany.Value)) != null
                    && company.IsEnabled && company.IsApproved;
                // already approved by an admin
                var isAlreadyApproved = model.ProcessDataModel.IsApprovedBy.HasValue && this.CanApproveCompany(model.ProcessDataModel.IsApprovedBy.Value);

                if (((isEmailDomainMatch || hasDefaultRegisterCompany) && !alwaysRequireValidation) || isAlreadyApproved)
                {
                    var acceptRequest = new AcceptApplyRequestRequest
                    {
                        ApplyKey = model.Key,
                        NotifyNewlyCreatedUser = false,
                    };

                    var acceptResult = this.AcceptApplyRequest(acceptRequest);
                    if (!acceptResult.Succeed)
                    {
                        var error = acceptResult.Errors.First();
                        this.Services.Logger.Error(
                            "PeopleService.ConfirmApplyRequestEmailAddress",
                            ErrorLevel.Critical,
                            "PeopleService.AcceptApplyRequest contains an error ({0}: {1})",
                            error != null ? error.Code.ToString() : "",
                            error != null ? error.DisplayMessage : "");

                        result.Errors.Add(ConfirmApplyRequestEmailAddressError.AcceptOnEmailDomainMatch, NetworksEnumMessages.ResourceManager);
                        result.Succeed = false;
                        return this.LogResult(result, "ApplyRequest=" + item);
                    }
                    else
                    {
                        result.EmailDomainMatch = true;
                    }
                }
                else
                {
                    this.NotifyAdminsOfPendingApplyRequest(model);
                }
            }
            else if (model.IsPendingAccept)
            {
                result.EmailWasAlreadyConfirmed = true;
                result.Succeed = true;
            }
            else if (model.IsRefused)
            {
                result.Errors.Add(ConfirmApplyRequestEmailAddressError.Refused, NetworksEnumMessages.ResourceManager);
            }
            else if (model.IsAccepted)
            {
                result.Errors.Add(ConfirmApplyRequestEmailAddressError.Accepted, NetworksEnumMessages.ResourceManager);
            }
            else
            {
                result.Errors.Add(ConfirmApplyRequestEmailAddressError.UnexpectedState, NetworksEnumMessages.ResourceManager);
            }

            return this.LogResult(result, "ApplyRequest=" + item);
        }

        public void InsertManyTags(IList<TagModel> items, int userId)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                switch (item.Type)
                {
                    case TagModelType.Skill:
                        this.Services.PeoplesSkills.AddPeopleSkill(item, userId);
                        break;
                    case TagModelType.Interest:
                        this.Services.PeoplesInterests.AddPeopleInterest(item, userId);
                        break;
                    case TagModelType.Recreation:
                        this.Services.PeoplesRecreations.AddPeopleRecreation(item, userId);
                        break;
                    default:
                        break;
                }
            }
        }

        public IList<ApplyRequestModel> GetAllApplyRequests(int userId)
        {
            return this.GetAllApplyRequests(userId, 0, 0).Items;
        }

        public PagedListModel<ApplyRequestModel> GetAllApplyRequests(int userId, int offset, int pageSize)
        {
            var user = this.SelectWithId(userId);
            if (user != null)
            {
                IList<ApplyRequest> items = null;
                int total;
                if (user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.AddCompany, NetworkAccessLevel.ManageCompany, NetworkAccessLevel.ManageRegisterRequests, NetworkAccessLevel.ValidatePendingUsers))
                {
                    items = this.Repo.ApplyRequests.GetAll(this.Services.NetworkId, offset, pageSize);
                    total = this.Repo.ApplyRequests.CountAll(this.Services.NetworkId);
                }
                else if (user.CompanyAccess.HasFlag(CompanyAccessLevel.Administrator))
                {
                    items = this.Repo.ApplyRequests.GetByJoinCompanyId(this.Services.NetworkId, user.CompanyID);
                    total = this.Repo.ApplyRequests.CountByJoinCompanyId(this.Services.NetworkId, user.CompanyID);
                }
                else
                {
                    return null;
                }

                var models = items
                    .Select(a => new ApplyRequestModel(a))
                    .ToList();
                return new PagedListModel<ApplyRequestModel>(models, total, offset, pageSize);
            }

            return null;
        }

        public int CountAllApplyRequests(int userId)
        {
            var user = this.SelectWithId(userId);
            if (user != null)
            {
                int total;
                if (user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.AddCompany, NetworkAccessLevel.ManageCompany, NetworkAccessLevel.ManageRegisterRequests, NetworkAccessLevel.ValidatePendingUsers))
                {
                    total = this.Repo.ApplyRequests.CountAll(this.Services.NetworkId);
                }
                else if (user.CompanyAccess.HasFlag(CompanyAccessLevel.Administrator))
                {
                    total = this.Repo.ApplyRequests.CountByJoinCompanyId(this.Services.NetworkId, user.CompanyID);
                }
                else
                {
                    total = -1;
                }

                return total;
            }

            return -1;
        }

        public ValidateApplyRequestResult ValidateApplyRequestToJoin(Guid key, string secret)
        {
            const string logPath = "PeopleService.ValidateApplyRequestToJoin";
            var result = new ValidateApplyRequestResult(new ValidateApplyRequestRequest());

            var item = this.GetApplyRequest(key);
            if (item == null)
            {
                result.Errors.Add(ValidateApplyRequestError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequestKey=" + key);
            }

            if (!item.GetSecretKey().Equals(secret, StringComparison.InvariantCultureIgnoreCase))
            {
                result.Errors.Add(ValidateApplyRequestError.InvalidSecret, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath, "ApplyRequest=" + item);
            }

            if (item.Status == ApplyRequestStatus.Accepted)
            {
                result.Succeed = true;
                result.GoUrl = this.GetPasswordRecoveryUrl(item.CreatedUserId.Value);
            }
            else if (item.Status == ApplyRequestStatus.Refused)
            {
                result.Errors.Add(ValidateApplyRequestError.Refused, NetworksEnumMessages.ResourceManager);
            }
            else if (item.Status == ApplyRequestStatus.PendingAccept)
            {
                result.Errors.Add(ValidateApplyRequestError.PendingAccept, NetworksEnumMessages.ResourceManager);
            }
            else
            {
                result.Errors.Add(ValidateApplyRequestError.UnknownState, NetworksEnumMessages.ResourceManager);
            }

            return this.LogResult(result, logPath, "ApplyRequest=" + item);
        }

        public IList<UsersView> GetAllUsersViewActive()
        {
            return this.Repo.People.GetAllUsersViewActive(this.Services.NetworkId);
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="withLastActivity">if set to <c>true</c> [with last activity].</param>
        /// <param name="withProfileFields">The with profile fields.</param>
        /// <returns></returns>
        public IList<UserModel> GetAll(
            bool withLastActivity = false,
            ProfileFieldType[] withProfileFields = null)
        {
            var items = this.Repo.People.GetUsersView(this.Services.NetworkId);
            var result = items.Values
                .Select(u =>
                {
                    var model = new UserModel(u);
                    model.IsActive = this.IsActive(u);
                    return model;
                })
                .ToList();

            if (withLastActivity)
            {
                var activities = this.Services.Live.GetAllLastActivityDate();
                foreach (var user in result)
                {
                    if (activities.ContainsKey(user.Id))
                    {
                        user.LastActivityUtc = activities[user.Id].AsLocal().ToUniversalTime();
                    }
                }
            }

            if (withProfileFields != null)
            {
                var userIds = items.Keys.ToArray();
                var values = this.Repo.UserProfileFields.GetByUserIdAndFieldType(userIds, withProfileFields);
                foreach (var user in result)
                {
                    if (values.ContainsKey(user.Id))
                    {
                        user.SetProfileFields(values[user.Id]);
                    }
                }
            }

            return result;
        }

        public void FillRegionSettingsRequestLists(RegionSettingsRequest model)
        {
            model.AvailableTimezones = new Dictionary<string, string> { { "", "" }, };
            TimeZoneInfo.GetSystemTimeZones().ToList().ForEach(o => { model.AvailableTimezones.Add(o.Id, o.DisplayName); });

            model.AvailableCultures = new Dictionary<string, string> { { "", "" }, };
            this.Services.SupportedCultures.Where(o => !o.IsNeutralCulture).ToList().ForEach(o => { model.AvailableCultures.Add(o.Name, o.DisplayName); });
        }

        public RegionSettingsRequest GetRegionSettingsRequest(int userId)
        {
            var request = new RegionSettingsRequest();

            var user = this.SelectWithId(userId);

            request.TimezoneId = user.Timezone;
            request.CultureName = user.Culture;
            request.DefaultTimezone = this.Services.DefaultTimezone;

            this.FillRegionSettingsRequestLists(request);

            return request;
        }

        public RegionSettingsResult SaveRegionSettings(RegionSettingsRequest request, int userId)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleServices.SaveRegionSettings";
            var result = new RegionSettingsResult(request);

            var user = this.SelectWithId(userId);
            if (user == null || !this.IsActive(user))
            {
                result.Errors.Add(RegionSettingsError.NoSuchUserOrInactive, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            if (!string.IsNullOrEmpty(request.TimezoneId) && !TimeZoneInfo.GetSystemTimeZones().Any(o => o.Id == request.TimezoneId))
            {
                result.Errors.Add(RegionSettingsError.TimezoneNotSupported, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }
            if (!string.IsNullOrEmpty(request.CultureName) && !this.Services.SupportedCultures.Any(o => o.Name == request.CultureName))
            {
                result.Errors.Add(RegionSettingsError.CultureNotSupported, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            user.Timezone = request.TimezoneId;
            user.Culture = request.CultureName;
            this.Update(user);

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public ConnectWithLinkedInResult GetUserFromLinkedInId(ConnectWithLinkedInRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "PeopleServices.GetUserFromLinkedInId";
            var result = new ConnectWithLinkedInResult(request);

            if (!this.Services.AppConfiguration.Tree.Externals.LinkedIn.AllowLogin)
            {
                result.Errors.AddDetail(ConnectWithLinkedInError.LinkedInLoginIsDisabled, "Configuration does not allow login. ", NetworksEnumMessages.ResourceManager);
                return result;
            }

            var linkedInState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.LinkedIn);
            if (linkedInState == null || linkedInState.Entity == null ||
                string.IsNullOrEmpty(linkedInState.Entity.OAuthAccessToken) ||
                string.IsNullOrEmpty(linkedInState.Entity.OAuthAccessSecret))
            {
                result.Errors.Add(ConnectWithLinkedInError.LinkedInNotConfigured, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // API call
            var apiConfig = new LinkedInApiConfiguration(linkedInState.Entity.OAuthAccessToken, linkedInState.Entity.OAuthAccessSecret);
            var linkedInApi = new LinkedInApi(apiConfig);

            var cultures = new List<CultureInfo>();
            cultures.AddRange(this.Services.SupportedCultures);

            var profileFields = FieldSelector.For<LinkedInNET.Profiles.Person>()
                .WithId()
                .WithEmailAddress();

            Person linkedInPerson;
            var apiCallWatch = Stopwatch.StartNew();
            try
            {
                linkedInPerson = linkedInApi.Profiles.GetMyProfile(new UserAuthorization(request.AccessToken.AccessToken), cultures.Select(o => o.Name).ToArray(), profileFields);
                apiCallWatch.Stop();
                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "linkedInApi.Profiles.GetMyProfile took  " + apiCallWatch.Elapsed);
            }
            catch (LinkedInApiException ex)
            {
                apiCallWatch.Stop();
                this.Services.Logger.Error(
                    logPath,
                    ErrorLevel.ThirdParty,
                    "Api call failed in " + apiCallWatch.Elapsed + " with the exception: {0}",
                    ex.ToString());

                result.Errors.Add(ConnectWithLinkedInError.ApiCallFailed, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // match a linkedInId
            var user = this.Repo.People.GetActiveByLinkedInId(linkedInPerson.Id);
            if (user == null || (user.NetworkId != this.Services.NetworkId && !user.NetworkAccess.HasFlag(NetworkAccessLevel.SparkleStaff)))
            {
                // if linkedIn email match a user
                user = this.Repo.People.GetActiveByEmail(linkedInPerson.EmailAddress);
                if (user == null || (user.NetworkId != this.Services.NetworkId && !user.NetworkAccess.HasFlag(NetworkAccessLevel.SparkleStaff)))
                {
                    result.Errors.Add(ConnectWithLinkedInError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath, "no user match at level 2; " + (user != null ? ("user.id=" + user.Id) : "user=null"));
                }

                result.Errors.Add(ConnectWithLinkedInError.UserEmailMatch, NetworksEnumMessages.ResourceManager);
                result.LinkedInUserId = linkedInPerson.Id;
                result.LinkedInEmail = linkedInPerson.EmailAddress;
                result.PartialMatch = true;
                return this.LogResult(result, logPath, "no user match at level 1; " + (user != null ? ("user.id=" + user.Id) : "user=null"));
            }

            result.UserMatch = user;

            result.Succeed = true;
            return this.LogResult(result, logPath, "user match; " + (user != null ? ("user.id=" + user.Id) : "user=null"));
        }

        public void TryUpdateLinkedInId(User user, string linkedInId, string email)
        {
            if (user.Email != email || !this.IsLinkedInIdAvailable(linkedInId))
                return;

            user.LinkedInUserId = linkedInId;
            this.Repo.People.Update(user);
        }

        public IList<UserModel> GetNotSubscribedUsers(bool includeInactive)
        {
#warning Performance issue: fetching MANY rows.
            // TODO: Performance issue: the best would be a stored procedure here. with pagination.
            var activeSubscriptions = this.Services.Subscriptions.GetActiveUserSubscriptions();

            var result = this.Repo.People.GetAllWithoutIds(
                this.Services.NetworkId,
                activeSubscriptions
                    .Select(o => o.AppliesToUserId.Value)
                    .Distinct()
                    .ToArray(),
                PersonOptions.Company);

            return result
                .Select(o => new UserModel(o))
                .Where(u => includeInactive || u.IsActive)
                .ToList();
        }

        public IList<UsersView> GetUsersViewFromCompany(int companyId)
        {
            return this.PeopleRepository.GetUsersViewByCompanyId(companyId);
        }

        public IDictionary<int, UsersView> GetUsersViewById(int[] userIds)
        {
            return this.PeopleRepository.GetUsersViewById(userIds, this.Services.NetworkId);
        }

        public IDictionary<string, UsersView> GetUsersViewByLogin(string[] userLogins)
        {
            return this.PeopleRepository.GetUsersViewByLogin(userLogins, this.Services.NetworkId);
        }

        public void CleanNotSubmittedApplyRequests()
        {
            this.Repo.ApplyRequests.DeleteAllNotSubmitted(this.Services.NetworkId);
        }

        public string GetInviteWithApplyUrl(int inviterUserId, int networkId)
        {
            var inviteCode = new Sparkle.Services.Networks.Objects.ApplyInviteCode(inviterUserId, DateTime.UtcNow, networkId);
            var query = new Dictionary<string, string>
            {
                { "InviterCode", inviteCode.ToEncryptedHex().ToLower() },
            };

            return this.Services.GetUrl("Apply", query);
        }

        public bool IsApplyInviterCodeValid(string code)
        {
            try
            {
                var inviterCode = Sparkle.Services.Networks.Objects.ApplyInviteCode.FromHex(code);
                if (inviterCode == null
                    || inviterCode.NetworkId != this.Services.NetworkId
                    || this.SelectWithId(inviterCode.UserId) == null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                this.Services.Logger.Error("PeopleServices.IsApplyInviterCodeValid", ErrorLevel.ThirdParty, ex);
            }

            return false;
        }

        public UserModel WhoUsesThisEmail(string email)
        {
            var item = this.Repo.People.WhoUsesThisEmail(this.Services.NetworkId, email);
            if (item == null)
                return null;

            return new UserModel(item);
        }

        public IList<SearchResultModel<UserModel>> Search(string query, bool searchName = true, bool searchEmail = false, bool searchTags = false, bool includeInactive = false)
        {
            var list = new List<SearchResultModel<UserModel>>();

            var splitter = new StringSplitter();
            var words = splitter.Split(query)
                .Where(s => s.SplitType == Split1Type.Word)
                .Select(s => s.Value)
                .ToArray();

            foreach (var item in this.Services.Cache.AllUsers.Values)
            {
                if (!item.IsActive && !includeInactive)
                    continue;

                if (searchName)
                {
                    var strings = item.GetAspect<UserModel, StringSearchAspect>();
                    if (strings != null && strings.Contains(words))
                    {
                        list.Add(new SearchResultModel<UserModel>(item));
                        continue;
                    }
                }
            }

            return list;
        }

        public IDictionary<int, UserModel> GetAllForCache()
        {
            var items = this.Repo.People.GetUsersView(this.Services.NetworkId);
            return this.PrepareCacheItems(items);
        }

        public IDictionary<int, UserModel> GetForCache(int[] ids)
        {
            var items = this.Repo.People.GetUsersViewById(ids);
            return this.PrepareCacheItems(items);
        }

        public IList<UserModel> GetLatest(int count)
        {
            var minDate = DateTime.UtcNow.AddDays(-7D).AddYears(-1);
            var ids = this.Repo.People.GetLatestIds(this.Services.NetworkId, count, minDate);
            var items = this.Services.Cache.GetUsers(ids);
            return new List<UserModel>(items.Values);
        }

        public void Refresh(IEnumerable<UserModel> items, bool refreshPictureUrl = false, bool useFullUrls = false)
        {
            foreach (var item in items)
            {
                this.Refresh(item, refreshPictureUrl, useFullUrls);
            }
        }

        public void Refresh(UserModel item, bool refreshPictureUrl = false, bool useFullUrls = false)
        {
            UriKind uriKind = useFullUrls ? UriKind.Absolute : UriKind.Relative;

            if (refreshPictureUrl)
            {
                item.SmallProfilePictureUrl = this.GetProfilePictureUrl(item.Username, UserProfilePictureSize.Small, uriKind);
                item.MediumProfilePictureUrl = this.GetProfilePictureUrl(item.Username, UserProfilePictureSize.Medium, uriKind);
            }
        }

        public NetworkUserGender GetDefaultGender()
        {
            NetworkUserGender value;
            if (Enum.TryParse<NetworkUserGender>(this.Services.AppConfiguration.Tree.Features.Users.DefaultGender, out value))
            {
                return value;
            }

            return NetworkUserGender.Unspecified;
        }

        public InviteWithApplyResult InviteWithApply(InviteWithApplyRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            bool noNeedApproval = false;
            int? companyCategoryId = null;
            int? companyRelationshipTypeId = null;
            var result = new InviteWithApplyResult(request);

            var actingUser = this.GetByIdFromAnyNetwork(request.ActingUserId, PersonOptions.Company);
            if (actingUser != null)
            {
                if (actingUser.IsActive)
                {
                    if (request.SkipApproval)
                    {
                        var nwkFlags = new NetworkAccessLevel[] { NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff, };
                        var cpnFlags = new CompanyAccessLevel[] { CompanyAccessLevel.Administrator, };
                        if (actingUser.NetworkAccessLevel.Value.HasAnyFlag(nwkFlags))
                        {
                            noNeedApproval = true;
                        }
                        else
                        {
                            result.Errors.Add(InviteWithApplyError.NotAuthorized, NetworksLabels.ResourceManager);
                        }
                    }
                }
                else
                {
                    result.Errors.Add(InviteWithApplyError.NotAuthorized, NetworksLabels.ResourceManager);
                }
            }
            else
            {
                result.Errors.Add(InviteWithApplyError.NoSuchActingUser, NetworksLabels.ResourceManager);
            }

            EmailAddress email = null;
            if ((email = EmailAddress.TryCreate(request.Email)) != null)
            {
                var report = this.GetEmailAddressReport(email, false);
                if (report.MainAddressUsersOnNetwork > 0 || report.MainAddressUsersOtherNetworks > 0)
                {
                    result.Errors.Add(InviteWithApplyError.EmailAddressInUse, NetworksLabels.ResourceManager);
                }
            }
            else
            {
                result.Errors.Add(InviteWithApplyError.InvalidEmailAddress, NetworksLabels.ResourceManager);
            }

            if (request.CompanyCategoryId != null)
            {
                var category = this.Services.Company.GetCategoryById(request.CompanyCategoryId.Value);
                if (category != null)
                {
                    if (category.NetworkId == this.Services.NetworkId)
                    {
                        companyCategoryId = category.Id;
                    }
                    else
                    {
                        result.Errors.Add(InviteWithApplyError.InvalidCompanyCategory, NetworksLabels.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(InviteWithApplyError.InvalidCompanyCategory, NetworksLabels.ResourceManager);
                }
            }

            if (request.CompanyRelationshipTypeId != null)
            {
                var relationship = this.Services.CompanyRelationships.GetTypeById(request.CompanyRelationshipTypeId.Value);
                if (relationship != null)
                {
                    if (relationship.NetworkId == this.Services.NetworkId)
                    {
                        companyRelationshipTypeId = relationship.Id;
                    }
                    else
                    {
                        result.Errors.Add(InviteWithApplyError.InvalidCompanyRelationship, NetworksLabels.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.Add(InviteWithApplyError.InvalidCompanyRelationship, NetworksLabels.ResourceManager);
                }
            }

            if (result.Errors.Count > 0)
                return result;

            var item = new ApplyRequest();
            var key = Guid.NewGuid();
            var now = DateTime.UtcNow;
            item.NetworkId = this.Services.NetworkId;
            item.Key = key;
            item.DateCreatedUtc = now;
            item.DateInvitedUtc = now;
            item.InvitedByUserId = actingUser.Id;

            var model = new ApplyRequestModel(item);
            model.UserDataModel.User.Email = email.Value;

            if (noNeedApproval)
            {
                model.ProcessDataModel.IsApprovedBy = actingUser.Id;
                item.ProcessData = model.ProcessData;
            }

            if (companyCategoryId != null)
            {
                model.ProcessDataModel.CompanyCategoryId = (short)companyCategoryId.Value;
                item.ProcessData = model.ProcessData;
            }

            if (companyRelationshipTypeId != null)
            {
                model.ProcessDataModel.CompanyRelationships.Add(new Sparkle.Entities.Networks.Neutral.CompanyRelationshipPoco
                {
                    MasterId = actingUser.CompanyId,
                    TypeId = companyRelationshipTypeId.Value,
                });
                item.ProcessData = model.ProcessData;
            }

            item.Data = model.Data;

            this.Repo.ApplyRequests.Insert(item);

            result.Succeed = true;
            result.Item = new ApplyRequestModel(item);

            var query = new Dictionary<string, string> { { "Key", key.ToString() }, };
            var inviteUrl = this.Services.GetUrl("Apply", query);
            this.Services.Email.SendInvitationWithApply(actingUser, new UserModel { Email = email, }, inviteUrl);
            this.Services.Logger.Info(
                "PeopleService.InviteWithApply",
                ErrorLevel.Success,
                "Succesfuly invited " + email);

            return result;
        }

        public UserModel GetById(int id, PersonOptions options)
        {
            var item = this.GetEntityByIdInNetwork(id, options);
            if (item == null)
                return null;

            if ((options & PersonOptions.ProfileFields) == PersonOptions.ProfileFields)
            {
                var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(item.Id);
                return item != null ? new UserModel(item, profileFields) : null;
            }
            else
            {
                return item != null ? new UserModel(item) : null;
            }
        }

        public UserModel GetByUsername(string username, PersonOptions options)
        {
            var item = this.Repo.People.GetByLogin(username, this.Services.NetworkId, options);
            if (item == null)
                return null;

            if ((options & PersonOptions.ProfileFields) == PersonOptions.ProfileFields)
            {
                var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(item.Id);
                return item != null ? new UserModel(item, profileFields) : null;
            }
            else
            {
                return item != null ? new UserModel(item) : null;
            }
        }

        public IList<UserModel> GetByIds(int[] ids, PersonOptions options)
        {
            var items = this.Repo.People.GetByIds(ids, this.Services.NetworkId, options);

            if ((options & PersonOptions.ProfileFields) == PersonOptions.ProfileFields)
            {
                var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserIds(ids);
                return items.Select(o => new UserModel(o, profileFields.ContainsKey(o.Id) ? profileFields[o.Id] : null)).ToList();
            }
            else
            {
                return items.Select(o => new UserModel(o)).ToList();
            }
        }

        public ApplyRequestModel GetUsersApplyRequest(int userId)
        {
            var item = this.Repo.ApplyRequests.GetByUserId(userId).SingleOrDefault();
            if (item == null)
                return null;

            return new ApplyRequestModel(item);
        }

        public void UpdateApplyRequestData(ApplyRequestModel model)
        {
            var item = this.Repo.ApplyRequests.GetByKey(model.Key, this.Services.NetworkId);
            if (item != null)
            {
                item.Data = model.Data;
                item.CompanyData = model.CompanyData;
                item.ProcessData = model.ProcessData;
                this.Repo.ApplyRequests.Update(item);
            }
        }

        public AdminProceduresResult ChangeUserCompany(AdminProceduresRequest request, int actingUserId)
        {
            if (request == null)
                throw new ArgumentNullException("This value cannot be empty.", "request");

            const string logPath = "PeopleService.ChangeUserCompany";
            var result = new AdminProceduresResult(request);

            var user = this.SelectWithLogin(request.Login);
            if (user == null)
            {
                result.Errors.Add(AdminProceduresError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var newCompany = this.Services.Company.GetById(request.NewCompanyId);
            if (newCompany == null)
            {
                result.Errors.Add(AdminProceduresError.NoSuchCompany, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            user.CompanyID = newCompany.ID;
            user.CompanyAccess = request.NewRight;
            this.Repo.People.Update(user);
            this.Services.Logger.Info(logPath, ErrorLevel.Success, "User " + user.Id + " moved in company " + newCompany.ID + " successfuly.");

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public TeamPageModel GetTeamPageModel()
        {
            var teamGroupFields = this.Services.ProfileFields.GetUserProfileFieldsByType(ProfileFieldType.NetworkTeamGroup).ToDictionary(o => o.UserId, o => o);
            var users = this.Repo.People.GetUsersViewActiveByIds(teamGroupFields.Select(o => o.Key).ToArray(), this.Services.NetworkId);
            var userIds = users.Select(o => o.Id).ToArray();

            var teamRoleFields = this.Services.ProfileFields.GetUniqueUserProfileFieldsByUserIdsAndType(userIds, ProfileFieldType.NetworkTeamRole);
            var teamDescFields = this.Services.ProfileFields.GetUniqueUserProfileFieldsByUserIdsAndType(userIds, ProfileFieldType.NetworkTeamDescription);
            var teamGroupOrderFields = this.Services.ProfileFields.GetUniqueUserProfileFieldsByUserIdsAndType(userIds, ProfileFieldType.NetworkTeamOrder);

            var usersTeamMembers = users
                .Select(o =>
                {
                    var user = new UserModel(o)
                    {
                        Picture = this.GetProfilePictureUrl(o.Login, UserProfilePictureSize.Medium, UriKind.Relative),
                    };
                    return new UserTeamMemberModel(
                        user,
                        teamRoleFields.ContainsKey(o.Id) ? teamRoleFields[o.Id] : null,
                        teamDescFields.ContainsKey(o.Id) ? teamDescFields[o.Id] : null,
                        teamGroupFields[o.Id],
                        teamGroupOrderFields[o.Id]);
                })
                .GroupBy(o => o.NetworkTeamGroup)
                .OrderBy(o => o.Key)
                .ToDictionary(o => o.Key, o => (IList<UserTeamMemberModel>)o.OrderBy(r => r.NetworkTeamOrder).ThenBy(r => r.User.FirstName).ToList());

            var networkRoles = this.Repo.People.GetAllUsersForRolesStats(this.Services.NetworkId);
            var usersWithoutRole = new List<UserModel>();
            foreach (var item in networkRoles)
            {
                if (!usersTeamMembers.ContainsKey(item.Id))
                {
                    usersWithoutRole.Add(new UserModel(item) { Picture = this.GetProfilePictureUrl(item.Login, UserProfilePictureSize.Small, UriKind.Relative), });
                }
            }

            return new TeamPageModel(usersTeamMembers, usersWithoutRole);
        }

        public EditNetworkRoleRequest GetEditNetworkRoleRequest(string login)
        {
            var user = this.GetActiveByLogin(login, PersonOptions.None);
            if (user == null)
                return null;

            var request = new EditNetworkRoleRequest();
            request.Login = user.Login;
            request.Firstname = user.FirstName;
            request.Lastname = user.LastName;
            request.PictureUrl = this.GetProfilePictureUrl(user.Login, UserProfilePictureSize.Medium, UriKind.Relative);

            UserProfileFieldModel profileFieldTemp = null;
            var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(user.Id);
            if ((profileFieldTemp = profileFields.SingleByType(ProfileFieldType.NetworkTeamRole)) != null)
            {
                request.RoleTitle = profileFieldTemp.Value;
            }
            if ((profileFieldTemp = profileFields.SingleByType(ProfileFieldType.NetworkTeamDescription)) != null)
            {
                request.RoleDescription = profileFieldTemp.Value;
            }

            if ((profileFieldTemp = profileFields.SingleByType(ProfileFieldType.NetworkTeamGroup)) != null)
            {
                request.ActualGroup = int.Parse(profileFieldTemp.Value);
            }

            request.ExistingRoleNames = this.Services.ProfileFields.GetUserProfileFieldsByType(ProfileFieldType.NetworkTeamRole).Select(o => o.Value).Distinct().OrderBy(o => o).ToArray();

            var teamGroupFields = this.Services.ProfileFields.GetUserProfileFieldsByType(ProfileFieldType.NetworkTeamGroup).ToDictionary(o => o.UserId, o => o);
            var users = this.Repo.People.GetUsersViewActiveByIds(teamGroupFields.Select(o => o.Key).ToArray(), this.Services.NetworkId);
            var userIds = users.Select(o => o.Id).ToArray();
            var teamGroupOrderFields = this.Services.ProfileFields.GetUniqueUserProfileFieldsByUserIdsAndType(userIds, ProfileFieldType.NetworkTeamOrder);
            var usersTeamMembers = users
                .Select(o => new UserTeamMemberModel(new UserModel(o), null, null, teamGroupFields[o.Id], teamGroupOrderFields[o.Id]))
                .GroupBy(o => o.NetworkTeamGroup)
                .OrderBy(o => o.Key)
                .ToDictionary(o => o.Key, o => (IList<UserTeamMemberModel>)o.OrderBy(r => r.NetworkTeamOrder).ThenBy(r => r.User.FirstName).ToList());
            request.Groups = usersTeamMembers
                .ToDictionary(
                    o => o.Key,
                    o => string.Format("{0} - ({1})", o.Key, string.Join(" ; ", o.Value.Select(i => i.User.FirstName).ToArray())));
            var lastGroup = usersTeamMembers.Count > 0 ? usersTeamMembers.Last().Key : 0;
            request.Groups.Add(lastGroup + 1, this.Services.Lang.T("Nouveau groupe"));

            return request;
        }

        public EditNetworkRoleResult UpdateNetworkRole(EditNetworkRoleRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("This value cannot be null.", "request");

            const string logPath = "PeopleService.UpdateNetworkRole";
            var result = new EditNetworkRoleResult(request);

            var user = this.GetActiveByLogin(request.Login, PersonOptions.None);
            if (user == null)
            {
                result.Errors.Add(EditNetworkRoleError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            if (!string.IsNullOrEmpty(request.RoleTitle) && request.RoleTitle.Length > 120)
            {
                result.Errors.Add(EditNetworkRoleError.RoleTitleTooLong, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }
            if (!string.IsNullOrEmpty(request.RoleDescription) && request.RoleDescription.Length > 4000)
            {
                result.Errors.Add(EditNetworkRoleError.RoleDescriptionTooLong, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }
            if (!request.ActualGroup.HasValue || request.ActualGroup < 0)
            {
                result.Errors.Add(EditNetworkRoleError.MustSpecifyGroup, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var source = ProfileFieldSource.None;
            this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamRole, request.RoleTitle, source);
            this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamDescription, request.RoleDescription, source);
            this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamGroup, request.ActualGroup.Value.ToString(), source);
            this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamOrder, "1", source);

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public void DeleteUserNetworkRole(string login)
        {
            var user = this.SelectWithLogin(login, PersonOptions.None);
            if (user != null && this.Services.NetworkId == user.NetworkId)
            {
                this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamRole, null);
                this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamDescription, null);
                this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamGroup, null);
                this.Services.ProfileFields.SetUserProfileField(user.Id, ProfileFieldType.NetworkTeamOrder, null);
            }
        }

        public void EditTeamNetworkGroupOrder(int[] order)
        {
            // Get NetworkTeamGroup fields
            var groupFields = this.Services
                .ProfileFields
                .GetUserProfileFieldsByTypeAndNetworkId(ProfileFieldType.NetworkTeamGroup, this.Services.NetworkId)
                .GroupBy(o => int.Parse(o.Value))
                .ToDictionary(o => o.Key, o => o.ToList());
            int i = 1;
            foreach (var item in order)
            {
                var userFields = groupFields.ContainsKey(item) ? groupFields[item] : null;
                if (userFields != null)
                {
                    var newVal = i.ToString();
                    foreach (var field in userFields)
                    {
                        this.Services.ProfileFields.SetUserProfileField(field.UserId, ProfileFieldType.NetworkTeamGroup, newVal, ProfileFieldSource.None);
                    }

                    i++;
                }
            }
        }

        public void EditTeamNetworkPeopleOrder(int[] person)
        {
            // Verify users are within the network (person is given directly to controller)
            var users = this.Services.People.GetUsersViewById(person).Where(o => o.Value.NetworkId == this.Services.NetworkId).ToList();
            var userIds = users.Select(o => o.Value.Id).ToArray();
            person = person.Where(o => userIds.Contains(o)).ToArray();

            // Get NetworkTeamOrder fields
            var orderFields = this.Services
                .ProfileFields
                .GetUniqueUserProfileFieldsByUserIdsAndType(person, ProfileFieldType.NetworkTeamOrder);
            int i = 1;
            foreach (var item in person)
            {
                var userField = orderFields.ContainsKey(item) ? orderFields[item] : null;
                if (userField != null)
                {
                    this.Services.ProfileFields.SetUserProfileField(userField.UserId, ProfileFieldType.NetworkTeamOrder, i.ToString(), ProfileFieldSource.None);
                    i++;
                }
            }
        }

        public AjaxTagPickerModel GetAjaxTagPickerModel(User person, User actingUser)
        {
            var isSamePerson = person.Id == actingUser.Id;
            var isNetworkAdmin = actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);

            var categories = this.Services.Tags.GetCategoriesApplyingToUsers();
            var tags = this.Services.Tags.GetTagsByUserIdAndCategories(person.Id, categories);
            return new AjaxTagPickerModel(tags, isSamePerson || isNetworkAdmin);
        }

        public IDictionary<int, int> GetProfileFieldsCount()
        {
            var items = this.Repo.UserProfileFields.GetUsersCount(this.Services.NetworkId);
            return items;
        }

        public void UpdatePresence(int userId, DateTime day, DateTime now)
        {
            this.Repo.UserPresences.UpdateUserPresence(userId, day, now);
        }

        public int GetUserPresenceDaysCount(int userId)
        {
            return this.Repo.UserPresences.CountUsersDays(userId);
        }

        public UserPresenceStats GetUserPresenceStats()
        {
            var line = this.Repo.UserPresences.GetUserPresenceStats(this.Services.NetworkId);
            var model = new UserPresenceStats();
            model.PerDayPresenceAverageUsers = line.PerDayPresenceAverageUsers;
            model.PerDayPresenceMaxUsers = line.PerDayPresenceMaxUsers;
            model.UserPresenceAverageDays = line.UserPresenceAverageDays;
            model.UserPresenceMaxDays = line.UserPresenceMaxDays;
            model.TotalUsers = line.TotalUsers;
            model.TotalDays = line.TotalDays;

            return model;
        }

        public SetSingleProfileFieldResult SetSingleProfileFieldOnApply(SetSingleProfileFieldRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string logPath = "PeopleService.SetSingleProfileFieldOnApply";
            var result = new SetSingleProfileFieldResult(request);

            Guid key;
            if (request.ApplyKey == null || request.ApplyKey.Value.Equals(Guid.Empty))
            {
                result.Errors.Add(SetSingleProfileFieldError.Invalid, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }
            else
            {
                key = request.ApplyKey.Value;
            }

            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                // load or create
                var item = this.Repo.ApplyRequests.GetByKey(key, this.Services.NetworkId);
                if (item == null)
                {
                    item = new ApplyRequest
                    {
                        Key = key,
                        DateCreatedUtc = DateTime.UtcNow,
                        UserRemoteAddress = request.UserRemoteAddress,
                        NetworkId = this.Services.NetworkId,
                    };
                }
                else if (item.UserRemoteAddress == null)
                {
                    item.UserRemoteAddress = request.UserRemoteAddress;
                }


                var model = new ApplyRequestModel(item);
                if (model.Status != ApplyRequestStatus.New)
                {
                    result.Errors.Add(SetSingleProfileFieldError.RequestIsNotNew, NetworksEnumMessages.ResourceManager);
                }

                var field = tran.Repositories.ProfileFields.GetById(request.ProfileFieldId);

                if (field == null)
                {
                    result.Errors.Add(SetSingleProfileFieldError.NoSuchProfileField, NetworksEnumMessages.ResourceManager);
                }
                else
                {
                    if ("Company".Equals(request.Target))
                    {
                        if (!field.ApplyToCompanies)
                        {
                            result.Errors.Add(SetSingleProfileFieldError.InvalidTargetForThisProfileField, NetworksEnumMessages.ResourceManager);
                        }
                    }
                    ////else if ("User".Equals(request.Target)) // not implemented yet
                    ////{
                    ////    if (!field.ApplyToUsers)
                    ////    {
                    ////        result.Errors.Add(SetSingleProfileFieldError.InvalidTargetForThisProfileField, NetworksEnumMessages.ResourceManager);
                    ////    }
                    ////}
                    else
                    {
                        result.Errors.Add(SetSingleProfileFieldError.InvalidTarget, NetworksEnumMessages.ResourceManager);
                    }
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, logPath, "ApplyRequest=" + item);
                }

                if ("Company".Equals(request.Target))
                {
                    var fieldValue = model.CompanyDataModel.CompanyFields.FirstOrDefault(x => x.ProfileFieldId.Equals(field.Id));
                    if (fieldValue == null)
                    {
                        fieldValue = new Neutral.CompanyProfileFieldPoco
                        {
                            ProfileFieldId = field.Id,
                            SourceType = ProfileFieldSource.UserInput,
                        };
                        model.CompanyDataModel.CompanyFields.Add(fieldValue);
                    }

                    fieldValue.Value = request.Value;
                }

                item.Data = model.Data;
                item.CompanyData = model.CompanyData;

                if (item.Id == 0)
                {
                    tran.Repositories.ApplyRequests.Insert(item);
                    model.Id = item.Id;
                }
                else
                {
                    tran.Repositories.ApplyRequests.Update(item);
                }


                result.Succeed = true;
                tran.CompleteTransaction();

                return this.LogResult(result, logPath, "ApplyRequest=" + item);
            }
        }

        public UserModel GetFirst(PersonOptions options)
        {
            var entity = this.Repo.People.GetFirst(this.Services.NetworkId, options);
            if (entity != null)
            {
                var model = new UserModel(entity);
                return model;
            }
            else
            {
                return null;
            }
        }

        public DateTime? GetLastUserPresence(int userId)
        {
            var item = this.Repo.UserPresences.GetLastByUserId(userId);
            if (item == null)
                return null;

            return item.TimeTo.AsUtc();
        }

        internal static void RegisterTags(TagsService tags)
        {
            tags.RegisterEntityValidator("User", PeopleService.ValidateEntity);
            tags.RegisterTagValidator("User", PeopleService.ValidateTag);
            tags.RegisterTagRepository("User", EntityWithTagRepositoryType.Sql, "UserTags", "UserId");
            tags.RegisterEntityValidator("ApplyRequestCompany", PeopleService.ValidateApplyRequestEntity);
            tags.RegisterTagValidator("ApplyRequestCompany", PeopleService.ValidateApplyRequestCompanyTag);
            tags.RegisterTagRepository("ApplyRequestCompany", EntityWithTagRepositoryType.ApplyRequest, "Company", null);
        }

        private EmailAddressReportModel GetEmailAddressReport(EmailAddress email, bool bypassTag)
        {
            var report = new EmailAddressReportModel(email);

            report.MainAddressUsersOnNetwork = this.Repo.People.GetByEmail(email.Value, this.Services.NetworkId, PersonOptions.None) != null ? 1 : 0;
            report.MainAddressUsersOtherNetworks = this.Repo.People.GetByEmailOtherNetwork(email.Value, this.Services.NetworkId).Count;
            report.PersonalAddressUsers = this.Repo.People.GetByPersonalEmail(email.Value).Count;

            var companyEmailFields = this.Repo.CompanyProfileFields.GetByFieldTypeAndValue(ProfileFieldType.Email, email.Value);
            report.CompanyContacts = companyEmailFields.Count;

            // TODO: search in ApplyRequests

            if (bypassTag)
            {
                if (email.TagPart != null)
                {
                    report.MainAddressUsersOnNetwork += this.Repo.People.GetByEmail(email.ValueWithoutTag, this.Services.NetworkId, PersonOptions.None) != null ? 1 : 0;
                    report.MainAddressUsersOtherNetworks += this.Repo.People.GetByEmailOtherNetwork(email.ValueWithoutTag, this.Services.NetworkId).Count;
                    report.PersonalAddressUsers += this.Repo.People.GetByPersonalEmail(email.ValueWithoutTag).Count;

                    companyEmailFields = this.Repo.CompanyProfileFields.GetByFieldTypeAndValue(ProfileFieldType.Email, email.ValueWithoutTag);
                    report.CompanyContacts = companyEmailFields.Count;

                    // TODO: search in ApplyRequests
                }
                else
                {
                    // TODO: find items that have a tag with the specified account and domain
                }
            }

            return report;
        }

        private void UpdateProfileFields(ProfileEditRequest request, ProfileFieldSource source)
        {
            var customHandledFields = new List<int>();
            customHandledFields.Add((int)ProfileFieldType.Email);

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.ZipCode.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.ZipCode);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.ZipCode, request.ZipCode.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.City.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.City);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.City, request.City.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.Phone.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.Phone);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.Phone, request.Phone.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.About.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.About);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.About, request.About.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.CurrentTarget.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.CurrentTarget);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.CurrentTarget, request.CurrentTarget.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.FavoriteQuotes.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.FavoriteQuotes);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.FavoriteQuotes, request.FavoriteQuotes.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.Contribution.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.Contribution);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.Contribution, request.Contribution.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.Industry.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.Industry);
                // this is handled by the calling method
                ////this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.Industry, request.IndustryLibelle.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.Country.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.Country);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.Country, request.CountryId.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.ContactGuideline.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.ContactGuideline);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.ContactGuideline, request.ContactGuideline.NullIfEmptyOrWhitespace(), source);
            }

            if (request.ProfileFields != null && request.ProfileFields.Contains(ProfileFieldType.LinkedInPublicUrl.ToString()))
            {
                customHandledFields.Add((int)ProfileFieldType.LinkedInPublicUrl);
                this.Services.ProfileFields.SetUserProfileField(request.Id, ProfileFieldType.LinkedInPublicUrl, request.LinkedInPublicUrl.NullIfEmptyOrWhitespace(), source);
            }

            var allUserFields = this.Services.ProfileFields.GetUserFields();
            foreach (var field in allUserFields)
            {
                if (customHandledFields.Contains(field.Id))
                {
                    // this field has been handled above
                    continue;
                }

                var values = request.ProfileFieldValues != null ? request.ProfileFieldValues.Where(x => x.TypeId == field.Id).ToArray() : null;
                if (values == null || values.Length == 0)
                {
                    // field is not part of the request. do nothing.
                }
                else if (values.Length == 1)
                {
                    var userField = values[0];
                    var value = userField.Value.NullIfEmptyOrWhitespace();

                    if (field.AvailableValuesCount > 0 && value != null)
                    {
                        var availableValue = this.Repo.ProfileFieldsAvailiableValues.GetByValue((ProfileFieldType)field.Id, userField.Value);
                        if (availableValue == null)
                        {
                            // the user chose a value that is not allowed. skip.
                            continue;
                        }
                    }

                    var result = this.Services.ProfileFields.SetUserProfileField(request.Id, (ProfileFieldType)field.Id, value, source);
                }
                else
                {
                    // too many values to handle. right now we only support 1 value for each field.
                }
            }
        }

        private void FillProfileEditList(ProfileEditRequest model)
        {
            var user = this.SelectWithId(model.Id);

            // jobs
            IList<Job> jobs = new List<Job> { new Job { Alias = "", Libelle = "", Id = 0 }, };
            jobs.AddRange(this.Services.Job.SelectAll());
            model.Jobs = jobs.Select(o => new JobModel(o)).ToList();

            // tags V1 (obsolete)
            if (!this.Services.AppConfiguration.Tree.Features.Tags.DisableV1)
            {
                // skills
                var skills = this.Services.PeoplesSkills.SelectPeoplesSkillsByUserId(user.Id, UserTagOptions.Tag).Select(o => o.Skill).ToList();
                model.Skills = new Services.Networks.Models.Tags.TagsListEditable();
                model.Skills.UserId = user.Id;
                model.Skills.CompanyId = -1;
                model.Skills.GroupId = -1;
                model.Skills.Items = skills.Select(o => new Sparkle.Services.Networks.Models.Tags.TagModel(o)).ToList();

                // interests
                var interests = this.Services.PeoplesInterests.SelectPeoplesInterestsByUserId(user.Id, UserTagOptions.Tag).Select(o => o.Interest).ToList();
                model.Interests = new Sparkle.Services.Networks.Models.Tags.TagsListEditable();
                model.Interests.UserId = user.Id;
                model.Interests.CompanyId = -1;
                model.Interests.GroupId = -1;
                model.Interests.Items = interests.Select(o => new Sparkle.Services.Networks.Models.Tags.TagModel(o)).ToList();

                // recreations / loisirs
                var recreations = this.Services.PeoplesRecreations.SelectPeoplesRecreationsByUserId(user.Id, UserTagOptions.Tag).Select(o => o.Recreation).ToList();
                model.Recreations = new Sparkle.Services.Networks.Models.Tags.TagsListEditable();
                model.Recreations.UserId = user.Id;
                model.Recreations.CompanyId = -1;
                model.Recreations.GroupId = -1;
                model.Recreations.Items = recreations.Select(o => new Sparkle.Services.Networks.Models.Tags.TagModel(o)).ToList();
            }

            // industries
            IList<IndustryModel> industries = new List<IndustryModel> { new IndustryModel(0, ""), };
            industries.AddRange(this.Services.ProfileFields.GetAllAvailiableValuesByType(ProfileFieldType.Industry).Select(o => new IndustryModel(o)).ToList());
            model.Industries = industries;

            // countries
            model.Countries = this.GetCountriesList(true);

            var fieldIds = model.AvailableProfileFields.Where(f => f.Id >= 1000).Select(f => f.Id).ToArray();
            model.ProfileFieldsAvailableValues = this.Services.ProfileFields.GetAvailiableValuesByType(fieldIds);
        }

        private IList<RegionInfo> GetCountriesList(bool includeEmptyCountry)
        {
            List<RegionInfo> list;
            var theBests = new System.Collections.ArrayList
            {
                "USA",
                "GBR",
                "FRA",
                "CAN",
            };
            list = SrkToolkit.Globalization.CultureInfoHelper.GetCountries()
                .OrderBy(o => o.NativeName)
                .OrderBy(o =>
                {
                    return -theBests.IndexOf(o.ThreeLetterISORegionName);
                })
                .ToList();

            if (includeEmptyCountry)
            {
                list.Insert(0, null);
            }

            return list;
        }

        private void NotifyAdminsOfPendingApplyRequest(ApplyRequestModel model)
        {
            // notify admins
            var adminModel = this.Services.Email.GetAdminWorkModel();

            bool isCompaniesEnabled = this.Services.AppConfiguration.Tree.Features.EnableCompanies;
            bool notifyCompanyAdmins = model.JoinCompanyId != null && isCompaniesEnabled;
            bool notifyNetworkAdmins = false;

            if (notifyCompanyAdmins)
            {
                // notify company admins
                var adminWork = AdminWorkModel.For(this.Services, model, AdminWorkPriority.Current);
                adminModel.Items.Add(adminWork);
                var companyAdmins = this.Services.Company.GetActiveUsersByAccessLevel(model.JoinCompanyId.Value, CompanyAccessLevel.Administrator, PersonOptions.None);
                if (companyAdmins.Count > 0)
                {
                    this.AddCompanyPendingRequests(model.JoinCompanyId.Value, companyAdmins);
                    this.Services.Email.SendAdminWorkEmail(adminModel, true, companyAdmins);
                }
                else
                {
                    notifyNetworkAdmins = true;
                }
            }

            if (notifyNetworkAdmins)
            {
                // notify network admins
                var adminWork = AdminWorkModel.For(this.Services, model, AdminWorkPriority.Current);
                adminModel.Items.Add(adminWork);
                this.Services.Email.SendAdminWorkEmail(adminModel, true, NetworkAccessLevel.AddCompany, NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);
            }
        }

        private void AddCompanyPendingRequests(int companyId, IList<UserModel> companyAdmins)
        {
            var company = this.Services.Company.GetById(companyId);
            foreach (var item in companyAdmins)
            {
                this.Services.Activities.Insert(new Activity
                {
                    Message = "Demande d'ajout en attente",
                    Type = (int)ActivityType.PendingCompanyApplyRequest,
                    UserId = item.Id,
                    CompanyId = companyId,
                    Date = DateTime.UtcNow,
                });
            }
        }

        private bool GetInvitedNotificationFromConfig(string defaultNotificationKey)
        {
            var configValue = this.Services.AppConfiguration.Tree.Features.Newsletter.InvitedNotifications;
            if (string.IsNullOrEmpty(configValue))
                return false;

            return configValue == defaultNotificationKey;
        }

        private bool DoesPeopleHaveAnyNewsByDefault()
        {
            return this.Services.Notifications.GetDefaultNotificationFronConfig(UserNotificationKey_Newsletter)
                || this.Services.Notifications.GetDefaultNotificationFronConfig(UserNotificationKey_DailyNewsletter);
        }

        private IList<WeeklyMailSubscriber> SelectForNewsletter(bool invited, bool registered, string defaultNotificationKey, Expression<Func<User, bool>> optedInPredicate, Expression<Func<User, bool>> notOptedInPredicate)
        {
            IList<WeeklyMailSubscriber> result = null;
            var defaultSubscribedToWeekly = this.Services.Notifications.GetDefaultNotificationFronConfig(defaultNotificationKey);
            var defaultInvitedToWeekly = this.GetInvitedNotificationFromConfig(defaultNotificationKey);

            IEnumerable<WeeklyMailSubscriber> invitedQuery;
            if (defaultSubscribedToWeekly && defaultInvitedToWeekly)
            {
                // find invited (and not registered) people willing to receive
                invitedQuery = this.Repo.Invited
                    .Select()
                    .Where(i => !i.Unregistred //    filter emails where user did not unregistered from newsletter
                             && i.UserId == null // filter registered users
                             && i.Company.NetworkId == this.Services.NetworkId
                             && i.Company.IsEnabled)
                    .Select(i => new WeeklyMailSubscriber
                    {
                        Email = i.Email,
                        UserId = 0,
                        Registered = false,
                        InvitedCodeGuid = i.Code,
                        InvitedBy = i.InvitedByUserId,
                        OptedIn = i.Unregistred == false,
                    });
            }
            else
            {
                invitedQuery = new WeeklyMailSubscriber[0];
            }

            // find registered people willing to receive
            var registeredQuery = this.PeopleRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount()
                .Where(optedInPredicate)
                .Select(o => new WeeklyMailSubscriber
                {
                    Email = o.Email,
                    UserId = o.Id,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Registered = true,
                    OptedIn = true,
                });

            // find registered people without newsletter preference if default preference exists
            IEnumerable<WeeklyMailSubscriber> registeredByDefaultQuery;
            if (defaultSubscribedToWeekly)
            {
                registeredByDefaultQuery = this.PeopleRepository.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount()
                    .Where(notOptedInPredicate)
                    .Select(o => new WeeklyMailSubscriber
                    {
                        Email = o.Email,
                        UserId = o.Id,
                        FirstName = o.FirstName,
                        LastName = o.LastName,
                        Registered = true,
                        OptedIn = false,
                    });
            }
            else
            {
                registeredByDefaultQuery = new WeeklyMailSubscriber[0];
            }

            // merge lists
            if (invited && registered)
            {
                var invitedList = invitedQuery.ToList();
                var registeredList = registeredQuery.ToList();
                var registeredDefaultList = registeredByDefaultQuery.ToList();
                result = new List<WeeklyMailSubscriber>(invitedList.Count + registeredDefaultList.Count + registeredList.Count);
                result.AddRange(registeredList);
                result.AddRange(registeredDefaultList);
                result.AddRange(invitedList);
            }
            else if (invited)
            {
                result = invitedQuery.ToList();
            }
            else if (registered)
            {
                var registeredList = registeredQuery.ToList();
                var registeredDefaultList = registeredByDefaultQuery.ToList();
                result = new List<WeeklyMailSubscriber>(registeredDefaultList.Count + registeredList.Count);
                result.AddRange(registeredList);
                result.AddRange(registeredDefaultList);
            }
            return result;
        }

        private DateTime GetProfilePictureLastChangeDate(string username, string pictureName)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("The value cannot be empty", "username");

            var basePath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
            var commonPeoplesPath = Path.Combine(basePath, "Networks", "Common", "Peoples");
            var networkPeoplesPath = Path.Combine(basePath, "Networks", this.Services.Network.Name, "Peoples");

            var formats = userPictureFormats;
            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];

                // search user's picture path
                string path = Path.Combine(networkPeoplesPath, username, pictureName + format.FileNameFormat + ".jpg");
                if (File.Exists(path))
                {
                    return File.GetLastWriteTimeUtc(path);
                }
            }

            return DateTime.MinValue;
        }

        private IDictionary<int, UserModel> PrepareCacheItems(IDictionary<int, UsersView> items)
        {
            var result = items.Values.ToDictionary(u => u.Id, u => new UserModel(u));

            foreach (var item in result.Values)
            {
                item.SetAspect<UserModel, StringSearchAspect>(new StringSearchAspect(
                    item.FirstName,
                    item.LastName,
                    item.Username,
                    item.CompanyName));
            }

            return result;
        }

        private bool CanApproveCompany(int userId)
        {
            var user = this.GetActiveById(userId, PersonOptions.None);
            if (user.NetworkAccessLevel.HasValue && user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.AddCompany, NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
                return true;

            return false;
        }
    }
}
