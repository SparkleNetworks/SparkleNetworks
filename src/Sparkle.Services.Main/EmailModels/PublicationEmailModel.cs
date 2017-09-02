
namespace Sparkle.Services.Main.EmailModels
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.UI;

    public class PublicationEmailModel : BaseEmailModel
    {
        public PublicationEmailModel(string recipientEmailAddress, string accentColor, Strings lang)
            : base(recipientEmailAddress, accentColor, lang)
        {
        }

        public User Person { get; set; }

        public Group Group { get; set; }

        public Event Event { get; set; }

        public TimelineItem WallItem { get; set; }

        public string PublicationTitle { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string ContactUrl { get; set; }

        public string ContactPictureUrl { get; set; }

        public string Subject { get; set; }

        public string LinkName { get; set;}

        public string LinkUrl { get; set; }

        public string ContactLogin { get; set; }

        public bool IsReplyToEmailEnabled { get; set; }

        public string Intro { get; set; }

        public string PostedByName { get; set; }
        public string PostedByUrl { get; set; }
        public string PostedByPictureUrl { get; set; }
        public string PostedByColor { get; set; }

        public Services.Networks.Timelines.BasicTimelineItemModel TimelineItem { get; set; }
    }
}
