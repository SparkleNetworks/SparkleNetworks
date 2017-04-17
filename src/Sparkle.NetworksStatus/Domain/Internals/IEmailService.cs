
namespace Sparkle.NetworksStatus.Domain.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    public interface IEmailService
    {
        SendEmailResult SendMessage(MailMessage message);
    }
}
