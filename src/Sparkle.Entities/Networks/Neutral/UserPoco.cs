
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class UserPoco
    {
        /// <summary>
        /// Returns a concatenated string of the firstname and lastname.
        /// </summary>
        public string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
        }

        public CompanyAccessLevel CompanyAccess
        {
            get { return (CompanyAccessLevel)this.CompanyAccessLevel; }
            set { this.CompanyAccessLevel = (int)value; }
        }

        public NetworkAccessLevel NetworkAccess
        {
            get { return (NetworkAccessLevel)this.NetworkAccessLevel; }
            set { this.NetworkAccessLevel = (int)value; }
        }

        public NetworkUserGender GenderValue
        {
            get { return this.Gender != null ? (NetworkUserGender)this.Gender : NetworkUserGender.Unspecified; }
            set { this.Gender = (int)value; }
        }

        public override string ToString()
        {
            return this.Id + " " + this.Login;
        }
    }
}
