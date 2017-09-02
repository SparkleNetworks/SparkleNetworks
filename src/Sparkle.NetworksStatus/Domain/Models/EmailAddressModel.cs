
namespace Sparkle.NetworksStatus.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EmailAddressModel
    {
        public EmailAddressModel()
        {
        }

        public EmailAddressModel(int id)
        {
            this.Id = id;
        }

        public EmailAddressModel(Data.EmailAddress item)
        {
            this.Id = item.Id;
            this.DateConfirmedUtc = item.DateConfirmedUtc;
            this.DateCreatedUtc = item.DateCreatedUtc;
            this.AccountPart = item.AccountPart;
            this.TagPart = item.TagPart;
            this.DomainPart = item.DomainPart;
            this.IsClosed = item.IsClosed;
            this.EmailAddress = new SrkToolkit.Common.Validation.EmailAddress(item.AccountPart, item.TagPart, item.DomainPart);
        }

        public int Id { get; set; }

        public DateTime? DateConfirmedUtc { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public string AccountPart { get; set; }

        public string TagPart { get; set; }

        public string DomainPart { get; set; }

        public bool IsClosed { get; set; }

        public SrkToolkit.Common.Validation.EmailAddress EmailAddress { get; set; }
    }
}
