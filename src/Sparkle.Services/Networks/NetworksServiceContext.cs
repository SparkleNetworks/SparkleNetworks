
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NetworksServiceContext
    {
        private static TimeZoneInfo defaultTimezone = TimeZoneInfo.Utc;
        private TimeZoneInfo timezone = TimeZoneInfo.Utc;

        public ContextParallelismMode ParallelismMode { get; set; }

        public TimeZoneInfo Timezone
        {
            get { return this.timezone ?? defaultTimezone; }
            set { this.timezone = value; }
        }

        public int? UserId { get; set; }

        public NetworksServiceContext(ContextParallelismMode mode = ContextParallelismMode.None)
        {
            this.ParallelismMode = mode;
        }
    }

    public enum ContextParallelismMode
    {
        None = 0,
        ThreadPool,
    }
}
