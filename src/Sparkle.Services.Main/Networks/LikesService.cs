
namespace Sparkle.Services.Main.Networks
{
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Diagnostics;
    using Sparkle.Services.Networks.Models.Profile;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;

    public class LikesService : ServiceBase, ILikesService
    {
        [DebuggerStepThrough]
        internal LikesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ITimelineItemLikesRepository TimelineItemLikes
        {
            get { return this.Repo.TimelineItemLikes; }
        }

        protected ITimelineItemCommentLikesRepository TimelineItemCommentLikes
        {
            get { return this.Repo.TimelineItemCommentLikes; }
        }

        /// <summary>
        /// Adds the like.
        /// </summary>
        /// <param name="timelineItem">The wall.</param>
        /// <param name="userId">The id person.</param>
        public void Like(TimelineItem timelineItem, int userId)
        {
            var item = this.TimelineItemLikes.GetByTimelineItemIdAndUserId(timelineItem.Id, userId);
            if (item == null)
            {
                item = new TimelineItemLike
                {
                    TimelineItemId = timelineItem.Id,
                    UserId = userId,
                    FirstDateLikedUtc = DateTime.UtcNow,
                    IsLiked = true,
                };
                this.TimelineItemLikes.Insert(item);

                // Ajout de l'activié pour l'auteur du post
                if (timelineItem.PostedByUserId != userId)
                {
                    var activity = new Activity
                    {
                        UserId = timelineItem.PostedByUserId,
                        Type = (int)ActivityType.TimelineItemLike,
                        Message = timelineItem.Id.ToString(),
                        Date = DateTime.UtcNow,
                        ProfileID = userId,
                        Displayed = false
                    };
                    this.Services.Activities.Insert(activity);
                }
            }
            else
            {
                if (item.FirstDateLikedUtc == null)
                    item.FirstDateLikedUtc = DateTime.UtcNow;
                item.IsLiked = true;
                this.TimelineItemLikes.Update(item);
            }
        }

        /// <summary>
        /// Removes the like.
        /// </summary>
        /// <param name="wall">The wall.</param>
        /// <param name="IdPerson">The id person.</param>
        public void UnLike(TimelineItem wall, int IdPerson)
        {
            var item = this.TimelineItemLikes.GetByTimelineItemIdAndUserId(wall.Id, IdPerson);
            
            item.IsLiked = false;
            this.TimelineItemLikes.Update(item);
        }

        /// <summary>
        /// Adds the like comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="userId">The id person.</param>
        public void LikeComment(TimelineItemComment timelineItemComment, int userId)
        {
            var item = this.TimelineItemCommentLikes.GetByTimelineItemCommentIdAndUserId(timelineItemComment.Id, userId);
            if (item == null)
            {
                item = new TimelineItemCommentLike
                {
                    TimelineItemCommentId = timelineItemComment.Id,
                    UserId = userId,
                    FirstDateLikedUtc = DateTime.UtcNow,
                    IsLiked = true,
                };
                this.TimelineItemCommentLikes.Insert(item);

                // Ajout de l'activié pour l'auteur du post
                if (timelineItemComment.PostedByUserId != userId)
                {
                    var activity = new Activity
                    {
                        UserId = timelineItemComment.PostedByUserId,
                        Type = (int)ActivityType.TimelineItemCommentLike,
                        Message = timelineItemComment.TimelineItemId.ToString(),
                        Date = DateTime.UtcNow,
                        ProfileID = userId,
                        Displayed = false
                    };
                    this.Services.Activities.Insert(activity);
                }
            }
            else
            {
                if (item.FirstDateLikedUtc == null)
                    item.FirstDateLikedUtc = DateTime.UtcNow;
                item.IsLiked = true;
                this.TimelineItemCommentLikes.Update(item);
            }
        }

        /// <summary>
        /// Removes the like comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="IdPerson">The id person.</param>
        public void UnLikeComment(TimelineItemComment comment, int IdPerson)
        {
            var item = this.TimelineItemCommentLikes.GetByTimelineItemCommentIdAndUserId(comment.Id, IdPerson);

            item.IsLiked = false;
            this.TimelineItemCommentLikes.Update(item);
        }

        public void MarkTimelineItemAsNotified(int timelineItemId, int userId)
        {
            var item = this.TimelineItemLikes.GetByTimelineItemIdAndUserId(timelineItemId, userId);
            if (item != null)
            {
                item.WasInstantlyNotified = true;
                this.TimelineItemLikes.Update(item);
            }
            else
            {
                item = new TimelineItemLike
                {
                    TimelineItemId = timelineItemId,
                    UserId = userId,
                    WasInstantlyNotified = true,
                };
                this.TimelineItemLikes.Insert(item);
            }
        }

        public void MarkTimelineItemCommentAsNotified(int timelineItemCommentId, int userId)
        {
            var item = this.TimelineItemCommentLikes.GetByTimelineItemCommentIdAndUserId(timelineItemCommentId, userId);
            if (item != null)
            {
                item.WasInstantlyNotified = true;
                this.TimelineItemCommentLikes.Update(item);
            }
            else
            {
                item = new TimelineItemCommentLike
                {
                    TimelineItemCommentId = timelineItemCommentId,
                    UserId = userId,
                    WasInstantlyNotified = true,
                };
                this.TimelineItemCommentLikes.Insert(item);
            }
        }

        public bool MarkTimelineItemAsRead(int timelineItemId, int userId)
        {
            var user = this.Services.People.SelectWithId(userId);
            if (user != null && user.NetworkId == this.Services.NetworkId)
            {
                var item = this.TimelineItemLikes.GetByTimelineItemIdAndUserId(timelineItemId, userId);
                if (item != null)
                {
                    if (item.DateReadUtc.HasValue)
                        return true;

                    item.DateReadUtc = DateTime.UtcNow;
                    this.TimelineItemLikes.Update(item);
                }
                else
                {
                    var timelineItem = this.Services.Wall.SelectByPublicationId(timelineItemId);
                    if (timelineItem != null && timelineItem.NetworkId == this.Services.NetworkId)
                    {
                        item = new TimelineItemLike
                        {
                            TimelineItemId = timelineItemId,
                            UserId = userId,
                            DateReadUtc = DateTime.UtcNow,
                        };
                        this.TimelineItemLikes.Insert(item);
                    }
                    else
                        return false;
                }

                return true;
            }

            return false;
        }

        public bool MarkTimelineItemCommentAsRead(int timelineItemCommentId, int userId)
        {
            var user = this.Services.People.SelectWithId(userId);
            if (user != null && user.NetworkId == this.Services.NetworkId)
            {
                var item = this.TimelineItemCommentLikes.GetByTimelineItemCommentIdAndUserId(timelineItemCommentId, userId);
                if (item != null)
                {
                    item.DateReadUtc = DateTime.UtcNow;
                    this.TimelineItemCommentLikes.Update(item);
                }
                else
                {
                    var timelineComment = this.Services.WallComments.SelectById(timelineItemCommentId);
                    if (timelineComment != null && timelineComment.TimelineItem.NetworkId == this.Services.NetworkId)
                    {
                        item = new TimelineItemCommentLike
                        {
                            TimelineItemCommentId = timelineItemCommentId,
                            UserId = userId,
                            DateReadUtc = DateTime.UtcNow,
                        };
                        this.TimelineItemCommentLikes.Insert(item);
                    }
                    else
                        return false;
                }

                return true;
            }

            return false;
        }

        public IList<TimelineItemLike> GetLikesByTimelineItemId(int timelineItemId, int networkId, LikeOptions options)
        {
            return this.Repo.TimelineItemLikes.GetLikes(timelineItemId, networkId, options);
            ////return this.TimelineItemLikes
            ////    .Select()
            ////    .WithTimelineItemId(timelineItemId)
            ////    .ToList();
        }
    }
}
