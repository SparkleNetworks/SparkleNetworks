
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Filters;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class GroupsRepository : BaseNetworkRepositoryInt<Group>, IGroupsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Groups)
        {
        }

        public Group GetActiveById(int id)
        {
            return this.Set
                .Active()
                .Where(o => o.Id == id)
                .SingleOrDefault();
        }

        public IList<Group> GetById(int[] ids)
        {
            return this.Set
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Group> GetActiveById(int[] ids)
        {
            return this.Set
                .Active()
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Group> GetById(int[] ids, int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Group> GetActiveById(int[] ids, int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Active()
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IQueryable<Group> CreateQuery(GroupOptions options)
        {
            ObjectQuery<Group> query = this.Set;

            if ((options & GroupOptions.Category) == GroupOptions.Category)
                query = query.Include("GroupCategory");

            return query;
        }

        public IList<Group> GetById(int[] ids, GroupOptions options)
        {
            return this.CreateQuery(options)
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Group> GetActiveById(int[] ids, GroupOptions options)
        {
            return this.CreateQuery(options)
                .Active()
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Group> GetById(int[] ids, int networkId, GroupOptions options)
        {
            return this.CreateQuery(options)
                .ByNetwork(networkId)
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public Group GetById(int id, int networkId, GroupOptions options)
        {
            return this.CreateQuery(options)
                .ByNetwork(networkId)
                .Where(g => id == g.Id)
                .SingleOrDefault();
        }

        public IList<Group> GetActiveById(int[] ids, int networkId, GroupOptions options)
        {
            return this.CreateQuery(options)
                .ByNetwork(networkId)
                .Active()
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public Group GetByAlias(string alias, GroupOptions options)
        {
            var query = this.CreateQuery(options)
                .Where(x => x.Alias != null && x.Alias.Equals(alias));
            var item = query.SingleOrDefault();
            return item;
        }

        public Group GetByAlias(string alias, int networkId, GroupOptions options)
        {
            var query = this.CreateQuery(options)
                .Where(x => x.Alias != null && x.Alias.Equals(alias) && x.NetworkId == networkId);
            var item = query.SingleOrDefault();
            return item;
        }
    }

    public class TeamsRepository : BaseNetworkRepositoryInt<Team>, ITeamsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TeamsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Teams)
        {
        }
    }
}
