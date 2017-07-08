
namespace Sparkle.EmailTemplates.Internals
{
    using RazorEngine.Templating;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// Base class for email templates. This allows us to store interesting data for fast access.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomTemplateBase<T> : TemplateBase<T>
    {
        public HtmlHelper<T> Html { get; set; }

        public CultureInfo Culture { get; set; }

        public TimeZoneInfo Timezone { get; set; }

        public string _Layout { get; set; }
    }
}
