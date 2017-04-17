
namespace Sparkle.EmailUtility.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MessageBody
    {
        public string Html { get; set; }

        public string[] GenericRules { get; set; }

        // Not implmented yet
        ////public string Text { get; set; }

        public MessageBody(string html, string[] genericRules = null)
        {
            this.Html = html;
            this.GenericRules = genericRules ?? new string[0];
        }

    }
}
