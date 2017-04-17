
namespace MarkdownSharp.Internals
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// String-related operations.
    /// </summary>
    internal static partial class SrkStringTransformer
    {
        /// <summary>
        /// Escapes XML/HTML characters that needs to be escaped when outputing HTML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks>
        /// Code based on http://wonko.com/post/html-escaping and https://www.owasp.org/index.php/XSS_%28Cross_Site_Scripting%29_Prevention_Cheat_Sheet.
        /// </remarks>
        public static string ProperHtmlEscape(this string content)
        {
            if (content == null)
                return null;

            var builder = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                switch (c)
                {
                    case '<':
                        builder.Append("&lt;");
                        break;

                    case '>':
                        builder.Append("&gt;");
                        break;

                    ////case '&':
                    ////    builder.Append("&amp;");
                    ////    break;

                    case '"':
                        builder.Append("&quot;");
                        break;

                    case '\'':
                        builder.Append("&#x27;");
                        break;

                    ////case '/':
                    ////    builder.Append("&#x2F;");
                    ////    break;

                    default:
                        builder.Append(content[i]);
                        break;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Escapes XML/HTML characters that needs to be escaped for an attribute (&lt;, &gt;, &amp;, &quot;).
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        /// <remarks>
        /// Code based on http://wonko.com/post/html-escaping and https://www.owasp.org/index.php/XSS_%28Cross_Site_Scripting%29_Prevention_Cheat_Sheet.
        /// </remarks>
        public static string ProperHtmlAttributeEscape(this string content)
        {
            if (content == null)
                return null;

            var builder = new StringBuilder();
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                switch (c)
                {
                    case '<':
                        builder.Append("&lt;");
                        break;

                    case '>':
                        builder.Append("&gt;");
                        break;

                    case '&':
                        builder.Append("&amp;");
                        break;

                    case '"':
                        builder.Append("&quot;");
                        break;

                    case '\'':
                        builder.Append("&#x27;");
                        break;

                    //case '/':
                    //    builder.Append("&#x2F;");
                    //    break;

                    //case ' ':
                    //    builder.Append("&nbsp;");
                    //    break;

                    case '`':
                        builder.Append("&#x60;");
                        break;

                    case '!':
                        builder.Append("&#x21;");
                        break;

                    case '@':
                        builder.Append("&#x40;");
                        break;
                        
                    case '$':
                        builder.Append("&#x24;");
                        break;

                    case '%':
                        builder.Append("&#x25;");
                        break;

                    case '(':
                        builder.Append("&#x28;");
                        break;

                    case ')':
                        builder.Append("&#x29;");
                        break;

                    case '=':
                        builder.Append("&#x3D;");
                        break;

                    case '+':
                        builder.Append("&#x2B;");
                        break;
                        
                    case '{':
                        builder.Append("&#x7B;");
                        break;

                    case '}':
                        builder.Append("&#x7D;");
                        break;

                    case '[':
                        builder.Append("&#x5B;");
                        break;

                    case ']':
                        builder.Append("&#x5D;");
                        break;

                    default:
                        builder.Append(content[i]);
                        break;
                }
            }

            return builder.ToString();
        }
    }
}
