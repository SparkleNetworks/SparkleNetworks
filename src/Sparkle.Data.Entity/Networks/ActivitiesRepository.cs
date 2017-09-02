
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public class ActivitiesRepository : BaseNetworkRepositoryInt<Activity>, IActivitiesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ActivitiesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Activities)
        {
        }

        public int MarkRead(int[] ids)
        {
            return this.Context.MarkActivitiesAsReadByIds(string.Join(";", ids));
        }

        public IList<Activity> GetByTypeAndUserId(int type, int userId)
        {
            return this.Set
                .Where(o => o.Type == type && o.UserId == userId)
                .ToList();
        }
    }
}
