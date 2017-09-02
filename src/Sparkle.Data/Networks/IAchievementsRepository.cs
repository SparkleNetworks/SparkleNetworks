
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IAchievementsRepository : IBaseNetworkRepository<Achievement, int>
    {
        int Count();
    }
}
