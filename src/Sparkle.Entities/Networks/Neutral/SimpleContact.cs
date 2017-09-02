
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SimpleContact
    {
        public SimpleContact()
        {
        }

        public SimpleContact(Person user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.Firstname = user.FirstName;
            this.Lastname = user.LastName;
            this.EmailAddress = user.Email;
            this.Username = user.Username;
        }

        public string EmailAddress { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Civility { get; set; }
        public string Username { get; set; }

        public string FullName
        {
            get
            {
                if (this.Firstname != null && this.Lastname != null)
                    return this.Firstname + " " + this.Lastname;
                else if (this.Firstname != null)
                    return this.Firstname;
                else if (this.Lastname != null)
                    return this.Lastname;
                else
                    return null;
            }
        }
    }
}
