
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Sparkle.Data.Filters;

    public class PartnerResourcesRepository : BaseNetworkRepositoryInt<PartnerResource>, IPartnerResourcesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PartnerResourcesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PartnerResources)
        {
        }

        private IQueryable<PartnerResource> CreateQuery(PartnerResourceOptions options)
        {
            var query = this.Set;

            if ((options & PartnerResourceOptions.Tags) == PartnerResourceOptions.Tags)
            {
                query.Include("Tags.TagDefinition");
            }

            if ((options & PartnerResourceOptions.TagsCategory) == PartnerResourceOptions.TagsCategory)
            {
                query.Include("Tags.TagDefinition.Category");
            }

            return query;
        }

        public PartnerResource GetByAlias(string alias)
        {
            return this.Set
                .ByAlias(alias)
                .NotDeleted()
                .SingleOrDefault();
        }

        public PartnerResource GetActiveByAlias(string alias)
        {
            return this.Set
                .Active()
                .ByAlias(alias)
                .SingleOrDefault();
        }

        public PartnerResource GetActiveById(int id)
        {
            return this.Set
                .Active()
                .ById(id)
                .SingleOrDefault();
        }

        public IList<PartnerResource> GetActiveByIds(int networkId, int[] ids)
        {
            return this.Set
                .ByNetwork(networkId)
                .Active()
                .ByIds(ids)
                .OrderBy(r => r.Name)
                .ToList();
        }

        public IList<PartnerResource> GetAllActive(int networkId, PartnerResourceOptions options)
        {
            return this.CreateQuery(options)
                .ByNetwork(networkId)
                .Active()
                .OrderBy(r => r.Name)
                .ToList();
        }

        public IList<PartnerResource> GetAllPending(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .ToApprove()
                .ToList();
        }
    }
}
