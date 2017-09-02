
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IWallCommentsRepository : IBaseNetworkRepository<TimelineItemComment, int>
    {
        IQueryable<TimelineItemComment> SelectWallComments(IList<string> options);

        IQueryable<TimelineItemComment> NewQuery(TimelineItemCommentOptions options);

        IList<TimelineItemComment> GetByImportedIdExpression(int networkId, string expression);

        IList<TimelineItemComment> GetById(int[] ids, TimelineItemCommentOptions options);

        int CountByItem(int timelineItemId);

        ////IDictionary<int, int> Count(int[] commentIds);

        int[] GetCommentedUserIds(int timelineItemId, bool excludeDeleted);

        int CountCreatedByUserId(int userId, int networkId);
    }
}
