
namespace Sparkle.Services.Authentication
{
    using System;
    using System.Web.Security;

    /// <summary>
    /// Membership implementation of <see cref="IMembershipService"/>.
    /// </summary>
    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        public AccountMembershipService()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public AccountMembershipService(MembershipProvider provider)
        {
            this.provider = provider ?? Membership.Provider;
        }

        /// <summary>
        /// Gets the length of the min password.
        /// </summary>
        /// <value>
        /// The length of the min password.
        /// </value>
        public int MinPasswordLength
        {
            get
            {
                return this.provider.MinRequiredPasswordLength;
            }
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public ValidateUserStatus ValidateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", "password");

            return this.provider.ValidateUser(userName, password) ? ValidateUserStatus.Ok : ValidateUserStatus.UnknownReason;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email))
                throw new ArgumentException("Value cannot be null or empty.", "email");

            MembershipCreateStatus status;
            this.provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword))
                throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                var currentUser = this.provider.GetUser(userName, true /* userIsOnline */);
                if (currentUser != null)
                    return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        public bool ChangePassword(string userName, string newPassword)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                var currentUser = provider.GetUser(userName, false /* userIsOnline */);
                if (currentUser != null)
                {
                    string oldPw = currentUser.ResetPassword();
                    return currentUser.ChangePassword(oldPw, newPassword);
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public Sparkle.Services.Authentication.MembershipUser GetUser(string username)
        {
            var mbsUser = this.provider.GetUser(username, false);
            if (mbsUser == null)
                return null;

            return new Sparkle.Services.Authentication.MembershipUser(mbsUser);
        }

        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public Sparkle.Services.Authentication.MembershipUser GetUserByEmail(string email)
        {
            var username = this.provider.GetUserNameByEmail(email);
            if (username == null)
                return null;

            var mbsUser = this.provider.GetUser(username, false);
            if (mbsUser == null)
                return null;

            return new Sparkle.Services.Authentication.MembershipUser(mbsUser);
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public Sparkle.Services.Authentication.MembershipUser GetUser(Guid userId)
        {
            var mbsUser = this.provider.GetUser(userId, false);
            if (mbsUser == null)
                return null;
            
            return new Sparkle.Services.Authentication.MembershipUser(mbsUser);
        }

        public void Unlock(string username)
        {
            this.provider.UnlockUser(username);
        }

        public void DeleteUser(string username)
        {
            this.provider.DeleteUser(username, true);
        }

        public void UpdateUser(Guid userId, string newEmail)
        {
            var mbsUser = this.provider.GetUser(userId, false);
            mbsUser.Email = newEmail;
            this.provider.UpdateUser(mbsUser);
        }
    }
}
