
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Networks.Models;
    using Sparkle.UI;

    public class InviteEmailModel : BaseEmailModel
    {
        public InviteEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public User Contact { get; set; }

        public string InvitationUrl { get; set; }

        public string Infos { get; set; }

        public NetworkModel Network { get; set; }

        public string UnregisterUrl { get; set; }
    }
}
