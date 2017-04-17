
namespace Sparkle.Services.Authentication
{
    public interface IFormsAuthenticationService
    {
        /// <summary>
        /// Creates an authentication ticket for the supplied user name and adds it to
        /// the cookies collection of the response.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        void SignIn(string userName, bool createPersistentCookie);

        /// <summary>
        /// Removes the forms-authentication ticket from the browser.
        /// </summary>
        void SignOut();
    }
}
