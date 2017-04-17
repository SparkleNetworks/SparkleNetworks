
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IDevicesRepository : IBaseNetworkRepository<Device, int>
    {
    }
}
