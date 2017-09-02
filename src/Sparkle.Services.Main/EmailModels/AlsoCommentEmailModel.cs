using Sparkle.Services.EmailTemplates;
using Sparkle.UI;

namespace Sparkle.Services.Main.EmailModels
{
    public class AlsoCommentEmailModel : BaseEmailModel
    {
        public AlsoCommentEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string OriginalPost { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ContactUrl { get; set; }

        public string Comment { get; set; }

        public string PublicationUrl { get; set; }

        public string LastName { get; set; }

        public string ProfileUrl { get; set; }

        public string OwnerFirstName { get; set; }

        public string OwnerLastName { get; set; }

        public string OwnerUrl { get; set; }

        public bool CommentIsOwner { get; set; }
    }
}
