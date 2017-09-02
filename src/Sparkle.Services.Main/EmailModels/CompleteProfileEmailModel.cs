
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class CompleteProfileEmailModel : BaseEmailModel
    {
        public CompleteProfileEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string CompleteProfileUrl { get; set; }

        public string Comment { get; set; }

        public string ContactUrl { get; set; }
    }
}
