
namespace Sparkle.Services.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DeviceLayouts
    {
        private readonly static IList<DeviceLayout> layouts = new List<DeviceLayout>();

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
