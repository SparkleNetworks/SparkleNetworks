
namespace Sparkle.EmailUtility.Splitters
{
    using Sparkle.EmailUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SplitterGeneric : ISplitter
    {
        public int Priority
        {
            get { return int.MaxValue; }
        }

        public bool IsMatch(MessageBody request)
        {
            return true;
        }

        public MessageBodyParts Process(MessageBody request)
        {
            var result = new MessageBodyParts();
            string signature;
            var html = request.Html;

            foreach (var rule in request.GenericRules)
            {
                if (html.Contains(rule))
                {
                    var idx = html.IndexOf(rule);
                    result.ReplyQuote = html.Substring(idx, html.Length - idx);
                    html = html.Substring(0, idx);
                }
            }

            var noHtml = SplitterCommon.ScrubHtml(html);
            var noSignature = SplitterCommon.SplitSignature(noHtml, out signature);
            result.UserMessage = noSignature.TrimEnd(new char[] { ' ', '\n', '\u00a0' });
            result.Signature = signature != null ? signature.TrimEnd(new char[] { ' ', '\n', '\u00a0' }) : signature;

            return result;
        }
    }
}
