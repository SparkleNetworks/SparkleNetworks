
namespace Sparkle.WebStatus.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security;
    using System.Text;

    public class SmtpEmailProvider : IDisposable, IEmailProvider
    {
        private string Host { get; set; }
        private int Port { get; set; }
        private bool EnableSsl { get; set; }
        private string Username { get; set; }
        private SecureString Password { get; set; }

        private SmtpClient client;

        public void Initialize(ServiceFactory services)
        {
            dynamic config = JsonConvert.DeserializeObject(services.Configuration.SmtpConfiguration);

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

        public void SimpleSend(MailMessage message, string[] tags)
        {
            this.SimpleSendImpl(message, tags);
        }

        private void SimpleSendImpl(MailMessage message, string[] tags)
        {
            var client = this.GetClient();
            client.Send(message);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (client != null)
                {
                    this.client.Dispose();
                    this.client = null;
                }
            }
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

                this.client = smtpClient;
            }

            return this.client;
        }
    }
}
