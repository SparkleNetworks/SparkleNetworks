
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ISeekFriendsRepository : IBaseNetworkRepository<SeekFriend>
    {
        IQueryable<SeekFriend> SelectSeekFriendsBySeekerId(IList<string> options, int seekerId);

        IQueryable<SeekFriend> SelectSeekFriendsByTargetId(int targetId, SeekFriendOptions options);

        IQueryable<SeekFriend> CreateQuery(SeekFriendOptions options);
        IQueryable<SeekFriend> QuerySeekFriendsByTargetId(int targetId, SeekFriendOptions options);
        IQueryable<SeekFriend> QuerySeekFriendsBySeekerId(int seekerId, SeekFriendOptions options);

        SeekFriend SelectSeekFriendsByTargetIdAndSeekerId(int targetId, int seekerId);

        void Delete(SeekFriend item);

        SeekFriend Update(SeekFriend item);

        IList<SeekFriend> GetPendingByTargetId(int userId, SeekFriendOptions options);
    }
}