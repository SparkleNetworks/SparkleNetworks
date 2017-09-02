
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository]
    public interface IStatsCounterHitsRepository : IBaseNetworkRepository<StatsCounterHit, int>
    {
        IList<GetNewsletterStatsRow> GetNewsletterStats();
        IList<GetNewsletterStatsRow> GetNewsletterStats(int networkId);
    }
}
