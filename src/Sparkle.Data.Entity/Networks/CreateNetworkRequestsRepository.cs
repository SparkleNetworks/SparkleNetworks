
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class CreateNetworkRequestsRepository : BaseNetworkRepositoryInt<CreateNetworkRequest>, ICreateNetworkRequestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CreateNetworkRequestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CreateNetworkRequests)
        {
        }

        public IList<CreateNetworkRequest> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }
    }
}
