
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Globalization;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Filters;
    using Sparkle.Entities.Networks;

    public class SubscriptionTemplatesRepository : BaseNetworkRepositoryIntNetwork<SubscriptionTemplate>, ISubscriptionTemplatesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SubscriptionTemplatesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SubscriptionTemplates)
        {
        }

        public SubscriptionTemplate GetById(int id, int networkId)
        {
            return this.Set.SingleOrDefault(t => t.Id == id && t.NetworkId == networkId);
        }

        public IList<SubscriptionTemplate> GetAll(int networkId)
        {
            return this.Set.Where(t => t.NetworkId == networkId).ToList();
        }

        public IList<SubscriptionTemplate> GetUserSubscribable(int networkId)
        {
            return this.Set
                .Where(t => t.NetworkId == networkId && t.IsUserSubscribable)
                .ToList();
        }

        public SubscriptionTemplate GetDefaultUserTemplate(int networkId)
        {
            return this.Set
                .Where(t => t.IsDefaultOnAccountCreate && t.IsSubscriptionEnabled && t.NetworkId == networkId)
                .SingleOrDefault();
        }
    }
}
