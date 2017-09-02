
namespace Sparkle.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Utility that helps save the prefered user's culture by storing it into cookies.
    /// </summary>
    public static class CultureTools
    {
        public static CultureInfo GetCookieCulture(HttpContext context, IEnumerable<CultureInfo> cultures)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var cookieName = "Culture";
            var cooks = context.Request.Cookies;
            var cook = cooks.Get(cookieName);
            if (cook == null)
                return null;

            if (cook.Values.Count <= 1)
            {
                CultureInfo info = null;
                try
                {
                    info = new CultureInfo(cook.Value);
                }
                catch (ArgumentException) { }
                if (info != null && cultures.Contains(info))
                    return info;
            }
            else
            {
                foreach (var item in cook.Values)
                {
                    CultureInfo info = null;
                    try
                    {
                        var value = item as string;
                        if (value == null)
                            continue;
                        info = new CultureInfo(value);
                    }
                    catch (ArgumentException) { }
                    if (info != null && cultures.Contains(info))
                        return info;
                }
            }

            return null;
        }

        public static CultureInfo GetSessionCulture(HttpContext context, IEnumerable<CultureInfo> cultures)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Session == null || context.Session["Culture"] == null)
                return null;
            var culture = context.Session["Culture"] as string;
            if (culture != null)
            {
                CultureInfo info = null;
                try
                {
                    info = new CultureInfo(culture);
                }
                catch (ArgumentException) { }

                if (info != null && cultures.Any(c => c == info))
                    return info;
            }
            return null;
        }

        public static CultureInfo GetBrowserCulture(HttpContextBase context, IEnumerable<CultureInfo> supportedCultures)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return GetBrowserCulture(context.Request.UserLanguages, supportedCultures);
        }

        public static CultureInfo GetBrowserCulture(HttpContext context, IEnumerable<CultureInfo> supportedCultures)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return GetBrowserCulture(context.Request.UserLanguages, supportedCultures);
        }

        public static CultureInfo GetBrowserCulture(string[] userLanguages, IEnumerable<CultureInfo> supportedCultures)
        {
            var langs = userLanguages;
            if (langs == null || langs.Length == 0)
                return null;

            foreach (var item in langs)
            {
                if (item.Length < 2 || item.Length > 32)
                    continue;

                string value = null;
                if (item.Length == 2 || item.Length == 5 && item[2] == '-')
                {
                    value = item;
                }
                else
                {
                    var sep = item.IndexOf(';');
                    if (sep > 1)
                        value = item.Substring(0, sep);
                }

                if (value == null)
                    continue;

                CultureInfo info = null;
                try
                {
                    info = new CultureInfo(value);
                }
                catch (ArgumentException) { }
                if (info != null && supportedCultures.Contains(info))
                    return info;
            }

            return null;
        }

        public static CultureInfo[] GetBrowserCultures(string[] userLanguages)
        {
            var langs = userLanguages;
            if (langs == null || langs.Length == 0)
                return null;

            IList<CultureInfo> cultures = new List<CultureInfo>(userLanguages.Length);
            foreach (var item in langs)
            {
                if (item.Length < 2 || item.Length > 32)
                    continue;

                string value = null;
                if (item.Length == 2 || item.Length == 5 && item[2] == '-')
                {
                    value = item;
                }
                else
                {
                    var sep = item.IndexOf(';');
                    if (sep > 1)
                        value = item.Substring(0, sep);
                }

                if (value == null)
                    continue;

                CultureInfo info = null;
                try
                {
                    info = new CultureInfo(value);
                    cultures.Add(info);
                }
                catch (ArgumentException) { }
            }

            return cultures.ToArray();
        }

        public static CultureInfo GetClientCulture(HttpContext context, IEnumerable<CultureInfo> cultures)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return GetSessionCulture(context, cultures)
                ?? GetCookieCulture(context, cultures)
                ?? GetBrowserCulture(context, cultures);
        }

        public static void SaveClientCulture(HttpContext context, CultureInfo lang)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (lang == null)
                throw new ArgumentNullException("lang");

            if (context.Request.Cookies == null)
                return;

            HttpCookie c = context.Request.Cookies["Culture"];
            if (c == null)
            {
                c = new HttpCookie("Culture", lang.Name);
                c.Expires = DateTime.Now.AddYears(1);
                context.Response.Cookies.Add(c);
            }
            else
            {
                c.Value = lang.Name;
                c.Expires = DateTime.Now.AddYears(1);
                context.Response.Cookies.Add(c);
            }

            if (context.Session != null)
            {
                if (context.Session["Culture"] == null || (string)context.Session["Culture"] != lang.Name)
                {
                    context.Session["Culture"] = lang.Name;
                }
            }
        }
    }
}
