
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class EventMemberFilter
    {
        public static IQueryable<EventMember> WithEventId(this IQueryable<EventMember> query, int EventId)
        {
            return query.Where(o => o.EventId == EventId);
        }

        public static IQueryable<EventMember> WithUserId(this IQueryable<EventMember> query, int UserId)
        {
            return query.Where(o => o.UserId == UserId);
        }

        public static IQueryable<EventMember> Participation(this IQueryable<EventMember> query, int UserId)
        {
            return query.Where(o => o.UserId == UserId && o.State == (int)EventMemberState.HasAccepted);
        }

        public static IQueryable<EventMember> WithEventIdAndUserId(this IQueryable<EventMember> query, int EventId, int UserId)
        {
            return query.Where(o => o.EventId == EventId && o.UserId == UserId);
        }            
    }
}