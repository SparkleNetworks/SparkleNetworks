
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Models.Timelines;
    using Sparkle.Services.Networks.Timelines;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Security;
    using AdModel = Sparkle.Services.Networks.Ads.AdModel;

    public class WallService : ServiceBase, IWallService
    {
        private bool displayDeleted = false;

        [DebuggerStepThrough]
        internal WallService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IWallRepository WallRepository
        {
            get { return this.Repo.Wall; }
        }

        private IList<string> IncludeLikeInList(IList<string> includes)
        {
            if (!includes.Contains("Likes"))
                includes.Add("Likes");
            return includes;
        }

        public int? CurrentUserId { get; set; }

        public IList<TimelineItem> SelectFromUser(int userId)
        {
            OptionsList = this.IncludeLikeInList(this.OptionsList);

            return this.WallRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithUserId(userId)
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public TimelineItem SelectByPublicationId(int publicationId)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            TimelineItem item = this.WallRepository
                ////.Select(this.OptionsList)
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithPublicationId(publicationId)
                .FirstOrDefault();

            if (item != null)
            {
                return item.GetLike(this.CurrentUserId);
            }

            return null;
        }

        public TimelineItem Update(TimelineItem item)
        {
            this.VerifyNetwork(item);

            item = this.WallRepository.Update(item);
            this.Services.Logger.Info("WallService.Update", ErrorLevel.Success, "Manual edition of wallitem " + item.Id);
            return item;
        }

        public bool HasPeopleProfileAlreadyUpdated(int profileId)
        {
            DateTime begin = DateTime.Now.AddDays(-1D);
            DateTime end = begin.AddDays(2D);
            var count = this.WallRepository.CountByDateRangeAndTypeAndUser(begin, end, (int)TimelineItemType.PeopleProfileUpdated, profileId);
            return count > 0;
        }

        public bool HasCompanyProfileAlreadyUpdated(int companyId)
        {
            DateTime begin = DateTime.Now.AddDays(-1D);
            DateTime end = begin.AddDays(2D);
            var count = this.WallRepository.CountByDateRangeAndTypeAndCompany(begin, end, (int)TimelineItemType.CompanyProfileUpdated, companyId);
            return count > 0;
        }

        public bool HasPartnerResourceAlreadyUpdated(int partnerId)
        {
            var oneDayAgo = DateTime.Now.AddDays(-1D);

            var count = this.WallRepository
                .Select()
                .Where(o => o.PartnerResourceId == partnerId && o.ItemType == (int)TimelineItemType.PartnerResourceUpdate)
                .Where(o => o.CreateDate > oneDayAgo)
                .Count();
            return count > 0;
        }

        public IList<TimelineItem> SelectPublicFromUser(int userId)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithUserId(userId)
                .PublicMode()
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectMyProfileWallItems(int userId, int profileId, int take, int skip, int firstItem = 0)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .FullAccess(userId, profileId)
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectWallItemsFromProfile(int userId, int profileId, bool isContact, bool isMyProfile)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithUserId(userId)
                .WithProfileIdOrNull(profileId)
                .FullAccess(userId, profileId)
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectWallItemsFromPlace(int placeId, int take, int skip)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithPlaceId(placeId)
                .OrderByDescending(o => o.Id)
                .Skip(skip)
                .Take(take)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectWallItemsFromTeam(int teamId, int take, int skip)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithTeamId(teamId)
                .OrderByDescending(o => o.Id)
                .Skip(skip)
                .Take(take)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectWallItemsFromProject(int projectId, int take, int skip)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithProjectId(projectId)
                .OrderByDescending(o => o.Id)
                .Skip(skip)
                .Take(take)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectPrivateWallItemsFromCompany(int companyId, int take, int skip, int firstItem = 0)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithCompanyId(companyId)
                .PrivateMode()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectPublicWallItemsFromCompany(int companyId, int take, int skip, int firstItem = 0)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithCompanyId(companyId)
                .PublicMode()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public TimelineItem SelectWallItemById(int id)
        {
            return this.WallRepository.GetWallItemById(this.Services.NetworkId, id);
        }

        public IList<TimelineItem> SelectWeekCompaniesPublications(DateTime start)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WeekCompaniesPublications(start)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public IList<TimelineItem> SelectCompaniesPublicationsInDateRange(DateTime from, DateTime to)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(o => o.ItemType == 0
                         && o.PrivateMode <= 0
                         && o.CompanyId > 0
                         && from <= o.CreateDate && o.CreateDate <= to)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public IList<TimelineItem> SelectNewRegistrants()
        {
            return this.WallRepository
              .Select(this.OptionsList)
              .ByNetwork(this.Services.NetworkId)
              .ExcludeDeleted(this.displayDeleted)
              .NewRegistrants()
              .OrderByDescending(o => o.Id)
              .ToList();
        }

        public IList<TimelineItem> GetLastRegistrants(short max)
        {
            return this.Repo.Wall
                .GetLastFiveRegistrants(this.Services.NetworkId)
                .OrderByDescending(o => o.CreateDate)
                .ToList();
        }

        public IList<TimelineItem> GetRegistrantsInDateRange(int max, DateTime from, DateTime to)
        {
            return this.WallRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .LastRegistrants()
                .Where(i => from <= i.CreateDate && i.CreateDate <= to)
                .OrderByDescending(o => o.Id)
                .Take(max)
                .ToList();
        }

        public int CountRegistrantsInDateRange(DateTime from, DateTime to)
        {
            return this.WallRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .LastRegistrants()
                .Where(i => from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public IList<TimelineItem> SelectFromEvent(int eventId)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithEventId(eventId)
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectFromEvent(int eventId, int take, int skip, int firstItem = 0)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithEventId(eventId)
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectFromGroup(int groupId, int take, int skip, int firstItem = 0)
        {
            //OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithGroupId(groupId)
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectFromPlace(int placeId)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithPlaceId(placeId)
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> GetNetwork(List<int> contacts, int take, int skip, int firstItem = 0)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithoutJoinNetwork()
                .WithoutRelations()
                //.Network(contacts)
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> GetContactsRelations(List<int> contacts)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Relations()
                .Network(contacts)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public IList<TimelineItem> GetCompaniesPublications()
        {
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public IList<TimelineItem> SelectFromGroup(int groupId, DateTime start, DateTime now)
        {
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(w => w.GroupId == groupId
                         && w.CreateDate > start
                         && w.CreateDate <= now)
                .ToList();
        }

        public IList<TimelineItem> SelectProfileWallItems(int userId, int profileId, bool isContact, int take, int skip, int firstItem)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .IsContact(isContact, userId, profileId)
                .OrderByDescending(o => o.Id)
                .Skip(skip)
                .Take(take)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public int GetCompaniesPublicationsCount()
        {
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(o => o.CompanyId > 0)
                .Count();
        }

        public int CountGroupsPublications()
        {
            return this.WallRepository
                 .Select(this.OptionsList)
                 .ByNetwork(this.Services.NetworkId)
                 .ExcludeDeleted(this.displayDeleted)
                 .Where(o => o.GroupId > 0)
                 .Count();
        }

        public int Count()
        {
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Count();
        }

        public int CountLast24Hours()
        {
            var to = DateTime.UtcNow;
            var from = to.AddDays(-1D);
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(i => from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public int CountToday()
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            var to = from.AddDays(1D);
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(i => from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public int GetPeoplePublicationsCount()
        {
            return this.WallRepository
                 .Select(this.OptionsList)
                 .ByNetwork(this.Services.NetworkId)
                 .ExcludeDeleted(this.displayDeleted)
                 .Where(i => !i.CompanyId.HasValue)
                 .Count();
        }

        public IList<TimelineItem> GetSponsoredNetwork(List<int> myContacts, int take, int skip, int firstItem)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithoutRelations()
                .SponsoredNetwork()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> GetFromCompanies(int take, int skip, int firstItem)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithoutRelations()
                .CompaniesNews()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> GetFromPeople(List<int> myContacts, int take, int skip, int firstItem)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .IsContact(myContacts)
                .PeopleNews()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> GetExternalFromCompany(int companyId, int take, int skip, int firstItem)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WithCompanyId(companyId)
                .ExternalCompanyNews()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList();
        }

        public IList<TimelineItem> GetExternalFromCompanies(int take, int skip, int firstItem)
        {
            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .ExternalCompanyNews()
                .OrderByDescending(o => o.Id)
                .Range(take, skip, firstItem)
                .ToList();
        }

        public IList<TimelineItem> GetWeekCompaniesPublicationsForDevices(DateTime start)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .WeekCompaniesPublicationsForDevices(start)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public IList<TimelineItem> GetCompaniesPublicationsToValidate()
        {
            OptionsList = this.IncludeLikeInList(OptionsList);
            DateTime start = DateTime.Now;

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .External()
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        public int CountCompaniesPublicationsToValidate()
        {
            return this.WallRepository
                .GetCompaniesPublicationToValidate(this.Services.NetworkId);

            return this.WallRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .External()
                .Count();
        }

        public IList<TimelineItem> SelectPublic()
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .PublicMode()
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public IList<TimelineItem> SelectPeoplePublicationsInDateRange(DateTime from, DateTime to)
        {
            var validTypes = new int[] { 0, 9, 13 };
            return this.WallRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(i => i.CompanyId == null
                    && i.CreateDate >= from
                    && i.CreateDate <= to
                    && validTypes.Contains(i.ItemType)
                    && i.GroupId == null
                    && i.EventId == null
                    && i.CompanyId == null
                    && i.AdId == null
                    && i.ProjectId == null
                    && i.TeamId == null)
                .OrderByDescending(i => i.Id)
                .ToList();
        }

        public IList<TimelineItem> SelectPartnersPublicationsInDateRange(DateTime from, DateTime to)
        {
            var validTypes = new int[] { 18, 19 };
            return this.WallRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .Where(o => o.CompanyId == null
                    && o.CreateDate >= from
                    && o.CreateDate <= to
                    && validTypes.Contains(o.ItemType)
                    && o.GroupId == null
                    && o.EventId == null
                    && o.AdId == null
                    && o.ProjectId == null
                    && o.TeamId == null
                    && o.PartnerResourceId != null)
                .OrderByDescending(o => o.Id)
                .ToList();

        }

        public IList<TimelineItem> SelectHomeContactsWallItems(IList<int> contacts)
        {
            OptionsList = this.IncludeLikeInList(OptionsList);

            return this.WallRepository
                .Select(this.OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .ExcludeDeleted(this.displayDeleted)
                .IsContact(contacts)
                .OrderByDescending(o => o.Id)
                .ToList()
                .GetLike(this.CurrentUserId)
                .ToList();
        }

        public TimelineItem Insert(TimelineItem item)
        {
            this.SetNetwork(item);
            return this.WallRepository.Insert(item);
        }

        public TimelineItem Import(TimelineItem item)
        {
            this.SetNetwork(item);
            return this.WallRepository.Insert(item);
        }

        public void Delete(int timelineItemId)
        {
            TimelineItem item = this.SelectByPublicationId(timelineItemId);

            if (item == null)
                throw new ArgumentException("No timeline item for id " + timelineItemId, "timelineItemId");

            this.VerifyNetwork(item);

            if (this.CurrentUserId == item.PostedByUserId)
            {

                // Suppresion des commentaires et commentaires.likes de la publication
                if (!this.Services.WallComments.OptionsList.Contains("Likes"))
                    this.Services.WallComments.OptionsList.Add("Likes");

                IList<TimelineItemComment> comments = this.Services.WallComments.SelectFromWallItem(timelineItemId);
                foreach (var comment in comments)
                {
                    this.Services.WallComments.Delete(comment);
                }

                this.WallRepository.Delete(item);
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        public TimelinePublishResult Publish(int userId, int itemType, string text, string timelineMode, int timelineId, bool timelinePrivacy = false, int postAs = 0, int? extraType = null, string extra = null)
        {
            var request = new TimelinePublishRequest(userId, itemType, text, timelineMode, timelineId, timelinePrivacy, postAs, extraType, extra);
            return this.Publish(request);
        }

        public TimelinePublishResult Publish(TimelinePublishRequest request)
        {/*
            int userId, 
            int itemType, 
            string text,
            string timelineMode,
            int timelineId
            bool timelinePrivacy = false
            int postAs = 0
            int? extraType = null
            string extra = null;
            */
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new TimelinePublishResult(request);

            int privateMode = 0;
            int? profileId = null;
            int? companyId = null;
            int? eventId = null;
            int? groupId = null;
            int? placeId = null;
            int? adId = null;
            int? projectId = null;
            int? teamId = null;
            TimelineType timelineType;
            TimelineItemScope? scope = null;

            var user = this.Repo.People.GetActiveById(request.UserId, PersonOptions.Company);
            if (user == null || user.NetworkId != this.Services.NetworkId && !user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff))
            {
                result.Errors.Add(TimelinePublishError.CannotPostInDifferentNetwork, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var userModel = new UserModel(user);

            if (!this.Services.People.IsActive(user))
            {
                result.Errors.Add(TimelinePublishError.PostingUserIsInactive, NetworksEnumMessages.ResourceManager);
                return result;
            }


            switch (request.Mode)
            {
                case "public":
                    timelineType = TimelineType.Public;
                    privateMode = 0;
                    break;

                case "private":
                    timelineType = TimelineType.Private;
                    privateMode = 1;
                    break;

                case "profile":
                    timelineType = TimelineType.Profile;
                    privateMode = 1;
                    profileId = request.ItemId;
                    break;

                case "company":
                case "companiesnews":
                    timelineType = TimelineType.Company;
                    privateMode = request.Privacy ? -1 : 0;
                    scope = request.Privacy ? TimelineItemScope.Public : TimelineItemScope.Network;
                    companyId = request.ItemId;

                    if (user.CompanyID != companyId.Value && !user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff))
                    {
                        result.Errors.Add(TimelinePublishError.PostingInOtherCompanyIsNotAllowed, NetworksEnumMessages.ResourceManager);
                        return result;
                    }
                    break;

                case "companynetwork":
                    timelineType = TimelineType.CompanyNetwork;
                    privateMode = 1;
                    companyId = request.ItemId;

                    if (user.CompanyID != companyId.Value)
                    {
                        result.Errors.Add(TimelinePublishError.PostingInOtherCompanyIsNotAllowed, NetworksEnumMessages.ResourceManager);
                        return result;
                    }
                    break;

                case "event":
                    timelineType = TimelineType.Event;
                    privateMode = 1;
                    eventId = request.ItemId;
                    {
                        var eventModel = this.Services.Events.GetByModelId(eventId.Value, EventOptions.None);

                        if (eventModel != null)
                        {
                            var eventMember = this.Services.EventsMembers.GetMembership(eventModel.Id, userModel.Id);
                            var groupMember = eventModel.GroupId != null ? this.Services.GroupsMembers.GetMembershipModel(eventModel.GroupId.Value, userModel.Id) : null;
                            eventModel.SetupUserRights(userModel, eventMember, groupMember);
                            if (!eventModel.AccessModel.IsVisible)
                            {
                                result.Errors.Add(TimelinePublishError.NoEventVisibility, NetworksEnumMessages.ResourceManager);
                                return result;
                            }
                        }
                        else
                        {
                            result.Errors.Add(TimelinePublishError.NoSuchEvent, NetworksEnumMessages.ResourceManager);
                            return result;
                        }
                    }
                    break;

                case "group":
                    timelineType = TimelineType.Group;
                    privateMode = 1;
                    groupId = request.ItemId;
                    {
                        var group = this.Services.Groups.SelectGroupById(groupId.Value);
                        if (group != null)
                        {
                            if (!this.Services.GroupsMembers.IsGroupMember(user.Id, group.Id))
                            {
                                result.Errors.Add(TimelinePublishError.NoAuthorizedInGroup, NetworksEnumMessages.ResourceManager);
                                return result;
                            }
                        }
                        else
                        {
                            result.Errors.Add(TimelinePublishError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                            return result;
                        }
                    }
                    break;

                case "place":
                    timelineType = TimelineType.Place;
                    privateMode = 1;
                    placeId = request.ItemId;
                    break;

                case "ad":
                    timelineType = TimelineType.Ad;
                    privateMode = 1;
                    adId = request.ItemId;
                    break;

                case "project":
                    timelineType = TimelineType.Project;
                    privateMode = 1;
                    projectId = request.ItemId;
                    break;

                case "team":
                    timelineType = TimelineType.Team;
                    privateMode = 1;
                    teamId = request.ItemId;
                    break;

                default:
                    result.Errors.Add(TimelinePublishError.InvalidMode, NetworksEnumMessages.ResourceManager);
                    return result;
            }

            Company postAsCompany = null;
            if (request.PostAs == 1)
            {
                postAsCompany = this.Services.Company.GetById(user.CompanyID);
            }

            var item = this.Publish(
                user,
                postAsCompany,
                (TimelineItemType)request.ItemType,
                request.Text,
                (TimelineType)timelineType,
                request.ItemId,
                request.ExtraType,
                request.ExtraValue,
                (TimelineItemScope)privateMode);

            result.Item = item;
            result.Succeed = true;
            return result;
        }

        public TimelineItem Publish(
            User user,
            Company postAsCompany,
            TimelineItemType type,
            string text,
            TimelineType mode,
            int timelineId,
            int? extraType = null,
            string extra = null,
            TimelineItemScope? scope = null)
        {
            // this method DOES NOT check posting authorizations
            // the old one does

            var publishDate = DateTime.Now;

            var item = new TimelineItem
            {
                PostedByUserId = user.Id,
                ItemType = (int)type,
                Text = text,
                CreateDate = publishDate,
                PrivateMode = (int)mode,
                CompanyId = postAsCompany != null ? postAsCompany.ID : default(int?),
                Extra = extra,
                ExtraType = extraType,
                TimelineItemScope = scope ?? TimelineItemScope.Network,
            };

            switch (type)
            {
                case TimelineItemType.TextPublication:
                    break;
                case TimelineItemType.UserJoined:
                    break;
                case TimelineItemType.UserInContact:
                    break;
                case TimelineItemType.Poll:
                    break;
                case TimelineItemType.Event:
                    item.EventId = timelineId;
                    break;
                case TimelineItemType.Obsolete5:
                    break;
                case TimelineItemType.Introducing:
                    break;
                case TimelineItemType.CompanyProfileUpdated:
                    break;
                case TimelineItemType.JoinedGroup:
                    break;
                case TimelineItemType.PeopleProfileUpdated:
                    break;
                case TimelineItemType.Obsolete14:
                    break;
                case TimelineItemType.Ad:
                    break;
                case TimelineItemType.Deal:
                    break;
                case TimelineItemType.CompanyJoined:
                    break;
                case TimelineItemType.NewPartnerResource:
                case TimelineItemType.PartnerResourceUpdate:
                    item.PartnerResourceId = timelineId;
                    break;
                case TimelineItemType.Twitter:
                    break;
                default:
                    break;
            }

            switch (mode)
            {
                case TimelineType.Public:
                    break;
                case TimelineType.Private:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    break;
                case TimelineType.Profile:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.UserId = timelineId;
                    break;
                case TimelineType.Company:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Network;
                    item.CompanyId = timelineId;
                    break;
                case TimelineType.CompanyNetwork:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.CompanyId = timelineId;
                    break;
                case TimelineType.Event:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.EventId = timelineId;
                    break;
                case TimelineType.Group:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.GroupId = timelineId;
                    break;
                case TimelineType.Place:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.PlaceId = timelineId;
                    break;
                case TimelineType.Ad:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.AdId = timelineId;
                    break;
                case TimelineType.Project:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.ProjectId = timelineId;
                    break;
                case TimelineType.Team:
                    item.TimelineItemScope = scope ?? TimelineItemScope.Self;
                    item.TeamId = timelineId;
                    break;
                default:
                    throw new ArgumentException("Timeline mode '" + mode + "' is not valid", "mode");
            }

            this.SetNetwork(item);
            item = this.WallRepository.Insert(item);

            this.Services.Likes.MarkTimelineItemAsRead(item.Id, item.PostedByUserId);

            this.Services.Parallelize(services => PublishNotify(services, item.Id, mode, user.Id));

            return item;
        }

        private static void PublishNotify(IServiceFactory services, int timelineItemId, TimelineType timelineType, int userId)
        {
            // this method is built for parallelization
            // be careful to threading stuff
            // 'this' may not be the 'this' you think of

            try
            {
                ////var itemToPublish = services.Wall.SelectByPublicationId(timelineItemId);
                var itemToPublish = services.Wall.SelectWallItemById(timelineItemId);
                var currentUser = services.People.SelectWithId(userId);

                services.Wall.Notify(itemToPublish, timelineType, currentUser, null);
            }
            catch (Exception ex)
            {
                services.Logger.Critical("WallService.Publish.ThreadPoolDelegate", ErrorLevel.Critical, ex);
            }
        }

        public void Notify(TimelineItem item, TimelineType type, User currentUser, TimelineItemComment comment)
        {
            const string logPath = "WallService.Notify";

            bool notify = true;
            switch (type)
            {
                case TimelineType.Public:
                case TimelineType.Event:
                case TimelineType.Group:
                case TimelineType.CompanyNetwork:
                    notify &= true;
                    break;
                default:
                    notify &= false;
                    break;
            }

            switch (item.TimelineItemType)
            {
                case TimelineItemType.TextPublication:
                case TimelineItemType.Poll:
                case TimelineItemType.Ad:
                    notify &= true;
                    break;
                default:
                    notify &= false;
                    break;
            }

            string logDescription = "item " + item + " of type " + type.ToString() + "/" + item.TimelineItemType.ToString() + " by " + currentUser;
            if (!notify)
            {
                this.Services.Logger.Info(logPath, ErrorLevel.Success, "Not notifying " + logDescription + ".");
                return;
            }

            int[] commentedUserIds = new int[0];//// = null;
            IDictionary<int, User> commentedUsers = new Dictionary<int, User>();
            
            ////if (this.Services.Notifications.IsUserNotified(NotificationType.Comment))
            if (comment != null)
            {
                commentedUserIds = this.Repo.WallComments.GetCommentedUserIds(item.Id, true);
                if (!commentedUserIds.Contains(item.PostedByUserId))
                {
                    commentedUserIds = commentedUserIds.CombineWith(item.PostedByUserId);
                }

                var subscribed = this.Services.Notifications.GetSubscribedUsers(NotificationType.Comment, commentedUserIds);
                var subscribedIds = subscribed.Select(s => s.Id).ToArray();
                commentedUsers = this.Repo.People.GetActiveById(subscribedIds, this.Services.NetworkId, PersonOptions.None);
            }

            IDictionary<User, NotificationType?> usersToNotify = new Dictionary<User, NotificationType?>();
            NotificationType? notifType = null;
            User postedBy;
            Event evt = null;
            Group group = null;

            switch (type)
            {
                case TimelineType.Public:
                    {
                        if (comment == null)
                        {
                            notifType = NotificationType.MainTimelineItems;
                            usersToNotify = this.Services.People.GetSubscribedToMainTimelineItems().ToDictionary(o => o, o => notifType);
                        }
                        else
                        {
                            // those have subscribed for all comments
                            notifType = NotificationType.MainTimelineComments;
                            usersToNotify = this.Services.People.GetSubscribedToMainTimelineComments().ToDictionary(o => o, o => notifType);
                            var usersToNotifyIds = usersToNotify.Select(u => u.Key.Id).ToArray();

                            notifType = NotificationType.Comment;
                            foreach (var commentedUser in commentedUsers.Values)
                            {
                                if (!usersToNotifyIds.Contains(commentedUser.Id))
                                    usersToNotify.Add(commentedUser, notifType);
                            }
                        }
                    }
                    break;

                case TimelineType.Event:
                    {
                        evt = this.Services.Events.GetById(item.EventId.Value, EventOptions.Category | EventOptions.Place);
                        if (comment == null)
                        {
                            #warning Performance issue: use an optimized query for this
                            // TODO: Performance issue: use SQL to filter out those who are "Away"
                            // TODO: Performance issue: use SQL to filter out those who are not subscribed to this email?
                            var members = this.Services.EventsMembers.SelectEventMembers(item.EventId.Value);

                            foreach (var member in members)
                            {
                                if (member.UserId == currentUser.Id)
                                    continue;

                                int nb = member.Notifications ?? 0;
                                var updated = member;
                                updated.Notifications = ++nb;
                                this.Services.EventsMembers.UpdateRegistered(updated);
                            }

                            var emailMembers = members
                                .Where(u => u.UserId != currentUser.Id
                                    && (u.StateValue == EventMemberState.MaybeJoin || u.StateValue == EventMemberState.HasAccepted)
                                    && this.Services.People.IsActive(u.User))
                                .ToList();

                            notifType = null;
                            usersToNotify = emailMembers.ToDictionary(o => o.User, o => notifType);
                        }
                        else
                        {
                            notifType = NotificationType.Comment;
                            usersToNotify = commentedUsers.Values.ToDictionary(o => o, o => notifType);
                        }
                    }
                    break;

                case TimelineType.Group:
                    {
                        group = this.Services.Groups.SelectGroupById(item.GroupId.Value);
                        var members = this.Services.GroupsMembers.SelectGroupMembers(item.GroupId.Value);

                        foreach (var groupMember in members)
                        {
                            if (groupMember.UserId == currentUser.Id)
                                continue;

                            int nb = groupMember.Notifications ?? 0;
                            groupMember.Notifications = ++nb;
                            this.Services.GroupsMembers.Update(groupMember);

                            // Ajout de l'activié pour le membre du groupe
                            var activity = new Activity
                            {
                                UserId = groupMember.UserId,
                                Type = (int)ActivityType.GroupPublication,
                                Date = DateTime.UtcNow,
                                ProfileID = item.PostedByUserId,
                                GroupId = group.Id,
                                Displayed = false
                            };
                            this.Services.Activities.Insert(activity);
                        }

                        var emailMembers = members
                            .Where(u => u.UserId != currentUser.Id
                                && (NotificationFrequencyType)(u.NotificationFrequency ?? group.NotificationFrequency) == NotificationFrequencyType.Immediate)
                            .ToList();

                        notifType = null;
                        foreach (var groupMember in emailMembers)
                        {
                            var person = this.Services.People.SelectWithId(groupMember.UserId);
                            if (this.Services.People.IsActive(person))
                            {
                                ////person.Picture = this.GetPersonPictureUrl(groupMember.User.Login, UserProfilePictureSize.Medium);
                                ////this.Services.Email.SendNotification(currentUser, person, item, group);
                                ////this.Services.Likes.MarkTimelineItemAsNotified(item.Id, groupMember.UserId);
                                usersToNotify.Add(person, notifType);
                            }
                        }
                    }
                    break;

                case TimelineType.CompanyNetwork:
                    if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                    {
                        notifType = NotificationType.CompanyTimelineItems;
                        var users = this.Services.People.GetSubscribedToCompanyNetworkItems(currentUser.CompanyID)
                            .Where(u => u.Id != currentUser.Id && this.Services.People.IsActive(u))
                            .ToDictionary(o => o, o => notifType);

                        usersToNotify = users;
                    }
                    break;
            }
            var areSubscriptionsEnabled = this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced;
            var usersSubscribed = new int[0];
            if (areSubscriptionsEnabled)
            {
                usersSubscribed = this.Repo.Subscriptions.GetUserIdsSubscribedAmongIds(
                    this.Services.NetworkId,
                    usersToNotify.Select(o => o.Key.Id).ToArray(),
                    DateTime.UtcNow);
            }

            this.Services.Logger.Info(logPath, ErrorLevel.Success, "Notifying " + type.ToString() + " of " + logDescription + " to " + usersToNotify.Count + " users.");
            foreach (var user in usersToNotify)
            {
                if (user.Key.Id == currentUser.Id)
                    continue;
                if (areSubscriptionsEnabled && !usersSubscribed.Contains(user.Key.Id))
                    continue;

                this.Services.Email.SendNotification(currentUser, user.Key, item, group: group, @event: evt, comment: comment, notifType: user.Value);
                if (comment == null)
                {
                    this.Services.Likes.MarkTimelineItemAsNotified(item.Id, user.Key.Id);
                }
                else
                {
                    this.Services.Likes.MarkTimelineItemCommentAsNotified(comment.Id, user.Key.Id);
                }
            }
        }

        private string GetPersonPictureUrl(string username, UserProfilePictureSize size)
        {
            var https = this.Services.AppConfiguration.Tree.Site.ForceSecureHttpGet
                     || this.Services.AppConfiguration.Tree.Site.ForceSecureHttpRequests;
            var domainName = this.Services.AppConfiguration.Tree.Site.MainDomainName;

            return string.Format(
                "{0}://{1}/Data/PersonPicture/{2}/{3}",
                https ? "https" : "http",
                domainName,
                Uri.EscapeUriString(username),
                Uri.EscapeUriString(size.ToString()));
        }

        public void HasJustJoined(int userId)
        {
            Publish(userId, 1, "vient de nous rejoindre.", "public", 0);
        }

        public void HadANewContact(int userId, int contactId)
        {
            var user = this.Services.People.SelectWithId(userId);
            var contact = this.Services.People.SelectWithId(contactId);

            // Publie l'information sur le profil de l'utilisateur
            ////var wallItem = new TimelineItem
            ////{
            ////    PostedByUserId = userId,
            ////    Text = contactId.ToString(),
            ////    ItemType = 2,
            ////    CreateDate = date,
            ////    PrivateMode = 1,
            ////};
            ////this.SetNetwork(wallItem);
            ////this.Insert(wallItem);
            this.Publish(user, null, TimelineItemType.UserInContact, contactId.ToString(), TimelineType.Profile, userId);


            // Publie l'information sur le profil du contact
            ////TimelineItem contactWallItem = new TimelineItem
            ////{
            ////    PostedByUserId = contactId,
            ////    Text = userId.ToString(),
            ////    ItemType = 2,
            ////    CreateDate = date,
            ////    PrivateMode = 1,
            ////};
            ////this.SetNetwork(contactWallItem);
            ////this.Insert(contactWallItem);
            this.Publish(contact, null, TimelineItemType.UserInContact, userId.ToString(), TimelineType.Profile, contactId);
        }

        public IList<TimelineItem> GetContactsRelations(IList<int> contacts)
        {
            throw new NotImplementedException();
        }

        public SocialNetworkState InsertManyAndUpdateSocialState(List<TimelineItem> inserts, SocialNetworkState connection)
        {
            this.VerifyNetwork(connection);

            foreach (var item in inserts)
            {
                item.NetworkId = this.Services.NetworkId;
            }

            return this.Repo.Wall.InsertManyAndUpdateSocialState(this.Services.NetworkId, inserts, connection);
        }

        public IList<TimelineItem> GetByImportedId(string importedId)
        {
            return this.WallRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(t => t.ImportedId == importedId)
                .ToList();
        }

        public TimelineItem Delete(int id, WallItemDeleteReason reason, int userId)
        {
            var item = this.WallRepository.GetById(id);
            item.DeleteReasonValue = reason;
            item.DeletedByUserId = userId;
            item.DeleteDateUtc = DateTime.UtcNow;
            return this.WallRepository.Update(item);
        }

        /// <summary>
        /// Loads data related to a page of the timeline (events, groups, ads...) as fast as we can.
        /// </summary>
        /// <param name="conveyor"></param>
        public void LoadOptionnalData(TimelineItemsConveyor conveyor)
        {
            // The plan is:
            // 1. gather the ids of everything we need to load
            // 2. load entities by id
            // 3. for each timline item, verify whether the user can see it 
            // 4. put entities back into each timeline item
            //

            // stuff to clean/verify
            int[] friendsRelatedIds = null;
            IList<CompanySkill> companySkills = null;

            // events
            int[] eventIds = null;
            IDictionary<int, EventModel> events = null;
            IDictionary<int, EventMemberModel> eventMemberships = null;
            IDictionary<int, IList<EventMemberModel>> eventMembers = null;

            // groups
            int[] groupIds = null;
            IDictionary<int, GroupMemberModel> groupMemberships = null;
            IDictionary<int, GroupModel> groups = null;
            IDictionary<int, Group> groupEntities = null;
            UserModel user = null;

            // users
            int[] userIds = null;
            IDictionary<int, UserModel> users = null;

            // ads
            int[] adIds = null;
            IDictionary<int, AdModel> ads = null;

            // companies
            int[] companyIds = null;
            IDictionary<int, Company> companies = new Dictionary<int, Company>();

            // polls
            int[] pollIds = null;
            IDictionary<int, Poll> polls = null;
            IDictionary<int, IList<PollChoice>> pollChoicess = null;

            // places
            int[] placeIds = null;
            IDictionary<int, Place> places = null;

            /*
             * See work item 504
             * 
            var adIds = new List<int>();
            var companyIds = new List<int>();
            var userIds = new List<int>();
            var eventIds = new List<int>();
            var groupIds = new List<int>();
            var importedEmailIds = new List<int>();
            var placeIds = new List<int>();
            var projectIds = new List<int>();
            var teamIds = new List<int>();

            foreach (var item in conveyor.TimelineItems)
            {
                if (item.AdId != null)
                    adIds.Add(item.AdId.Value);
                if (item.CompanyId != null)
                    companyIds.Add(item.CompanyId.Value);
                if (item.DeletedByUserId != null)
                    userIds.Add(item.DeletedByUserId.Value);
                if (item.EventId != null)
                    eventIds.Add(item.EventId.Value);
                if (item.GroupId != null)
                    groupIds.Add(item.GroupId.Value);
                if (item.InboundEmailId != null)
                    importedEmailIds.Add(item.InboundEmailId.Value);
                if (item.PlaceId != null)
                    placeIds.Add(item.PlaceId.Value);

                if (item.ProjectId != null)
                    projectIds.Add(item.ProjectId.Value);
                if (item.TeamId != null)
                    teamIds.Add(item.TeamId.Value);
                if (item.UserId != null)
                    userIds.Add(item.UserId.Value);

                userIds.Add(item.PostedByUserId);
            }

            conveyor.RelatedAds = this.Services.Ads.GetByIdFromAnyNetwork(adIds, AdOptions.None);
            conveyor.RelatedCompanyIds = this.Services.Company.GetByIdFromAnyNetwork(companyIds, CompanyOptions.None);
            conveyor.RelatedUserIds = this.Services.People.GetModelByIdFromAnyNetwork(userIds, PersonOptions.None);
            conveyor.RelatedEventIds = new List<int>();
            conveyor.RelatedGroupIds = new List<int>();
            conveyor.RelatedImportedEmailIds = new List<int>();
            conveyor.RelatedPlaceIds = new List<int>();
            conveyor.RelatedProjectIds = new List<int>();
            conveyor.RelatedTeamIds = new List<int>();
            */

            // load non-timeline data
            {
                // collect IDs
                var companyIdsList = new List<int>();
                var eventIdsList = new List<int>();
                var groupIdsList = new List<int>();
                var userIdsList = new List<int>();
                var adIdsList = new List<int>();
                var pollIdsList = new List<int>();
                var placeIdsList = new List<int>();
                foreach (var item in conveyor.TimelineItems)
                {
                    if (!userIdsList.Contains(item.PostedByUserId))
                    {
                        userIdsList.Add(item.PostedByUserId);
                    }

                    if (item.UserId != null && !userIdsList.Contains(item.UserId.Value))
                    {
                        userIdsList.Add(item.UserId.Value);
                    }
                    
                    if (item.DeletedByUserId != null && !userIdsList.Contains(item.DeletedByUserId.Value))
                    {
                        userIdsList.Add(item.DeletedByUserId.Value);
                    }
                    
                    if (item.EventId != null)
                    {
                        eventIdsList.Add(item.EventId.Value);
                    }

                    if (item.GroupId != null)
                    {
                        groupIdsList.Add(item.GroupId.Value);
                    }

                    if (item.TimelineItemType == TimelineItemType.UserInContact)
                    {
                        int contactId;
                        if (int.TryParse(item.Text, out contactId) && !userIdsList.Contains(contactId))
                        {
                            userIdsList.Add(contactId);
                        }
                    }

                    if (item.AdId != null)
                    {
                        adIdsList.Add(item.AdId.Value);
                    }

                    if (item.CompanyId != null)
                    {
                        companyIdsList.Add(item.CompanyId.Value);
                    }

                    if (item.TimelineItemType == TimelineItemType.Ad)
                    {
                        int adId;
                        if (int.TryParse(item.Text, out adId) && !adIdsList.Contains(adId))
                        {
                            adIdsList.Add(adId);
                        }
                    }

                    if (item.TimelineItemType == TimelineItemType.Poll)
                    {
                        int pollId;
                        if (int.TryParse(item.Text, out pollId) && !pollIdsList.Contains(pollId))
                        {
                            pollIdsList.Add(pollId);
                        }
                    }

                    if (item.PlaceId != null && !placeIdsList.Contains(item.PlaceId.Value))
                    {
                        placeIdsList.Add(item.PlaceId.Value);
                    }
                }

                // get users
                userIds = userIdsList.ToArray();
                if (userIds.Length > 0)
                {
                    users = this.Services.People.GetModelByIdFromAnyNetwork(userIds, PersonOptions.None);
                }

                // get events
                eventIds = eventIdsList.ToArray();
                if (eventIds.Length > 0)
                {
                    events = this.Services.Events.GetModelById(eventIds, EventOptions.None);
                    eventMembers = this.Services.Events.GetEventMembers(eventIds, new EventMemberState[] { EventMemberState.HasAccepted, EventMemberState.MaybeJoin, });
                    if (conveyor.CurrentUserId != null)
                    {
                        eventMemberships = this.Services.EventsMembers.GetMyMembershipForEvents(eventIds, conveyor.CurrentUserId.Value);
                    }

                    foreach (var item in events.Values)
                    {
                        if (item.GroupId != null && !groupIdsList.Contains(item.GroupId.Value))
                        {
                            groupIdsList.Add(item.GroupId.Value);
                        }
                    }
                }

                // get groups
                groupIds = groupIdsList.ToArray();
                if (groupIds.Length > 0)
                {
                    groupEntities = this.Repo.Groups.GetById(groupIds, this.Services.NetworkId, GroupOptions.None)
                        .ToDictionary(x => x.Id, x => x);
                    groups = groupEntities.ToDictionary(x => x.Key, x => new GroupModel(x.Value));
                    if (conveyor.CurrentUserId != null)
                    {
                        groupMemberships = this.Services.GroupsMembers.GetUsersGroupMemberships(conveyor.CurrentUserId.Value, groupIds, GroupMemberOptions.None);
                    }
                }

                // get ads
                adIds = adIdsList.ToArray();
                if (adIds.Length > 0)
                {
                    ads = this.Services.Ads.GetById(adIds, AdOptions.None);
                }

                // get companies
                companyIds = companyIdsList.ToArray();
                if (companyIds.Length > 0)
                {
                    companies = this.Repo.Companies.GetById(companyIds, this.Services.NetworkId).ToDictionary(i => i.ID, i => i);
                    ////companies = this.Services.Ads.GetByIdInNetwork(adIds, AdOptions.None);
                    companySkills = this.Repo.CompaniesSkills.GetListByCompanyId(companyIds, true);
                }

                // get polls
                pollIds = pollIdsList.ToArray();
                if (pollIds.Length > 0)
                {
                    polls = this.Services.Polls.GetById(pollIds);
                    pollChoicess = new Dictionary<int, IList<PollChoice>>(polls.Count);
                    for (int i = 0; i < polls.Count; i++)
                    {
                        pollChoicess.Add(polls[i].Id, this.Repo.PollsChoices.GetByPollId(polls[i].Id));
                    }
                }

                // get places
                placeIds = placeIdsList.ToArray();
                if (placeIds.Length > 0)
                {
                    places = this.Services.Places.GetEntityById(placeIds, PlaceOptions.None);
                }
            }

            if (conveyor.CurrentUserId != null)
            {
                user = this.Services.People.GetByIdFromAnyNetwork(conveyor.CurrentUserId.Value, PersonOptions.None);
                if (user == null)
                    Trace.TraceWarning("WallService.LoadOptionnalData: conveyor.CurrentUserId != null (" + conveyor.CurrentUserId + ") but user is null");
            }

            // Load data for ItemType 0
            var toEvaluate = conveyor.TimelineItems.Where(o => o.TimelineItemType == TimelineItemType.TextPublication).ToList();
            if (toEvaluate.Count > 0)
            {
                if (conveyor.CurrentUserId.HasValue)
                {
                    // WTF?
                    var isFriendIds = toEvaluate.Where(o => o.PlaceId == null && o.GroupId == null && o.EventId == null).Select(o => o.PostedByUserId).ToArray();
                    friendsRelatedIds = this.Repo.Friends.GetUsersContactIds(conveyor.CurrentUserId.Value);
                }

                ////var companyIds = toEvaluate.Where(t => t.CompanyId != null).Select(t => t.CompanyId.Value).ToArray();
                ////companies = this.Repo.Companies.GetById(companyIds, this.Services.NetworkId).ToDictionary(i => i.ID, i => i);
            }

            // Load data for ItemType 7
            toEvaluate = conveyor.TimelineItems.Where(o => o.TimelineItemType == TimelineItemType.CompanyProfileUpdated).ToList();
            if (toEvaluate.Count > 0)
            {
                ////var companyIds = toEvaluate.Select(o => o.CompanyId.Value).ToArray();
                ////companySkills = this.Repo.CompaniesSkills.GetListByCompanyId(companyIds, true);
                ////companySkills = this.Repo.CompaniesSkills.Select().Where(o => companyIds.Contains(o.CompanyId)).ToList();
                ////if (companySkills.con
            }

            // Fill conveyor with data
            var userIsModerator = user != null && user.NetworkAccessLevel != null && user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);
            foreach (var item in conveyor.TimelineItems)
            {
                // check event access
                if (item.EventId != null)
                {
                    EventAccessModel eventAccess = null;
                    EventModel evt = null;
                    int eventId = item.EventId.Value;

                    evt = events.ContainsKey(eventId) ? events[eventId] : null;
                    var member = eventMemberships.ContainsKey(eventId) ? eventMemberships[eventId] : null;
                    var members = eventMembers.ContainsKey(eventId) ? eventMembers[eventId] : null;
                    conveyor.Optionnal.Add(item.Id, members);

                    if (conveyor.CurrentUserId != null)
                    {
                        var groupMember = evt.GroupId != null && groupMemberships.ContainsKey(evt.GroupId.Value) ? groupMemberships[evt.GroupId.Value] : null;
                        eventAccess = evt.GetUserRights(user, member, groupMember);
                    }
                    else
                    {
                        eventAccess = evt.GetAnonymousRights();
                    }

                    conveyor.OptionnalCheck.Add(item.Id, eventAccess.IsVisible);
                }

                // check group access
                if (item.GroupId != null)
                {
                    int groupId = item.GroupId.Value;

                    if (groupMemberships.ContainsKey(groupId))
                    {
                        var membership = groupMemberships[groupId];
                        conveyor.OptionnalCheck.Add(item.Id, membership.Status == GroupMemberStatus.Accepted);
                    }
                    else if (userIsModerator)
                    {
                        conveyor.OptionnalCheck.Add(item.Id, true);
                    }
                    else
                    {
                        conveyor.OptionnalCheck.Add(item.Id, false);
                    }
                }

                switch (item.TimelineItemType)
                {
                    case TimelineItemType.TextPublication:
                        if (item.PlaceId != null)
                        {
                            conveyor.Optionnal.Add(item.Id, places.ContainsKey(item.PlaceId.Value) ? places[item.PlaceId.Value] : null);
                        }
                        else if (conveyor.CurrentUserId.HasValue)
                        {
                            if (item.PlaceId == null && item.GroupId != null)
                            {
                                // @SandRock commented this, the check is now done above
                                ////conveyor.OptionnalCheck.Add(item.Id, groupsRelated.Where(o => o.GroupId == item.GroupId).SingleOrDefault() == null ? false : true);
                            }
                            else if (item.PlaceId == null && item.GroupId == null && item.EventId != null)
                            {
                                // code moved to the beginning of the loop
                            }
                            else if (item.PlaceId == null && item.GroupId == null && item.EventId == null)
                            {
                                conveyor.OptionnalCheck.Add(item.Id, friendsRelatedIds.Contains(item.PostedByUserId) ? false : true);
                            }
                        }
                        break;

                    case TimelineItemType.UserJoined:
                        conveyor.Optionnal.Add(item.Id, users.ContainsKey(item.PostedByUserId) ? users[item.PostedByUserId] : null);
                        break;

                    case TimelineItemType.UserInContact:
                        {
                            int contactId;
                            if (int.TryParse(item.Text, out contactId))
                            {
                                conveyor.Optionnal.Add(item.Id, users.ContainsKey(contactId) ? users[contactId] : null);
                            }
                        }
                        break;

                    case TimelineItemType.Poll:
                        {
                            int pollId;
                            if (int.TryParse(item.Text, out pollId))
                            {
                                var value = new KeyValuePair<Poll, IList<PollChoice>>(
                                    polls.ContainsKey(pollId) ? polls[pollId] : null,
                                    pollChoicess.ContainsKey(pollId) ? pollChoicess[pollId] : null);
                                conveyor.Optionnal.Add(item.Id, value);
                            }
                        }
                        break;

                    case TimelineItemType.Event:
                        // code moved to the beginning of the loop
                        break;

                    case TimelineItemType.CompanyProfileUpdated:
                        conveyor.Optionnal.Add(item.Id, (object)companySkills.Where(o => o.CompanyId == item.CompanyId).ToList());
                        break;

                    case TimelineItemType.Ad:
                        {
                            int adId;
                            if (int.TryParse(item.Text, out adId))
                            {
                                conveyor.Optionnal.Add(item.Id, ads.ContainsKey(adId) ? ads[adId] : null);
                            }
                        }
                        break;

                    default:
                        break;
                }

                ////if (!conveyor.OptionnalCheck.ContainsKey(item.Id))
                ////{
                ////    conveyor.OptionnalCheck.Add(item.Id, false);
                ////}
            }

            conveyor.Companies = companies;
            conveyor.Users = users;
            conveyor.Groups = groups;
            conveyor.GroupEntities = groupEntities;
        }

        private static int ParseInt(TimelineItem item, string value, string remark)
        {
            int number;
            if (int.TryParse(value, out number))
                return number;
            throw new FormatException("Failed to parse identifier in timeline item " + item + ". " + (remark != null ? ("Remark: " + remark + ".") : string.Empty));
        }

        public string MakeTimelineKeywordsRequest(string query)
        {
            var split = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var toRet = "%" + split[0] + "%\'";
            if (split.Length == 1)
                return toRet;

            for (int i = 1; i < split.Length; i++)
            {
                toRet += " OR T.Text LIKE \'%" + split[i] + "%";
                if (i + 1 < split.Length)
                    toRet += "\'";
            }

            return toRet;
        }

        /*
         * See work item 504
         * 
        public TimelineItemsModel NewNewGetByContext(TimelineDisplayContext timelineDisplayContext, int id, int? userid, DateTime dateMax, bool isContactOrSearchAccrued = false, IList<int> myContacts = null, int? itemOrList = null, string searchContent = null)
        {
            var model = new TimelineItemsModel();

            var conveyor = this.NewGetByContext(timelineDisplayContext, id, userid, dateMax, isContactOrSearchAccrued, myContacts, itemOrList, searchContent);

            model.Fill(this.Services, conveyor);



            return model;
        }
        */
        public TimelineItemsConveyor NewGetByContext(TimelineDisplayContext timelineDisplayContext, int id, int? userid, DateTime dateMax, bool isContactOrSearchAccrued = false, IList<int> myContacts = null, int? itemOrList = null, string searchContent = null)
        {
            IDictionary<string, Stopwatch> diag = new Dictionary<string, Stopwatch>();
            diag.Add("Service global", Stopwatch.StartNew());

            var toRet = new TimelineItemsConveyor(userid);
            toRet.TimelineDisplayContext = timelineDisplayContext;
            IList<int> idList = new List<int>();

            // Get wanted TimelineItems by Id
            diag.Add("SQL_ID:" + timelineDisplayContext.ToString(), Stopwatch.StartNew());
            if (itemOrList != null)
            {
                idList.Add(itemOrList.Value);
            }
            else
            {
                switch (timelineDisplayContext)
                {
                    case TimelineDisplayContext.Public:
                        idList = this.WallRepository.GetTimelineListIdPublic(this.Services.NetworkId, dateMax);
                        break;

                    case TimelineDisplayContext.CompanyNetwork:
                        idList = this.WallRepository.GetTimelineListIdCompanyNetwork(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.CompaniesNews:
                        idList = this.WallRepository.GetTimelineListIdCompaniesNews(this.Services.NetworkId, dateMax);
                        break;

                    case TimelineDisplayContext.PeopleNews:
                        idList = this.WallRepository.GetTimelineListIdPeopleNews(this.Services.NetworkId, dateMax, myContacts);
                        break;

                    case TimelineDisplayContext.Private:
                        idList = this.WallRepository.GetTimelineListIdPrivate(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Profile:
                        idList = this.WallRepository.GetTimelineListIdProfile(this.Services.NetworkId, dateMax, id, isContactOrSearchAccrued);
                        break;

                    case TimelineDisplayContext.Company:
                        idList = this.WallRepository.GetTimelineListIdCompany(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.ExternalCompany:
                        idList = this.WallRepository.GetTimelineListIdExternalCompany(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.ExternalCompanies:
                        idList = this.WallRepository.GetTimelineListIdExternalCompanies(this.Services.NetworkId, dateMax);
                        break;

                    case TimelineDisplayContext.Event:
                        idList = this.WallRepository.GetTimelineListIdEvent(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Group:
                        idList = this.WallRepository.GetTimelineListIdGroup(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Place:
                        idList = this.WallRepository.GetTimelineListIdPlace(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Project:
                        idList = this.WallRepository.GetTimelineListIdProject(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Team:
                        idList = this.WallRepository.GetTimelineListIdTeam(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Topic:
                        idList = this.WallRepository.GetTimelineListIdTopic(this.Services.NetworkId, dateMax, id);
                        break;

                    case TimelineDisplayContext.Search:
                        var searchContentItems = searchContent.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray();
                        idList = this.WallRepository.GetTimelineListIdSearchByContent(this.Services.NetworkId, dateMax, searchContentItems, isContactOrSearchAccrued);
                        ////idList = this.WallRepository.GetTimelineListIdSearchByContent(this.Services.NetworkId, dateMax, new string[] { searchContent });
                        break;

                    default:
                        throw new NotSupportedException("TimelineDisplayContext '" + timelineDisplayContext.ToString() + "' is not supported.");
                }
            }

            diag["SQL_ID:" + timelineDisplayContext].Stop();

            // Load TimelineItems from IdsList
            diag.Add("SQL:TimelineItems", Stopwatch.StartNew());
            toRet.TimelineItems = this.WallRepository
                .NewQuery(/*TimelineItemOptions.PostedBy | TimelineItemOptions.Company | TimelineItemOptions.Group | TimelineItemOptions.Event*/ TimelineItemOptions.None)
                .Where(t => idList.Contains(t.Id))
                .OrderByDescending(o => o.CreateDate)
                .ToList();
            diag["SQL:TimelineItems"].Stop();

            // Load optionnal data
            this.LoadOptionnalData(toRet);

            // Check items are visibles
            User user = null;
            if (userid.HasValue)
            {
                user = this.Services.People.SelectWithId(userid.Value);
            }

            var tmp = new List<TimelineItem>();
            foreach (var item in toRet.TimelineItems)
            {
                var optionnal = toRet.OptionnalCheck.ContainsKey(item.Id) ? toRet.OptionnalCheck[item.Id] : false;
                var authorized = this.TimelineItemVisibilityCheck(item, user, optionnal, optionnal, optionnal);
                if (authorized)
                    tmp.Add(item);
            }

            toRet.TimelineItems = tmp;
            idList = toRet.TimelineItems.Select(o => o.Id).ToList();

            // Load Comments from TimelineItems
            diag.Add("SQL:TimelineItemsComments", Stopwatch.StartNew());
            toRet.TimelineItemComments = this.Repo.WallComments
                .NewQuery(TimelineItemCommentOptions.TimelineItemUser)
                .Where(t => idList.Contains(t.TimelineItemId))
                .OrderBy(t => t.CreateDate)
                .ToList();
            diag["SQL:TimelineItemsComments"].Stop();

            // Load skills relation from TimelineItems
            diag.Add("SQL:TimelineItemsSkills", Stopwatch.StartNew());
            toRet.TimelineItemSkills = this.Repo.TimelineItemSkills
                .NewQuery(TimelineItemSkillOptions.Skill)
                .Where(t => idList.Contains(t.TimelineItemId))
                .ToList();
            diag["SQL:TimelineItemsSkills"].Stop();

            // Load like count from TimelineItems
            diag.Add("SQL:TimelineItemsLikeCount", Stopwatch.StartNew());
            toRet.TimelineItemLikesCount = this.Repo.TimelineItemLikes
                .GetTimelineItemsLikes(this.Services.NetworkId, idList.ToArray());
            diag["SQL:TimelineItemsLikeCount"].Stop();

            // Load like count from Comments
            diag.Add("SQL:TimelineItemsCommentsLikeCount", Stopwatch.StartNew());
            toRet.TimelineItemCommentsLikesCount = this.Repo.TimelineItemCommentLikes
                .GetTimelineCommentsLikes(this.Services.NetworkId, idList.ToArray());
            diag["SQL:TimelineItemsCommentsLikeCount"].Stop();

            if (userid.HasValue)
            {
                // Load IsRead from TimelineItems
                diag.Add("SQL:TimelineItemReadDates", Stopwatch.StartNew());
                toRet.TimelineItemReadDates = this.Repo.TimelineItemLikes
                    .GetReadDates(this.Services.NetworkId, userid.Value, idList.ToArray());
                diag["SQL:TimelineItemReadDates"].Stop();

                // Load IsRead from TimelineItems
                diag.Add("SQL:TimelineItemCommentReadDates", Stopwatch.StartNew());
                toRet.TimelineItemCommentReadDates = this.Repo.TimelineItemCommentLikes
                    .GetReadDates(this.Services.NetworkId, userid.Value, toRet.TimelineItemComments.Select(o => o.Id).ToArray());
                diag["SQL:TimelineItemCommentReadDates"].Stop();
            }

            // Load ReadCount from TimelineItems
            diag.Add("SQL:TimelineItemsReadCount", Stopwatch.StartNew());
            toRet.TimelineItemReadCount = this.Repo.TimelineItemLikes
                .GetTimelineItemsReads(this.Services.NetworkId, idList.ToArray());
            diag["SQL:TimelineItemsReadCount"].Stop();

            diag["Service global"].Stop();
            return toRet;
        }

        public IList<TagModel> GetAssociatedSkills(int timelineItemId)
        {
            return this.Repo.TimelineItemSkills.NewQuery(TimelineItemSkillOptions.Skill)
                .Where(r => r.TimelineItemId == timelineItemId)
                .ToTagModel()
                .ToList();
        }

        public IList<Sparkle.Services.Networks.Models.Tags.TagModel> GetAssociatedSkillsWithWeight(int? userId)
        {
            var query = this.WallRepository.GetTimelineSkillsListIdForCount(this.Services.NetworkId);

            // Check timeline visibility rights
            var timelineListId = query.Select(o => o.TimelineItemId).Distinct().ToArray();
            var conveyorTemp = new TimelineItemsConveyor(userId);
            conveyorTemp.TimelineItems = this.WallRepository
                .NewQuery(TimelineItemOptions.PostedBy | TimelineItemOptions.Company | TimelineItemOptions.Group | TimelineItemOptions.Event)
                .Where(t => timelineListId.Contains(t.Id))
                .ToList();
            this.LoadOptionnalData(conveyorTemp);
            var user = this.Services.People.SelectWithId(userId.Value);
            var skillListId = new List<int>();
            foreach (var item in conveyorTemp.TimelineItems)
            {
                var optionnal = conveyorTemp.OptionnalCheck.ContainsKey(item.Id) ? conveyorTemp.OptionnalCheck[item.Id] : false;
                var authorized = this.TimelineItemVisibilityCheck(
                                    item,
                                    user,
                                    optionnal,
                                    optionnal,
                                    optionnal);
                if (authorized)
                    skillListId.AddRange(query.Where(o => o.TimelineItemId == item.Id).Select(o => o.SkillId).ToList());
            }

            IDictionary<int, int> skillsOccurance = new Dictionary<int, int>();
            foreach (var item in skillListId)
            {
                if (skillsOccurance.ContainsKey(item))
                    skillsOccurance[item]++;
                else
                    skillsOccurance.Add(item, 1);
            }

            var skills = this.Services.Skills.GetById(skillsOccurance.Select(o => o.Key).ToArray());

            return skills.Select(o => new TagModel(o) { Weight = skillsOccurance[o.Id] }).OrderByDescending(i => i.Weight).ThenBy(i => i.Name).ToList();
        }

        public TimelineItemSkill AddSkillToItem(int skillId, int timelineItemId, User currentUser)
        {
            var timelineItem = this.Services.Wall.SelectWallItemById(timelineItemId);
            if (timelineItem == null)
                return null;

            if (timelineItem.PostedByUserId == currentUser.Id || currentUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
            {
                TimelineItemSkill item = new TimelineItemSkill();
                item.TimelineItemId = timelineItemId;
                item.SkillId = skillId;
                item.DateUtc = DateTime.UtcNow;
                item.CreatedByUserId = currentUser.Id;

                return this.Repo.TimelineItemSkills.Insert(item);
            }
            else
            {
                throw new InvalidOperationException("Not authorized to tag this item");
            }
        }

        public AddTimelineTagResult AssociateSkill(AddTimelineTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new AddTimelineTagResult(request);

            // Check de l'existence du groupe
            var item = this.Services.Wall.SelectByPublicationId(request.TimelineItemidId);
            if (item == null)
            {
                result.Errors.Add(AddTimelineTagError.NoSuchTimelineItem, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var user1 = this.Repo.People.GetActiveById(new int[] { request.UserId, }, this.Services.NetworkId, PersonOptions.None);
            var user = user1.ContainsKey(request.UserId) ? user1[request.UserId] : null;
            if (!this.CanUserManagePublicationTags(user, item))
            {
                result.Errors.Add(AddTimelineTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // Check de l'existence du tag
            Skill tag = null;
            if (request.TagId <= 0)
            {
                // S'il n'existe pas on le créé
                tag = this.Services.Skills.Insert(new Skill
                {
                    TagName = request.TagName,
                    Date = DateTime.UtcNow,
                    CreatedByUserId = request.UserId,
                });
            }
            else
            {
                tag = this.Services.Skills.GetById(request.TagId);
            }

            // Check de l'existence du tag dans le groupe
            var relation = this.Repo.TimelineItemSkills.GetByIds(request.TimelineItemidId, request.TagId, TimelineItemSkillOptions.None);
            if (relation != null)
            {
                // Soit le tag est toujours présent
                if (relation.DeletedByUserId == null)
                {
                    result.Errors.Add(AddTimelineTagError.AlreadyAdded, NetworksEnumMessages.ResourceManager);
                }
                // Soit il est présent mais marqué comme supprimé
                else
                {
                    // Si le user qui l'a supprimé est le user courant, on rajoute le tag
                    if (relation.DeletedByUserId == request.UserId)
                    {
                        relation.DeletedByUserId = null;
                        relation.DeletedDateUtc = null;
                        relation.DeleteReason = null;
                        this.Repo.TimelineItemSkills.Update(relation);
                        result.TagId = request.TagId;
                        result.Succeed = true;
                        return result;
                    }
                    else
                    {
                        var userDeleted = this.Services.People.SelectWithId(relation.DeletedByUserId.Value);
                        if (userDeleted != null)
                        {
                            result.Errors.Add(
                                AddTimelineTagError.CannotAddDeletedTag,
                                NetworksEnumMessages.ResourceManager,
                                userDeleted.FirstName + " " + userDeleted.LastName,
                                EnumTools.GetDescription(relation.DeleteReasonValue ?? WallItemDeleteReason.None, NetworksEnumMessages.ResourceManager));
                        }
                        else
                        {
                            result.Errors.Add(AddTimelineTagError.CannotAddDeletedTagDefault, NetworksEnumMessages.ResourceManager);
                        }
                    }
                }

                return result;
            }

            // Enfin on ajoute le tag au group
            relation = this.Repo.TimelineItemSkills.Insert(new TimelineItemSkill
            {
                TimelineItemId = item.Id,
                SkillId = tag.Id,
                DateUtc = DateTime.UtcNow,
                CreatedByUserId = request.UserId,
            });
            result.TagId = tag.Id;
            result.Succeed = true;
            result.Tag = new TagModel(tag);
            return result;
        }

        public BasicResult RemoveSkillFromItem(int skillId, int timelineItemId, int userId, WallItemDeleteReason reason)
        {
            var result = new BasicResult();

            var relation = this.Repo.TimelineItemSkills.GetByIds(
                timelineItemId,
                skillId,
                TimelineItemSkillOptions.Skill | TimelineItemSkillOptions.TimelineItem);

            if (relation == null)
            {
                result.Errors.Add(RemoveTimelineTagError.NoSuchRelation, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var user1 = this.Repo.People.GetActiveById(new int[] { userId, }, this.Services.NetworkId, PersonOptions.None);
            var user = user1.ContainsKey(userId) ? user1[userId] : null;
            if (!this.CanUserManagePublicationTags(user, relation.TimelineItem))
            {
                result.Errors.Add(RemoveTimelineTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // Si il est deja marque comme supprime on arrete
            if (relation.DeletedByUserId != null && relation.DeletedDateUtc != null)
            {
                result.Errors.Add(RemoveTimelineTagError.AlreadyDeleted, NetworksEnumMessages.ResourceManager);
                return result;
            }

            relation.DeletedByUserId = userId;
            relation.DeletedDateUtc = DateTime.UtcNow;
            relation.DeleteReasonValue = reason;
            this.Repo.TimelineItemSkills.Update(relation);
            result.Succeed = true;
            return result;
        }

        private bool CanUserManagePublicationTags(User user, TimelineItem timelineItem)
        {
            if (timelineItem == null)
                throw new ArgumentNullException("timelineItem");

            if (user == null)
                return false;

            if (!this.Services.People.IsActive(user))
                return false;

            if (timelineItem.PostedByUserId == user.Id || user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
                return true;

            return false;
        }

        public TimelineOwnerChangeModel GetOwnerChangePreview(int timelineItemId)
        {
            var mainItem = this.SelectByPublicationId(timelineItemId);

            if (mainItem.ExtraTypeValue == TimelineItemExtraType.GoogleGroupImportedMessage)
            {
                var lowerUserId = mainItem.ImportedId.Substring(0, mainItem.ImportedId.IndexOf("||||"));
                var identifier = mainItem.ImportedId.Substring(mainItem.ImportedId.IndexOf("||||") + 4);
                identifier = identifier.Substring(0, identifier.IndexOf("[{}]"));
                var model = new TimelineOwnerChangeModel
                {
                    UserIdentifier = identifier,
                    TimelineItems = this.WallRepository.GetByImportedIdExpression(this.Services.NetworkId, lowerUserId + "||||%"),
                    TimelineComments = this.Repo.WallComments.GetByImportedIdExpression(this.Services.NetworkId, lowerUserId + "||||%"),
                };
                return model;
            }

            return null;
        }

        public ChangeTimelineItemOwnerResult ChangeItemOwner(ChangeTimelineItemOwnerRequest request)
        {
            var result = new ChangeTimelineItemOwnerResult(request);

            var mainItem = this.SelectByPublicationId(request.TimelineItemId);
            var newUser = this.Services.People.GetLiteById(new int[] { request.NewUserId, }).SingleOrDefault();
            var actingUser = this.Services.People.GetLiteByIdFromAnyNetwork(new int[] { request.ActingUserId, }).SingleOrDefault();

            if (newUser == null || actingUser == null)
            {
                result.Errors.Add(ChangeTimelineItemOwnerCode.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (mainItem == null)
            {
                result.Errors.Add(ChangeTimelineItemOwnerCode.NoSuchItem, NetworksEnumMessages.ResourceManager);
            }

            if (mainItem.ImportedId.IndexOf("||||") < 2)
            {
                result.Errors.Add(ChangeTimelineItemOwnerCode.NotSupported, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var lowerUserId = mainItem.ImportedId.Substring(0, mainItem.ImportedId.IndexOf("||||"));

            var items = this.WallRepository.GetByImportedIdExpression(this.Services.NetworkId, lowerUserId + "||||%");
            foreach (var item in items)
            {
                if (item.ExtraTypeValue == TimelineItemExtraType.GoogleGroupImportedMessage && !string.IsNullOrEmpty(item.Extra))
                {
                    var data = new Dictionary<string, string>();
                    data.FromHttpHeaderText(item.Extra);
                    bool supportOwner = data.ContainsKey("Owner") ? data["Owner"] == "undefined" : false;
                    if (supportOwner)
                    {
                        data["Owner"] = newUser.Id.ToString();
                        item.Extra = data.ToHttpHeaderText();
                        item.PostedByUserId = newUser.Id;
                        this.WallRepository.Update(item);
                        result.UpdatedItems.Add(item);
                    }
                    else
                    {
                        result.NotSupportedItems.Add(item);
                    }
                }
                else
                {
                    result.NotSupportedItems.Add(item);
                }
            }

            var comments = this.Repo.WallComments.GetByImportedIdExpression(this.Services.NetworkId, lowerUserId + "||||%");
            foreach (var item in comments)
            {
                if (item.ExtraTypeValue == TimelineItemExtraType.GoogleGroupImportedMessage && !string.IsNullOrEmpty(item.Extra))
                {
                    var data = new Dictionary<string, string>();
                    data.FromHttpHeaderText(item.Extra);
                    bool supportOwner = data.ContainsKey("Owner") ? data["Owner"] == "undefined" : false;
                    if (supportOwner)
                    {
                        data["Owner"] = newUser.Id.ToString();
                        item.Extra = data.ToHttpHeaderText();
                        item.PostedByUserId = newUser.Id;
                        this.Repo.WallComments.Update(item);
                        result.UpdatedComments.Add(item);
                    }
                    else
                    {
                        result.NotSupportedComments.Add(item);
                    }
                }
                else
                {
                    result.NotSupportedComments.Add(item);
                }
            }

            result.Succeed = true;
            return result;
        }

        public TimelineItem AddCompanyJoinItem(Company company, int postedByUserId)
        {
            var user = this.Services.People.SelectWithId(postedByUserId);
            var item = this.Services.Wall.Publish(user, company, TimelineItemType.CompanyJoined, string.Empty, TimelineType.Company, company.ID);
            return item;
            ////var item = new TimelineItem();
            ////item.ItemType = 17;
            ////item.Text = string.Empty;
            ////item.CreateDate = DateTime.UtcNow;
            ////item.PrivateMode = 0;
            ////item.CompanyId = company.ID;
            ////item.PostedByUserId = postedByUserId;
            ////item.NetworkId = company.NetworkId;
            ////item = this.Insert(item);
            ////return item;
        }

        public TimelineItem PublishCompanyProfileUpdate(Company company, int userId, bool avoidMultiplePosts)
        {
            bool already = avoidMultiplePosts && this.Services.Wall.HasCompanyProfileAlreadyUpdated(company.ID);
            if (!already)
            {
                var user = this.Services.People.SelectWithId(userId);
                return this.Publish(user, company, TimelineItemType.CompanyProfileUpdated, DateTime.Now.ToString(), TimelineType.Company, company.ID);
            }

            return null;
        }

        public TimelineItem PublishPartnerResourceUpdate(PartnerResource item, int userId, bool isNew)
        {
            if (isNew || !this.Services.Wall.HasPartnerResourceAlreadyUpdated(item.Id))
            {
                var user = this.Services.People.SelectWithId(userId);
                return this.Publish(user, null, isNew ? TimelineItemType.NewPartnerResource : TimelineItemType.PartnerResourceUpdate, DateTime.Now.ToString(), TimelineType.Public, item.Id);
            }

            return null;
        }

        public TimelineItem PublishEvent(int eventId, int userId, int? companyId)
        {
            var user = this.Services.People.SelectWithId(userId);
            var eventItem = this.Services.Events.GetById(eventId, EventOptions.None);
            var company = companyId != null ? this.Services.Company.GetById(companyId.Value) : null;

            TimelineType timelineType;
            if (eventItem.Scope == EventVisibility.Company)
            {
                timelineType = TimelineType.CompanyNetwork;
            }
            else if (eventItem.Scope == EventVisibility.Personal)
            {
                timelineType = TimelineType.Profile;
            }
            else
            {
                timelineType = TimelineType.Public;
            }

            var item = this.Publish(user, company, TimelineItemType.Event, "msg", timelineType, eventItem.Id);
            return item;
        }

        private bool TimelineItemVisibilityCheck(TimelineItem publication, User user, GroupMember groupMemberShip, EventMember eventMemberShip, bool contact)
        {
            if (publication == null)
            {
                return false;
            }

            if (!this.displayDeleted && publication.DeleteDateUtc != null)
            {
                return false;
            }

            if (user == null)
            {
                return publication.PrivateMode <= -1;
            }

            if (publication.PrivateMode <= 0 ||
                (publication.PrivateMode == 1 && contact) ||
                (publication.PlaceId != null) ||
                (publication.AdId != null) ||
                (publication.TeamId != null) ||
                (publication.ProjectId != null) ||
                (user.CompanyID == publication.CompanyId))
            {
                return true;
            }

            var userIsModerator = user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);

            if (publication.EventId.HasValue)
            {
                if (eventMemberShip != null)
                {
                    return eventMemberShip.StateValue == EventMemberState.HasAccepted || eventMemberShip.StateValue == EventMemberState.MaybeJoin;
                }
                else
                {
                    var evt = this.Services.Events.GetById(publication.EventId.Value, EventOptions.None);
                    if (evt != null)
                    {
                        if (evt.CompanyId != null)
                        {
                            if (user.CompanyID == evt.CompanyId.Value)
                            {
                                return evt.Scope != EventVisibility.CompanyPrivate;
                            }
                            else
                            {
                                return evt.Scope == EventVisibility.Public || evt.Scope == EventVisibility.External || evt.Scope == EventVisibility.Devices;
                            }
                        }
                        else if (evt.GroupId != null)
                        {
                            var group = this.Services.Groups.SelectGroupById(evt.GroupId.Value);
                            if (group != null)
                            {
                                if (this.Services.GroupsMembers.IsGroupMember(user.Id, evt.GroupId.Value))
                                {
                                    return true;
                                }
                                else
                                {
                                    return evt.Scope < EventVisibility.Company;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return evt.Scope == EventVisibility.Public || evt.Scope == EventVisibility.External || evt.Scope == EventVisibility.Devices;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

            }

            if (publication.GroupId.HasValue)
            {
                return userIsModerator || groupMemberShip.AcceptedStatus == GroupMemberStatus.Accepted;
            }

            return false;
        }

        private bool TimelineItemVisibilityCheck(TimelineItem publication, User user, bool groupMemberShip, bool eventMemberShip, bool contact)
        {
            if (publication == null)
            {
                return false;
            }

            if (!this.displayDeleted && publication.DeleteDateUtc != null)
            {
                return false;
            }

            if (user == null)
            {
                return publication.PrivateMode <= -1;
            }

            if (publication.PrivateMode <= 0 ||
                (publication.PrivateMode == 1 && contact) ||
                (publication.PlaceId != null) ||
                (publication.AdId != null) ||
                (publication.TeamId != null) ||
                (publication.ProjectId != null) ||
                (user.CompanyID == publication.CompanyId))
            {
                return true;
            }

            if (groupMemberShip || eventMemberShip)
            {
                return true;
            }

            return false;
        }

        public bool IsVisible(int timelineItemId, int? userId)
        {
            TimelineItem item = null;
            User user = null;
            GroupMember groupMember = null;
            EventMember eventMember = null;
            bool isContact = false;

            item = this.SelectByPublicationId(timelineItemId);
            if (userId.HasValue)
            {
                user = this.Services.People.SelectWithId(userId.Value);

                isContact = userId.Value == item.PostedByUserId ? true : this.Services.Friend.CheckIfBothAreFriends(userId.Value, item.PostedByUserId);
                if (item != null)
                {
                    if (item.GroupId.HasValue)
                    {
                        groupMember = this.Services.GroupsMembers.GetMembership(item.GroupId.Value, userId.Value);
                    }

                    if (item.EventId.HasValue)
                    {
                        var evt = this.Repo.Events.GetById(item.EventId.Value);
                        var evtModel = new EventModel(evt);
                        eventMember = this.Services.EventsMembers.SelectEventMemberByEventIdAndUserId(item.EventId.Value, userId.Value);
                        eventMember = eventMember ?? new EventMember
                        {
                            UserId = user.Id,
                            EventId = evt.Id,
                            StateValue = EventMemberState.None,
                        };
                        var memberModel = evt != null
                            ? new EventMemberModel(evt.Id, user.Id, eventMember != null ? eventMember.StateValue : EventMemberState.None)
                            : null;
                        var groupModel = groupMember != null
                            ? new GroupMemberModel(groupMember)
                            : null;
                        var rights = evtModel.GetUserRights(new UserModel(user), memberModel, groupModel);
                        return rights.IsVisible;
                    }
                }
            }

            return TimelineItemVisibilityCheck(item, user, groupMember, eventMember, isContact);
        }

        public bool IsVisible(TimelineItem item, User user)
        {
            GroupMember groupMember = null;
            EventMember eventMember = null;
            bool isContact = false;

            if (user != null)
            {
                isContact = user.Id == item.PostedByUserId
                          ? true
                          : this.Services.Friend.CheckIfBothAreFriends(user.Id, item.PostedByUserId);
                if (item != null)
                {
                    if (item.GroupId.HasValue)
                    {
                        groupMember = this.Services.GroupsMembers.GetMembership(item.GroupId.Value, user.Id);
                    }

                    if (item.EventId.HasValue)
                    {
                        eventMember = this.Services.EventsMembers.SelectEventMemberByEventIdAndUserId(item.EventId.Value, user.Id);
                    }
                }
            }

            return TimelineItemVisibilityCheck(item, user, groupMember, eventMember, isContact);
        }

        public TimelineType GetTimelineType(int timelineItemId)
        {
            var item = this.Services.Wall.SelectByPublicationId(timelineItemId);
            if (item == null)
                throw new InvalidOperationException("Item with id " + timelineItemId + "does not exist.");

            return this.GetTimelineType(item);
        }

        public TimelineType GetTimelineType(TimelineItem item)
        {
            TimelineType type;
            if (this.TryGetTimelineType(item, out type))
                return type;
            else
                throw new InvalidOperationException("Something went wrong with timelineItem " + item.Id + ", it may be broken.");
        }

        public bool TryGetTimelineType(int timelineItemId, out TimelineType timelineType)
        {
            var item = this.Services.Wall.SelectByPublicationId(timelineItemId);
            if (item == null)
                throw new InvalidOperationException("Item with id " + timelineItemId + "does not exist.");

            TimelineType type;
            var ok = this.TryGetTimelineType(item, out type);
            timelineType = type;
            return ok;
        }

        public bool TryGetTimelineType(TimelineItem item, out TimelineType timelineType)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!item.AdId.HasValue  && !item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Public;
            else if (item.AdId.HasValue && !item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Ad;
            else if (!item.AdId.HasValue && item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
            {
                if (item.PrivateMode > 0)
                    timelineType = TimelineType.CompanyNetwork;
                else
                    timelineType = TimelineType.Company;
            }
            else if (!item.AdId.HasValue && !item.CompanyId.HasValue && item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Event;
            else if (!item.AdId.HasValue && !item.CompanyId.HasValue && !item.EventId.HasValue && item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Group;
            else if (!item.AdId.HasValue && !item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Place;
            else if (!item.AdId.HasValue && !item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && item.ProjectId.HasValue && !item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Project;
            else if (!item.AdId.HasValue && !item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && item.TeamId.HasValue && !item.UserId.HasValue)
                timelineType = TimelineType.Team;
            else if (!item.AdId.HasValue && !item.CompanyId.HasValue && !item.EventId.HasValue && !item.GroupId.HasValue && !item.PlaceId.HasValue && !item.ProjectId.HasValue && !item.TeamId.HasValue && item.UserId.HasValue)
                timelineType = TimelineType.Profile;
            else
            {
                timelineType = TimelineType.Ad;
                return false;
            }

            return true;
        }

        public IList<int> GetUserIdsToNotifiedForTimelineComments(TimelineType type, TimelineItem item)
        {
            IList<int> userIds = null;

            switch (type)
            {
                case TimelineType.Public:
                    var subscribedToMainNotif = this.Services.People.GetSubscribedToMainTimelineComments();
                    userIds = subscribedToMainNotif.Select(o => o.Id).ToList();
                    break;
                case TimelineType.Group:
                    var subscribedToInstantNotif = this.Services.GroupsMembers.GetSubscribedToImmediateNotifications(item.GroupId.Value);
                    userIds = subscribedToInstantNotif.Select(o => o.UserId).ToList();
                    break;
                case TimelineType.CompanyNetwork:
                    if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                    {
                        var subscribedToCompanyNotif = this.Services.People.GetSubscribedToCompanyNetworkComments(item.PostedBy.CompanyID);
                        userIds = subscribedToCompanyNotif.Select(o => o.Id).ToList();
                    }
                    else
                        userIds = new List<int>();
                    break;
                case TimelineType.Private:
                case TimelineType.Profile:
                case TimelineType.Company:
                case TimelineType.Event:
                case TimelineType.Place:
                case TimelineType.Ad:
                case TimelineType.Project:
                case TimelineType.Team:
                    userIds = new List<int>();
                    break;
                default:
                    throw new InvalidOperationException("TimelineType not recognized.");
            }

            return userIds;
        }

        public int CountLikes(int timelineItemId)
        {
            return this.Repo.TimelineItemLikes.GetTimelineItemLikes(timelineItemId);
        }

        public string GetUrl(int timelineItemId, UriKind uriKind)
        {
            var path = "Ajax/Item/" + timelineItemId;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetUrl(int timelineItemId, int timelineCommentId, UriKind uriKind)
        {
            var path = "Ajax/Item/" + timelineItemId;
            var uri = new Uri(this.Services.GetUrl(path, fragment: "#comment" + timelineCommentId), uriKind);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public IList<TinyUserModel> GetTimelineItemLikers(int timelineItemId, int userId)
        {
            var likes = this.Services.Likes.GetLikesByTimelineItemId(timelineItemId, this.Services.NetworkId, LikeOptions.User);

            var toRet = likes
                .Where(o => o.IsLiked == true && this.Services.People.IsActive(o.User))
                .Select(o => new TinyUserModel
                {
                    Id = o.UserId,
                    FullName = o.User.FullName,
                    Login = o.User.Login,
                    PictureUrl = this.Services.People.GetProfilePictureUrl(o.User, UserProfilePictureSize.Small, UriKind.Relative),
                    ////PictureUrl = "/Data/PersonPicture/" + o.User.Login + "/Small",
                    ProfileUrl = this.Services.People.GetProfileUrl(o.User, UriKind.Relative),
                    ////ProfileUrl = "/Person/" + o.User.Login,
                    DateUtc = o.FirstDateLikedUtc.AsUtc(),
                    IsDisplayed = true,
                })
                .OrderBy(o => o.DateUtc)
                .ToList();

            if (!toRet.Any(o => o.Id == userId))
            {
                var user = this.Services.People.SelectWithId(userId);
                toRet.Add(new TinyUserModel
                {
                    Id = userId,
                    FullName = user.FullName,
                    Login = user.Login,
                    PictureUrl = this.Services.People.GetProfilePictureUrl(user, UserProfilePictureSize.Small, UriKind.Relative),
                    ////PictureUrl = "/Data/PersonPicture/" + user.Login,
                    ProfileUrl = this.Services.People.GetProfileUrl(user, UriKind.Relative),
                    ////ProfileUrl = "/Person/" + user.Login,
                    IsDisplayed = false,
                });
            }

            return toRet;
        }

        public IList<TinyUserModel> GetTimelineItemReaders(int timelineItemId, int userId)
        {
            var reads = this.Services.Likes.GetLikesByTimelineItemId(timelineItemId, this.Services.NetworkId, LikeOptions.User);

            var toRet = reads
                .Where(o => o.DateReadUtc != null && this.Services.People.IsActive(o.User))
                .Select(o => new TinyUserModel
                {
                    Id = o.UserId,
                    FullName = o.User.FullName,
                    Login = o.User.Login,
                    PictureUrl = this.Services.People.GetProfilePictureUrl(o.User, UserProfilePictureSize.Small, UriKind.Relative),
                    ////PictureUrl = "/Data/PersonPicture/" + o.User.Login + "/Small",
                    ProfileUrl = this.Services.People.GetProfileUrl(o.User, UriKind.Relative),
                    ////ProfileUrl = "/Person/" + o.User.Login,
                    DateUtc = o.DateReadUtc.AsUtc(),
                    IsDisplayed = true,
                })
                .OrderBy(o => o.DateUtc)
                .ToList();

            if (!toRet.Any(o => o.Id == userId))
            {
                var user = this.Services.People.SelectWithId(userId);
                toRet.Add(new TinyUserModel
                {
                    Id = userId,
                    FullName = user.FullName,
                    Login = user.Login,
                    PictureUrl = this.Services.People.GetProfilePictureUrl(user, UserProfilePictureSize.Small, UriKind.Relative),
                    ////PictureUrl = "/Data/PersonPicture/" + user.Login,
                    ProfileUrl = this.Services.People.GetProfileUrl(user, UriKind.Relative),
                    ////ProfileUrl = "/Person/" + user.Login,
                    IsDisplayed = false,
                });
            }

            return toRet;
        }

        public int CountByUser(int userId)
        {
            return this.WallRepository.CountCreatedByUserId(userId, this.Services.NetworkId);
        }
    }
}
