
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IStatsCountersRepository : IBaseNetworkRepository<StatsCounter, int>
    {
        StatsCounter GetCounter(string category, string name);
    }
}
