
namespace Sparkle.Data.Networks
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;
    using System.Linq;

    [Repository]
    public interface ISocialNetworkUserSubscriptionsRepository : IBaseNetworkRepository<SocialNetworkUserSubscription, int>
    {
        IList<SocialNetworkUserSubscription> GetAllActive(int networkId);
        IQueryable<SocialNetworkUserSubscription> NewQuery(SocialNetworkUserSubscriptionOptions options);

        IList<SocialNetworkUserSubscription> GetByNetworkConnectionUsername(int networkId, SocialNetworkConnectionType socialNetworkType, string networkConnectionUsername);
    }
}
