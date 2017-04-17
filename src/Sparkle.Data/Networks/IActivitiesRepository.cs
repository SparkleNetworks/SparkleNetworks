
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository("Activities")]
    public interface IActivitiesRepository : IBaseNetworkRepository<Activity, int>
    {
        int MarkRead(int[] ids);

        IList<Activity> GetByTypeAndUserId(int type, int userId);
    }
}
