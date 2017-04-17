
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    public class EmailSendResult
    {
        public string RecipientAddress { get; set; }
        public string RecipientName { get; set; }

        public bool IsSent { get; set; }
        public bool? IsDelivered { get; set; }
        public bool? IsPendingDelivery { get; set; }

        public string ErrorDetail { get; set; }

        public static EmailSendResult[] CreateErrorResults(MailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var results = new EmailSendResult[message.To.Count];
            for (int i = 0; i < message.To.Count; i++)
            {
                var result = new EmailSendResult
                {
                    RecipientName = message.To[i].DisplayName,
                    RecipientAddress = message.To[i].Address,
                    IsSent = false,
                    IsDelivered = false,
                    IsPendingDelivery = false,
                };
                results[i] = result;
            }

            return results;
        }
    }
}
