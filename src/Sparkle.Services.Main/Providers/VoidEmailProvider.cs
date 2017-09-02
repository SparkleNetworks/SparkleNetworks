
namespace Sparkle.Services.Main.Providers
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    /// <summary>
    /// Implementation of <see cref="IEmailProvider"/> that DOES NOT SEND emails. May be usefull when the email provider encounters issues.
    /// </summary>
    public class VoidEmailProvider : LoggingEmailProvider
    {
        public override EmailSendResult[] SimpleSend(MailMessage message, string[] tags)
        {
            var results = new EmailSendResult[message.To.Count];

            for (int i = 0; i < message.To.Count; i++)
            {
                var recipient = message.To[i];
                var email = new EmailMessage
                {
                    DateSentUtc = DateTime.UtcNow,
                    EmailRecipient = recipient.ToString(),
                    EmailSender = message.From.ToString(),
                    EmailSubject = message.Subject,
                    ProviderName = this.GetType().Name,
                    Tags = tags != null ? string.Join(",", tags) : null,
                };
                email.SendSucceed = true;
                this.Log(email);

                results[i] = new EmailSendResult
                {
                    IsSent = true,
                    IsDelivered = false,
                    IsPendingDelivery = false,
                    RecipientAddress = recipient.Address,
                    RecipientName = recipient.DisplayName,
                };
            }

            return results;
        }
    }
}
