
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SubscriptionNotificationsRepository : BaseNetworkRepositoryInt<SubscriptionNotification>, ISubscriptionNotificationsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SubscriptionNotificationsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SubscriptionNotifications)
        {
        }

        public void DeleteNotSentFromSubcriptionId(int subscriptionId)
        {
            this.GetBySubscriptionId(subscriptionId)
                .Where(o => !o.DateSentUtc.HasValue)
                .ToList()
                .ForEach(o => this.Delete(o));
        }

        public IList<SubscriptionNotification> GetBySubscriptionId(int subscriptionId)
        {
            return this.Set
                .Where(o => o.SubscriptionId == subscriptionId)
                .ToList();
        }

        public IList<SubscriptionNotification> GetBySubscriptionIds(int[] ids)
        {
            return this.Set
                .Where(o => ids.Contains(o.SubscriptionId))
                .ToList();
        }
    }
}
