
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class TouchCommunicationItemsRepository : BaseNetworkRepositoryInt<TouchCommunicationItem>, ITouchCommunicationItemsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TouchCommunicationItemsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TouchCommunicationItems)
        {
        }
    }
}
