
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface IInboundEmailMessageRepository : IBaseNetworkRepository<InboundEmailMessage>
    {
    }
}
