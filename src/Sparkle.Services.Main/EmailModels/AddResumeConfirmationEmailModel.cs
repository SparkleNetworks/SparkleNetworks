using Sparkle.Services.EmailTemplates;
using Sparkle.UI;

namespace Sparkle.Services.Main.EmailModels
{
    public class AddResumeConfirmationEmailModel : BaseEmailModel
    {
        public AddResumeConfirmationEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public int Id { get; set; }

        public string Pin { get; set; }

        public string FirstName { get; set; }

    }
}
