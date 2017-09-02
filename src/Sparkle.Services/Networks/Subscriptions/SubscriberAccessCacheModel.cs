
namespace Sparkle.Services.Networks.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubscriberAccessCacheModel
    {
        public IList<SubscriptionModel> Items { get; set; }

        public SubscriptionTemplateModel UserDefaultTemplate { get; set; }
    }
}
