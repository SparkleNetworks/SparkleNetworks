
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Networks.Options;
    using System.Data.Objects;

    public class SocialNetworkUserSubscriptionsRepository : BaseNetworkRepositoryInt<SocialNetworkUserSubscription>, ISocialNetworkUserSubscriptionsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SocialNetworkUserSubscriptionsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SocialNetworkUserSubscriptions)
        {
        }

        public IQueryable<SocialNetworkUserSubscription> NewQuery(SocialNetworkUserSubscriptionOptions options)
        {
            ObjectQuery<SocialNetworkUserSubscription> query = this.Set;
            if ((options & SocialNetworkUserSubscriptionOptions.Connection) == SocialNetworkUserSubscriptionOptions.Connection)
                query = query.Include("SocialNetworkConnection");

            return query;
        }

        public IList<SocialNetworkUserSubscription> GetAllActive(int networkId)
        {
            return this.Set.Where(s => s.User.NetworkId == networkId).ToList();
        }

        public IList<SocialNetworkUserSubscription> GetByNetworkConnectionUsername(int networkId, SocialNetworkConnectionType socialNetworkType, string networkConnectionUsername)
        {
            return this.Set
                .Where(s => s.SocialNetworkConnection.Username == networkConnectionUsername
                         && s.User.NetworkId == networkId)
                .Where(o => o.User.CompanyAccessLevel > 0
                            && o.User.NetworkAccessLevel > 0
                            && o.User.IsEmailConfirmed
                            && (!o.User.AccountClosed.Value || !o.User.AccountClosed.HasValue))
                .ToList();
        }
    }
}
