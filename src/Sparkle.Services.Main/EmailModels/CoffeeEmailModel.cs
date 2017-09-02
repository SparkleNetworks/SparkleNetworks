
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class CoffeeEmailModel : BaseEmailModel
    {
        public CoffeeEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ContactUrl { get; set; }

        public string Message { get; set; }

        public string ConversationUrl { get; set; }

        public string Time { get; set; }
    }
}
