
namespace Sparkle.Data.Networks
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Repository]
    public interface ITimelineItemLikesRepository : IBaseNetworkRepository<TimelineItemLike>
    {
        void Update(TimelineItemLike like);
        TimelineItemLike GetByTimelineItemIdAndUserId(int timelineItemId, int userId);

        IQueryable<TimelineItemLike> NewQuery(LikeOptions options);

        IDictionary<int, int[]> GetTimelineItemsLikes(int networkId, int[] timelineItemIds);

        IDictionary<int, DateTime?> GetReadDates(int networkId, int userId, int[] timelineItemIds);

        int GetTimelineItemLikes(int timelineItemId);

        IDictionary<int, int[]> GetTimelineItemsReads(int networkIs, int[] timelineItemIds);

        IList<TimelineItemLike> GetLikes(int timelineItemId, int networkId, LikeOptions options);
    }

    [Repository]
    public interface ITimelineItemCommentLikesRepository : IBaseNetworkRepository<TimelineItemCommentLike>
    {
        void Update(TimelineItemCommentLike like);
        TimelineItemCommentLike GetByTimelineItemCommentIdAndUserId(int timelineItemCommentId, int userId);

        IDictionary<int, int[]> GetTimelineCommentsLikes(int networkId, int[] timelineItemIds);
        IDictionary<int, int> CountTimelineCommentsLikes(int[] commentIds);

        IDictionary<int, DateTime?> GetReadDates(int networkId, int userId, int[] timelineItemIds);
    }
}
