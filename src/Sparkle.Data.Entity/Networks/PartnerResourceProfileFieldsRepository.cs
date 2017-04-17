
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PartnerResourceProfileFieldsRepository : BaseNetworkRepositoryInt<PartnerResourceProfileField>, IPartnerResourceProfileFieldsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PartnerResourceProfileFieldsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PartnerResourceProfileFields)
        {
        }

        public IList<PartnerResourceProfileField> GetByPartnerResourceId(int partnerResourceId)
        {
            return this.Set
                .Where(o => o.PartnerResourceId == partnerResourceId)
                .ToList();
        }

        public PartnerResourceProfileField GetByPartnerIdAndFieldType(int partnerId, ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.PartnerResourceId == partnerId)
                .Where(o => o.ProfileFieldId == (int)type)
                .SingleOrDefault();
        }

        public IDictionary<int, PartnerResourceProfileField[]> GetByPartnerIdAndFieldType(int[] resourceIds, ProfileFieldType[] fields)
        {
            var typeIds = fields.Cast<int>().ToArray();
            var items = this.Set
                .Where(o => resourceIds.Contains(o.PartnerResourceId) && typeIds.Contains(o.ProfileFieldId))
                .ToList();
            return items
                .GroupBy(a => a.ProfileFieldId)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }

        public IList<PartnerResourceProfileField> GetAll()
        {
            return this.Set
                .ToList();
        }
    }
}
