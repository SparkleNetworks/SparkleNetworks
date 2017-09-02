
namespace Sparkle.Services.Authentication
{
    using System;
    using System.Web.Security;

    public sealed class FormsAuthenticationService : IFormsAuthenticationService
    {
        /// <summary>
        /// Creates an authentication ticket for the supplied user name and adds it to
        /// the cookies collection of the response.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <exception cref="System.ArgumentException">Value cannot be null or empty.;userName</exception>
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("Value cannot be null or empty.", "userName");

            // TODO: WARNING! STATIC CALL TO FormsAuthentication.SetAuthCookie();
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// Removes the forms-authentication ticket from the browser.
        /// </summary>
        public void SignOut()
        {
            // TODO: WARNING! STATIC CALL TO FormsAuthentication.SignOut();
            FormsAuthentication.SignOut();
        }
    }
}
