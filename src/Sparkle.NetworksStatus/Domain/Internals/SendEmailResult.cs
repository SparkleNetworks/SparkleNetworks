
namespace Sparkle.NetworksStatus.Domain.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    public class SendEmailResult
    {
        public SendEmailResult(MailAddress recipient)
            : this(recipient, null)
        {
        }

        public SendEmailResult(MailAddress recipient, Exception ex)
        {
            this.Recipient = recipient;
            this.Exception = ex;
            this.Success = ex == null;
        }

        public MailAddress Recipient { get; set; }

        public Exception Exception { get; set; }

        public bool Success { get; set; }
    }
}
