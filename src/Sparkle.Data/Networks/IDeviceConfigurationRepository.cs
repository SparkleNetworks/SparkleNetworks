
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IDeviceConfigurationRepository : IBaseNetworkRepository<DeviceConfiguration, int>
    {
    }
}
