
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ISubscriptionNotificationsRepository : IBaseNetworkRepository<SubscriptionNotification, int>
    {
        IList<SubscriptionNotification> GetBySubscriptionId(int subscriptionId);
        IList<SubscriptionNotification> GetBySubscriptionIds(int[] ids);

        void DeleteNotSentFromSubcriptionId(int subscriptionId);
    }
}
