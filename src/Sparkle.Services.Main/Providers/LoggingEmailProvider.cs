
namespace Sparkle.Services.Main.Providers
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    /// <summary>
    /// Intermediate implementation class of <see cref="IEmailProvider"/> that provides database LOGGING support.
    /// </summary>
    public class LoggingEmailProvider : IEmailProvider, IDisposable
    {
        private bool isDisposed;
        private Action<EmailMessage> logMethod;

        protected LoggingEmailProvider()
        {
            // this constructors prevents instantiation without inheriting
        }

        public virtual void Initialize(IServiceFactory services)
        {
            this.logMethod = new Action<EmailMessage>(message =>
            {
                services.EmailMessages.Insert(message);
            });
        }

        public virtual void Configure(string configuration)
        {
        }

        public virtual EmailSendResult[] SimpleSend(MailMessage message, string[] tags)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(this.ToString());

            throw new NotImplementedException();
        }

        protected void Log(EmailMessage email)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(this.ToString());

            if (logMethod != null)
            {
                this.logMethod(email);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.logMethod = null;
                }

                this.isDisposed = true;
            }
        }
    }
}
