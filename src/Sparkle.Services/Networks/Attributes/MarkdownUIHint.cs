
namespace Sparkle.Services.Networks.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class MarkdownUIHint : UIHintAttribute
    {
        private const string hintName = "Markdown";

        public MarkdownUIHint()
            : base(hintName)
        {
        }

        public MarkdownUIHint(string presentationLayer)
            : base(hintName, presentationLayer)
        {
        }

        public MarkdownUIHint(string presentationLayer, params object[] controlParameters)
            : base(hintName, presentationLayer, controlParameters)
        {
        }
    }
}
