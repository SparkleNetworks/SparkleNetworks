
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CompanyEmailModel : BaseEmailModel
    {
        public CompanyEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public Company Company { get; set; }

        public int CompanyId { get; set; }

        public int RequestId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public CompanyRequest CompanyRequest { get; set; }

        public Guid RequestUniqueId { get; set; }

        public string[] AdminEmails { get; set; }

        public string[] OtherEmails { get; set; }

        public string Reason { get; set; }

        public string SenderPictureUrl { get; set; }
    }
}
