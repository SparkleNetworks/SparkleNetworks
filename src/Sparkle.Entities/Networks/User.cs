
namespace Sparkle.Entities.Networks
{
    using System;

    public partial class User : IPerson, IEntityInt32Id, INetworkEntity
    {
        public User()
        {
            this.IsFriendWithCurrentId = null;
        }

        public User(string email, string username, Guid membershipId, string firstName, string lastName, int companyId)
        {
            this.Email = email;
            this.Login = username;
            this.UserId = membershipId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.CompanyID = companyId;
        }

        public bool IsDisplayWithCurrentId { get; set; }
        public bool? IsFriendWithCurrentId { get; set; }

        /// <summary>
        /// Returns a concatenated string of the firstname and lastname.
        /// </summary>
        public string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
        }

        /// <summary>
        /// Gets or sets the username.
        /// This is the same property as <see cref="Login"/>.
        /// </summary>
        public string Username
        {
            get { return this.Login; }
            set { this.Login = value; }
        }

        [Obsolete("This field should not be used anymore. Use CompanyAccess and NetworkAccess instead.")]
        public AccountRightLevel AccountAccess
        {
            get { return (AccountRightLevel)this.AccountRight; }
            set { this.AccountRight = (byte)value; }
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

    public enum NetworkUserGender : int
    {
        Unspecified = -1,
        Male = 0,
        Female = 1,
    }
}
