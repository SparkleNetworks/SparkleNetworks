
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using System.Data;
    using System.Data.Objects.SqlClient;
    using Sparkle.Entities.Networks.Neutral;
    using System.Data.SqlClient;
    using System.Data.Common;

    public class WallRepository : BaseNetworkRepositoryInt<TimelineItem>, IWallRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public WallRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TimelineItems)
        {
        }

        public IQueryable<TimelineItem> SelectWalls(IList<string> options)
        {
            if (options.Count > 0)
            {
                return SelectWithOptions(this.Set, options);
            }

            return this.Set;
        }

        protected override void OnDeleteOverride(NetworksEntities model, TimelineItem itemToDelete, TimelineItem actualItemToDelete)
        {
            base.OnDeleteOverride(model, itemToDelete, actualItemToDelete);

            if (!actualItemToDelete.Likes.IsLoaded)
            {
                actualItemToDelete.Likes.Load();
            }
        }

        public IQueryable<TimelineItem> NewQuery(TimelineItemOptions options)
        {
            ObjectQuery<TimelineItem> query = this.Set;

            if ((options & TimelineItemOptions.PostedBy) == TimelineItemOptions.PostedBy)
                query = query.Include("PostedBy");

            if ((options & TimelineItemOptions.User) == TimelineItemOptions.User)
                query = query.Include("User");

            if ((options & TimelineItemOptions.UserLikes) == TimelineItemOptions.UserLikes)
                query = query.Include("peoples_Likes");

            if ((options & TimelineItemOptions.Company) == TimelineItemOptions.Company)
                query = query.Include("Company");

            if ((options & TimelineItemOptions.Group) == TimelineItemOptions.Group)
                query = query.Include("Group");

            if ((options & TimelineItemOptions.Event) == TimelineItemOptions.Event)
                query = query.Include("Event");

            return query;
        }

        public SocialNetworkState InsertManyAndUpdateSocialState(int networkId, List<TimelineItem> inserts, SocialNetworkState connection)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("WallRepository.InsertManyAndUpdateSocialState: This cannot be used within a transaction.");
            
            using (var model = this.GetNewContext())
            {
                // insert multiple timeline items
                var itemsSet = this.GetSet(model);
                foreach (var item in inserts)
                {
                    itemsSet.AddObject(item);
                }

                // update connection state
                var set = model.SocialNetworkStates;
                model.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = model.CreateEntityKey(set.EntitySet.Name, connection);
                object outItem;
                if (model.TryGetObjectByKey(key, out outItem))
                {
                    set.ApplyCurrentValues(connection);
                    model.SaveChanges();
                }

                // and save!
                model.SaveChanges();

                return connection;
            }
        }

        public int CountByCompany(int companyId)
        {
            return this.Set
                .Where(t => t.CompanyId == companyId)
                .Count();
        }

        public IList<TimelineItem> GetByImportedIdExpression(int networkId, string expression)
        {
            var ids = this.Context.GetTimelineItemIdsByImportedIdExpression(networkId, expression).Select(i => i.Id).ToArray();
            return this.GetById(ids, TimelineItemOptions.None);
        }

        public IList<TimelineItem> GetById(int[] ids, TimelineItemOptions options)
        {
            return this.NewQuery(options)
                .Where(i => ids.Contains(i.Id))
                .OrderByDescending(i => i.Id)
                .ToList();
        }

        public IList<TimelineItem> GetRangedTimelineItems(int minId, int maxId)
        {
            return this.Context.GetRangedTimelineItem(minId, maxId)
                .ToList();
        }

        public TimelineItem GetWallItemById(int networkId, int wallId)
        {
            return this.Context.GetWallItemById(networkId, wallId).SingleOrDefault();
        }

        public IList<TimelineItem> GetLastFiveRegistrants(int networkId)
        {
            return this.Context.GetLastFiveRegistrants(networkId).ToList();
        }

        public int GetCompaniesPublicationToValidate(int networkId)
        {
            return this.Context.GetCompaniesPublicationToValidate(networkId).Single() ?? -1;
        }

        public IList<int> GetTimelineListIdPublic(int networkId, DateTime dateMax)
        {
            return this.Context.GetTimelineListIdPublic(networkId, dateMax).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdCompanyNetwork(int networkId, DateTime dateMax, int companyId)
        {
            return this.Context.GetTimelineListIdCompanyNetwork(networkId, dateMax, companyId).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdCompaniesNews(int networkId, DateTime dateMax)
        {
            return this.Context.GetTimelineListIdCompaniesNews(networkId, dateMax).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdPeopleNews(int networkId, DateTime dateMax, IList<int> myContacts)
        {
            return this.Context.GetTimelineListIdPeopleNews(networkId, dateMax).Where(o => myContacts.Contains(o.PostedByUserId)).Select(o => o.Id).ToList();
        }

        public IList<int> GetTimelineListIdPrivate(int networkId, DateTime dateMax, int userId)
        {
            return this.Context.GetTimelineListIdPrivate(networkId, dateMax, userId).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdProfile(int networkId, DateTime dateMax, int id, bool isContact)
        {
            return this.Context.GetTimelineListIdProfile(networkId, dateMax, id, isContact ? 1 : 0).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdCompany(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdCompany(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdExternalCompany(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdExternalCompany(networkId,  dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdExternalCompanies(int networkId, DateTime dateMax)
        {
            return this.Context.GetTimelineListIdExternalCompanies(networkId, dateMax).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdEvent(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdEvent(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdGroup(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdGroup(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdPlace(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdPlace(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdProject(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdProject(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdTeam(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdTeam(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public IList<int> GetTimelineListIdTopic(int networkId, DateTime dateMax, int id)
        {
            return this.Context.GetTimelineListIdTopic(networkId, dateMax, id).Select(o => o.Value).ToList();
        }

        public TimelineItemSkill[] GetTimelineSkillsListIdForCount(int networkId)
        {
            return this.Context.GetTimelineSkillsListIdForCount(networkId).ToArray();
        }

        public IList<int> GetTimelineListIdSearchByContent(int networkId, DateTime dateMax, string[] searchContent, bool accrued)
        {
            if (searchContent.Length == 0)
                return new List<int>();

            var sqlQuery = 
@"SELECT TOP 42 T.Id 
FROM dbo.TimelineItems T 
WHERE T.NetworkId = @p0 
AND T.CreateDate < @p1 
AND T.DeleteReason IS NULL 
AND T.ItemType != 2 
AND CONTAINS(T.Text, @p2) 
ORDER BY T.CreateDate DESC, T.Id ASC";
          
            object[] param = new object[searchContent.Length + 2];
            param[0] = networkId;
            param[1] = dateMax;
          
            var containsVar = searchContent[0];
            for (int i = 1; i < searchContent.Length; i++)
                containsVar += (accrued ? " AND " : " OR ") + searchContent[i];
            param[2] = containsVar;

            ////var query = this.Context.ExecuteStoreQuery<int>(sqlQuery, param);

            var result = new List<int>();
            var cxxString = this.Context.Connection.ConnectionString.Split(new char[] { '\"', })[1];
            using (var cxx = new SqlConnection(cxxString))
            {
                var query = cxx.CreateCommand();
                query.CommandText = sqlQuery;
                query.AddParameter("p0", networkId, DbType.Int32)
                    .AddParameter("p1", dateMax, DbType.DateTime)
                    .AddParameter("p2", containsVar, DbType.String);
                if (cxx.State == ConnectionState.Closed)
                    cxx.Open();
                using (var reader = query.ExecuteReader(CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetInt32(0));
                    }
                }
            }

            return result;
        }

        public int CountByDateRangeAndTypeAndCompany(DateTime begin, DateTime end, int itemType, int companyId)
        {
            return this.Set
                .Where(i => i.ItemType == itemType
                         && i.CompanyId == companyId
                         && begin <= i.CreateDate
                         && i.CreateDate <= end)
                .Count();
        }

        public int CountByDateRangeAndTypeAndUser(DateTime begin, DateTime end, int itemType, int userId)
        {
            return this.Set
                .Where(i => i.ItemType == itemType
                         && i.UserId == userId
                         && begin <= i.CreateDate
                         && i.CreateDate <= end)
                .Count();
        }

        public int CountCreatedByUserId(int userId, int networkId)
        {
            return this.Set
                .Where(x => x.NetworkId == networkId && x.PostedByUserId == userId)
                .Count();
        }
    }
}