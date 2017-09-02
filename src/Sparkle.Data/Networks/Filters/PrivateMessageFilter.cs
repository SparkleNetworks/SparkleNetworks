
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class PrivateMessageFilter
    {
        public static IQueryable<Message> WithId(this IQueryable<Message> qry, int id)
        {
            return qry.Where(o => o.FromUserId == id);
        }

        public static IQueryable<Message> From(this IQueryable<Message> qry, int id)
        {
            return qry.Where(o => o.FromUserId == id);
        }

        public static IQueryable<Message> To(this IQueryable<Message> qry, int id)
        {
            return qry.Where(o => o.ToUserId == id);
        }

        public static IQueryable<Message> Unread(this IQueryable<Message> qry)
        {
            return qry.Where(o => o.Displayed == false);
        }

        public static IQueryable<Message> WithMessageId(this IQueryable<Message> qry, int messageId)
        {
            return qry.Where(o => o.Id == messageId);
        }

        public static IQueryable<Message> FromId(this IQueryable<Message> qry, int messageId)
        {
            return qry.Where(o => o.Id > messageId);
        }

        public static IQueryable<Message> Between(this IQueryable<Message> qry, int userId1, int userId2)
        {
            return qry.Where(o => (o.FromUserId == userId1 && o.ToUserId == userId2) || (o.FromUserId == userId2 && o.ToUserId == userId1));
        }

        public static IQueryable<Message> FromOrTo(
        this IQueryable<Message> qry, int guid)
        {
            return qry.Where(o => o.FromUserId == guid || o.ToUserId == guid);
        }

        public static IEnumerable<Message> Conversations(this IQueryable<Message> qry, int guid)
        {

            List<Message> conversations = qry.Where(o => o.FromUserId == guid || o.ToUserId == guid).ToList();
            IEnumerable<Message> convDistinct = conversations.Distinct(new MessagesComparer());
            return convDistinct;
        }
    }

    public class MessagesComparer : IEqualityComparer<Message>
    {
        public bool Equals(Message x, Message y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.FromUserId == y.FromUserId || x.ToUserId == y.ToUserId || x.FromUserId == y.ToUserId || x.ToUserId == y.FromUserId;
        }

        public int GetHashCode(Message message)
        {
            if (Object.ReferenceEquals(message, null)) return 0;
            int hashProductName = message.Subject == null ? 0 : message.Subject.GetHashCode();
            int hashProductCode = message.Id.GetHashCode();
            return hashProductName ^ hashProductCode;
        }
    }
}