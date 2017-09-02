
namespace Sparkle.EmailUtility.Splitters
{
    using Sparkle.EmailUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SplitterWindowsMail : ISplitter
    {
        private static readonly string[] Rules = { "<div data-signatureblock=" };

        public int Priority
        {
            get { return 10; }
        }

        public bool IsMatch(MessageBody request)
        {
            if (request.Html == null)
                throw new NullReferenceException("The html data cannot be null");

            return GetSplitIndex(request).HasValue;
        }

        public MessageBodyParts Process(MessageBody request)
        {
            var result = new MessageBodyParts();

            var isId = GetSplitIndex(request);
            if (!isId.HasValue)
                return result;

            int idxToSplit = isId.Value;
            result.ReplyQuote = request.Html.Substring(idxToSplit, request.Html.Length - idxToSplit);

            var bodyStrip = request.Html.Substring(0, idxToSplit);
            var signature = "";
            result.UserMessage = SplitterCommon.SplitSignature(SplitterCommon.ScrubHtml(bodyStrip), out signature).TrimEnd(new char[] { ' ', '\n', '\u00a0' });
            result.Signature = signature != null ? signature.TrimEnd(new char[] { ' ', '\n', '\u00a0' }) : signature;

            return result;
        }

        private static int? GetSplitIndex(MessageBody request)
        {
            int? idx = null;

            foreach (var rule in Rules)
            {
                if (request.Html.Contains(rule))
                {
                    var id = request.Html.IndexOf(rule);
                    if (idx == null || idx > id)
                        idx = id;
                }
            }

            return idx;
        }
    }
}
