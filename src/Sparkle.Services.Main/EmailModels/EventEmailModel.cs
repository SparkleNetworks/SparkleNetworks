
namespace Sparkle.Services.Main.EmailModels
{
    using System;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class EventEmailModel : BaseEmailModel
    {
        public EventEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }


        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ProfileUrl { get; set; }

        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public string EventUrl { get; set; }

        public string EventReponseYesUrl { get; set; }
        public string EventReponseMaybeUrl { get; set; }
        public string EventReponseNoUrl { get; set; }

        public string EventDescription { get; set; }
    }
}
