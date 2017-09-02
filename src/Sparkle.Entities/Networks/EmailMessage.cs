
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class EmailMessage : INetworkEntity, IEntityInt32Id
    {
        public void SetLastSendErrorSafe(string value)
        {
            if (value != null)
            {
                this.LastSendError = value.Length > 800 ? value.Substring(0, 800) : value;
            }
            else
            {
                this.LastSendError = null;
            }
        }

        public override string ToString()
        {
            return "EmailMessage " + this.Id + " to " + this.EmailRecipient + " on " + this.DateSentUtc.ToString("s");
        }
    }
}
