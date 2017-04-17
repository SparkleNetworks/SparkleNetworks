
namespace Sparkle.WebBase
{
    using Sparkle.UI;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// High level service container for views.
    /// </summary>
    public class NetworkViewServices
    {
        /// <summary>
        /// Gets the session service (may be null).
        /// </summary>
        public NetworkSessionService Session { get; set; }

        /// <summary>
        /// Get the user's prefered language (to format dates and stuff).
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Get the user's prefered language (for loading localized resources).
        /// </summary>
        public CultureInfo UICulture { get; set; }

        /// <summary>
        /// The application configuration tree.
        /// </summary>
        public Infrastructure.ConfigTree AppConfigTree { get; set; }

        /// <summary>
        /// The current Network.
        /// </summary>
        public Services.Networks.Models.NetworkModel CurrentNetwork { get; set; }

        public bool DebugBuild { get; set; }

        /// <summary>
        /// The prefered user's time zone.
        /// </summary>
        public TimeZoneInfo Timezone { get; set; }
    }
}