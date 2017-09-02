
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;

    [Repository]
    public interface IWallRepository : IBaseNetworkRepository<TimelineItem, int>
    {
        IQueryable<TimelineItem> NewQuery(TimelineItemOptions options);

        SocialNetworkState InsertManyAndUpdateSocialState(int p, System.Collections.Generic.List<TimelineItem> inserts, SocialNetworkState connection);

        int CountByCompany(int companyId);

        IList<TimelineItem> GetByImportedIdExpression(int networkId, string expression);

        IList<TimelineItem> GetById(int[] ids, TimelineItemOptions options);

        IList<int> GetTimelineListIdPublic(int networkId, DateTime dateMax);

        IList<TimelineItem> GetRangedTimelineItems(int minId, int maxId);

        TimelineItem GetWallItemById(int networkId, int wallId);

        IList<TimelineItem> GetLastFiveRegistrants(int networkId);

        int GetCompaniesPublicationToValidate(int networkId);

        IList<int> GetTimelineListIdCompanyNetwork(int networkId, DateTime dateMax, int companyId);

        IList<int> GetTimelineListIdCompaniesNews(int networkId, DateTime dateMax);

        IList<int> GetTimelineListIdPeopleNews(int networkId, DateTime dateMax, IList<int> myContacts);

        IList<int> GetTimelineListIdPrivate(int networkId, DateTime dateMax, int userId);

        IList<int> GetTimelineListIdProfile(int networkId, DateTime dateMax, int id, bool isContact);

        IList<int> GetTimelineListIdCompany(int networkId, DateTime dateMax, int id);

        IList<int> GetTimelineListIdExternalCompany(int networkId, DateTime dateMax, int id);

        IList<int> GetTimelineListIdExternalCompanies(int networkId, DateTime dateMax);

        IList<int> GetTimelineListIdEvent(int networkId, DateTime dateMax, int id);

        IList<int> GetTimelineListIdGroup(int networkId, DateTime dateMax, int id);

        IList<int> GetTimelineListIdPlace(int networkId, DateTime dateMax, int id);
                                                
        IList<int> GetTimelineListIdProject(int networkId, DateTime dateMax, int id);

        IList<int> GetTimelineListIdTeam(int networkId, DateTime dateMax, int id);

        IList<int> GetTimelineListIdTopic(int networkId, DateTime dateMax, int id);

        TimelineItemSkill[] GetTimelineSkillsListIdForCount(int networkId);

        IList<int> GetTimelineListIdSearchByContent(int networkId, DateTime dateMax, string[] searchContent, bool accrued);

        int CountByDateRangeAndTypeAndCompany(DateTime begin, DateTime end, int itemType, int companyId);

        int CountByDateRangeAndTypeAndUser(DateTime begin, DateTime end, int itemType, int userId);

        int CountCreatedByUserId(int userId, int networkId);
    }
}