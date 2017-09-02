
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public class StatsCounterHitsRepository : BaseNetworkRepositoryInt<StatsCounterHit>, IStatsCounterHitsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public StatsCounterHitsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.StatsCounterHits)
        {
        }

        public IList<GetNewsletterStatsRow> GetNewsletterStats()
        {
            return this.Context.GetNewsletterStats().ToArray();
        }

        public IList<GetNewsletterStatsRow> GetNewsletterStats(int networkId)
        {
            return this.Context.GetNetworksNewsletterStats(networkId).ToArray();
        }
    }
}
