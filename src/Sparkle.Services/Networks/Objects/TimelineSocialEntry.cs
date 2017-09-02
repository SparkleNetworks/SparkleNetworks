
namespace Sparkle.Services.Networks.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TimelineSocialEntry
    {
        /// <summary>
        /// Gets or sets the source of this item (Twitter, Facebook...)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of this item from the source network.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the link to the web page showing the entry.
        /// </summary>
        public string DisplayUrl { get; set; }

        public IList<TimelineSocialPictureEntry> Pictures { get; set; }

        /// <summary>
        /// Gets or sets the user id (typically an integer).
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
    }
}
