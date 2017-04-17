
namespace Sparkle.Services.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DeviceLayouts
    {
        private readonly static IList<DeviceLayout> layouts = new List<DeviceLayout>()
        {
            new DeviceLayout
            {
                Id = 1,
                Name = "News",
                Type = "SparkleTV.Layouts.NewsLayout",
                CanBeUsedAsDefault = true,
            },
            new DeviceLayout
            {
                Id = 2,
                Name = "Salle vide",
                Type = "SparkleTV.Layouts.FreeRoomLayout",
                CanBeUsedAsDefault = true,
            },
            new DeviceLayout
            {
                Id = 3,
                Name = "Salle de réunion occupée",
                Type = "SparkleTV.Layouts.MeetingRoomLayout",
                CanBeUsedAsDefault = false,
            },
        };

        public static IEnumerable<DeviceLayout> DefaultLayouts
        {
            get { return layouts.Where(l => l.CanBeUsedAsDefault); }
        }

        public static IEnumerable<DeviceLayout> PlanningLayouts
        {
            get { return layouts.Where(l => !l.CanBeUsedAsDefault); }
        }
    }
}
