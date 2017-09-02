
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public class BuildingRepository : BaseNetworkRepositoryInt<Building>, IBuildingRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public BuildingRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Buildings)
        {
        }
    }
}
