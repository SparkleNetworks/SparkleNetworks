
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Timelines;

    public interface IWallCommentsService
    {
        IList<string> OptionsList { get; set; }
        [Obsolete("Use this.Services.Identity instead")]
        int CurrentId { get; set; }
        void Delete(TimelineItemComment item);

        [Obsolete("Use Publish or Import instead")]
        long Insert(TimelineItemComment item);

        TimelineItemComment Import(TimelineItemComment item);

        TimelineItemComment SelectById(int commentId);
        TimelineItemComment SelectById(int commentId, TimelineItemCommentOptions options);
        IList<TimelineItemComment> SelectFromWallItem(int wallId);

        int Count();
        int CountToday();
        int CountLast24Hours();

        IList<TimelineItemComment> GetAllWithImportedId(TimelineItemCommentOptions options);

        TimelineItemComment Update(TimelineItemComment item);

        bool IsVisible(int timelineItemCommentId, int? userId);

        TimelineCommentResult Publish(TimelineCommentRequest request);

        int CountByItem(int timelineItemId);

        IDictionary<int, int> CountLikes(int[] commentIds);

        int CountByUser(int userId);
    }
}
