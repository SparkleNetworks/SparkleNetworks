
namespace Sparkle.Services.Main.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security;
    using System.Text;
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Networks.Models;

    /// <summary>
    /// Implementation of <see cref="IEmailProvider"/> that uses the standard .NET SMTP client.
    /// </summary>
    public class SmtpEmailProvider : LoggingEmailProvider
    {
        private SmtpClient client;
        private int emailsSent;
        private long dataLengthSent;

        private string Host { get; set; }
        private int Port { get; set; }
        private bool EnableSsl { get; set; }
        private string Username { get; set; }
        private SecureString Password { get; set; }

        public override void Configure(string configuration)
        {
            base.Configure(configuration);

            dynamic config = JsonConvert.DeserializeObject(configuration);

            // the configuration entry in sparkle systems should look like:
            // Sparkle.Services.Main.Providers.SmtpEmailProvider, {Host:"localhost",Port:25,Username:null,Password:null,EnableSsl:false}
            // Sparkle.Services.Main.Providers.SmtpEmailProvider, {Host:"smtp.myserver.com",Port:587,Username:"blah",Password:"foo",EnableSsl:true}

            this.Host = config.Host;
            if (config.Port != null)
                this.Port = config.Port;
            if (config.Username != null)
                this.Username = config.Username;
            if (config.EnableSsl != null)
                this.EnableSsl = config.EnableSsl;
            if (config.Password != null)
            {
                string value = config.Password;
                var charValue = value.ToCharArray();
                this.Password = new SecureString();
                for (int i = 0; i < charValue.Length; i++)
                {
                    this.Password.InsertAt(i, charValue[i]);
                }
            }
        }

        public override EmailSendResult[] SimpleSend(MailMessage message, string[] tags)
        {
            var result = this.SimpleSendImpl(message, tags);
            return result;
        }

        private long GetSentLength(MailMessage message)
        {
            long dataLength = message.Body.Length;
            foreach (var head in message.Headers.AllKeys)
            {
                dataLength += head.Length;
                dataLength += message.Headers[head].Length;
            }

            return dataLength;
        }

        private EmailSendResult[] SimpleSendImpl(MailMessage message, string[] tags)
        {
            var result = new EmailSendResult[message.To.Count];
            var email = new EmailMessage
            {
                DateSentUtc = DateTime.UtcNow,
                EmailRecipient = string.Join(" ; ", message.To.Select(r => r.ToString())),
                EmailSender = message.From.ToString(),
                EmailSubject = message.Subject,
                ProviderName = this.GetType().Name,
                Tags = tags != null ? string.Join(",", tags) : null,
            };

            var client = this.GetClient();
            try
            {
                client.Send(message);
                emailsSent+=message.To.Count;
                dataLengthSent += GetSentLength(message);

                for (int i = 0; i < message.To.Count; i++)
                {
                    result[i] = new EmailSendResult
                    {
                        RecipientName = message.To[i].DisplayName,
                        RecipientAddress = message.To[i].Address,
                        IsSent = true,
                    };
                }

                email.SendSucceed = true;
                this.Log(email);
                return result;
            }
            catch
            {
                email.SendSucceed = false;
                email.SendErrors = 1;

                var additionalError = "LastSendSession sent " + emailsSent + " emails, " + dataLengthSent + " chars transmitted";
                email.LastSendError = additionalError;
                
                this.Log(email);

                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (client != null)
                {
                    this.client.Dispose();
                    this.client = null;
                }
            }

            base.Dispose(disposing);
        }

        private SmtpClient GetClient()
        {
            if (this.client == null)
            {
                SmtpClient smtpClient;
                if (this.Port > 0)
                    smtpClient = new SmtpClient(this.Host, this.Port);
                else
                    smtpClient = new SmtpClient(this.Host);

                smtpClient.EnableSsl = this.EnableSsl;
                if (this.Username != null)
                {
                    smtpClient.Credentials = new NetworkCredential(this.Username, this.Password);
                }

                this.emailsSent = 0;
                this.dataLengthSent = 0;
                this.client = smtpClient;
            }

            return this.client;
        }
    }
}
