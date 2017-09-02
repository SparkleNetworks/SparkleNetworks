
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class ContactRequestAcceptedEmailModel : BaseEmailModel
    {
        public ContactRequestAcceptedEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public User Contact { get; set; }
        public string Coworkers { get; set; }
    }
}
