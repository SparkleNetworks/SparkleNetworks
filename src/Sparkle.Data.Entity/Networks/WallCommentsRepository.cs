
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class WallCommentsRepository : BaseNetworkRepositoryInt<TimelineItemComment>, IWallCommentsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public WallCommentsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TimelineItemComments)
        {
        }

        public IQueryable<TimelineItemComment> SelectWallComments(IList<string> options)
        {
            if (options.Count > 0)
            {
                return SelectWithOptions(this.Set, options);
            }
            return this.Set;
        }

        protected override void OnDeleteOverride(NetworksEntities model, TimelineItemComment itemToDelete, TimelineItemComment actualItemToDelete)
        {
            base.OnDeleteOverride(model, itemToDelete, actualItemToDelete);

            if (!actualItemToDelete.Likes.IsLoaded)
            {
                actualItemToDelete.Likes.Load();
            }
        }

        public IQueryable<TimelineItemComment> NewQuery(TimelineItemCommentOptions options)
        {
            var query = (ObjectQuery<TimelineItemComment>)this.Set;
            
            if ((options & TimelineItemCommentOptions.TimelineItem) == TimelineItemCommentOptions.TimelineItem)
                query = query.Include("TimelineItem");

            if ((options & TimelineItemCommentOptions.TimelineItemUser) == TimelineItemCommentOptions.TimelineItemUser)
                query = query.Include("TimelineItem.PostedBy");

            if ((options & TimelineItemCommentOptions.User) == TimelineItemCommentOptions.User)
                query = query.Include("PostedBy");

            return query;
        }

        public IList<TimelineItemComment> GetByImportedIdExpression(int networkId, string expression)
        {
            var ids = this.Context.GetTimelineCommentIdsByImportedIdExpression(networkId, expression)
                .Select(i => i.Id)
                .ToArray();
            return this.GetById(ids, TimelineItemCommentOptions.None);
        }

        public IList<TimelineItemComment> GetById(int[] ids, TimelineItemCommentOptions options)
        {
            return this.NewQuery(options)
                .Where(i => ids.Contains(i.Id))
                .OrderByDescending(i => i.Id)
                .ToList();
        }

        public int CountByItem(int timelineItemId)
        {
            return this.Set
                .Where(c => c.TimelineItemId == timelineItemId)
                .Count();
        }

        public int[] GetCommentedUserIds(int timelineItemId, bool excludeDeleted)
        {
            return this.Set
                .WithWallId(timelineItemId)
                .ExcludeDeleted(excludeDeleted)
                .GroupBy(c => c.PostedByUserId)
                .Select(g => g.Key)
                .ToArray();
        }

        public int CountCreatedByUserId(int userId, int networkId)
        {
            return this.Set
                .Where(x => x.TimelineItem.NetworkId == networkId && x.PostedByUserId == userId)
                .Count();
        }
    }
}
