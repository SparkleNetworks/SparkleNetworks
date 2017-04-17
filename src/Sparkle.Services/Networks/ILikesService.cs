
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;

    public interface ILikesService
    {
        void Like(TimelineItem wall, int IdPerson);
        void LikeComment(TimelineItemComment comment, int IdPerson);
        void UnLike(TimelineItem wall, int IdPerson);
        void UnLikeComment(TimelineItemComment comment, int IdPerson);

        void MarkTimelineItemAsNotified(int timelineItemId, int userId);
        void MarkTimelineItemCommentAsNotified(int timelineItemCommentId, int userId);

        bool MarkTimelineItemAsRead(int timelineItemId, int userId);
        bool MarkTimelineItemCommentAsRead(int timelineItemCommentId, int userId);

        IList<TimelineItemLike> GetLikesByTimelineItemId(int timelineItemId, int networkId, LikeOptions options);
    }
}
