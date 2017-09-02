
namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Diagnostics;
    using Sparkle.Data.Options;

    public class FriendService : ServiceBase, IFriendService
    {
        [DebuggerStepThrough]
        internal FriendService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IFriendsRepository FriendRepository
        {
            get { return this.Repo.Friends; }
        }

        /// <summary>
        /// Selects the friends by user id.
        /// </summary>
        /// <param name="userId">The id.</param>
        /// <returns></returns>
        public IList<User> GetContactsByUserId(int userId)
        {
            return FriendRepository
                .GetContactsByUserId(userId)
                .ActiveAccount()
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        public IList<UsersView> AltGetContactsByUserId(int userId)
        {
            return FriendRepository
                .AltGetContactsByUserId(userId)
                .ActiveAccount()
                .OrderBy(o => o.FirstName)
                .ToList();
        }

        /// <summary>
        /// Checks if both are friends.
        /// </summary>
        /// <param name="firstUserId">The first user id.</param>
        /// <param name="secondUserId">The second user id.</param>
        /// <returns></returns>
        public bool CheckIfBothAreFriends(int firstUserId, int secondUserId)
        {
            return this.GetContactsByUserId(firstUserId).Where(o => o.Id == secondUserId).Count() > 0;
        }

        /// <summary>
        /// Selects the friend by user id and friend id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="contactId">The friend id.</param>
        /// <returns></returns>
        public Contact SelectFriendByUserIdAndFriendId(int userId, int contactId)
        {
            return this.FriendRepository.SelectFriendByUserIdAndFriendId(userId, contactId);
        }

        /// <summary>
        /// Gets the contacts count.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public int GetContactsCount(int userId)
        {
            return FriendRepository
                .GetContactsByUserId(userId)
                .Count();
        }

        /// <summary>
        /// Gets the coworkers count.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        public int GetCoworkersCount(int userId, int companyId)
        {
            return FriendRepository
                .GetContactsByUserId(userId)
                .WithCompanyId(companyId)
                .Count();
        }

        /// <summary>
        /// Selects the friends coworkers.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        public IList<User> SelectFriendsCoworkers(int userId, int companyId)
        {
            return FriendRepository
                .GetContactsByUserId(userId)
                .WithCompanyId(companyId)
                .ToList();
        }

        /// <summary>
        /// Deletes the friends.
        /// </summary>
        /// <param name="item">The item.</param>
        public void DeleteFriends(Contact item)
        {
            FriendRepository.DeleteFriends(item);
        }

        /// <summary>
        /// Inserts the friends.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int InsertFriends(Contact item)
        {
            return FriendRepository.InsertFriends(item).UserId;
        }

        /// <summary>
        /// Updates the friends.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Contact UpdateFriends(Contact item)
        {
            return FriendRepository.UpdateFriends(item);
        }

        public int Count()
        {
            return this.Repo.Friends
                .Select()
                .Where(i => i.User.NetworkId == this.Services.NetworkId)
                .Count();
        }

        public int GetActiveContactsCountExcept(int userId, int[] friendIds)
        {
            return this.Repo.Friends
                .AltGetContactsByUserId(userId)
                .ActiveAccount()
                .Where(o => !friendIds.Contains(o.Id))
                .Count();
        }
    }
}