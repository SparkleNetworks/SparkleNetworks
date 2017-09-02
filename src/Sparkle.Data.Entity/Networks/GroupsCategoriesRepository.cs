
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class GroupsCategoriesRepository : BaseNetworkRepositoryInt<GroupCategory>, IGroupsCategoriesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupsCategoriesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.GroupCategories)
        {
        }

        public int Count()
        {
            return this.Set.Count();
        }
    }
}
