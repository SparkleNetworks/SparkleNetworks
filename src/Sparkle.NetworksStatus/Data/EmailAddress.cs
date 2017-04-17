
namespace Sparkle.NetworksStatus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class EmailAddress
    {
        public EmailAddress()
        {
        }

        public EmailAddress(SrkToolkit.Common.Validation.EmailAddress emailAddress)
        {
            this.AccountPart = emailAddress.AccountPart;
            this.TagPart = emailAddress.TagPart;
            this.DomainPart = emailAddress.DomainPart;
        }

        internal static EmailAddress Create(string emailAddress)
        {
            var email = new SrkToolkit.Common.Validation.EmailAddress(emailAddress);
            return new EmailAddress(email);
        }
    }
}
