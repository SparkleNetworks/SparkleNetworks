﻿
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class ExchangeMaterialsRepository : BaseNetworkRepositoryInt<ExchangeMaterial>, IExchangeMaterialsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ExchangeMaterialsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ExchangeMaterials)
        {
        }
    }
}
