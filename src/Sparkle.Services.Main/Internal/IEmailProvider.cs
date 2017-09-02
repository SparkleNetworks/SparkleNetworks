
namespace Sparkle.Services.Main.Internal
{
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    /// <summary>
    /// Abstraction of the email provider. 
    /// </summary>
    public interface IEmailProvider : IDisposable
    {
        /// <summary>
        /// Allows the implementer to prepare if necessary.
        /// </summary>
        /// <param name="services"></param>
        void Initialize(IServiceFactory services);

        /// <summary>
        /// Sets the provider configuration from a string in the app configuration.
        /// </summary>
        /// <param name="configuration"></param>
        void Configure(string configuration);

        // TODO: get rid of System.Net.Mail: make our own classes to abstract properly.

        /// <summary>
        /// Sends an email message. 
        /// Current implementations only support ONE recipient! This may change in the future.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        EmailSendResult[] SimpleSend(MailMessage message, string[] tags);
    }
}
