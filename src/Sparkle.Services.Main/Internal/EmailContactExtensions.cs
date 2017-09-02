
namespace Sparkle.Services.Main.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using Sparkle.Entities.Networks;

    public static class EmailContactExtensions
    {
        public static MailAddress ToMailAddress(this EmailContact contact)
        {
            if (contact.DisplayName != null)
                return new MailAddress(contact.EmailAddress, contact.DisplayName);
            else
                return new MailAddress(contact.EmailAddress);
        }
    }
}
