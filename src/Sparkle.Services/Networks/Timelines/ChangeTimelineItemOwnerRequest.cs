
namespace Sparkle.Services.Networks.Timelines
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChangeTimelineItemOwnerRequest : BaseRequest
    {
        public int TimelineItemId { get; set; }
        public int ActingUserId { get; set; }
        public int NewUserId { get; set; }
    }

    public class ChangeTimelineItemOwnerResult : BaseResult<ChangeTimelineItemOwnerRequest, ChangeTimelineItemOwnerCode>
    {
        public ChangeTimelineItemOwnerResult(ChangeTimelineItemOwnerRequest request)
            : base(request)
        {
            this.UpdatedItems = new List<TimelineItem>();
            this.NotSupportedItems = new List<TimelineItem>();
            this.UpdatedComments = new List<TimelineItemComment>();
            this.NotSupportedComments = new List<TimelineItemComment>();
        }

        public IList<TimelineItem> UpdatedItems { get; set; }

        public IList<TimelineItem> NotSupportedItems { get; set; }

        public IList<TimelineItemComment> UpdatedComments { get; set; }

        public IList<TimelineItemComment> NotSupportedComments { get; set; }
    }

    public enum ChangeTimelineItemOwnerCode
    {
        NotSupported,
        NoSuchUser,
        NoSuchItem,
    }
}
