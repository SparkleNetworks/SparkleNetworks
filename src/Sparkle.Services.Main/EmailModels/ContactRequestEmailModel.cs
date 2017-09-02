
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Networks.Models;
    using Sparkle.UI;

    public class ContactRequestEmailModel : BaseEmailModel
    {
        public ContactRequestEmailModel(string recipientEmailAddress, string accentColor, Strings lang) 
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public UserModel Contact { get; set; }
        public string Coworkers { get; set; }

        public Job ContactJob { get; set; }

        public Company ContactCompany { get; set; }
    }
}
