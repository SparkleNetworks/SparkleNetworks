
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EmailChangeEmailModel : BaseEmailModel
    {
        public EmailChangeEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }


        public string FirstName { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string Link { get; set; }

        public string CreateDateUtc { get; set; }
    }
}