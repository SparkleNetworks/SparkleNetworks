
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class AchievementsRepository : BaseNetworkRepositoryInt<Achievement>, IAchievementsRepository
    {
        [DebuggerStepThrough]
        public AchievementsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Achievements)
        {
        }

        public int Count()
        {
            return this.Set.Count();
        }
    }
}
