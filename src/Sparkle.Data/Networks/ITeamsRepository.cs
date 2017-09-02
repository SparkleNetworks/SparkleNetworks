
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ITeamsRepository : IBaseNetworkRepository<Team, int>
    {
    }
}