
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;

    [Repository("Live")]
    public interface ILiveRepository : IBaseNetworkRepository<Live, int>
    {
        IList<LivePerson> GetOnline();

        int GetUsersDaysCount(int userId);
    }
}
