
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Networks.Objects;

    [Repository]
    public interface IPrivateMessageRepository : IBaseNetworkRepository<Message, int>
    {
        IList<UsersConversationsGrouping> GetUsersConversations(int userId, int networkId);

        IList<Message> GetMessages(int myUserId, int otherUserId);

        void MarkReadUntil(int myUserId, int otherUserId, int messageId);

        IQueryable<Message> CreateQuery(MessageOptions options);
    }
}