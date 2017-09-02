
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Models;
    using System;
    using Sparkle.Data.Networks.Options;

    public class ActivitiesService : ServiceBase, IActivitiesService
    {
        [DebuggerStepThrough]
        internal ActivitiesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IActivitiesRepository activitiesRepository
        {
            get { return this.Repo.Activities; }
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int Insert(Activity item)
        {
            return this.activitiesRepository.Insert(item).Id;
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Activity Update(Activity item)
        {
            return this.activitiesRepository.Update(item);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(Activity item)
        {
            this.activitiesRepository.Delete(item);
        }

        /// <summary>
        /// Selects the five recent activities by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Activity> SelectFiveRecentActivitiesByUserId(int userId)
        {
            return activitiesRepository
                .Select()
                .UserId(userId)
                .OrderByDescending(o => o.Date)
                .Take(5)
                .ToList();
        }

        /// <summary>
        /// Selects the recent activities by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Activity> SelectRecentActivitiesByUserId(int userId)
        {
            return activitiesRepository
                .Select()
                .UserId(userId)
                .OrderBy(o => o.Date)
                .ToList();
        }

        /// <summary>
        /// Selects all activities by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Activity> SelectAllActivitiesByUserId(int userId)
        {
            return activitiesRepository
                .Select()
                .UserId(userId)
                .OrderBy(o => o.Date)
                .ToList();
        }

        /// <summary>
        /// Marks all activities from the current user as displayed.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void MarkAsDisplayed(int userId)
        {
            IList<Activity> items = SelectRecentActivitiesByUserId(userId);
            foreach (Activity item in items)
            {
                item.Displayed = true;
                this.activitiesRepository.Update(item);
            }
        }

        public IList<ActivityModel> GetUsersNotifications(int userId, bool markAsRead, int pageSize, int pageId)
        {
            var items = this.GetUsersActivitesQuery(userId);
            var query = items.Skip(pageSize * pageId).Take(pageSize);
            var list = query.ToList();

            if (markAsRead)
            {
                var ids = list.Where(a => a.IsActivity).Select(a => a.Id).ToArray();
                this.Repo.Activities.MarkRead(ids);
            }
            
            var groupIds = query.Where(x => x.GroupId != null).GroupBy(x => x.GroupId.Value).Select(g => g.Key).ToArray();
            var companyIds = query.Where(x => x.CompanyId != null).GroupBy(x => x.CompanyId.Value).Select(g => g.Key).ToArray();
            var eventIds = query.Where(x => x.EventId != null).GroupBy(x => x.EventId.Value).Select(g => g.Key).ToArray();
            var profileIds = query.Where(x => x.ProfileId != null).GroupBy(x => x.ProfileId.Value).Select(g => g.Key).ToArray();
            var userIds = query.Where(x => x.UserId != null).GroupBy(x => x.UserId).Select(g => g.Key).ToArray();

            var groups = groupIds.Length > 0 ? this.Repo.Groups.GetActiveById(groupIds, this.Services.NetworkId) : new List<Group>();
            var companies = companyIds.Length > 0 ? this.Repo.Companies.GetActiveById(companyIds, this.Services.NetworkId) : new List<Company>();
            var events = eventIds.Length > 0 ? this.Repo.Events.GetActiveById(eventIds, this.Services.NetworkId, EventOptions.Place) : new List<Event>();
            var profiles = profileIds.Length > 0 ? this.Repo.People.GetActiveLiteById(profileIds, this.Services.NetworkId) : new List<Sparkle.Entities.Networks.Neutral.Person>();
            var users = userIds.Length > 0 ? this.Repo.People.GetActiveLiteById(userIds) : new List<Sparkle.Entities.Networks.Neutral.Person>();

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var user = users.SingleOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.User = new UserModel(user);
                    item.User.Picture = this.Services.People.GetProfilePictureUrl(user.Username, user.PictureName, Sparkle.Services.Networks.Users.UserProfilePictureSize.Small, UriKind.Relative);
                }

                if (item.CompanyId != null)
                {
                    var match = companies.SingleOrDefault(c => c.ID == item.CompanyId.Value);
                    if (match != null)
                    {
                        item.Company = new CompanyModel(match);
                    }
                    else
                    {
                        list.RemoveAt(i--);
                        continue;
                    }
                }

                if (item.GroupId != null)
                {
                    var match = groups.SingleOrDefault(c => c.Id == item.GroupId.Value);
                    if (match != null)
                    {
                        item.Group = new GroupModel(match);
                    }
                    else
                    {
                        list.RemoveAt(i--);
                        continue;
                    }
                }

                if (item.EventId != null)
                {
                    var match = events.SingleOrDefault(c => c.Id == item.EventId.Value);
                    if (match != null)
                    {
                        item.Event = new EventModel(match);
                    }
                    else
                    {
                        list.RemoveAt(i--);
                        continue;
                    }
                }
                
                if (item.ProfileId != null)
                {
                    var match = profiles.SingleOrDefault(c => c.Id == item.ProfileId.Value);
                    if (match != null)
                    {
                        item.Profile = new UserModel(match);
                    }
                    else
                    {
                        list.RemoveAt(i--);
                        continue;
                    }
                }
            }

            return list;
        }

        private IEnumerable<ActivityModel> GetUsersActivitesQuery(int userId)
        {
            var seekFriends = this.Repo.SeekFriends
                .QuerySeekFriendsByTargetId(userId, SeekFriendOptions.None)
                .Where(o => o.HasAccepted == null)
                .OrderByDescending(o => o.CreateDate)
                .Select(item => new ActivityModel
                {
                    ProfileId = item.SeekerId,
                    Type = ActivityType.Relation,
                    Date = item.CreateDate.Value,
                    Id = item.SeekerId,
                    Displayed = true,
                    IsActivity = false,
                    UserId = item.TargetId,
                })
                .ToList();
            var activities = this.Repo.Activities
                .Select()
                .UserId(userId)
                .OrderByDescending(o => o.Date)
                .Select(item => new ActivityModel
                {
                    Type = (ActivityType)item.Type,
                    Date = item.Date,
                    Id = item.Id,
                    CompanyId = item.CompanyId,
                    Displayed = item.Displayed,
                    EventId = item.EventId,
                    GroupId = item.GroupId,
                    Message = item.Message,
                    ProfileId = item.ProfileID,
                    UserId = item.UserId,
                    IsActivity = true,
                    AdId = item.AdId,
                })
                .ToList();
            var groupInvites = this.Repo.GroupsMembers
                .GetInvitationsSentToUser(userId, this.Services.NetworkId, GroupMemberOptions.None)
                .Select(item => new ActivityModel
                {
                    ProfileId = item.InvitedByUserId,
                    Type = ActivityType.NewGroupInvitation,
                    Date = item.DateInvitedUtc ?? item.DateJoined.Value,
                    Id = item.Id,
                    Displayed = true,
                    IsActivity = false,
                    UserId = item.UserId,
                    GroupId = item.GroupId,
                })
                .ToList();
            var groupInvites1 = this.Repo.GroupsMembers
                .GetInvitationsAcceptedFromUser(userId, this.Services.NetworkId, GroupMemberOptions.None)
                .Select(item => new ActivityModel
                {
                    ProfileId = item.UserId,
                    Type = ActivityType.GroupInvitationAccepted,
                    Date = item.DateAcceptedUtc ?? item.DateJoined ?? item.DateInvitedUtc.Value,
                    Id = item.Id,
                    Displayed = true,
                    IsActivity = false,
                    UserId = item.InvitedByUserId.Value,
                    GroupId = item.GroupId,
                })
                .ToList();

            return activities
                .Union(seekFriends)
                .Union(groupInvites)
                .Union(groupInvites1)
                .OrderByDescending(x => x.Date);
        }

        public ActivityModel Create(int userId, ActivityType type, int? groupId = null, int? profileId = null, int? companyId = null, int? eventId = null, string message = null, int? adId = null)
        {
            var now = DateTime.UtcNow;
            var item = new Activity
            {
                CompanyId = companyId,
                Date = now,
                Displayed = false,
                EventId = eventId,
                GroupId = groupId,
                Message = message,
                ProfileID = profileId,
                Type = (int)type,
                UserId = userId,
                AdId = adId,
            };
            item = this.Repo.Activities.Insert(item);

            return new ActivityModel(item);
        }
    }
}
