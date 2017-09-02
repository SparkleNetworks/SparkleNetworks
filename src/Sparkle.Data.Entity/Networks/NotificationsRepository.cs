
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class NotificationsRepository : BaseNetworkRepository<Notification, int>, INotificationsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public NotificationsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Notifications)
        {
        }

        protected override Notification GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(n => n.UserId == id);
        }

        protected override int GetEntityId(Notification item)
        {
            return item.UserId;
        }
    }
}
