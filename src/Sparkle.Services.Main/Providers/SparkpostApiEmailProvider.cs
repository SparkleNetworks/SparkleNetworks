
namespace Sparkle.Services.Main.Providers
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using SparkPost;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using HttpStatusCode = System.Net.HttpStatusCode;
    using SparkPostClient = SparkPost.Client;

    /// <summary>
    /// Implementation of <see cref="IEmailProvider"/> that uses the https://www.sparkpost.com/ API.
    /// Depends on nuget package=SparkPost version=1.9.2.
    /// </summary>
    /// <remarks>
    /// ERROR REPORT 24719: Message generation rejected, recipient address was suppressed due to system policy, code=1902
    /// https://support.sparkpost.com/customer/portal/articles/2038351-554-5-7-1---recipient-address-was-suppressed-due-to-system-policy?b_id=7411
    /// The "system policy' that this error is referring to is actually our suppression list. SparkPost has its own suppression list that encompasses many alias email addresses that are often sent to by spammer and malware senders. An example of this would be unsubscribe@, and in some cases, individual email addresses that are now defunct but people recognize, such as sjobs@apple.com. The global suppression list also has a large list of commonly misspelled domains that arise from major domains, e.g. "gmial.com".
    /// ERROR REPORT 24798: Message generation rejected, recipient address suppressed due to customer policy, code=1902.
    /// https://support.sparkpost.com/customer/portal/articles/1929891-using-suppression-lists
    /// </remarks>
    public class SparkpostApiEmailProvider : IEmailProvider
    {
        private Action<EmailMessage> logMethod;
        private bool isDisposed;
        private SparkPostClient api;
        private Configuration config;

        public Configuration Config
        {
            get { return this.config ?? (this.config = new Configuration()); }
            set { this.config = value; }
        }

        public void Initialize(IServiceFactory services)
        {
            this.logMethod = new Action<EmailMessage>(message =>
            {
                services.EmailMessages.Insert(message);
            });
        }

        public void Configure(string configuration)
        {
            // the configuration entry in sparkle systems should look like:
            // Sparkle.Services.Main.Providers.SparkpostApiEmailProvider, {Password:"sparkpost API key here"}
            // you can pass as many options as the Configuration class (below) supports
            // Sparkle.Services.Main.Providers.SparkpostApiEmailProvider, {Password:"sparkpost API key here",ClickTracking:false,OpenTracking:true,Sandbox:false}
            this.Config = JsonConvert.DeserializeObject<Configuration>(configuration);
        }

        public EmailSendResult[] SimpleSend(MailMessage message, string[] tags)
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

            var results = new EmailSendResult[message.To.Count];
            var client = this.GetClient();
            try
            {
                /*
                 * Simulate error :)
                 *
                throw new SparkPost.ResponseException(new Response
                {
                    Content = JsonConvert.SerializeObject(new SparkpostErrorResponse
                    {
                        Errors = new List<SparkpostErrorResponseError>()
                        {
                            new SparkpostErrorResponseError
                            {
                                Code = "1902",
                                Message = "Message generation rejected",
                                Description = "recipient address suppressed due to customer policy",
                            },
                        },
                    }),

                });
                */
                var transmission = ToSparkpostMessage(message);
                transmission.Options.Transactional = true;
                transmission.Options.ClickTracking = this.Config.ClickTracking;
                transmission.Options.OpenTracking = this.Config.OpenTracking;
                transmission.Options.Sandbox = this.Config.Sandbox;

                // this client does not return recipient-specific errors :'(
                var result = Task.Run(() => client.Transmissions.Send(transmission)).Result;
                if (result != null)
                {
                    var item = result;
                    log.ProviderId = item.Id;

                    if (item.StatusCode == HttpStatusCode.Created || item.StatusCode == HttpStatusCode.OK)
                    {
                        log.SendSucceed = true;
                        log.ProviderDeliveryConfirmation = true;

                        for (int i = 0; i < message.To.Count; i++)
                        {
                            var recipient = message.To[i];
                            results[i] = new EmailSendResult
                            {
                                IsSent = true,
                                RecipientAddress = recipient.Address,
                                RecipientName = recipient.DisplayName,
                            };
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected API response: " + item.StatusCode);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Should have a API result with 1 item");
                }

                this.Log(log);
                return results;
            }
            catch (AggregateException aggregateException)
            {
                // because of Task.Run().Result, we end up here
                var ex = aggregateException.InnerException ?? aggregateException;

                log.SendSucceed = false;
                log.SendErrors = 1;

                Exception throwMe;
                if (ex is SparkPost.ResponseException)
                {
                    throwMe = ThrowFromSparkpostException((SparkPost.ResponseException)ex);
                }
                else
                {
                    throwMe = ThrowFromOtherException(ex, "Failed to send email with Sparkpost (AggregateException): " + ex.Message);
                }

                log.SetLastSendErrorSafe(throwMe.Message);
                this.Log(log);
                throw throwMe;
            }
            catch (InvalidOperationException ex)
            {
                // a special case in the try block may throw this one
                log.SendSucceed = false;
                log.SendErrors = 1;

                Exception throwMe;
                if (ex.InnerException is SparkPost.ResponseException)
                {
                    throwMe = ThrowFromSparkpostException((SparkPost.ResponseException)ex.InnerException);
                }
                else
                {
                    throwMe = ThrowFromOtherException(ex, "Failed to send email with Sparkpost (InvalidOperationException): " + ex.Message);
                }

                log.SetLastSendErrorSafe(throwMe.Message);
                this.Log(log);
                throw throwMe;
            }
            catch (SparkPost.ResponseException ex)
            {
                // this would be the code with a synchronous call (we never get here)
                log.SendSucceed = false;
                log.SendErrors = 1;

                var throwMe = ThrowFromSparkpostException(ex);

                log.SetLastSendErrorSafe(throwMe.Message);
                this.Log(log);
                throw throwMe;
            }
            catch (Exception ex)
            {
                // fallback code
                log.SendSucceed = false;
                log.SendErrors = 1;

                var throwMe = ThrowFromOtherException(ex, "Failed to send email with Sparkpost (Exception): " + ex.Message);
                log.SetLastSendErrorSafe(throwMe.Message);
                this.Log(log);
                throw throwMe;
            }
        }

        private static SparkleServicesException ThrowFromSparkpostException(SparkPost.ResponseException ex1)
        {
            string errorMessage = ex1.Message, displayMessage = null;

            if (ex1.Response != null && ex1.Response.Content != null)
            {
                try
                {
                    var errorObject = JsonConvert.DeserializeObject<SparkpostErrorResponse>(ex1.Response.Content);
                    if (errorObject != null && errorObject.Errors != null && errorObject.Errors.Count > 0)
                    {
                        var errors = errorObject.Errors;
                        errorMessage = "Sparkpost errors response (" + errors.Count + "). "
                            + string.Join(", ", errors.Select(x => x != null ? (x.Code + " " + x.Message + " (" + x.Description + ")") : "null"));
                        if (errors.Count == 1 && errors[0] != null)
                        {
                            var error = errors[0];
                            if ("1902".Equals(error.Code))
                            {
                                // { "errors": [ { "message": "Message generation rejected", "description": "recipient address suppressed due to customer policy", "code": "1902" } ], "results": { "total_rejected_recipients": 0, "total_accepted_recipients": 1, "id": "663376326886048846" } }
                                // { "errors": [ { "message": "Message generation rejected", "description": "recipient address was suppressed due to system policy", "code": "1902" } ], "results": { "total_rejected_recipients": 0, "total_accepted_recipients": 1, "id": "84334517670813300" } }
                                displayMessage = NetworksLabels.EmailProviderUnauthorizedAddressDisplayError;
                            }
                            else
                            {
                                displayMessage = string.Format(NetworksLabels.EmailProviderDefaultDisplayError, error.Code + " " + error.Message + " (" + error.Description + ")");
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    // we failed to parse the content, there is nothing we can do :(
                }
            }

            displayMessage = displayMessage ?? NetworksLabels.EmailProviderDefaultDisplayError;

            var ex = new SparkleServicesException(errorMessage, displayMessage, ex1);
            return ex;
        }

        private static SparkleServicesException ThrowFromOtherException(Exception ex1, string errorMessage)
        {
            string displayMessage = NetworksLabels.EmailProviderDefaultDisplayError;
            var ex = new SparkleServicesException(errorMessage, displayMessage, ex1);
            return ex;
        }

        private static Transmission ToSparkpostMessage(MailMessage message)
        {
            var item = new SparkPost.Transmission();

            if (message.IsBodyHtml)
            {
                item.Content.Html = message.Body;
            }
            else
            {
                item.Content.Text = message.Body;
            }

            item.Content.From = message.From != null ? ToSparkpostAddress(message.From) : null;

            item.Content.Subject = message.Subject;

            if (message.Headers != null && message.Headers.Count > 0)
            {
                foreach (string headerKey in message.Headers.AllKeys)
                {
                    item.Content.Headers.Add(headerKey, message.Headers[headerKey]);
                }
            }

            foreach (var to in message.To)
            {
                item.Recipients.Add(ToSparkpostRecipient(to));
            }

            return item;
        }

        public static Address ToSparkpostAddress(MailAddress address)
        {
            return new Address
            {
                Email = address.Address,
                Name = address.DisplayName,
            };
        }

        public static Recipient ToSparkpostRecipient(MailAddress address)
        {
            return new Recipient
            {
                Address = ToSparkpostAddress(address),
                Type = SparkPost.RecipientType.To,
            };
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

        private SparkPostClient GetClient()
        {
            if (this.api == null)
            {
                this.api = new SparkPostClient(this.Config.Password);
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
                    }

                    this.logMethod = null;
                }

                this.isDisposed = true;
            }
        }

        public class Configuration
        {
            public string Password { get; set; }
            public bool? ClickTracking { get; set; }
            public bool? OpenTracking { get; set; }
            public bool? Sandbox { get; set; }
        }

        [DataContract]
        public class SparkpostErrorResponse
        {
            [DataMember(Name = "errors")]
            public IList<SparkpostErrorResponseError> Errors { get; set; }
        }

        [DataContract]
        public class SparkpostErrorResponseError
        {
            [DataMember(Name = "message")]
            public string Message { get; set; }

            [DataMember(Name = "description")]
            public string Description { get; set; }

            [DataMember(Name = "code")]
            public string Code { get; set; }
        }
    }
}
