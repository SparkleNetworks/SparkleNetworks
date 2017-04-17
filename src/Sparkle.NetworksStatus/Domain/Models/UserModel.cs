
namespace Sparkle.NetworksStatus.Domain.Models
{
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserModel
    {
        public UserModel()
        {
        }

        public UserModel(Data.User item, Data.EmailAddress emailAddress)
        {
            this.Country = item.Country;
            this.Culture = item.Culture;
            this.DateCreatedUtc = item.DateCreatedUtc;
            this.DisplayName = item.DisplayName;
            this.Firstname = item.Firstname;
            this.Id = item.Id;
            this.Lastname = item.Lastname;
            this.PrimaryEmailAddressId = item.PrimaryEmailAddressId;
            this.Status = item.Status;
            this.Timezone = item.Timezone;

            if (emailAddress != null)
            {
                this.PrimaryEmailAddress = new EmailAddress(emailAddress.AccountPart, emailAddress.TagPart, emailAddress.DomainPart);
                this.EmailAddressString = this.PrimaryEmailAddress.Value;
            }
        }

        public int Id { get; set; }

        public int PrimaryEmailAddressId { get; set; }

        public string Culture { get; set; }

        public string Timezone { get; set; }

        public short Status { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public string Country { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string DisplayName { get; set; }

        public string EmailAddressString { get; set; }

        public EmailAddress PrimaryEmailAddress { get; set; }

        public IList<EmailAddressAuthenticationModel> EmailAddressAuthentications { get; set; }
    }
}
