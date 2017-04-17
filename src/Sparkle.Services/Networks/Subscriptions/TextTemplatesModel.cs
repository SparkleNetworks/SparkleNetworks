
namespace Sparkle.Services.Networks.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TextTemplatesModel
    {
        public IList<Texts.EditTextRequest> ConfirmTexts { get; set; }

        public IList<Texts.EditTextRequest> RenewTexts { get; set; }

        public IList<Texts.EditTextRequest> ExpireTexts { get; set; }
    }
}
