
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IPartnerResourceProfileFieldsRepository : IBaseNetworkRepository<PartnerResourceProfileField, int>
    {
        IList<PartnerResourceProfileField> GetByPartnerResourceId(int partnerResourceId);
        PartnerResourceProfileField GetByPartnerIdAndFieldType(int partnerId, ProfileFieldType type);
        IDictionary<int, PartnerResourceProfileField[]> GetByPartnerIdAndFieldType(int[] resourceIds, ProfileFieldType[] fields);

        IList<PartnerResourceProfileField> GetAll();
    }
}
