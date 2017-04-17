
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class SubscriptionNotification : IEntityInt32Id
    {
        public SubscriptionNotificationStatus StatusValue
        {
            get { return (SubscriptionNotificationStatus)this.Status; }
            set { this.Status = (byte)value; }
        }

        public override string ToString()
        {
            return "Notification " + this.Id + " to send " + this.DateSendUtc.ToShortDateString() + " is " + this.StatusValue.ToString();
        }
    }

    public enum SubscriptionNotificationStatus : byte
    {
        New = 0,
        Sent = 1,
        Ignored = 2,
    }
}
