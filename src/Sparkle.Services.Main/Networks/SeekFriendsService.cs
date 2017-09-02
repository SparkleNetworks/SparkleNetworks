
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;

    public class SeekFriendsService : ServiceBase, ISeekFriendsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekFriendsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal SeekFriendsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the seek friend repository.
        /// </summary>
        protected ISeekFriendsRepository SeekFriendRepository
        {
            get { return this.Repo.SeekFriends; }
        }

        /// <summary>
        /// Selects the seek friends by seeker id.
        /// </summary>
        /// <param name="seekerId">The seeker id.</param>
        /// <returns></returns>
        public IList<SeekFriend> SelectSeekFriendsBySeekerId(int seekerId)
        {
            return this.SeekFriendRepository
                .SelectSeekFriendsBySeekerId(OptionsList, seekerId)
                .Where(o => o.HasAccepted == null)
                .ToList();
        }

        /// <summary>
        /// Checks if contact request.
        /// </summary>
        /// <param name="FirstUserId">The first user id.</param>
        /// <param name="SecondUserId">The second user id.</param>
        /// <returns></returns>
        public int CheckIfContactRequest(int FirstUserId, int SecondUserId)
        {
            if (this.SelectSeekFriendsBySeekerId(FirstUserId).Where(o => o.TargetId == SecondUserId).Count() > 0)
            {
                return 1;
            }
            if (this.SelectSeekFriendsBySeekerId(SecondUserId).Where(o => o.TargetId == FirstUserId).Count() > 0)
            {
                return 2;
            }
            return 0;
        }

        /// <summary>
        /// Selects the seek friends by target id and seeker id.
        /// </summary>
        /// <param name="TargetId">The target id.</param>
        /// <param name="SeekerId">The seeker id.</param>
        /// <returns></returns>
        public SeekFriend SelectSeekFriendsByTargetIdAndSeekerId(int TargetId, int SeekerId)
        {
            return this.SeekFriendRepository.SelectSeekFriendsByTargetIdAndSeekerId(TargetId, SeekerId);
        }

        /// <summary>
        /// Selects the seek friends refused by target id.
        /// </summary>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public IList<SeekFriend> SelectSeekFriendsRefusedByTargetId(int targetId, SeekFriendOptions options = SeekFriendOptions.None)
        {
            return this.SeekFriendRepository
                .SelectSeekFriendsByTargetId(targetId, options)
                .Where(o => o.HasAccepted == false)
                .ToList();
        }

        public IList<SeekFriend> SelectSeekFriendsByTargetId(int targetId, SeekFriendOptions options = SeekFriendOptions.None)
        {
            return this.SeekFriendRepository
                .SelectSeekFriendsByTargetId(targetId, options)
                .Where(o => o.HasAccepted == null)
                .ToList();
        }

        /// <summary>
        /// Selects all the refused seek friends.
        /// </summary>
        /// <returns></returns>
        public IList<SeekFriend> SelectAllSeekFriendsRefused()
        {
            return this.SeekFriendRepository
                .Select()
                .Where(o => o.HasAccepted == false)
                .ToList();
        }

        /// <summary>
        /// Inserts the seek friend.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public SeekFriend Insert(SeekFriend item)
        {
            if (this.Services.SeekFriends.SelectSeekFriendsByTargetIdAndSeekerId(item.TargetId, item.SeekerId) != null)
                return null;

            User person = this.Services.People.SelectWithId(item.TargetId);
            if (!this.Services.People.IsActive(person))
                throw new InvalidOperationException("The user " + person.Id + " is not active.");
            
            this.SeekFriendRepository.Insert(item);

            Notification notify = this.Services.Notifications.SelectNotifications(person);
            if (notify.ContactRequest && (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(person)))
            {
                this.Services.Email.SendContactRequest(item.SeekerId, person);
            }

            return item;
        }

        /// <summary>
        /// Deletes the seek friend.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(SeekFriend item)
        {
            this.SeekFriendRepository.Delete(item);
        }

        /// <summary>
        /// Updates the seek friend.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public SeekFriend Update(SeekFriend item)
        {
            return this.SeekFriendRepository.Update(item);
        }

        public int CountPendingRequests(int userId)
        {
            return this.SeekFriendRepository.Select()
                .Where(r => r.TargetId == userId && r.HasAccepted == null)
                .Count();
        }

        public IList<SeekFriend> SelectAllSeekFriendsRelativeToId(int userId)
        {
            return this.SeekFriendRepository.Select()
                .Where(o => o.SeekerId == userId || o.TargetId == userId)
                .ToList();
        }

        public IList<SeekFriendModel> GetPendingRequests(int userId)
        {
            var items = this.Repo.SeekFriends.GetPendingByTargetId(userId, SeekFriendOptions.None);
            var userIds = items.GroupBy(s => s.SeekerId).Select(g => g.Key).ToArray();
            var users = this.Services.Cache.GetUsers(userIds);
            var models = new List<SeekFriendModel>(items.Count);
            models.AddRange(items
                .Where(s => users[s.SeekerId].IsActive)
                .Select(s => new SeekFriendModel(s, users[s.SeekerId], null)));
            return models;
        }
    }
}
