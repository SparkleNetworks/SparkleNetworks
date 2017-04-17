
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class DevicePlanningRepository : BaseNetworkRepositoryInt<DevicePlanning>, IDevicePlanningRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public DevicePlanningRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.DevicePlannings)
        {
        }
    }
}
