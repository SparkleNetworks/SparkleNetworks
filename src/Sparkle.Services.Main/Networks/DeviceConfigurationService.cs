
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Diagnostics;

    public class DeviceConfigurationService : ServiceBase, IDeviceConfigurationService
    {
        [DebuggerStepThrough]
        internal DeviceConfigurationService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public DeviceConfiguration Add(DeviceConfiguration item)
        {
            this.SetNetwork(item);

            return this.Repo.DeviceConfiguration.Insert(item);
        }

        public DeviceConfiguration Update(DeviceConfiguration item)
        {
            this.VerifyNetwork(item);

            return this.Repo.DeviceConfiguration.Update(item);
        }

        public DeviceConfiguration GetByKey(string key)
        {
            return this.Repo.DeviceConfiguration
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(i => i.Key == key)
                .SingleOrDefault();
        }

        public IList<DeviceConfiguration> GetAll()
        {
            return this.Repo.DeviceConfiguration
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ToList();
        }

        public int Count()
        {
            return this.Repo.DeviceConfiguration
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Count();
        }
    }
}
