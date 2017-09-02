
namespace Sparkle.Entities.Networks
{
    using System.Collections.Generic;

    public class MemberGroupNewsletter
    {
        public User Person { get; set; }

        public IList<MemberGroupNewsletterGroup> Groups { get; set; }
    }

    public class MemberGroupNewsletterGroup
    {
        public Group Group { get; set; }
        public IList<TimelineItem> Walls { get; set; }

        public int PendingRequestsCount { get; set; }

        public Sparkle.Services.Networks.Timelines.BasicTimelineItemModel Timeline { get; set; }
    }
}
