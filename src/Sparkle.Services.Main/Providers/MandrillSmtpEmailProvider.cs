
namespace Sparkle.Services.Main.Providers
{
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    /// <summary>
    /// Implementation of <see cref="IEmailProvider"/> that uses the http://mandrill.com/ SMTP endpoint.
    /// </summary>
    public class MandrillSmtpEmailProvider : SmtpEmailProvider
    {
        public override EmailSendResult[] SimpleSend(MailMessage message, string[] tags)
        {
            message.Headers.Add("X-MC-Track", "opens");
            message.Headers.Add("X-MC-Autotext", "yes");
            message.Headers.Add("X-MC-Tags", string.Join(",", tags));

            var results = base.SimpleSend(message, tags);
            return results;
        }
    }
}
