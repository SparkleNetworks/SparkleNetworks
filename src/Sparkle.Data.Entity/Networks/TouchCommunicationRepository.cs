
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using Sparkle.Data.Networks;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public class TouchCommunicationsRepository : BaseNetworkRepositoryInt<TouchCommunication>, ITouchCommunicationsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TouchCommunicationsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, mbox => mbox.TouchCommunications)
        {
        }
    }
}
