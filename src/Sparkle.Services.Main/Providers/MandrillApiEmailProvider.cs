
namespace Sparkle.Services.Main.Providers
{
    using Mandrill;
    using Mandrill.Utilities;
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of <see cref="IEmailProvider"/> that uses the http://mandrill.com/ API.
    /// </summary>
    public class MandrillApiEmailProvider : IEmailProvider
    {
        private int emailsSent;
        private long dataLengthSent;
        private Action<EmailMessage> logMethod;
        private bool isDisposed;
        private Mandrill.Models.UserInfo info;
        private MandrillApi api;

        private string Host { get; set; }

        private int Port { get; set; }

        private bool EnableSsl { get; set; }

        private string Username { get; set; }

        private string Password { get; set; }

        public void Initialize(IServiceFactory services)
        {
            this.logMethod = new Action<EmailMessage>(message =>
            {
                services.EmailMessages.Insert(message);
            });
        }

        public void Configure(string configuration)
        {
            dynamic config = JsonConvert.DeserializeObject(configuration);

            // the configuration entry in sparkle systems should look like:
            // Sparkle.Services.Main.Providers.MandrillApiEmailProvider, {Username:"account username",Password:"api key"}

            this.Host = config.Host;
            if (config.Port != null)
                this.Port = config.Port;
            if (config.Username != null)
                this.Username = config.Username;
            if (config.EnableSsl != null)
                this.EnableSsl = config.EnableSsl;
            if (config.Password != null)
                this.Password = config.Password;
        }

        public EmailSendResult[] SimpleSend(MailMessage message, string[] tags)
        {
            message.Headers.Add("X-MC-Track", "opens");
            message.Headers.Add("X-MC-Autotext", "yes");
            message.Headers.Add("X-MC-Tags", string.Join(",", tags));
            var result = this.SimpleSendImpl(message, tags);
            return result;
        }

        private EmailSendResult[] SimpleSendImpl(MailMessage message, string[] tags)
        {

            var results = new EmailSendResult[message.To.Count];
            var client = this.GetClient();
            var mmessage = Convert(message, tags);
            try
            {
                var response = Task.Run(() => client.SendMessage(new Mandrill.Requests.Messages.SendMessageRequest(mmessage))).Result;

                if (response != null && response.Count > 0)
                {
                    for (int i = 0; i < message.To.Count; i++)
                    {
                        var result = new EmailSendResult
                        {
                            RecipientName = message.To[i].DisplayName,
                            RecipientAddress = message.To[i].Address,
                        };
                        results[i] = result;

                        var log = new EmailMessage
                        {
                            DateSentUtc = DateTime.UtcNow,
                            EmailRecipient = string.Join(" ; ", message.To.Select(r => r.ToString())),
                            EmailSender = message.From.ToString(),
                            EmailSubject = message.Subject,
                            ProviderName = this.GetType().Name,
                            Tags = tags != null ? string.Join(",", tags) : null,
                        };

                        var item = response.FirstOrDefault(x => x.Email.Equals(result.RecipientAddress, StringComparison.OrdinalIgnoreCase));
                        if (item != null)
                        {
                            result.ErrorDetail = item.RejectReason != null ? ("Reject reason: " + item.RejectReason) : null;

                            if (item.Status == Mandrill.Models.EmailResultStatus.Sent)
                            {
                                log.SendSucceed = true;
                                log.ProviderDeliveryConfirmation = true;
                                result.IsSent = true;
                                result.IsDelivered = true;
                            }
                            else if (item.Status == Mandrill.Models.EmailResultStatus.Rejected)
                            {
                                log.ProviderDeliveryConfirmation = false;
                                log.SendErrors++;
                                result.IsSent = false;
                            }
                            else if (item.Status == Mandrill.Models.EmailResultStatus.Invalid)
                            {
                                log.ProviderDeliveryConfirmation = false;
                                log.SendErrors++;
                                result.IsSent = false;
                            }
                            else if (item.Status == Mandrill.Models.EmailResultStatus.Queued)
                            {
                                log.SendSucceed = true;
                                log.ProviderDeliveryConfirmation = false;
                                result.IsSent = true;
                                result.IsPendingDelivery = true;
                            }
                            else if (item.Status == Mandrill.Models.EmailResultStatus.Scheduled)
                            {
                                log.SendSucceed = true;
                                log.ProviderDeliveryConfirmation = false;
                                result.IsSent = true;
                                result.IsPendingDelivery = true;
                            }
                            else
                            {
                                // unknown mandrill status. update Mandrill library?
                            }
                        }
                        else
                        {
                            // the response does not contain an entry for the recipient
                        }

                        this.Log(log);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Should have a API result with 1 item");
                }

                return results;
            }
            catch (AggregateException ax)
            {
                var ex = ax.InnerException as MandrillException;
                if (ex != null)
                {
                    // TODO: MandrillApiEmailProvider: create 1 log per recipient
                    var log = new EmailMessage
                    {
                        DateSentUtc = DateTime.UtcNow,
                        EmailRecipient = string.Join(" ; ", message.To.Select(r => r.ToString())),
                        EmailSender = message.From.ToString(),
                        EmailSubject = message.Subject,
                        ProviderName = this.GetType().Name,
                        Tags = tags != null ? string.Join(",", tags) : null,
                    };
                    log.SendSucceed = false;
                    log.SendErrors = 1;

                    var additionalError = "LastSendSession sent " + emailsSent + " emails, " + dataLengthSent + " chars transmitted";
                    log.LastSendError = additionalError;

                    this.Log(log);

                    string exMessage = ex.Message;
                    if (ex.Error != null)
                    {
                        if (ex.Error.Message != null)
                        {
                            exMessage = ex.Error.Message;
                        }

                        exMessage += "\r\nCode: " + ex.Error.Code + " Name: " + ex.Error.Name + " Status: " + ex.Error.Status;
                    }

                    throw new SmtpException("Failed to send email via Mandrill API: " + exMessage, ex);
                }

                throw;
            }
            catch (MandrillException ex)
            {
                // TODO: MandrillApiEmailProvider: create 1 log per recipient
                var log = new EmailMessage
                {
                    DateSentUtc = DateTime.UtcNow,
                    EmailRecipient = string.Join(" ; ", message.To.Select(r => r.ToString())),
                    EmailSender = message.From.ToString(),
                    EmailSubject = message.Subject,
                    ProviderName = this.GetType().Name,
                    Tags = tags != null ? string.Join(",", tags) : null,
                };
                log.SendSucceed = false;
                log.SendErrors = 1;

                var additionalError = "LastSendSession sent " + emailsSent + " emails, " + dataLengthSent + " chars transmitted";
                log.LastSendError = additionalError;

                this.Log(log);

                string exMessage = ex.Message;
                if (ex.Error != null)
                {
                    if (ex.Error.Message != null)
                    {
                        exMessage = ex.Error.Message;
                    }

                    exMessage += "\r\nCode: " + ex.Error.Code + " Name: " + ex.Error.Name + " Status: " + ex.Error.Status;
                }

                throw new SmtpException("Failed to send email via Mandrill API: " + exMessage, ex);
            }
            catch
            {
                // TODO: MandrillApiEmailProvider: create 1 log per recipient
                var log = new EmailMessage
                {
                    DateSentUtc = DateTime.UtcNow,
                    EmailRecipient = string.Join(" ; ", message.To.Select(r => r.ToString())),
                    EmailSender = message.From.ToString(),
                    EmailSubject = message.Subject,
                    ProviderName = this.GetType().Name,
                    Tags = tags != null ? string.Join(",", tags) : null,
                };
                log.SendSucceed = false;
                log.SendErrors = 1;

                var additionalError = "LastSendSession sent " + emailsSent + " emails, " + dataLengthSent + " chars transmitted";
                log.LastSendError = additionalError;

                this.Log(log);

                throw;
            }
        }

        private void Log(EmailMessage email)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(this.ToString());

            if (this.logMethod != null)
            {
                this.logMethod(email);
            }
        }

        private static Mandrill.Models.EmailMessage Convert(MailMessage message, string[] tags)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var item = new Mandrill.Models.EmailMessage
            {
                FromEmail = message.From.Address,
                FromName = message.From.DisplayName,
                Html = message.Body,
                Subject = message.Subject,
                Tags = tags != null ? tags.ToArray() : null,
                To = message.To.Select(x => Convert(x)).ToArray(),
                TrackOpens = true,
                TrackClicks = false,
                AutoText = true,
            };
            return item;
        }

        private static Mandrill.Models.EmailAddress Convert(MailAddress item)
        {
            return new Mandrill.Models.EmailAddress(item.Address, item.DisplayName);
        }

        private MandrillApi GetClient()
        {
            if (this.api == null)
            {
                this.api = new MandrillApi(this.Password);
            }

            return this.api;
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
                    if (this.api != null)
                    {
                        //this.api.Dispose(); // it does not seem to be disposable on 2016-11-06 with Mandrill 2.4.1.0
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
