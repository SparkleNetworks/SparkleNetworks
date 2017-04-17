
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class TimelineItemLikesFilter
    {
        public static IEnumerable<TimelineItemLike> IsLiked(this IEnumerable<TimelineItemLike> qry)
        {
            return qry.Where(o => o.IsLiked != null && o.IsLiked == true);
        }

        public static IQueryable<TimelineItemLike> WithUserId(this IQueryable<TimelineItemLike> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }

        public static IQueryable<TimelineItemLike> WithTimelineItemId(this IQueryable<TimelineItemLike> qry, int timelineItemId)
        {
            return qry.Where(o => o.TimelineItemId == timelineItemId);
        }

    }

    public static class TimelineItemCommentLikesFilter
    {
        public static IEnumerable<TimelineItemCommentLike> IsLiked(this IEnumerable<TimelineItemCommentLike> qry)
        {
            return qry.Where(o => o.IsLiked != null && o.IsLiked == true);
        }

        public static IQueryable<TimelineItemCommentLike> WithUserId(this IQueryable<TimelineItemCommentLike> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }

        public static IQueryable<TimelineItemCommentLike> WithTimelineItemCommentId(this IQueryable<TimelineItemCommentLike> qry, int timelineItemCommentId)
        {
            return qry.Where(o => o.TimelineItemCommentId == timelineItemCommentId);
        }
    }
}
