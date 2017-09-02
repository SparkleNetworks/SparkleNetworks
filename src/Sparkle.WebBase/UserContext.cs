
namespace Sparkle.WebBase
{
    using System;
    using System.Web;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure.Constants;

    public static class UserContext
    {
        /// <summary>
        /// OBSOLETE. Returns the authenticated user. OBSOLETE. Use SessionService.User. OBSOLETE OBSOLETE OBSOLETE OBSOLETE.
        /// </summary>
        [Obsolete]
        public static User Me
        {
            get
            {
                return UserContext.GetValue<User>(SessionConstants.Me);
            }

            set
            {
                UserContext.SetValue(SessionConstants.Me, value);
            }
        }

        /// <summary>
        /// OBSOLETE. Sets the value corresponding to a key.
        /// </summary>
        /// <typeparam name="T">The type to set.</typeparam>
        /// <param name="key">The key parameter.</param>
        /// <param name="value">The value to set.</param>
        private static void SetValue<T>(string key, T value) //// where T : class
        {
            HttpContext.Current.Session[key] = value;
        }

        /// <summary>
        /// OBSOLETE. Gets the value corresponding to a key.
        /// </summary>
        /// <typeparam name="T">The type to retrieve.</typeparam>
        /// <param name="key">The key parameter.</param>
        /// <returns>The value corresponding to the key.</returns>
        private static T GetValue<T>(string key)
        {
            if (HttpContext.Current.Session == null)
            {
                return default(T);
            }
            else if (HttpContext.Current.Session[key] != null)
            {
                return (T)Convert.ChangeType(HttpContext.Current.Session[key], typeof(T));
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Checks if a key exists.
        /// </summary>
        /// <param name="key">The key parameter.</param>
        /// <returns>True if the key exists. Else, False.</returns>
        public static bool Exist(string key)
        {
            return HttpContext.Current.Session[key] != null ? true : false;
        }

        /// <summary>
        /// OBSOLETE. 
        /// </summary>
        [Obsolete]
        public static void Clear()
        {
            UserContext.Me = null;
            HttpContext.Current.Session.Clear();
        }

        public enum UserStatus
        {
            Offline = 0,
            Online = 1,
            Away = 2,
            Busy = 3,
        }
    }
}
