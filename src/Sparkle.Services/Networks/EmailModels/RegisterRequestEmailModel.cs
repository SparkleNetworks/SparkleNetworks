
namespace Sparkle.Services.Networks.EmailModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Networks.Models;
    using Sparkle.UI;

    public class RegisterRequestEmailModel : BaseEmailModel
    {
        public RegisterRequestEmailModel(SimpleContact recipient, string networkAccentColor, Strings lang)
            : base(recipient, networkAccentColor, lang)
        {
        }

        public RegisterRequestEmailModel(string recipient, string networkAccentColor, Strings lang)
            : base(recipient, networkAccentColor, lang)
        {
        }

        public RegisterRequestModel Request { get; set; }

        public bool CompanyHasNoAdmin { get; set; }

        public CompanyModel Company { get; set; }

        public int OtherRequestCount { get; set; }
    }
}
