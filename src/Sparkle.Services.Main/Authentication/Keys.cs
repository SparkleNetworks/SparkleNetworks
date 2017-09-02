
namespace Sparkle.Services.Authentication
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Security;

    public static class Keys
    {
        /// <summary>
        /// Generates a unique identifier from a user ID to authenticate calendar requests.
        /// WARNING: this method is subject to unit tests!
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The calendar authentication key.</returns>
        [Obsolete("This code is not safe. Replaced by EventsService.GetUserCalendarToken.")]
        public static string ComputeForCalendar(Guid userId)
        {
            // [OSP-Security] Keys.ComputeForCalendar: The result should not be publicly predictable. 
            using (var sha = SHA1.Create())
            using (var md = MD5.Create())
            {
                var data = userId.ToByteArray();
                for (int i = 0; i < 12; i++)
                {
                    data = sha.ComputeHash(data);
                    unchecked
                    {
                        data[0] = data[0]++;
                    }

                    data = md.ComputeHash(data);
                }

                return Convert.ToBase64String(data).Replace('/', 'a').Replace('=', 'b').Replace("+", "");
            }
        }

        /// <summary>
        /// Validates the specified calendar authentication key.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="computedKey">The calendar authentication key.</param>
        /// <returns></returns>
        [Obsolete("This code is not safe. Replaced by EventsService.ValidateUserCalendarToken.")]
        public static bool ValidateForCalendar(Guid userId, string computedKey)
        {
            // [OSP-Security] Keys.ComputeForCalendar: The result should not be publicly predictable. 
            return ComputeForCalendar(userId) == computedKey;
        }

        /// <summary>
        /// Generates a unique identifier from a user ID to authenticate account recovery requests.
        /// WARNING: this method is subject to unit tests!
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The account recovery authentication key.</returns>
        public static string ComputeForAccount(Guid userId, DateTime lastLoginDate)
        {
            // TODO: [OSP-Security] Keys.ComputeForAccount: The result should not be publicly predictable. 
            using (var sha = SHA1.Create())
            using (var md = MD5.Create())
            {
                var enc = Encoding.UTF8;
                var data = enc.GetBytes(userId + lastLoginDate.Ticks.ToString());
                for (byte i = 0; i < 12; i++)
                {
                    data = sha.ComputeHash(data);
                    unchecked
                    {
                        data[0] += i;
                        data[1] = data[1]++;
                    }
                    data = md.ComputeHash(data);
                }

                return Convert.ToBase64String(data).Replace('/', 'd').Replace('=', 'z').Replace("+", "x");
            }
        }

        /// <summary>
        /// Validates the specified account recovery authentication key.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="computedKey">The account recovery authentication key.</param>
        /// <returns></returns>
        public static bool ValidateForAccount(Guid userId, DateTime lastLoginDate, string computedKey)
        {
            return ComputeForAccount(userId, lastLoginDate) == computedKey;
        }
    }
}
