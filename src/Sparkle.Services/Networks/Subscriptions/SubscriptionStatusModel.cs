
namespace Sparkle.Services.Networks.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubscriptionStatusModel
    {
        public Dictionary<int, SubscriptionTemplateModel> Templates { get; set; }

        public IDictionary<DateTime, IDictionary<int, int>> Monthly { get; set; }

        public Dictionary<int, int> Actual { get; set; }

        public int ActiveUsers { get; set; }

        public int InactiveUsers { get; set; }
    }
}
