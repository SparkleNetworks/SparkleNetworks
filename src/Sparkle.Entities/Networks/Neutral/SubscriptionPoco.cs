
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class SubscriptionPoco
    {
        public PaymentMethod PaymentMethodValue
        {
            get { return (PaymentMethod)this.PaymentMethod; }
            set { this.PaymentMethod = (byte)value; }
        }
    }
}
