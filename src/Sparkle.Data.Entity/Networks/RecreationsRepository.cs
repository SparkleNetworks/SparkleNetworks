
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class RecreationsRepository : BaseNetworkRepository<Recreation, int>, IRecreationsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public RecreationsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Recreations)
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

        public IDictionary<int, Recreation> GetAll()
        {
            return this.Set.OrderBy(t => t.TagName).ToDictionary(t => t.Id, t => t);
        }

        protected override Recreation GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(Recreation item)
        {
            return item.Id;
        }

        public Recreation GetByName(string name)
        {
            return this.Set
                .Where(o => o.TagName == name)
                .SingleOrDefault();
        }

        IDictionary<int, ITagV1> ITagsV1Repository.GetAll()
        {
            return this.Set.ToDictionary(x => x.Id, x => (ITagV1)x);
        }
    }

    public class GroupsRecreationsRepository : BaseNetworkRepository<GroupRecreation, int>, IGroupsRecreationsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupsRecreationsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.GroupRecreations)
        {
        }

        protected override GroupRecreation GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(GroupRecreation item)
        {
            return item.Id;
        }

        public IQueryable<GroupRecreation> NewQuery(GroupTagOptions options)
        {
            ObjectQuery<GroupRecreation> query = this.Set;

            if ((options & GroupTagOptions.Group) == GroupTagOptions.Group)
                query = query.Include("Group");

            if ((options & GroupTagOptions.Tag) == GroupTagOptions.Tag)
                query = query.Include("Recreation");

            return query;
        }

        public IDictionary<int, int> GetStats()
        {
            var values = new Dictionary<int, int>();
            var cxx = ((System.Data.EntityClient.EntityConnection)this.Context.Connection);
            var cmd = cxx.StoreConnection.CreateCommand();
            cmd.CommandText = @"select gs.RecreationId, COUNT(gs.GroupId)
from dbo.GroupRecreations as gs
inner join dbo.Groups as g on g.Id = gs.GroupId and g.IsDeleted = 0
where gs.DeletedDateUtc is null
group by gs.RecreationId";
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
            return this.Set.Where(x => x.RecreationId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }
}
