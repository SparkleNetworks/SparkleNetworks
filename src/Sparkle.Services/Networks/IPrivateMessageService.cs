
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Services.Networks.PrivateMessages;

    public interface IPrivateMessageService
    {
        IList<string> OptionsList { get; set; }
        IList<Message> SelectConversation(int userId, int contactId);
        IList<Message> SelectConversationsFromUserId(int userId);
        IList<Message> SelectFromUserId(int userId);
        IList<Message> SelectSendedAndReceivedMessags(int userId);
        Message SelectLastReceivedMessage(int userId);
        long Insert(Message item);
        Message Update(Message item);

        IList<Message> GetUnread(int userId);
        IList<Message> GetUnreadAndMarkAsRead(int userId);
        IList<Message> GetFromId(int userId, int messageId);

        int Count();
        int CountToday();
        int CountLast24Hours();

        int CountUnread(int userId);

        void MarkAsReadByUserId(int fromUserId, int toUserId);

        IList<ConversationModel> GetConversationsFor(int userId);

        ConversationModel GetConversation(int userId, int otherUserId, bool markMessagesRead);

        ////[Obsolete("Use the overload")]
        ////void Send(Message message);

        SendPrivateMessageResult Send(SendPrivateMessageRequest request);

        Message GetPrivateMessageById(int timelineItemId);
    }
}
