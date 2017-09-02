
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class InboundEmailMessage : IEntityInt32Id, INetworkEntity
    {
        public InboundEmailProvider ProviderValue
        {
            get { return (InboundEmailProvider)this.Provider; }
            set { this.Provider = (int)value; }
        }

        public override string ToString()
        {
            return "Email " + this.Id + " from " + 
                SenderEmailAccount + (SenderEmailTag != null ? "+" + SenderEmailTag : "") + SenderEmailDomain + " to " +
                ReceiverEmailAccount + (ReceiverEmailTag != null ? "+" + ReceiverEmailTag : "") + ReceiverEmailDomain + 
                " brougth to us by " + this.ProviderValue.ToString();
        }
    }

    public enum InboundEmailProvider
    {
        Mandrill,
    }
}
