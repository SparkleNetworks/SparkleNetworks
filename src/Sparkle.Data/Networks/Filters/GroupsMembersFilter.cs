
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class GroupsMembersFilter
    {
        public static IQueryable<GroupMember> WithGroupId(this IQueryable<GroupMember> query, int groupId)
        {
            return query.Where(o => o.GroupId == groupId);
        }

        public static IQueryable<GroupMember> IsAdmin(this IQueryable<GroupMember> query)
        {
            return query.Where(o => o.Rights == 1);
        }

        public static IQueryable<GroupMember> WithUserId(this IQueryable<GroupMember> query, int userId)
        {
            return query.Where(o => o.UserId == userId);
        }

        public static IQueryable<GroupMember> Accepted(this IQueryable<GroupMember> query)
        {
            return query.Where(o => o.Accepted == 3);
        }
        public static IQueryable<GroupMember> NotAccepted(this IQueryable<GroupMember> query)
        {
            return query.Where(o => o.Accepted != 3);
        }

        public static IQueryable<GroupMember> IsRequest(this IQueryable<GroupMember> query)
        {
            return query.Where(o => o.Accepted == 1);
        }

        public static IQueryable<GroupMember> IsInvitation(this IQueryable<GroupMember> query)
        {
            return query.Where(o => o.Accepted == 2);
        }

        public static IQueryable<GroupMember> WithNotification(this IQueryable<GroupMember> query, NotificationFrequencyType frequence)
        {
            return query.Where(o => o.NotificationFrequency == (byte)frequence);
        }
    }

    public static class TeamsMembersFilter
    {
        public static IQueryable<TeamMember> WithTeamId(this IQueryable<TeamMember> query, int TeamId)
        {
            return query.Where(o => o.TeamId == TeamId);
        }

        public static IQueryable<TeamMember> WithUserId(this IQueryable<TeamMember> query, int peopleId)
        {
            return query.Where(o => o.UserId == peopleId);
        }
    }
}
