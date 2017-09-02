
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class NetworkTypesRepository : BaseNetworkRepositoryInt<NetworkType>, INetworkTypesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public NetworkTypesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.NetworkTypes)
        {
        }
    }
}
