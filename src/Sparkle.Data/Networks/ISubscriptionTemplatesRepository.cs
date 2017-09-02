
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ISubscriptionTemplatesRepository : IBaseNetworkRepositoryNetwork<SubscriptionTemplate, int>
    {
        SubscriptionTemplate GetById(int id, int networkId);
        IList<SubscriptionTemplate> GetAll(int networkId);
        IList<SubscriptionTemplate> GetUserSubscribable(int networkId);

        SubscriptionTemplate GetDefaultUserTemplate(int networkId);
    }
}
