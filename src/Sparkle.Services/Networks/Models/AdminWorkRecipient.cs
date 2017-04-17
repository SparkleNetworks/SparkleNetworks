
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public class AdminWorkRecipient
    {
        public UserModel User { get; set; }

        public IList<NetworkAccessLevel> NetworkAccessLevels { get; set; }

        public EmailContact Contact { get; set; }

        public string DisplayName
        {
            get
            {
                if (this.User != null)
                    return this.User.DisplayName;
                if (this.Contact != null)
                    return this.Contact.DisplayName;
                return "-";
            }
        }
    }
}
