
namespace Sparkle.WebBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Optimization;

    /*
     * inspired from System.Web.Optimization.CssRewriteUrlTransform
     * stuck on how to make strings obscure and decodable in js
     * NOT IS USE.
     * http://stackoverflow.com/questions/246801/how-can-you-encode-a-string-to-base64-in-javascript
     * */
    public class JsObscureStringTransform : IItemTransform, IBundleTransform
    {
        private static Regex regex = new Regex(@"(""/[^""\\]+""|'/[^'\\]+')", RegexOptions.Compiled);

        public string Process(string includedVirtualPath, string input)
        {
            if (includedVirtualPath == null)
                throw new ArgumentNullException("includedVirtualPath");

            return this.EscapeSlashes(input);
        }

        private string EscapeSlashes(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return content;
            
            return regex.Replace(
                content,
                (Match match) => 
                {
                    if (match.Groups.Count >= 2)
                    {
                        var result = match.Groups[1].Value.Replace("/", "\\x2F");
                        return match.Value.Replace(match.Groups[1].Value, result);
                    }
                    else
                    {
                        return match.Value;
                    }
                });
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (response == null)
                throw new ArgumentNullException("response");

            var result = EscapeSlashes(response.Content);
            response.Content = result;
        }
    }
}
