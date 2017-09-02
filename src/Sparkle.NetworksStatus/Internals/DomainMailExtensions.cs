
namespace Sparkle.NetworksStatus.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;

    public static class DomainMailExtensions
    {
        public static void AddRange(this MailAddressCollection collection, IEnumerable<SrkToolkit.Common.Validation.EmailAddress> emailAddresses)
        {
            foreach (var item in emailAddresses)
            {
                collection.Add(item.Value);
            }
        }

        public static void AddRange(this MailAddressCollection collection, IEnumerable<MailAddress> emailAddresses)
        {
            foreach (var item in emailAddresses)
            {
                collection.Add(item);
            }
        }
    }
}
