
namespace Sparkle.Services.Networks.Subscriptions
{
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubscriptionEmailModel
    {
        public SubscriptionModel Subscription { get; set; }

        public SubscriptionTemplateModel Template { get; set; }

        public UserModel User { get; set; }

        public string NetworkName { get; set; }

        public string NetworkDomain { get; set; }
    }
}
