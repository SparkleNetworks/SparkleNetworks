
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class ExchangeSurfacesRepository : BaseNetworkRepositoryInt<ExchangeSurface>, IExchangeSurfacesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ExchangeSurfacesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ExchangeSurfaces)
        {
        }
    }
}
