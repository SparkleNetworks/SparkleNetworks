
namespace Sparkle.Commands.Main.Internals
{
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class UserProfile
    {
        static string attributesRegex = @"(\s*[a-zA-Z0-9-]+=""?[^""<>]*""?\s*)*";
        static string attributesGarbageRegex = @"[^<>]*";
        static readonly Regex spanRegex = new Regex(@"</?(span|font)" + attributesGarbageRegex + ">", RegexOptions.IgnoreCase);
        static readonly Regex ttRegex = new Regex(@"</?(tt|div)" + attributesGarbageRegex + ">", RegexOptions.IgnoreCase);
        static readonly Regex preRegex = new Regex(@"</?pre" + attributesGarbageRegex + ">", RegexOptions.IgnoreCase);
        static readonly Regex strongRegex = new Regex(@"</?(strong|b)" + attributesRegex + ">", RegexOptions.IgnoreCase);
        static readonly Regex emRegex = new Regex(@"</?(em|i)" + attributesRegex + ">", RegexOptions.IgnoreCase);
        static readonly Regex brRegex = new Regex(@"</?br" + attributesGarbageRegex + "/?>", RegexOptions.IgnoreCase);
        static readonly Regex pRegex = new Regex(@"</?(p|div)" + attributesRegex + "/?>", RegexOptions.IgnoreCase);
        ////static readonly Regex dpRegex = new Regex(@"</p>\s*<p" + attributesRegex + ">");
        static readonly Regex aRegex = new Regex(@"<a" + attributesGarbageRegex + @"href=""([^""<>]+)""" + attributesGarbageRegex + @">(.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex axRegex = new Regex(@"<a" + attributesGarbageRegex + ">(.+?)</a>", RegexOptions.IgnoreCase);
        static readonly Regex hRegex = new Regex(@"<h(\d)" + attributesGarbageRegex + ">(.*?)</h\\1>", RegexOptions.IgnoreCase);
        static readonly Regex mdLinkRegex = new Regex(@"\[([^\]]*?)\]\(([^\)]+?)\)");
        static readonly Regex imgRegex = new Regex(@"\<img\s([^<>]+)\s*>", RegexOptions.IgnoreCase);
        static readonly Regex attributeRegex = new Regex(@"([^""]+)=""([^""]+)""");
        ////static readonly Regex ulLiRegex = new Regex(@"<ul>\s*((<ul>\s*)?<li>(.*)</li>(\s*</ul>)?)+\s*</ul>", RegexOptions.IgnoreCase);
        ////static Regex ulLiRegex = new Regex("<ul>\\s*(<li>(.*)</li>)+\\s*</ul>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static Regex ulRegex = new Regex("<(?:ul|ol)>(.*?)</(?:ul|ol)>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static Regex liRegex = new Regex("<li>(.*?)</li>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public static string CleanHtml(string value, bool allowBold = true, bool allowItalics = true)
        {
            if (value == null)
                return null;

            // clean html
            var clean = spanRegex.Replace(value, "");
            clean = ttRegex.Replace(clean, "");
            clean = preRegex.Replace(clean, "");
            clean = strongRegex.Replace(clean, allowBold ? "**" : "");
            clean = emRegex.Replace(clean, allowItalics ? "*" : "");
            clean = brRegex.Replace(clean, "\r\n");
            ////clean = dpRegex.Replace(clean, "\r\n");
            clean = pRegex.Replace(clean, "\r\n");
            clean = aRegex.Replace(clean, @"[$2]($1)");
            clean = axRegex.Replace(clean, @"[$1]($1)");

            // images
            var imgs = imgRegex.Matches(clean);
            var imgReplacements = new List<Tuple<string, string>>();
            foreach (Match match in imgs)
            {
                string src = null, alt = null, title = null;
                string replace = match.Value;

                var attrs = match.Groups[1].Value;
                var attributes = attributeRegex.Matches(attrs);
                foreach (Match attr in attributes)
                {
                    var key = attr.Groups[1].Value.Trim();
                    var val = attr.Groups[2].Value.Trim();
                    if (key.ToLowerInvariant() == "src")
                    {
                        src = val.NullIfEmpty();
                    }

                    if (key.ToLowerInvariant() == "alt")
                    {
                        alt = val.NullIfEmpty();
                    }

                    if (key.ToLowerInvariant() == "title")
                    {
                        title = val.NullIfEmpty();
                    }
                }

                string result;
                if (src != null)
                {
                    result = "![" + (title ?? alt ?? "image") + "](" + src + ")";
                }
                else
                {
                    result = "(image)";
                }

                if (!imgReplacements.Any(r => r.Item1 == replace))
                {
                    imgReplacements.Add(new Tuple<string,string>(replace, result));
                }
            }

            for (int i = 0; i < imgReplacements.Count; i++)
            {
                clean = clean.Replace(imgReplacements[i].Item1, imgReplacements[i].Item2);
            }

            imgReplacements.Clear();
            
            // ul li
            var ulLis = ulRegex.Matches(clean);
            foreach (Match match in ulLis)
            {
                string replace = match.Value;

                string content = "\r\n\r\n";
                var liMatches = liRegex.Matches(match.Groups[1].Value);
                foreach (Match liMatch in liMatches)
                {
                    var val = liMatch.Groups[1].Value;
                    content += " * " + val.Trim() + "\r\n";
                }

                content += "\r\n\r\n";

                if (!imgReplacements.Any(r => r.Item1 == replace))
                {
                    imgReplacements.Add(new Tuple<string, string>(replace, content));
                }
            }

            for (int i = 0; i < imgReplacements.Count; i++)
            {
                clean = clean.Replace(imgReplacements[i].Item1, imgReplacements[i].Item2);
            }

            imgReplacements.Clear();
            
            // headers
            var headers = hRegex.Matches(clean);
            foreach (Match item in headers)
            {
                string replace = item.Value;
                string result;
                var text = item.Groups[2].Value.Trim();
                var level = item.Groups[1].Value;
                int l;
                if (int.TryParse(level, out l) && l > 0 && l < 7)
                {
                    if (l == 1)
                    {
                        result = "\r\n\r\n" + text + "\r\n" + string.Concat(Enumerable.Repeat("=", Math.Min(32, Math.Max(text.Length, 10)))) + "\r\n\r\n";
                    }
                    else if (l == 2)
                    {
                        result = "\r\n\r\n" + text + "\r\n" + string.Concat(Enumerable.Repeat("-", Math.Min(32, Math.Max(text.Length, 10)))) + "\r\n\r\n";
                    }
                    else
                    {
                        result = "\r\n\r\n" + string.Concat(Enumerable.Repeat("#", l)) + " " + text + "\r\n\r\n";
                    }
                }
                else
                {
                    result = "\r\n\r\n" + text + "\r\n\r\n";
                }

                imgReplacements.Add(new Tuple<string,string>(replace, result));
            }

            for (int i = 0; i < imgReplacements.Count; i++)
            {
                clean = clean.Replace(imgReplacements[i].Item1, imgReplacements[i].Item2);
            }

            imgReplacements.Clear();
            
            // clean markdown
            var msLinks = mdLinkRegex.Matches(clean);
            foreach (Match match in msLinks)
            {
                var text = match.Groups[1].Value.NullIfEmptyOrWhitespace();
                var href = match.Groups[2].Value.Trim();
                string newHref = null;

                if (href.StartsWith("http://") || href.StartsWith("https://"))
                {
                }
                else if (href.StartsWith("mailto://"))
                {
                    var email = href.Substring(9);
                    if (email.IndexOf('&') > 0)
                    {
                        email = email.Substring(0, email.IndexOf('&'));
                    }

                    newHref = "mailto:" + email;
                }
                else if (href.StartsWith("mailto:"))
                {
                }
                else
                {
                    string email = Validate.EmailAddress(href);
                    if (email != null)
                    {
                        newHref = "mailto:" + href;
                    }
                    else
                    {
                        newHref = "http://" + href;
                    }
                }

                if (newHref != null || text == null)
                {
                    clean = clean.Replace(match.Value, "[" + (text ?? "[lien]") + "](" + (newHref ?? href) + ")");
                }
            }

            // clean lines
            clean = clean.Replace("\r\n\r\n\r\n\r\n", "\r\n\r\n");
            clean = clean.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            clean = clean.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            clean = clean.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            clean = clean.Replace("\r\n\r\n\r\n", "\r\n\r\n");

            return clean.Trim();
        }
    }
}
