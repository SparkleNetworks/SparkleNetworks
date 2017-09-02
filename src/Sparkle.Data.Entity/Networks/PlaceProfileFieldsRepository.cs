
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PlaceProfileFieldsRepository : BaseNetworkRepositoryInt<PlaceProfileField>, IPlaceProfileFieldsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PlaceProfileFieldsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PlaceProfileFields)
        {
        }
    }
}
