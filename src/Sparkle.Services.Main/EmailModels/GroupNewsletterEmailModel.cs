
namespace Sparkle.Services.Main.EmailModels
{
    using System.Collections.Generic;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class GroupNewsletterEmailModel : BaseEmailModel
    {
        public GroupNewsletterEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang) {
        }

        public User Person { get; set; }

        public string Subject { get; set; }

        public IList<MemberGroupNewsletterGroup> Groups { get; set; }
    }
}
