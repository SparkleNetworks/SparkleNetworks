
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Diagnostics;

    public class DevicesService : ServiceBase, IDevicesService
    {
        [DebuggerStepThrough]
        internal DevicesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IDevicesRepository devicesRepository
        {
            get { return this.Repo.Devices; }
        }

        public List<Device> GetAll()
        {
            return devicesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ToList();
        }

        public Device Get(Guid deviceId)
        {
            return devicesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(d => d.DeviceId == deviceId)
                .FirstOrDefault();
        }

        public int Insert(Device item)
        {
            this.SetNetwork(item);

            return this.devicesRepository.Insert(item).Id;
        }

        public Device Update(Device item)
        {
            this.VerifyNetwork(item);

            return this.devicesRepository.Update(item);
        }

        public void Delete(Device item)
        {
            int deviceId = item.Id;
            var planning = this.Repo.DevicePlanning
                .Select()
                .Where(p => p.DeviceId == deviceId)
                .ToArray();

            // TODO: replace this by dynamic SQL or a stored procedure. it's not a good thing to fetch many entities just to delete them.
            foreach (var pl in planning)
            {
                this.Repo.DevicePlanning.Delete(pl);
            }

            this.devicesRepository.Delete(item);
        }

        public Device Get(int deviceId)
        {
            return this.devicesRepository
                .GetById(deviceId);
        }
    }
}
