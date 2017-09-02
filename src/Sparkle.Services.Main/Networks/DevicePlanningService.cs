
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Diagnostics;

    public class DevicePlanningService : ServiceBase, IDevicePlanningService
    {
        [DebuggerStepThrough]
        internal DevicePlanningService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public IList<DevicePlanning> GetFutureEvents(int devicedId)
        {
            var now = DateTime.UtcNow.Date;
            return this.Repo.DevicePlanning
                .Select()
                .Where(d => d.DeviceId == devicedId
                         && d.DateEndUtc > now)
                .ToList();
        }

        public DevicePlanning Insert(DevicePlanning planning)
        {
            return this.Repo.DevicePlanning.Insert(planning);
        }

        public IList<DevicePlanning> GetEntriesForDay(DateTime dateTime)
        {
            var begin = dateTime.Date;
            var end = dateTime.Date.AddDays(1);
            return this.GetEntries(begin, end);
        }

        public IList<DevicePlanning> GetEntries(DateTime dateTimeFromUtc, DateTime dateTimeToUtc)
        {
            return this.Repo.DevicePlanning
                .Select()
                .Where(e => e.DateEndUtc   > dateTimeFromUtc && e.DateEndUtc   < dateTimeToUtc
                         || e.DateStartUtc > dateTimeFromUtc && e.DateStartUtc < dateTimeToUtc)
                .ToList();
        }

        public DevicePlanning Get(int planningId)
        {
            return this.Repo.DevicePlanning.Select().Single(p => p.Id == planningId);
        }

        public void Delete(int planningId)
        {
            var item = this.Get(planningId);
            this.Repo.DevicePlanning.Delete(item);
        }
    }
}
