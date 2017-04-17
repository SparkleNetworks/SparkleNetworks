
namespace Sparkle.Services.Networks.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class PhoneUIHint : UIHintAttribute
    {
        private const string hintName = "PhoneNumber";

        public PhoneUIHint()
            : base (hintName)
        {
        }

        public PhoneUIHint(string presentationLayer)
            : base(hintName, presentationLayer)
        {
        }

        public PhoneUIHint(string presentationLayer, params object[] controlParameters)
            : base(hintName, presentationLayer, controlParameters)
        {
        }
    }
}
