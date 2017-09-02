
namespace Sparkle.EmailUtility.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class HtmlAgilityNode
    {
        public string Text { get; set; }

        public HtmlAgilityBlockType BlockType { get; set; }

        public HtmlAgilityNodeType NodeType { get; set; }

        public IList<HtmlAgilityNode> Content { get; set; }

        public IDictionary<string, string> Attributes { get; set; }

        public HtmlAgilityNode()
        {
            Content = new List<HtmlAgilityNode>();
            Attributes = new Dictionary<string, string>();
        }

    }

    public enum HtmlAgilityBlockType
    {
        Block,
        Inline
    }

    public enum HtmlAgilityNodeType
    {
        Text,       // plain text
        Br,         // <br>
        Pre,        // <pre>
        Div,        // <div>
        Par,        // <p>
        Element,     // other, somehow unhandled
        Span,
        Link,
        Strong,
        Ita
    }
}
