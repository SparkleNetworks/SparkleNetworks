
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

    public class SocialNetworkCompanySubscriptionsRepository : BaseNetworkRepositoryInt<SocialNetworkCompanySubscription>, ISocialNetworkCompanySubscriptionsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SocialNetworkCompanySubscriptionsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SocialNetworkCompanySubscriptions)
        {
        }

        public IQueryable<SocialNetworkCompanySubscription> NewQuery(SocialNetworkCompanySubscriptionOptions options)
        {
            ObjectQuery<SocialNetworkCompanySubscription> query = this.Set;
            if ((options & SocialNetworkCompanySubscriptionOptions.Connection) == SocialNetworkCompanySubscriptionOptions.Connection)
                query = query.Include("SocialNetworkConnection");

            return query;
        }

        public IList<SocialNetworkCompanySubscription> GetAllActive(int networkId)
        {
            return this.Set.Where(s => s.Company.NetworkId == networkId).ToList();
        }

        public IList<SocialNetworkCompanySubscription> GetByNetworkConnectionUsername(int networkId, SocialNetworkConnectionType socialNetworkType, string networkConnectionUsername)
        {
            return this.Set
                .Where(s => s.SocialNetworkConnection.Username == networkConnectionUsername
                         && s.Company.NetworkId == networkId)
                .Where(s => s.Company.IsApproved && s.Company.IsEnabled)
                .ToList();
        }
    }
}
