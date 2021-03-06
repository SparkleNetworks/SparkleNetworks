﻿
namespace Sparkle.Services.Main.EmailModels
{
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;
    using System;

    public class ProposalEmailModel : BaseEmailModel
    {
        public ProposalEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ContactUrl { get; set; }

        public string Message { get; set; }

        public string EmailText { get; set; }

        public string ConversationUrl { get; set; }

        public string Time { get; set; }

        public string ContactPictureUrl { get; set; }

        public DateTime Date { get; set; }
    }
}
