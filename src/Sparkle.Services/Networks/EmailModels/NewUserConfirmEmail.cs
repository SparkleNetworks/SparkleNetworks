
namespace Sparkle.Services.Networks.EmailModels
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NewUserConfirmEmail : BaseEmailModel
    {
        public NewUserConfirmEmail(string recipientEmailAddress, string networkAccentColor, Strings lang)
            : base(recipientEmailAddress, networkAccentColor, lang)
        {
        }

        public string ConfirmUrl { get; set; }

        public UserActionKey ActionKey { get; set; }

        public User User { get; set; }

        public string Message { get; set; }
    }
}
