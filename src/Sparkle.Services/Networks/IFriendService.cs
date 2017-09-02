
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IFriendService
    {
        IList<string> OptionsList { get; set; }
        bool CheckIfBothAreFriends(int FirstUserId, int SecondUserId);
        void DeleteFriends(Contact item);
        int GetContactsCount(int userId);
        int GetCoworkersCount(int userId, int companyId);
        int InsertFriends(Contact item);
        Contact SelectFriendByUserIdAndFriendId(int userId, int contactId);
        IList<User> GetContactsByUserId(int userId);
        IList<User> SelectFriendsCoworkers(int userId, int companyId);
        Contact UpdateFriends(Contact item);

        int Count();

        IList<UsersView> AltGetContactsByUserId(int userId);

        int GetActiveContactsCountExcept(int userId, int[] friendIds);
    }
}
