
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class PrivateMessageEmailModel : BaseEmailModel
    {
        public PrivateMessageEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ContactUrl { get; set; }

        public string Message { get; set; }

        public string ConversationUrl { get; set; }

        public string Subject { get; set; }

        public bool IsReplyToEmailEnabled { get; set; }

        public int MessageId { get; set; }
    }
}
