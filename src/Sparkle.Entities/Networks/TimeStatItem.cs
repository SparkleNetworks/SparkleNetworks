
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TimeStatItem
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public TimeStatGrouping Grouping { get; set; }

        public int Count1 { get; set; }
    }

    public enum TimeStatGrouping
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
    }
}
