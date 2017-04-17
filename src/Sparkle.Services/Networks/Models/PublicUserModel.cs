
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PublicUserModel
    {
        public PublicUserModel(Entities.Networks.EventPublicMember item)
        {
            this.Company = item.Company;
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateUpdatedUtc = item.DateUpdatedUtc.AsUtc();
            this.Email = item.Email;
            this.FirstName = item.FirstName;
            this.Job = item.Job;
            this.LastName = item.LastName;
            this.Phone = item.Phone;
        }

        public string Company { get; set; }

        public DateTime? DateCreatedUtc { get; set; }

        public DateTime? DateUpdatedUtc { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Job { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }
    }
}
