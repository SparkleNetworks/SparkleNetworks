
namespace Sparkle.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.WebPages;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using MarkdownSharp;

    public static class HtmlExtention
    {
        private static readonly MarkdownSharp.MarkdownOptions markdownOptions = new MarkdownSharp.MarkdownOptions
        {
            IncreaseTitleLevel = 1,
            AutoHyperlink = true,
            HyperlinkTarget = "_blank",
            HyperlinkClass = "external accentColor",
            AutoNewlines = true,
        };

        /// <summary>
        /// Formats a user string as HTML;
        /// Replaces newlines with line breaks.
        /// Replaces URLs with links.
        /// Escapes HTML chars.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="content">the content to transform</param>
        /// <returns>an escaped HTML string</returns>
        public static MvcHtmlString Text(this HtmlHelper html, string content, bool makeLinks = true, bool makeParagraphs = true, bool makeLineBreaks = true, bool twitterLinks = false, string wrapClass = "markdown")
        {
            return SrkHtmlExtensions.DisplayText(html, content, makeLinks, makeParagraphs, makeLineBreaks, twitterLinks, "external accentColor", "_blank", wrapClass);

            ////if (content.IndexOf("@[") > 0)
            ////    content = content.AddHtmlMentions();

            //return MvcHtmlString.Create(content);
        }

        public static MvcHtmlString Markdown(this HtmlHelper html, string content, bool makeLinks = true, bool makeLineBreaks = true, bool twitterLinks = false)
        {
            var md = new Markdown(markdownOptions);
            if (!makeLineBreaks)
                md.AutoNewLines = false;

            var htmlstring = md.Transform(content);
            return MvcHtmlString.Create("<div class=\"markdown\">" + htmlstring + "</div>");
        }

        [Obsolete("User DisplayDate instead")]
        public static string Date(this HtmlHelper html, DateTime date)
        {
            return date.ToLocalTime().ToString("D");
        }

        [Obsolete("User DisplayDate instead")]
        public static string Date(this HtmlHelper html, DateTime? date)
        {
            if (date.HasValue)
                return Date(html, date.Value);
            else
                return "jamais";
        }

        [Obsolete("User DisplayDate instead")]
        public static string Date(this HtmlHelper html, DateTimeOffset date)
        {
            return Date(html, date.LocalDateTime);
        }

        [Obsolete("User DisplayDate instead")]
        public static string SmallDate(this HtmlHelper html, DateTime date)
        {
            return date.ToLocalTime().ToString("D").Replace(", " + date.ToLocalTime().Year, string.Empty).Replace(date.ToLocalTime().Year.ToString(), string.Empty);
        }

        [Obsolete("User DisplayDate instead")]
        public static string SmallDate(this HtmlHelper html, DateTimeOffset date)
        {
            return SmallDate(html, date.LocalDateTime);
        }

        [Obsolete("User DisplayDateTime instead")]
        public static string DateTime(this HtmlHelper html, DateTime date)
        {
            return date.ToLocalTime().ToString("F");
        }

        [Obsolete("User DisplayTime instead")]
        public static string Time(this HtmlHelper html, DateTime date)
        {
            return date.ToLocalTime().ToString("T");
        }

        [Obsolete("User DisplayTime instead")]
        public static string Time(this HtmlHelper html, TimeSpan time)
        {
            return time.ToString("hh\\:mm");
        }

        /// <summary>
        /// Transforms "www.google.com" into "http://www.google.com".
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string AbsoluteHttpUrl(this HtmlHelper html, string url)
        {
            if (url == null)
                return null;

            url = url.Trim();

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "http://" + url;

            return url;
        }

        public static string DeviceLayoutName(this HtmlHelper html, string layoutType)
        {
            switch (layoutType)
            {
                case "SparkleTV.Layouts.StartupLayout":
                    return "Désactivé";

                case "SparkleTV.Layouts.NewsLayout":
                    return "Actualités du réseau";

                case "SparkleTV.Layouts.FreeRoomLayout":
                    return "Salle libre";

                case "SparkleTV.Layouts.BusyRoomLayout":
                    return "Salle occupée";

                default:
                    return "Inconnu";
            }
        }

        public static HelperResult Each<T>(this IEnumerable<T> items, Func<IndexedItem<T>, HelperResult> template)
        {
            return new HelperResult(writer =>
            {
                int index = 0;
                foreach (var item in items)
                {
                    var result = template(new IndexedItem<T>(index++, item));
                    result.WriteTo(writer);
                }
            });
        }

        public static string DisplayValue(this HtmlHelper html, RegisterRequestStatus value)
        {
            switch (value)
            {
                case RegisterRequestStatus.New:
                    return "nouvelle demande";

                case RegisterRequestStatus.ExternalCommunication:
                    return "communication en cours";

                case RegisterRequestStatus.Refused:
                    return "demande refusée";

                case RegisterRequestStatus.Accepted:
                    return "demande acceptée";

                default:
                    return value.ToString();
            }
        }

        public static string DisplayValue(this HtmlHelper html, EventMemberState value)
        {
            switch (value)
            {
                case EventMemberState.IsInvited:
                    return "invité";
                case EventMemberState.HasAccepted:
                    return "présent";
                case EventMemberState.MaybeJoin:
                    return "peut-être";
                case EventMemberState.WontCome:
                    return "absent";
                case EventMemberState.WantJoin:
                    return "demande";
                default:
                    return value.ToString();
            }
        }

        public static string FlatVersionOfFile(this HtmlHelper html, string relativePath)
        {
            if (html == null)
                throw new ArgumentNullException("html");

            var path = html.ViewContext.HttpContext.Server.MapPath(relativePath);
            return System.IO.File.GetLastWriteTimeUtc(path).ToString("yyyyMMddTHHmmssZ");
        }
    }

    public class IndexedItem<T>
    {
        public IndexedItem(int index, T item)
        {
            this.Index = index;
            this.Item = item;
        }

        public int Index { get; set; }

        public T Item { get; set; }
    }
}
