
namespace Sparkle.UI
{
    using MarkdownSharp;
    using Newtonsoft.Json;
    using Sparkle.Common;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.WebBase;
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.WebPages;

    public static class HtmlExtention
    {
        /// <summary>
        /// Formats a date as a delay from the current time.
        /// Ex: "3 hours ago", "last month", "tomorrow"
        /// </summary>
        /// <param name="html"></param>
        /// <param name="date"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static MvcHtmlString ToNiceDelay(this HtmlHelper html, DateTime date, string dateFormat = "f")
        {
            var tz = html.GetTimezone();
            var userTime = tz.ConvertFromUtc(DateTime.UtcNow);

            var niceDelay = date.Kind == DateTimeKind.Utc ? tz.ConvertFromUtc(date).ToNiceDelay(userTime)
                : date.Kind == DateTimeKind.Local ? tz.ConvertFromUtc(date.ToUniversalTime()).ToNiceDelay(userTime)
                : date.ToNiceDelay(userTime);

            return new MvcHtmlString(string.Format(
                @"<span title=""{0}"" class=""nicedelay"">{1}</span>",
                html.Encode(date.ToString(dateFormat ?? "f")),
                html.Encode(niceDelay)));
        }

        /// <summary>
        /// Returns a markdown file as HTML.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="filename"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static MvcHtmlString FromMarkdownFile(this HtmlHelper html, string filename, Dictionary<string, string> variables = null)
        {
            string filePath = filename.StartsWith("~") ? html.ViewContext.HttpContext.Server.MapPath(filename) : filename;
            string content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
            if (variables != null)
            {
                foreach (var currentVariable in variables)
                {
                    content = content.Replace("[" + currentVariable.Key + "]", currentVariable.Value);
                }
            }
            var md = new MarkdownSharp.Markdown(new MarkdownOptions()
            {
                IncreaseTitleLevel = 1,
            });
            var htmlString = md.Transform(content);
            return MvcHtmlString.Create(htmlString);
        }

        /// <summary>
        /// Returns a network-specific markdown file as HTML.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="filename"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static MvcHtmlString FromMarkdownNetworkFile(this HtmlHelper html, string filename)
        {
            var rootPath = html.ViewContext.HttpContext.Server.MapPath("~/Content/Networks");
            var networkName = html.Services().CurrentNetwork.Name;
            var networkDirPath = System.IO.Path.Combine(html.Services().AppConfigTree.Storage.UserContentsDirectory, "Networks", networkName);
            var commmonDirPath = html.ViewContext.HttpContext.Server.MapPath("~/Content/NetworkDefaults");
            var basePaths = new string[] { networkDirPath, commmonDirPath, };
            string finalPath = null;
            for (int i = 0; i < basePaths.Length; i++)
            {
                var path = System.IO.Path.Combine(basePaths[i], filename);
                if (File.Exists(path))
                {
                    finalPath = path;
                    break;
                }
            }

            if (finalPath == null)
            {
                return MvcHtmlString.Create("<p>File not found.</p>");
            }

            return FromMarkdownFile(html, finalPath);
        }

        public static ConfigTree GetConfigTree(this HtmlHelper html)
        {
            return (ConfigTree)html.ViewData["AppConfigTree"];
        }
        
        /// <summary>
        /// Display a language in a abbreviated way (as HTML abbr element).
        /// </summary>
        /// <param name="html"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayCulture(this HtmlHelper html, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");

            return MvcHtmlString.Create(
                "<abbr title=\"" + culture.NativeName.ProperHtmlAttributeEscape() 
                + " - " + culture.DisplayName.ProperHtmlAttributeEscape()
                + "\">"
                + culture.Name + "</abbr>");
        }

        /// <summary>
        /// Display a language in a abbreviated way (as HTML abbr element).
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayCulture(this HtmlHelper html, string cultureName)
        {
            if (string.IsNullOrEmpty(cultureName))
                return null;

            var culture = new CultureInfo(cultureName);
            return html.DisplayCulture(culture);
        }

        /// <summary>
        /// Renders the JS object that contains contextual data.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static MvcHtmlString RenderJsContext(this HtmlHelper html, NetworkViewServices services)
        {
            var session = new NetworkSessionService(html.ViewContext.HttpContext.Session);
            var user = session.User;

            if (services == null)
                throw new ArgumentNullException("services");

            var culture = services.Culture ?? CultureInfo.CurrentCulture;
            var uiCulture = services.UICulture ?? CultureInfo.CurrentUICulture;

            var jsDateFormat = GetJavascriptShortDatePattern(culture.DateTimeFormat.ShortDatePattern);
            var jsTimeFormat = GetJavascriptShortTimePattern(culture.DateTimeFormat.ShortTimePattern);

            var config = new
            {
                EnableCompanies = (services.AppConfigTree != null ? services.AppConfigTree.Features != null ? services.AppConfigTree.Features.EnableCompanies : false : false),
                DateFormat = jsDateFormat,
                TimeFormat = jsTimeFormat,
                ShortCultureName = culture.TwoLetterISOLanguageName,
            };
            var userJs = user != null ? new
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
            } : null;
            var obj = new
            {
                Culture = culture.Name,
                UICulture = uiCulture.Name,
                Area = html.ViewContext.RouteData.DataTokens["aera"] ?? "Root",
                Controller = html.ViewContext.RouteData.DataTokens["controller"] ?? "Home",
                Action = html.ViewContext.RouteData.DataTokens["action"] ?? "Index",
                Config = config,
                User = userJs,
            };

            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings());
            return MvcHtmlString.Create(
                @"//<![CDATA[" + Environment.NewLine +
                @"  var Context = " + json + ";" + Environment.NewLine +
                @"//]]>" + Environment.NewLine);
        }

        private static string GetJavascriptShortDatePattern(string pattern)
        {
            return pattern
                    .Replace("M", "m")
                    .Replace("yy", "y");
        }

        public static string GetJSDateFormatForSession(this HtmlHelper html)
        {
            var session = new NetworkSessionService(html.ViewContext.HttpContext.Session);
            var user = session.User;
            var services = html.Services();

            return GetJavascriptShortDatePattern(services.Culture.DateTimeFormat.ShortDatePattern);
        }

        private static string GetJavascriptShortTimePattern(string pattern)
        {
            return pattern
                    .Replace("tt", "TT");
        }

        public static string GetJSTimeFormatForSession(this HtmlHelper html)
        {
            var session = new NetworkSessionService(html.ViewContext.HttpContext.Session);
            var user = session.User;
            var services = html.Services();

            return GetJavascriptShortTimePattern(services.Culture.DateTimeFormat.ShortTimePattern);
        }

        public static NetworkViewServices Services(this HtmlHelper view)
        {
            if (view.ViewContext.HttpContext.Items.Contains("ViewServices"))
                return (NetworkViewServices)view.ViewContext.HttpContext.Items["ViewServices"];

            return view.ViewData.ContainsKey("ViewServices") ? (NetworkViewServices)view.ViewData["ViewServices"] : new NetworkViewServices();
        }

        public static string DisplayValue(this HtmlHelper html, NetworkUserGender value)
        {
            return EnumTools.GetDescription(value, NetworksEnumMessages.ResourceManager);
        }

        public static MvcHtmlString DisplayValue(this HtmlHelper htmlHelper, NetworkAccessLevel values, string separator = ", ", bool asHtml = true)
        {
            var flags = Enum.GetValues(typeof(NetworkAccessLevel))
                .Cast<NetworkAccessLevel>()
                .Where(v => ((int)v) != 0 && (v & values) == v)
                .ToArray();

            if (asHtml)
            {
                return MvcHtmlString.Create(string.Join(
                    separator.ProperHtmlEscape(),
                    flags.Select(v => "<span title=\"" 
                        + EnumTools.GetDescription(v, NetworksEnumMessages.ResourceManager, null, "_Desc").ProperHtmlAttributeEscape() 
                        + "\">" 
                        + EnumTools.GetDescription(v, NetworksEnumMessages.ResourceManager).ProperHtmlEscape() 
                        + "</span>")));
            }
            else
            {
                return MvcHtmlString.Create(string.Join(
                    separator.ProperHtmlEscape(),
                    flags.Select(v => EnumTools.GetDescription(v, NetworksEnumMessages.ResourceManager).ProperHtmlEscape())));
            }
        }

        public static MvcHtmlString DisplayValue(this HtmlHelper htmlHelper, CompanyAccessLevel value, string separator = ", ", bool asHtml = true)
        {
            if (asHtml)
            {
                return MvcHtmlString.Create(
                    "<span title=\""
                    + EnumTools.GetDescription(value, NetworksEnumMessages.ResourceManager, null, "_Desc").ProperHtmlAttributeEscape() 
                    + "\">"
                    + EnumTools.GetDescription(value, NetworksEnumMessages.ResourceManager).ProperHtmlEscape() 
                    + "</span>");
            }
            else
            {
                return MvcHtmlString.Create(EnumTools.GetDescription(value, NetworksEnumMessages.ResourceManager));
            }
        }

        public static MvcHtmlString DisplayAmount(this HtmlHelper html, decimal value, AmountCurrency currency, bool asHtml = true)
        {
            string result;
            string number;
            switch (currency)
            {
                case AmountCurrency.USD:
                    number = value.ToString("F2");
                    result = "$ " + number;
                    break;

                case AmountCurrency.EUR:
                    number = value.ToString("F2");
                    result = number + " €";
                    break;

                default:
                    throw new ArgumentException("Unknown currency " + currency, "currency");
            }

            if (asHtml)
            {
                var tag = new TagBuilder("span");
                tag.AddCssClass("AmountCurrency");
                tag.SetInnerText(result);
                return MvcHtmlString.Create(tag.ToString());
            }
            else
            {
                return MvcHtmlString.Create(result.ProperHtmlEscape());
            }
        }
    }

    public enum AmountCurrency : short
    {
        USD = 840,
        EUR = 978,
    }
}
