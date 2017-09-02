
namespace Sparkle.Services.Main.EmailModels
{
    using System;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class ReminderEmailModel : BaseEmailModel
    {
        public ReminderEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string ContactName { get; set; }

        public string ContactLogin { get; set; }

        public string InvitedEmail { get; set; }
    }
}
