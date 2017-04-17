
namespace Sparkle.EmailUtility.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MessageBodyParts
    {
        public string UserMessage { get; set; }

        public string Signature { get; set; }

        public string ReplyQuote { get; set; }

        public string Other { get; set; } // <- just in case
    }
}
