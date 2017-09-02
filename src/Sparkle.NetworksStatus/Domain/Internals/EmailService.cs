
namespace Sparkle.NetworksStatus.Domain.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Security;

    public class EmailService : BaseService, IEmailService
    {
        private SmtpClient client;
        [System.Diagnostics.DebuggerStepThrough]
        internal EmailService(IServiceFactoryEx serviceFactory)
            : base(serviceFactory)
        {
        }

        public BasicEmail<TModel> PrepareBasicMarkdownEmail<TModel>(string template, string master)
        {
            var email = new BasicEmail<TModel>();
            email.MasterTempalte = master;
            email.Template = template;
            email.Replacer
                .Setup("Recipient.Address", m => m.Model.Recipient.Address)
                .Setup("Recipient.DisplayName", m => m.Model.Recipient.DisplayName)
                .Setup("Recipient.FullName", m => m.Model.Recipient.ToString())
                .Setup("Sender.Address", m => m.Model.Sender.Address)
                .Setup("Sender.DisplayName", m => m.Model.Sender.DisplayName)
                .Setup("Sender.FullName", m => m.Model.Sender.ToString())
                .Setup("Footer.AppName", m => this.Services.Configuration.AppName)
                .Setup("Footer.SiteUrl", m => this.Services.Configuration.AppUrl)
                .Setup("Footer.AppCompanyName", m => this.Services.Configuration.AppCompanyName)
                .Setup("Footer.AppCompanyLegalName", m => this.Services.Configuration.AppCompanyLegalName)
                .Setup("Footer.SupportEmail", m => this.Services.Configuration.SupportEmail)
                .Setup("Header.Subject", m => m.Model.Subject)
                .Setup("Header.SendDate", m => DateTime.UtcNow.ToString(m.Parameter ?? "d"))
                ;
            return email;
        }

        public SendEmailResult Send<TModel>(BasicEmail<TModel> email, BasicEmailModel<TModel> model)
        {
            if (email == null)
                throw new ArgumentNullException("email");
            if (model == null)
                throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(email.MasterTempalte))
                throw new ArgumentException("The value cannot be empty", "email.MasterTempalte");
            if (string.IsNullOrEmpty(email.Template))
                throw new ArgumentException("The value cannot be empty", "email.Template");

            var masterContent = email.Replacer.Replace(email.MasterTempalte, model);
            var content = email.Replacer.Replace(email.Template, model);
            content = masterContent.Replace("@CONTENT", content);

            var message = new MailMessage();
            message.Subject = model.Subject;
            message.Body = content;
            message.IsBodyHtml = false;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.Never;
            message.From = model.Sender;
            message.To.Add(model.Recipient);

            try
            {
                var client = this.GetSmtpClient();
                client.Send(message);

                return new SendEmailResult(model.Recipient);
            }
            catch (SmtpException ex)
            {
                return new SendEmailResult(model.Recipient, ex);
            }
            catch (InvalidOperationException ex)
            {
                return new SendEmailResult(model.Recipient, ex);
            }
            catch (NotImplementedException ex)
            {
                return new SendEmailResult(model.Recipient, ex);
            }
        }

        public SendEmailResult SendMessage(MailMessage message)
        {
            try
            {
                var client = this.GetSmtpClient();
                client.Send(message);

                return new SendEmailResult(message.To.First());
            }
            catch (SmtpException ex)
            {
                return new SendEmailResult(message.To.First(), ex);
            }
            catch (InvalidOperationException ex)
            {
                return new SendEmailResult(message.To.First(), ex);
            }
            catch (NotImplementedException ex)
            {
                return new SendEmailResult(message.To.First(), ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var client = this.client;
                this.client = null;
                if (client != null)
                {
                    client.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private SmtpClient GetSmtpClient()
        {
            if (this.client == null)
            {
                this.client = new SmtpClient(this.Services.Configuration.SmtpHost, this.Services.Configuration.SmtpPort);
                this.client.EnableSsl = this.Services.Configuration.SmtpSsl;
                if (this.Services.Configuration.SmtpUsername != null)
                {
                    this.client.Credentials = new NetworkCredential(this.Services.Configuration.SmtpUsername, this.Services.Configuration.SmtpPassword);
                }
            }

            return this.client;
        }
    }
}
