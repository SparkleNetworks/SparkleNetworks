
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Timelines;
    using SrkToolkit.Domain;

    public class WallCommentsService : ServiceBase, IWallCommentsService
    {
        [Obsolete]
        public WallCommentsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory, int CurrentId)
            : base(repositoryFactory, serviceFactory)
        {
            this.CurrentId = CurrentId;
        }

        [DebuggerStepThrough]
        internal WallCommentsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IWallCommentsRepository WallCommentsRepository
        {
            get { return this.Repo.WallComments; }
        }
        
        public int CurrentId { get; set; }

        public IList<TimelineItemComment> SelectFromWallItem(int wallId)
        {
            this.OptionsList.Add("PostedBy");

            return  WallCommentsRepository
                .SelectWallComments(OptionsList)
                .WithWallId(wallId)
                .OrderBy(o => o.CreateDate)
                .ToList()
                .GetLike(this.CurrentId);
        }

        public int Count()
        {
            return WallCommentsRepository
                .SelectWallComments(OptionsList)
                .ExcludeDeleted(false)
                .Where(o => o.TimelineItem.NetworkId == this.Services.NetworkId)
                .Count();
        }

        public int CountToday()
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            var to = from.AddDays(1D);
            return WallCommentsRepository
                .SelectWallComments(OptionsList)
                .Where(i => i.TimelineItem.NetworkId == this.Services.NetworkId && from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public int CountLast24Hours()
        {
            var to = DateTime.UtcNow;
            var from = to.AddDays(-1D);
            return WallCommentsRepository
                .SelectWallComments(OptionsList)
                .Where(i => i.TimelineItem.NetworkId == this.Services.NetworkId && from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public TimelineItemComment SelectById(int commentId)
        {
            return WallCommentsRepository
                .SelectWallComments(OptionsList)
                .WithCommentId(commentId)
                .FirstOrDefault();
        }

        public TimelineItemComment SelectById(int commentId, TimelineItemCommentOptions options)
        {
            return WallCommentsRepository
                .NewQuery(options)
                .WithCommentId(commentId)
                .FirstOrDefault();
        }
        
        [Obsolete("Use Publish or Import instead")]
        public long Insert(TimelineItemComment item)
        {
            WallCommentsRepository.Insert(item);
            return 1;
        }

        public TimelineItemComment Import(TimelineItemComment item)
        {
            item = WallCommentsRepository.Insert(item);
            return this.SelectById(item.Id);
        }

        public TimelineCommentResult Publish(TimelineCommentRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new TimelineCommentResult(request);

            var user = this.Repo.People.GetActiveById(request.UserId, Data.Options.PersonOptions.None);
            if (user == null)
            {
                result.Errors.Add(TimelineCommentError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }
            else if (user.NetworkId != this.Services.NetworkId && !user.NetworkAccess.HasFlag(NetworkAccessLevel.SparkleStaff))
            {
                result.Errors.Add(TimelineCommentError.CannotActOnDifferentNetwork, NetworksEnumMessages.ResourceManager);
            }

            var timeline = this.Services.Wall.SelectWallItemById(request.TimelineItemId);
            if (timeline == null)
            {
                result.Errors.Add(TimelineCommentError.NoSuchTimelineItem, NetworksEnumMessages.ResourceManager);
            }
            else if (!this.Services.Wall.IsVisible(request.TimelineItemId, user.Id))
            {
                result.Errors.Add(TimelineCommentError.NoSuchTimelineItem, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
                return result;

            // create comment
            var item = new TimelineItemComment
            {
                PostedByUserId = user.Id,
                CreateDate = DateTime.Now,
                Text = request.Comment,
                TimelineItemId = timeline.Id,
            };
            this.Repo.WallComments.Insert(item);
            item = this.Services.WallComments.SelectById(item.Id, TimelineItemCommentOptions.TimelineItem);
            result.CommentEntity = item;

            // notify owner
            var owner = this.Services.People.GetByIdFromAnyNetwork(timeline.PostedByUserId, Data.Options.PersonOptions.None);
            if (owner.Id != user.Id)
            {
                var activity = new Activity
                {
                    UserId = owner.Id,
                    Type = (int)ActivityType.NewComment,
                    Message = timeline.Id.ToString(),
                    Date = DateTime.Now,
                    ProfileID = user.Id,
                    Displayed = false,
                };
                this.Services.Activities.Insert(activity);
            }

            result.Succeed = true;

            // notify 
            ////this.Services.Email.SendNotification(item);
            ////this.Services.Wall.Notify(timeline, timelineType ,user, item);
            TimelineType timelineType;
            if (this.Services.Wall.TryGetTimelineType(timeline, out timelineType))
            {
                this.Services.Parallelize(services => PublishNotify(services, timeline.Id, timelineType, user.Id, item.Id));
            }
            else
            {
                this.Services.Logger.Error("WallCommentsService.Publish", ErrorLevel.Critical, "Could not determine type of timeline item " + timeline);
            }

            return result;
        }

        private static void PublishNotify(IServiceFactory services, int timelineItemId, TimelineType timelineType, int userId, int commentId)
        {
            // this method is built for parallelization
            // be careful to threading stuff
            // 'this' may not be the 'this' you think of

            try
            {
                var itemToPublish = services.Wall.SelectByPublicationId(timelineItemId);
                var currentUser = services.People.SelectWithId(userId);
                var comment = services.WallComments.SelectById(commentId);

                services.Wall.Notify(itemToPublish, timelineType, currentUser, comment);
            }
            catch (Exception ex)
            {
                services.Logger.Critical("WallCommentsService.Publish.ThreadPoolDelegate", ErrorLevel.Critical, ex);
            }
        }

        public void Delete(TimelineItemComment item)
        {
            WallCommentsRepository.Delete(item);
        }

        private IList<string> IncludeLikeInList(IList<string> includes)
        {

            if (!includes.Contains("PeoplesLike")) includes.Add("PeoplesLike");
            return includes;
        }

        public IList<TimelineItemComment> GetAllWithImportedId(TimelineItemCommentOptions options)
        {
            return this.WallCommentsRepository
                .NewQuery(options)
                .Where(i => i.ImportedId != null)
                .ToList();
        }

        public TimelineItemComment Update(TimelineItemComment item)
        {
            var result = this.WallCommentsRepository.Update(item);
            this.Services.Logger.Info("WallCommentsService.Update", ErrorLevel.Success, "Manual edition of wallcomment " + item.Id);
            return result;
        }

        public bool IsVisible(int timelineItemCommentId, int? userId)
        {
            var item = this.SelectById(timelineItemCommentId);
            if (item == null)
            {
                return false;
            }

            return this.Services.Wall.IsVisible(item.TimelineItemId, userId);
        }

        public int CountByItem(int timelineItemId)
        {
            return this.Repo.WallComments.CountByItem(timelineItemId);
        }

        public IDictionary<int, int> CountLikes(int[] commentIds)
        {
            return this.Repo.TimelineItemCommentLikes.CountTimelineCommentsLikes(commentIds);
        }

        public int CountByUser(int userId)
        {
            return this.WallCommentsRepository.CountCreatedByUserId(userId, this.Services.NetworkId);
        }
    }
}
