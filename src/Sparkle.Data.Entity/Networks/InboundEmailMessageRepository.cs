
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class InboundEmailMessageRepository : BaseNetworkRepositoryInt<InboundEmailMessage>, IInboundEmailMessageRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public InboundEmailMessageRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.InboundEmailMessages)
        {
        }

    }
}
