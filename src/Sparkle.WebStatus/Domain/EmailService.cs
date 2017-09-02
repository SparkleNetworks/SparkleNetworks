
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    public class EmailService : BaseService
    {
        private  IEmailProvider provider;
        
        public EmailService(ServiceFactory serviceFactory)
            : base(serviceFactory, "Email")
        {
        }

        private IEmailProvider Provider
        {
            get
            {
                if (this.provider == null)
                {
                    this.provider = new SmtpEmailProvider();
                    this.provider.Initialize(this.Services);
                }

                return this.provider;
            }
        }

        internal void SendTextMessage(string toEmail, string toDisplayName, string subject, string contents, string tag)
        {
            var message = new MailMessage();
            message.From = new MailAddress("noreply@sparklenetworks.net", "sparklenetworks.net");
            message.To.Add(new MailAddress(toEmail, toDisplayName));
            message.Subject = subject;
            message.Body = contents;
            message.IsBodyHtml = false;

            this.Provider.SimpleSend(message, new[] { "sparklenetworksnet", tag, });
        }
    }
}
