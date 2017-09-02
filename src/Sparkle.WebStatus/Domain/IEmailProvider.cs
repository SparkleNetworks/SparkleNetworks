
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    public interface IEmailProvider
    {
        void Initialize(ServiceFactory serviceFactory);

        void SimpleSend(MailMessage message, string[] tags);
    }
}
