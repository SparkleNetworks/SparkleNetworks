
namespace Sparkle.Services.Networks.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserPresenceStats
    {
        public double? PerDayPresenceAverageUsers { get; set; }

        public double? PerDayPresenceMaxUsers { get; set; }

        public double? UserPresenceAverageDays { get; set; }

        public double? UserPresenceMaxDays { get; set; }

        public int? TotalUsers { get; set; }

        public int? TotalDays { get; set; }
    }
}
