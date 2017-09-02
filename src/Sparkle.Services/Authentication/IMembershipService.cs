
namespace Sparkle.Services.Authentication
{
    using System;
    using System.Web.Security;

    /// <summary>
    /// The FormsAuthentication type is sealed and contains static members, so it is difficult to
    /// unit test code that calls its members. The interface and helper class below demonstrate
    /// how to create an abstract wrapper around such a type in order to make the AccountController
    /// code unit testable.
    /// </summary>
    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        ValidateUserStatus ValidateUser(string username, string password);
        MembershipCreateStatus CreateUser(string username, string password, string email);
        bool ChangePassword(string username, string oldPassword, string newPassword);
        bool ChangePassword(string username, string newPassword);
        Sparkle.Services.Authentication.MembershipUser GetUser(string username);
        Sparkle.Services.Authentication.MembershipUser GetUserByEmail(string email);
        Sparkle.Services.Authentication.MembershipUser GetUser(Guid userId);

        void Unlock(string username);

        void DeleteUser(string username);

        void UpdateUser(Guid userId, string newEmail);
    }
}
