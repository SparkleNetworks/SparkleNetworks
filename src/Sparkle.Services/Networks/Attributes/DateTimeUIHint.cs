
namespace Sparkle.Services.Networks.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class DateTimeUIHint : UIHintAttribute
    {
        private const string hintName = "DateTime";

        public DateTimeUIHint()
            : base(hintName)
        {
        }

        public DateTimeUIHint(string presentationLayer)
            : base(hintName, presentationLayer)
        {
        }

        public DateTimeUIHint(string presentationLayer, params object[] controlParameters)
            : base(hintName, presentationLayer, controlParameters)
        {
        }
    }
}
