
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IPartnerResourcesRepository : IBaseNetworkRepository<PartnerResource, int>
    {
        PartnerResource GetByAlias(string alias);
        PartnerResource GetActiveByAlias(string alias);

        PartnerResource GetActiveById(int id);
        IList<PartnerResource> GetActiveByIds(int networkId, int[] ids);

        IList<PartnerResource> GetAllActive(int networkId, PartnerResourceOptions options);

        IList<PartnerResource> GetAllPending(int networkId);

    }
}
