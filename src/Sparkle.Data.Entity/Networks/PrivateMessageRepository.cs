
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Networks.Objects;
    using System.Diagnostics;
    using System.Data.Objects;


    public class PrivateMessageRepository : BaseNetworkRepository<Message, int>, IPrivateMessageRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PrivateMessageRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory,  m => m.Messages)
        {
        }

        protected override Message GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(Message item)
        {
            return item.Id;
        }

        public IList<UsersConversationsGrouping> GetUsersConversations(int userId, int networkId)
        {
            var list = this.Context.GetUsersConversations(userId, networkId);
            var result = new List<UsersConversationsGrouping>();

            foreach (var item in list)
            {
                int otherId = 0;
                if (item.FromUserId != userId)
                {
                    // messages I received
                    otherId = item.FromUserId;
                }
                else if (item.ToUserId != userId)
                {
                    // messages I sent
                    otherId = item.ToUserId;
                }
                else
                {
                    // messages with myself
                    otherId = userId;
                }

                if (otherId != 0)
                {
                    var match = result.SingleOrDefault(r => r.OtherUserId == otherId);
                    if (match == null)
                    {
                        match = new UsersConversationsGrouping
                        {
                            MyUserId = userId,
                            OtherUserId = otherId,
                        };
                        result.Add(match);
                    }

                    if (item.FromUserId == userId && item.ToUserId != userId)
                    {
                        // messages I sent
                        if (item.Displayed)
                        {
                            match.SentDisplayedCount = item.Count;
                            match.SentDisplayedLastDate = item.CreateDate;
                            match.SentDisplayedLastId = item.MaxId;
                        }
                        else
                        {
                            match.SentUndisplayedCount = item.Count;
                            match.SentUndisplayedLastDate = item.CreateDate;
                            match.SentUndisplayedLastId = item.MaxId;
                        }
                    }

                    if (item.FromUserId != userId && item.ToUserId == userId)
                    {
                        // messages I received
                        if (item.Displayed)
                        {
                            match.ReceivedDisplayedCount = item.Count;
                            match.ReceivedDisplayedLastDate = item.CreateDate;
                            match.ReceivedDisplayedLastId = item.MaxId;
                        }
                        else
                        {
                            match.ReceivedUndisplayedCount = item.Count;
                            match.ReceivedUndisplayedLastDate = item.CreateDate;
                            match.ReceivedUndisplayedLastId = item.MaxId;
                        }
                    }
                }
            }

            return result;
        }

        public IList<Message> GetMessages(int myUserId, int otherUserId)
        {
            return this.Set
                .Where(m => (m.FromUserId == myUserId && m.ToUserId == otherUserId)
                         || (m.FromUserId == otherUserId && m.ToUserId == myUserId))
                .OrderBy(m => m.CreateDate)
                .ToList();
        }

        public void MarkReadUntil(int myUserId, int otherUserId, int messageId)
        {
            this.Context.MarkMessagesReadUntil(myUserId, otherUserId, messageId);
        }

        public IQueryable<Message> CreateQuery(MessageOptions options)
        {
            ObjectQuery<Message> query = this.Set;

            if ((options & MessageOptions.From) == MessageOptions.From)
                query = query.Include("From");
            if ((options & MessageOptions.To) == MessageOptions.To)
                query = query.Include("To");

            return query;
        }
    }
}