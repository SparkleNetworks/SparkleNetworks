
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IDevicePlanningService
    {
        IList<DevicePlanning> GetFutureEvents(int deviceId);

        DevicePlanning Insert(DevicePlanning planning);

        IList<DevicePlanning> GetEntriesForDay(DateTime dateTime);
        IList<DevicePlanning> GetEntries(DateTime dateTimeFromUtc, DateTime dateTimeToUtc);

        DevicePlanning Get(int planningId);

        void Delete(int planningId);
    }
}
