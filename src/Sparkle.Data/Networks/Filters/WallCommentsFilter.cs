
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class TimelineItemCommentsFilter
    {
        public static IQueryable<TimelineItemComment> WithWallId(this IQueryable<TimelineItemComment> qry, int wallId)
        {
            return qry.Where(o => o.TimelineItemId == wallId);
        }

        public static IQueryable<TimelineItemComment> WithCommentId(this IQueryable<TimelineItemComment> qry, int commentId)
        {
            return qry.Where(o => o.Id == commentId);
        }

        public static List<TimelineItemComment> GetLike(this List<TimelineItemComment> query, int? CurrentId)
        {
            if (CurrentId.HasValue)
            {
                query.ForEach(
                    o => o.IsLikeByCurrentId = o.Likes.Any(n => n.UserId == CurrentId)
               );
            }
            return query;
        }

        public static TimelineItemComment GetLike(this TimelineItemComment wall, int? CurrentId)
        {
            if (CurrentId.HasValue)
            {
                wall.IsLikeByCurrentId = wall.Likes.Any(o => o.UserId == CurrentId);
            }
            return wall;
        }

        public static IQueryable<TimelineItemComment> ExcludeDeleted(this IQueryable<TimelineItemComment> qry, bool showDeleted)
        {
            return qry;////.Where(i => showDeleted || i.DeleteReason == null);
        }
    }
}
