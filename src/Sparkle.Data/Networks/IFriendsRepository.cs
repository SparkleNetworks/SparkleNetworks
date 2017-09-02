
namespace Sparkle.Data.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IFriendsRepository : IBaseNetworkRepository<Contact>
    {
        IQueryable<User> GetContactsByUserId(int id);
        IQueryable<UsersView> AltGetContactsByUserId(int userId);

        Contact SelectFriendByUserIdAndFriendId(int userId, int friendId);
        void DeleteFriends(Contact item);
        Contact InsertFriends(Contact item);
        Contact UpdateFriends(Contact item);

        int[] GetUsersContactIds(int userId);
    }
}