
namespace Sparkle.Services.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IDeviceConfigurationService
    {
        DeviceConfiguration Add(DeviceConfiguration item);
        
        DeviceConfiguration Update(DeviceConfiguration item);

        DeviceConfiguration GetByKey(string key);

        IList<DeviceConfiguration> GetAll();

        int Count();
    }
}
