
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PartnerResourceTagsRepository : BaseNetworkRepositoryInt<PartnerResourceTag>, IPartnerResourceTagsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PartnerResourceTagsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PartnerResourceTags)
        {
        }

        public IList<PartnerResourceTag> GetByPartnerResourceId(int partnerId)
        {
            return this.Set
                .Where(o => o.PartnerResourceId == partnerId)
                .ToList();
        }

        public IList<PartnerResourceTag> GetByTagId(int tagId)
        {
            return this.Set
                .Where(o => o.TagId == tagId)
                .ToList();
        }

        public IList<PartnerResourceTag> GetByPartnerResourceIds(int[] partnerIds)
        {
            return this.Set
                .Where(o => partnerIds.Contains(o.PartnerResourceId))
                .ToList();
        }

        public IDictionary<int, int[]> GetTagIds(int[] resourceIds)
        {
            var items = this.Set
                .Where(x => resourceIds.Contains(x.PartnerResourceId))
                .Select(x => new
                {
                    ResourceId = x.PartnerResourceId,
                    TagId = x.TagId,
                }).ToArray();
            return items
                .GroupBy(x => x.ResourceId)
                .ToDictionary(x => x.Key, x => x.Select(y => y.TagId).ToArray());
        }
    }
}
