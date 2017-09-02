
namespace Sparkle.Services.Networks.Timelines
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimelinePublishRequest : BaseRequest
    {
        public TimelinePublishRequest()
        {
        }

        public TimelinePublishRequest(
            int userId, int itemType, string text, string timelineMode, int timelineId, bool timelinePrivacy = false, int postAs = 0,
            int? extraType = null, string extra = null)
        {
            this.UserId = userId;
            this.ItemType = itemType;
            this.Text = text;
            this.Mode = timelineMode;
            this.ItemId = timelineId;
            this.Privacy = timelinePrivacy;
            this.PostAs = postAs;
            this.ExtraType = extraType;
            this.ExtraValue = extra;
        }

        public int UserId { get; set; }

        public int ItemType { get; set; }

        public string Text { get; set; }

        public string Mode { get; set; }

        public int ItemId { get; set; }

        public bool Privacy { get; set; }

        public int PostAs { get; set; }

        public int? ExtraType { get; set; }

        public string ExtraValue { get; set; }
    }

    public class TimelinePublishResult : BaseResult<TimelinePublishRequest, TimelinePublishError>
    {
        public TimelinePublishResult(TimelinePublishRequest request)
            : base(request)
        {
        }

        public TimelineItem Item { get; set; }
    }

    public enum TimelinePublishError
    {
        CannotPostInDifferentNetwork,
        PostingUserIsInactive,
        PostingInOtherCompanyIsNotAllowed,
        NoSuchGroup,
        NoSuchEvent,
        NoAuthorizedInGroup,
        InvalidMode,
        NoEventVisibility,
    }
}
