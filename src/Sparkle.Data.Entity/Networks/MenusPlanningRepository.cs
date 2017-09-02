
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class MenusPlanningRepository : BaseNetworkRepositoryInt<MenuPlanning>, IMenusPlanningRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public MenusPlanningRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.MenuPlannings)
        {
        }
    }
}
