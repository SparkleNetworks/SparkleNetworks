
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimelineItemsConveyor
    {
        public TimelineItemsConveyor()
        {
        }

        public TimelineItemsConveyor(int? userid)
        {
            this.CurrentUserId = userid;
            this.OptionnalCheck = new Dictionary<int, bool>();
            this.Optionnal = new Dictionary<int, object>();
        }

        public int? CurrentUserId { get; set; }

        public TimelineDisplayContext TimelineDisplayContext { get; set; }

        public IList<TimelineItem> TimelineItems { get; set; }

        public IList<TimelineItemComment> TimelineItemComments { get; set; }

        public IList<TimelineItemSkill> TimelineItemSkills { get; set; }

        public IDictionary<int, int[]> TimelineItemLikesCount { get; set; }

        public IDictionary<int, int[]> TimelineItemCommentsLikesCount { get; set; }

        public IDictionary<int, int[]> TimelineItemReadCount { get; set; }

        public IDictionary<int, bool> OptionnalCheck { get; set; }

        public IDictionary<int, object> Optionnal { get; set; }

        public IDictionary<int, Company> Companies { get; set; }

        public IDictionary<int, DateTime?> TimelineItemReadDates { get; set; }

        public IDictionary<int, DateTime?> TimelineItemCommentReadDates { get; set; }

        public IDictionary<int, Ad> RelatedAds { get; set; }

        public IDictionary<int, Sparkle.Services.Networks.Models.UserModel> Users { get; set; }

        public IDictionary<int, Services.Networks.Models.GroupModel> Groups { get; set; }

        public IDictionary<int, Group> GroupEntities { get; set; }
    }
}
