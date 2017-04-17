
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class StatsCountersRepository : BaseNetworkRepositoryInt<StatsCounter>, IStatsCountersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public StatsCountersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.StatsCounters)
        {
        }

        public StatsCounter GetCounter(string category, string name)
        {
            return this.Set.SingleOrDefault(c => c.Category == category && c.Name == name);
        }
    }
}
