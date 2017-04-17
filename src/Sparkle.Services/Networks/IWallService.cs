
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Models.Timelines;
    using Sparkle.Services.Networks.Timelines;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;

    public interface IWallService
    {
        [Obsolete("Use this.Services.Identity instead")]
        int? CurrentUserId { get; set; }

        bool HasCompanyProfileAlreadyUpdated(int companyId);
        bool HasPeopleProfileAlreadyUpdated(int profileId);
        bool HasPartnerResourceAlreadyUpdated(int partnerId);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        ////void Delete(int timelineItemId);

        void HadANewContact(int userId, int contactId);
        void HasJustJoined(int userId);

        [Obsolete]
        TimelineItem Insert(TimelineItem item);
        TimelineItem Import(TimelineItem item);

        [Obsolete]
        IList<string> OptionsList { get; set; }

        TimelinePublishResult Publish(int userId, int itemType, string text, string timelineMode, int timelineId, bool timelinePrivacy = false, int postAs = 0, int? extraType = null, string extra = null);
        TimelinePublishResult Publish(TimelinePublishRequest request);

        TimelineItem Publish(
            User user,
            Company postAscompany,
            TimelineItemType type,
            string text,
            TimelineType mode,
            int timelineId,
            int? extraType = null,
            string extra = null,
            TimelineItemScope? scope = null);

        TimelineItem SelectByPublicationId(int publicationId);
        TimelineItem Update(TimelineItem item);
        IList<TimelineItem> SelectFromEvent(int eventId);
        IList<TimelineItem> SelectFromEvent(int eventId, int take, int skip, int firstItem = 0);
        IList<TimelineItem> SelectFromGroup(int groupId, int take, int skip, int firstItem = 0);
        IList<TimelineItem> SelectFromPlace(int placeId);
        IList<TimelineItem> SelectFromUser(int userId);
        IList<TimelineItem> SelectHomeContactsWallItems(IList<int> contacts);
        IList<TimelineItem> SelectMyProfileWallItems(int userId, int profileId, int take, int skip, int firstItem = 0);
        IList<TimelineItem> GetNetwork(System.Collections.Generic.List<int> contacts, int take, int skip, int firstItem = 0);
        IList<TimelineItem> SelectPrivateWallItemsFromCompany(int companyId, int take, int skip, int firstItem = 0);
        IList<TimelineItem> SelectPublic();
        IList<TimelineItem> SelectPublicFromUser(int userId);
        IList<TimelineItem> SelectPublicWallItemsFromCompany(int companyId, int take, int skip, int firstItem = 0);
        IList<TimelineItem> SelectWallItemsFromPlace(int placeId, int take, int skip);
        IList<TimelineItem> SelectWallItemsFromProfile(int userId, int profileId, bool isContact, bool isMyProfile);
        IList<TimelineItem> SelectWallItemsFromProject(int projectId, int take, int skip);
        IList<TimelineItem> SelectWallItemsFromTeam(int teamId, int take, int skip);
        TimelineItem SelectWallItemById(int id);

        IList<TimelineItem> SelectWeekCompaniesPublications(DateTime start);
        IList<TimelineItem> SelectCompaniesPublicationsInDateRange(DateTime from, DateTime to);
        IList<TimelineItem> SelectNewRegistrants();
        IList<TimelineItem> GetLastRegistrants(short max);
        IList<TimelineItem> GetRegistrantsInDateRange(int max, DateTime from, DateTime to);
        int CountRegistrantsInDateRange(DateTime from, DateTime to);
        IList<TimelineItem> GetContactsRelations(IList<int> contacts);

        IList<TimelineItem> GetCompaniesPublications();

        IList<TimelineItem> SelectFromGroup(int groupId, DateTime start, DateTime now);

        IList<TimelineItem> SelectProfileWallItems(int userId, int profileId, bool isContact, int take, int skip, int firstItem);

        int GetCompaniesPublicationsCount();

        int CountGroupsPublications();

        int Count();
        int CountToday();
        int CountLast24Hours();

        int GetPeoplePublicationsCount();

        IList<TimelineItem> GetSponsoredNetwork(List<int> myContacts, int take, int skip, int firstItem);

        IList<TimelineItem> GetFromCompanies(int take, int skip, int firstItem);

        IList<TimelineItem> GetFromPeople(List<int> myContacts, int take, int skip, int firstItem);

        IList<TimelineItem> GetExternalFromCompany(int Id, int take, int skip, int firstItem);

        IList<TimelineItem> GetExternalFromCompanies(int take, int skip, int firstItem);

        IList<TimelineItem> GetWeekCompaniesPublicationsForDevices(DateTime dateTime);

        IList<TimelineItem> GetCompaniesPublicationsToValidate();
        int CountCompaniesPublicationsToValidate();

        SocialNetworkState InsertManyAndUpdateSocialState(List<TimelineItem> inserts, SocialNetworkState connection);

        IList<TimelineItem> GetByImportedId(string importedId);

        TimelineItem Delete(int id, WallItemDeleteReason reason, int userId);

        ////IList<TimelineItem> GetBySkill(int[]skillIds, TimelineDisplayContext displayContext, int displayContextItemId, int take, int skip, int firstItem);

        ////IList<Skill> GetAssociatedSkills();

        IList<Sparkle.Services.Networks.Models.Tags.TagModel> GetAssociatedSkillsWithWeight(int? userId);

        TimelineItemSkill AddSkillToItem(int skillId, int timelineItemId, User currentUser);
        AddTimelineTagResult AssociateSkill(AddTimelineTagRequest request);

        BasicResult RemoveSkillFromItem(int skillId, int timelineItemId, int userId, WallItemDeleteReason reason);
        IList<TimelineItem> SelectPeoplePublicationsInDateRange(DateTime from, DateTime to);
        IList<TimelineItem> SelectPartnersPublicationsInDateRange(DateTime from, DateTime to);

        TimelineOwnerChangeModel GetOwnerChangePreview(int timelineItemId);
        ChangeTimelineItemOwnerResult ChangeItemOwner(ChangeTimelineItemOwnerRequest request);

        TimelineItemsConveyor NewGetByContext(TimelineDisplayContext timelineDisplayContext, int id, int? userid, DateTime dateMax, bool isContact = false, IList<int> myContacts = null, int? itemOrList = null, string searchContent = null);
        void LoadOptionnalData(TimelineItemsConveyor conveyor);

        IList<TagModel> GetAssociatedSkills(int timelineItemId);

        TimelineItem AddCompanyJoinItem(Company company, int postedByUserId);

        TimelineItem PublishCompanyProfileUpdate(Company company, int userId, bool avoidMultiplePosts);

        TimelineItem PublishEvent(int eventId, int userId, int? companyId);

        void Notify(TimelineItem item, TimelineType type, User currentUser, TimelineItemComment comment);

        bool IsVisible(int timelineItemId, int? userId);
        bool IsVisible(TimelineItem item, User user);

        IList<TinyUserModel> GetTimelineItemLikers(int timelineItemId, int userId);

        IList<TinyUserModel> GetTimelineItemReaders(int timelineItemId, int userId);

        TimelineType GetTimelineType(int timelineItemId);
        TimelineType GetTimelineType(TimelineItem item);
        bool TryGetTimelineType(int timelineItemId, out TimelineType timelineType);
        bool TryGetTimelineType(TimelineItem item, out TimelineType timelineType);

        IList<int> GetUserIdsToNotifiedForTimelineComments(TimelineType type, TimelineItem item);

        ////TimelineItemModel GetModelById(int timelineItemId); // see work item 504

        int CountLikes(int timelineItemId);

        string GetUrl(int timelineItemId, UriKind uriKind);
        string GetUrl(int timelineItemId, int timelineCommentId, UriKind uriKind);

        TimelineItem PublishPartnerResourceUpdate(PartnerResource item, int userId, bool isNew);

        int CountByUser(int userId);
    }
}
