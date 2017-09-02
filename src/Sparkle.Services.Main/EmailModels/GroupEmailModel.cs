
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class GroupEmailModel : BaseEmailModel
    {
        public GroupEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ProfileUrl { get; set; }

        public string GroupName { get; set; }

        public string GroupUrl { get; set; }

        public string GroupDescription { get; set; }

        public string GroupPicture { get; set; }
    }
}
