
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IDevicesService
    {
        List<Device> GetAll();
        Device Get(Guid deviceId);
        int Insert(Device item);
        Device Update(Device item);
        void Delete(Device item);

        Device Get(int deviceId);
    }
}
