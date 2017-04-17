
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class AlbumsRepository : BaseNetworkRepositoryInt<Album>, IAlbumsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public AlbumsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Albums)
        {
        }
    }

    public class PicturesRepository : BaseNetworkRepositoryInt<Picture>, IPicturesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PicturesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Pictures)
        {
        }
    }
}
