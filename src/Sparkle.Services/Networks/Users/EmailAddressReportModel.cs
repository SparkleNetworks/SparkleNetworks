
namespace Sparkle.Services.Networks.Users
{
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EmailAddressReportModel
    {
        public EmailAddressReportModel()
        {

        }

        public EmailAddressReportModel(EmailAddress email)
        {
            this.Address = email;
        }

        public EmailAddress Address { get; set; }

        public int MainAddressUsersOnNetwork { get; set; }

        public int MainAddressUsersOtherNetworks { get; set; }

        public int PersonalAddressUsers { get; set; }

        public int CompanyContacts { get; set; }
    }
}
