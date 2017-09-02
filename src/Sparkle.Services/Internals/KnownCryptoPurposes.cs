
namespace Sparkle.Services.Internals
{
    using System;

    public static class KnownCryptoPurposes
    {
        /// <summary>
        /// The events module allows users to sync events with a iCal URL. A token must be generated to protect each user's calendar.
        /// </summary>
        public const string PrivateCalendarUserToken = "PrivateCalendarUserToken";

        /// <summary>
        /// The email+password authentication mecanism needs an IV to generate tokens.
        /// </summary>
        public const string EmailPasswordAuthentication = "EmailPasswordAuthentication";

        /// <summary>
        /// The user can change notification settings without being authenticated. The access page needs a non-predictable hash for simplified authentication.
        /// </summary>
        public const string EmailNotificationUserPreferences = "EmailNotificationUserPreferences";

        /// <summary>
        /// A secret token is used to confirm the user's email address. 
        /// </summary>
        public const string ApplyRequestEmailToken = "ApplyRequestEmailToken";
    }
}
