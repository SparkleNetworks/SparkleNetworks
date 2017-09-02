
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class MenusRepository : BaseNetworkRepositoryInt<Menu>, IMenusRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public MenusRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Menus)
        {
        }
    }
}
