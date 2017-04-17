
namespace Sparkle.Services.Networks.EmailModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class PendingUserRegistrationsModel : BaseEmailModel
    {
        public PendingUserRegistrationsModel(SimpleContact recipient, string networkAccentColor, Strings lang)
            : base(recipient, networkAccentColor, lang)
        {
        }

        public IList<Entities.Networks.User> PendingUsers { get; set; }

        public int PendingUsersCount { get; set; }

        public Entities.Networks.User NewPendingUser { get; set; }
    }
}
