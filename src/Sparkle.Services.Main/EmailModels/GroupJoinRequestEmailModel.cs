
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GroupJoinRequestEmailModel : BaseEmailModel
    {
        public GroupJoinRequestEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string AdminFirstName { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string GroupName { get; set; }

        public string RequestLink { get; set; }
    }

    public class GroupJoinResponseEmailModel : BaseEmailModel
    {
        public GroupJoinResponseEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string GroupLink { get; set; }

        public string GroupListLink { get; set; }

        public string GroupName { get; set; }

        public bool Success { get; set; }
    }
}
