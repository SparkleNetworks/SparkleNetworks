
namespace Sparkle.Services.Networks.Timelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class TimelineCommentRequest : BaseRequest
    {
        public int UserId { get; set; }

        public int TimelineItemId { get; set; }

        public string Comment { get; set; }
    }

    public class TimelineCommentResult : BaseResult<TimelineCommentRequest, TimelineCommentError>
    {
        public TimelineCommentResult(TimelineCommentRequest request)
            : base(request)
        {
        }

        public Entities.Networks.TimelineItemComment CommentEntity { get; set; }
    }

    public enum TimelineCommentError
    {
        NoSuchUser,
        CannotActOnDifferentNetwork,
        NoSuchTimelineItem,
    }
}
