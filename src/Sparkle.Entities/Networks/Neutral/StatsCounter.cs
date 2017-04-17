
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StatsCounter
    {
        public StatsCounter()
        {
        }

        public StatsCounter(string category, string name)
        {
            this.Category = category;
            this.Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public static StatsCounter FromEntity(Sparkle.Entities.Networks.StatsCounter item)
        {
            return new StatsCounter
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
            };
        }

        public override string ToString()
        {
            return "Counter " + this.Id + " " + this.Category + "/" + this.Name;
        }

        public static class WellKnown
        {
            public static class WeeklyNewsletter
            {
                private static readonly StatsCounter display = new StatsCounter("Display", "WeeklyNewsletter");
                private static readonly StatsCounter follow = new StatsCounter("Follow", "WeeklyNewsletter");
                private static readonly StatsCounter links = new StatsCounter("Links", "WeeklyNewsletter");
                private static readonly StatsCounter send = new StatsCounter("Send", "WeeklyNewsletter");

                public static StatsCounter Display
                {
                    get { return display; }
                }

                public static StatsCounter Follow
                {
                    get { return follow; }
                }

                public static StatsCounter Links
                {
                    get { return links; }
                }

                public static StatsCounter Send
                {
                    get { return send; }
                }
            }

            public static class DailyNewsletter
            {
                private static readonly StatsCounter display = new StatsCounter("Display", "DailyNewsletter");
                private static readonly StatsCounter follow = new StatsCounter("Follow", "DailyNewsletter");
                private static readonly StatsCounter links = new StatsCounter("Links", "DailyNewsletter");
                private static readonly StatsCounter send = new StatsCounter("Send", "DailyNewsletter");

                public static StatsCounter Display
                {
                    get { return display; }
                }

                public static StatsCounter Follow
                {
                    get { return follow; }
                }

                public static StatsCounter Links
                {
                    get { return links; }
                }

                public static StatsCounter Send
                {
                    get { return send; }
                }
            }

            public static class Affiliates
            {
                private static readonly StatsCounter display = new StatsCounter("Display", "Affiliates"); // info page
                private static readonly StatsCounter follow = new StatsCounter("Follow", "Affiliates");   // link to external site
                private static readonly StatsCounter listDisplay = new StatsCounter("ListDisplay", "Affiliates"); // list page

                public static StatsCounter Display
                {
                    get { return display; }
                }

                public static StatsCounter Follow
                {
                    get { return follow; }
                }

                public static StatsCounter ListDisplay
                {
                    get { return listDisplay; }
                }
            }

            public static class PartnerResources
            {
                private static readonly StatsCounter display = new StatsCounter("Display", "PartnerResources"); // partner details
                private static readonly StatsCounter follow = new StatsCounter("Follow", "PartnerResources");   // link to partner website / show contact info
                private static readonly StatsCounter listDisplay = new StatsCounter("ListDisplay", "PartnerResources"); // partners list

                public static StatsCounter Display
                {
                    get { return display; }
                }

                public static StatsCounter Follow
                {
                    get { return follow; }
                }

                public static StatsCounter ListDisplay
                {
                    get { return listDisplay; }
                }
            }
        }
    }
}
