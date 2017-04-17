
namespace Sparkle.NetworksStatus.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EmailAddressAuthenticationModel
    {
        public EmailAddressAuthenticationModel()
        {
        }

        ////public EmailAddressAuthenticationModel(Data.EmailAddressAuthentication item, Data.EmailAddress emailAddress)
        ////{
        ////    this.DateCreatedUtc = item.DateCreatedUtc;
        ////    this.EmailAddressId = item.EmailAddressId;
        ////    this.Id = item.Id;
        ////    this.IsDeleted = item.IsDeleted;
        ////    this.UserId = item.UserId;

        ////    if (emailAddress != null)
        ////    {
        ////        this.EmailAddress = new EmailAddressModel(emailAddress);
        ////        this.EmailAddressString = this.EmailAddress.EmailAddress.Value;
        ////    }
        ////    else
        ////    {
        ////        this.EmailAddress = new EmailAddressModel(item.EmailAddressId);
        ////    }
        ////}

        public int Id { get; set; }

        public int EmailAddressId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public int UserId { get; set; }

        public bool IsDeleted { get; set; }

        public EmailAddressModel EmailAddress { get; set; }

        public string EmailAddressString { get; set; }
    }
}
