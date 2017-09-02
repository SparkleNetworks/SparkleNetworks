
namespace Sparkle.EmailUtility.Splitters
{
    using Sparkle.EmailUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SimpleKnownSeparatorsSplitter : ISplitter
    {
        private static readonly string[] separators = new string[]
        {
            "-------- Message d'origine --------",
        };

        public int Priority
        {
            get { return 1; }
        }

        public bool IsMatch(MessageBody request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            return separators.Any(s => request.Html.Contains(s));
        }

        public MessageBodyParts Process(MessageBody request)
        {
            var result = new MessageBodyParts();
            var index = GetSplitIndex(request);
            if (index != null)
            {
                result.ReplyQuote = request.Html.Substring(index.Value);
                var body = request.Html.Substring(0, index.Value);
                string signature;
                result.UserMessage = SplitterCommon.SplitSignature(SplitterCommon.ScrubHtml(body), out signature);
                result.Signature = signature;
            }

            return result;
        }

        private static int? GetSplitIndex(MessageBody request)
        {
            foreach (var separator in separators)
            {
                var idx = request.Html.IndexOf(separator);
                if (idx > -1)
                {
                    return idx;
                }
            }

            return null;
        }
    }
}
