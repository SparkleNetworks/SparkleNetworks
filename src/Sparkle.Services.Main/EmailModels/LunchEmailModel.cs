
namespace Sparkle.Services.Main.EmailModels
{
    using System;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class LunchEmailModel : BaseEmailModel
    {
        public LunchEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public string FirstName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ProfileUrl { get; set; }

        public DateTime EventDate { get; set; }
        public string EventUrl { get; set; }
        public string EventDescription { get; set; }
        public string EventName { get; set; }

        public string PlaceName { get; set; }

        public string PlaceUrl { get; set; }

        public string LunchReponseYesUrl { get; set; }
        public string LunchReponseOtherUrl { get; set; }

    }
}
