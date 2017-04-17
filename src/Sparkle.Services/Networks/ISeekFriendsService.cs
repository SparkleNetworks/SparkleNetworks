
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface ISeekFriendsService
    {
        int CheckIfContactRequest(int FirstUserId, int SecondUserId);
        void Delete(SeekFriend item);
        SeekFriend Insert(SeekFriend item);
        IList<SeekFriend> SelectSeekFriendsBySeekerId(int seekerId);
        IList<SeekFriend> SelectSeekFriendsByTargetId(int targetId, SeekFriendOptions options = SeekFriendOptions.None);
        SeekFriend SelectSeekFriendsByTargetIdAndSeekerId(int TargetId, int SeekerId);
        IList<SeekFriend> SelectSeekFriendsRefusedByTargetId(int targetId, SeekFriendOptions options = SeekFriendOptions.None);
        SeekFriend Update(SeekFriend item);
        IList<SeekFriend> SelectAllSeekFriendsRefused();

        IList<SeekFriend> SelectAllSeekFriendsRelativeToId(int userId);

        int CountPendingRequests(int userId);

        IList<SeekFriendModel> GetPendingRequests(int userId);
    }
}
