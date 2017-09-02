
namespace Sparkle.Services.Main.EmailModels
{
    using System;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class RegisterEmailModel : BaseEmailModel
    {
        public RegisterEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string Login { get; set; }

        public string Infos { get; set; }

        public string Message { get; set; }
    }
}
