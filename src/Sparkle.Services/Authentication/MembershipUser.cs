
namespace Sparkle.Services.Authentication
{
    using System;

    /// <summary>
    /// Abstraction of <see cref="System.Web.Security.MembershipUser"/>.
    /// </summary>
    public class MembershipUser
    {
        private System.Web.Security.MembershipUser user;

        public MembershipUser()
        {
        }

        public MembershipUser(System.Web.Security.MembershipUser user)
            : this(user.ProviderName, user.UserName, user.ProviderUserKey, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate, user.LastLockoutDate)
        {
            this.user = user;
        }

        public MembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
        {
            this.ProviderName = providerName;
            this.UserName = name;
            this.ProviderUserKey = (Guid)providerUserKey;
            this.Email = email;
            this.PasswordQuestion = passwordQuestion;
            this.Comment = comment;
            this.IsApproved = isApproved;
            this.IsLockedOut = isLockedOut;
            this.CreationDate = creationDate;
            this.LastLoginDate = lastLoginDate;
            this.LastActivityDate = lastActivityDate;
            this.LastPasswordChangedDate = lastPasswordChangedDate;
            this.LastLockoutDate = lastLockoutDate;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MembershipUser))
                return false;

            return (Guid)this.ProviderUserKey == (Guid)(((MembershipUser)obj).ProviderUserKey);
        }

        public override int GetHashCode()
        {
            return ((Guid)this.ProviderUserKey).GetHashCode();
        }

        #region Properties

        // Summary:
        //     Gets or sets application-specific information for the membership user.
        //
        // Returns:
        //     Application-specific information for the membership user.
        public virtual string Comment { get; set; }
        //
        // Summary:
        //     Gets the date and time when the user was added to the membership data store.
        //
        // Returns:
        //     The date and time when the user was added to the membership data store.
        public virtual DateTime CreationDate { get; private set; }
        //
        // Summary:
        //     Gets or sets the e-mail address for the membership user.
        //
        // Returns:
        //     The e-mail address for the membership user.
        public virtual string Email { get; set; }
        //
        // Summary:
        //     Gets or sets whether the membership user can be authenticated.
        //
        // Returns:
        //     true if the user can be authenticated; otherwise, false.
        public virtual bool IsApproved { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the membership user is locked out and unable
        //     to be validated.
        //
        // Returns:
        //     true if the membership user is locked out and unable to be validated; otherwise,
        //     false.
        public virtual bool IsLockedOut { get; private set; }
        //
        // Summary:
        //     Gets whether the user is currently online.
        //
        // Returns:
        //     true if the user is online; otherwise, false.
        public virtual bool IsOnline { get; private set; }
        //
        // Summary:
        //     Gets or sets the date and time when the membership user was last authenticated
        //     or accessed the application.
        //
        // Returns:
        //     The date and time when the membership user was last authenticated or accessed
        //     the application.
        public virtual DateTime LastActivityDate { get; set; }
        //
        // Summary:
        //     Gets the most recent date and time that the membership user was locked out.
        //
        // Returns:
        //     A System.DateTime object that represents the most recent date and time that
        //     the membership user was locked out.
        public virtual DateTime LastLockoutDate { get; private set; }
        //
        // Summary:
        //     Gets or sets the date and time when the user was last authenticated.
        //
        // Returns:
        //     The date and time when the user was last authenticated.
        public virtual DateTime LastLoginDate { get; set; }
        //
        // Summary:
        //     Gets the date and time when the membership user's password was last updated.
        //
        // Returns:
        //     The date and time when the membership user's password was last updated.
        public virtual DateTime LastPasswordChangedDate { get; private set; }
        //
        // Summary:
        //     Gets the password question for the membership user.
        //
        // Returns:
        //     The password question for the membership user.
        public virtual string PasswordQuestion { get; private set; }
        //
        // Summary:
        //     Gets the name of the membership provider that stores and retrieves user information
        //     for the membership user.
        //
        // Returns:
        //     The name of the membership provider that stores and retrieves user information
        //     for the membership user.
        public virtual string ProviderName { get; private set; }
        //
        // Summary:
        //     Gets the user identifier from the membership data source for the user.
        //
        // Returns:
        //     The user identifier from the membership data source for the user.
        public virtual Guid ProviderUserKey { get; set; }
        //
        // Summary:
        //     Gets the logon name of the membership user.
        //
        // Returns:
        //     The logon name of the membership user.
        public virtual string UserName { get; set; }

        #endregion
    }
}
