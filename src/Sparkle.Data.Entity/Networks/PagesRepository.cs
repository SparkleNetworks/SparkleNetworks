
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Filters;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class PagesRepository : BaseNetworkRepositoryIntNetwork<Page>, IPagesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PagesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Pages)
        {
        }
    }
}
