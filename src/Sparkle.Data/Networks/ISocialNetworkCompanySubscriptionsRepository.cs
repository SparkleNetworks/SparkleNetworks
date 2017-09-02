
namespace Sparkle.Data.Networks
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;    using System.Collections.Generic;
    using System.Linq;

    [Repository]
    public interface ISocialNetworkCompanySubscriptionsRepository : IBaseNetworkRepository<SocialNetworkCompanySubscription, int>
    {
        IList<SocialNetworkCompanySubscription> GetAllActive(int networkId);

        IList<SocialNetworkCompanySubscription> GetByNetworkConnectionUsername(int networkId, SocialNetworkConnectionType socialNetworkType, string networkConnectionUsername);
        IQueryable<SocialNetworkCompanySubscription> NewQuery(SocialNetworkCompanySubscriptionOptions options);
    }
}
