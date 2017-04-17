
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;
    using System.Data.Objects;

    public class InterestsRepository : BaseNetworkRepositoryInt<Interest>, IInterestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public InterestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Interests)
        {
        }

        public bool AppliesToUsers
        {
            get { return true; }
        }

        public bool AppliesToCompanies
        {
            get { return false; }
        }

        public bool AppliesToGroups
        {
            get { return true; }
        }

        public bool AppliesToTimelineItems
        {
            get { return false; }
        }

        public IList<Interest> GetByNames(string[] names)
        {
            return this.Set
                .Where(o => names.Contains(o.TagName))
                .ToList();
        }

        public Interest GetByName(string name)
        {
            return this.Set
                .Where(o => o.TagName == name)
                .SingleOrDefault();
        }

        public IDictionary<int, Interest> GetAll()
        {
            return this.Set.OrderBy(t => t.TagName).ToDictionary(t => t.Id, t => t);
        }

        IDictionary<int, ITagV1> ITagsV1Repository.GetAll()
        {
            return this.Set.ToDictionary(x => x.Id, x => (ITagV1)x);
        }
    }

    public class GroupsInterestsRepository : BaseNetworkRepositoryInt<GroupInterest>, IGroupsInterestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupsInterestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.GroupInterests)
        {
        }

        public IQueryable<GroupInterest> NewQuery(GroupTagOptions options)
        {
            ObjectQuery<GroupInterest> query = this.Set;

            if ((options & GroupTagOptions.Group) == GroupTagOptions.Group)
                query = query.Include("Group");

            if ((options & GroupTagOptions.Tag) == GroupTagOptions.Tag)
                query = query.Include("Interest");

            return query;
        }

        public IDictionary<int, int> GetStats()
        {
            var values = new Dictionary<int, int>();
            var cxx = ((System.Data.EntityClient.EntityConnection)this.Context.Connection);
            var cmd = cxx.StoreConnection.CreateCommand();
            cmd.CommandText = @"select gs.InterestId, COUNT(gs.GroupId)
from dbo.GroupInterests as gs
inner join dbo.Groups as g on g.Id = gs.GroupId and g.IsDeleted = 0
where gs.DeletedDateUtc is null
group by gs.InterestId";
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(reader.GetInt32(0), reader.GetInt32(1));
                }
            }

            return values;
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.InterestId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }
}
