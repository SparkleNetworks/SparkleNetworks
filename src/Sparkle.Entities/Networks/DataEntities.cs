
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class DataEntities
    {
        private static readonly TimeZoneInfo databaseTimezone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

        public static TimeZoneInfo DatabaseTimezone
        {
            get { return databaseTimezone; }
        }
    }
}
