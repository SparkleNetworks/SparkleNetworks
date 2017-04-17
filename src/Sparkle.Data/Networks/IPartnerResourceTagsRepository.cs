
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IPartnerResourceTagsRepository : IBaseNetworkRepository<PartnerResourceTag, int>
    {
        IList<PartnerResourceTag> GetByPartnerResourceId(int partnerId);

        IList<PartnerResourceTag> GetByTagId(int tagId);

        IList<PartnerResourceTag> GetByPartnerResourceIds(int[] partnerIds);

        IDictionary<int, int[]> GetTagIds(int[] resourceIds);
    }
}
