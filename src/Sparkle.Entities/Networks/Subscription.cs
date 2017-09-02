
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class Subscription : IEntityInt32Id
    {
        public PaymentMethod PaymentMethodValue
        {
            get { return (PaymentMethod)this.PaymentMethod; }
            set { this.PaymentMethod = (byte)value; }
        }
    }

    public enum PaymentMethod
    {
        Unknown = 0,
        Cash = 1,
        Stripe = 2,
    }
}
