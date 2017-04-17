
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class DeviceConfigurationRepository : BaseNetworkRepositoryInt<DeviceConfiguration>, IDeviceConfigurationRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public DeviceConfigurationRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.DeviceConfigurations)
        {
        }
    }
}
