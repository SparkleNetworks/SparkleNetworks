
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using Sparkle.Data;
    using System.Collections.Generic;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks.Options;
    using System.Data.Objects;

    public class TimelineItemLikesRepository : BaseNetworkRepository<TimelineItemLike>, ITimelineItemLikesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TimelineItemLikesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, c => c.TimelineItemLikes)
        {
        }

        public IQueryable<TimelineItemLike> NewQuery(LikeOptions options)
        {
            ObjectQuery<TimelineItemLike> query = this.Set;

            if ((options & LikeOptions.User) == LikeOptions.User)
                query = query.Include("User");

            if ((options & LikeOptions.TimelineItem) == LikeOptions.TimelineItem)
                query = query.Include("TimelineItem");

            return query;
        }

        public void Update(TimelineItemLike like)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("TimelineItemLikesRepository.Update: This cannot be used within a transaction.");
            
            using (var model = this.GetNewContext())
            {
                var set = this.GetSet(model);
                model.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = model.CreateEntityKey(set.EntitySet.Name, like);
                object outItem;
                if (model.TryGetObjectByKey(key, out outItem))
                {
                    set.ApplyCurrentValues(like);
                    if (like is INetworkEntity)
                        this.VerifyNetworkId((INetworkEntity)like);
                    model.SaveChanges();
                }
            }
        }

        public TimelineItemLike GetByTimelineItemIdAndUserId(int timelineItemId, int userId)
        {
            return this.NewQuery(LikeOptions.None)
                .WithTimelineItemId(timelineItemId)
                .WithUserId(userId)
                .SingleOrDefault();
        }

        public IDictionary<int, int[]> GetTimelineItemsLikes(int networkdId, int[] timelineItemIds)
        {
            var tmp = this.Context.TimelineItems
                .Where(i => timelineItemIds.Contains(i.Id))
                .Select(i => new 
                {
                    Id = i.Id,
                    Count = i.Likes
                        .Where(o => o.IsLiked != null && o.IsLiked == true)
                        .Select(o => o.UserId),
                });
            return tmp
                .ToDictionary(i => i.Id, i => i.Count.ToArray());
        }

        public IDictionary<int, DateTime?> GetReadDates(int networkId, int userId, int[] timelineItemIds)
        {
            return this.Context.TimelineItemLikes
                .Where(i => i.UserId == userId && timelineItemIds.Contains(i.TimelineItemId))
                .ToDictionary(i => i.TimelineItemId, i => i.DateReadUtc);
        }

        public IDictionary<int,int[]> GetTimelineItemsReads(int networkId, int[] timelineItemIds)
        {
            var tmp = this.Context.TimelineItems
                .Where(i => timelineItemIds.Contains(i.Id))
                .Select(i => new
                {
                    Id = i.Id,
                    Count = i.Likes
                        .Where(o => o.DateReadUtc != null)
                        .Select(o => o.UserId),
                });
            return tmp.ToDictionary(i => i.Id, i => i.Count.ToArray());
        }

        public int GetTimelineItemLikes(int timelineItemId)
        {
            return this.Set
                .Where(l => l.TimelineItemId == timelineItemId && l.IsLiked == true)
                .Count();        }

        public IList<TimelineItemLike> GetLikes(int timelineItemId, int networkId, LikeOptions options)
        {
            return this.NewQuery(options)
                .Where(l => l.TimelineItemId == timelineItemId && l.TimelineItem.NetworkId == networkId)
                .ToList();
        }
    }

    public class TimelineItemCommentLikesRepository : BaseNetworkRepository<TimelineItemCommentLike>, ITimelineItemCommentLikesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TimelineItemCommentLikesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, c => c.TimelineItemCommentLikes)
        {
        }

        public IQueryable<TimelineItemCommentLike> NewQuery(LikeOptions options)
        {
            ObjectSet<TimelineItemCommentLike> query = this.Set;

            if ((options & LikeOptions.User) == LikeOptions.User)
                query.Include("User");

            if ((options & LikeOptions.TimelineItem) == LikeOptions.TimelineItem)
                query.Include("TimelineItem");

            return query;
        }

        public void Update(TimelineItemCommentLike like)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("TimelineItemCommentLikesRepository.Update: This cannot be used within a transaction.");

            using (var model = this.GetNewContext())
            {
                var set = this.GetSet(model);
                model.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = model.CreateEntityKey(set.EntitySet.Name, like);
                object outItem;
                if (model.TryGetObjectByKey(key, out outItem))
                {
                    set.ApplyCurrentValues(like);
                    if (like is INetworkEntity)
                        this.VerifyNetworkId((INetworkEntity)like);
                    model.SaveChanges();
                }
            }
        }

        public TimelineItemCommentLike GetByTimelineItemCommentIdAndUserId(int timelineItemCommentId, int userId)
        {
            return this.NewQuery(LikeOptions.None)
                .WithTimelineItemCommentId(timelineItemCommentId)
                .WithUserId(userId)
                .SingleOrDefault();
        }

        public IDictionary<int, int[]> GetTimelineCommentsLikes(int networkId, int[] timelineItemIds)
        {
            var tmp = this.Context.TimelineItemComments
                .Where(i => timelineItemIds.Contains(i.TimelineItemId))
                .Select(i => new 
                {
                    Id = i.Id,
                    Count = i.Likes
                        .Where(o => o.IsLiked != null && o.IsLiked == true)
                        .Select(o => o.UserId),
                });
            return tmp
                .ToDictionary(i => i.Id, i => i.Count.ToArray());
        }

        public IDictionary<int, DateTime?> GetReadDates(int networkId, int userId, int[] timelineItemCommentIds)
        {
            return this.Context.TimelineItemCommentLikes
                .Where(i => i.UserId == userId && timelineItemCommentIds.Contains(i.TimelineItemCommentId))
                .ToDictionary(i => i.TimelineItemCommentId, i => i.DateReadUtc);
        }

        public IDictionary<int, int> CountTimelineCommentsLikes(int[] commentIds)
        {
            return this.Context.TimelineItemCommentLikes
                .Where(i => commentIds.Contains(i.TimelineItemCommentId))
                .GroupBy(i => i.TimelineItemCommentId)
                .ToDictionary(c => c.Key, c => c.Count());
        }
    }
}
