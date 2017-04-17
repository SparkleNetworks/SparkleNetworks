
namespace Sparkle.Services.Networks.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TimelineSocialPictureEntry
    {
        /// <summary>
        /// Gets or sets the link to the web page showing the picture (shortened).
        /// </summary>
        public string ShortDisplayUrl { get; set; }

        /// <summary>
        /// Gets or sets the link to the web page showing the picture (not shortened).
        /// </summary>
        public string FullDisplayUrl { get; set; }

        /// <summary>
        /// Gets or sets the link to the media element.
        /// </summary>
        public string MediaUrl { get; set; }

        /// <summary>
        /// Gets or sets the link to the media element.
        /// </summary>
        public string MediaUrlHttps { get; set; }

        public IList<TimelineSocialPictureSizeEntry> Sizes { get; set; }
    }
}
