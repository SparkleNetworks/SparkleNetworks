
namespace Sparkle.Services.Networks.PartnerResources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ContactProfileFieldModel
    {
        public ContactProfileFieldModel(string firstName, string lastName, string job, string phone, string email)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Job = job;
            this.Phone = phone;
            this.Email = email;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Job { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
