
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class DevicesRepository : BaseNetworkRepositoryInt<Device>, IDevicesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public DevicesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Devices)
        {
        }
    }
}
