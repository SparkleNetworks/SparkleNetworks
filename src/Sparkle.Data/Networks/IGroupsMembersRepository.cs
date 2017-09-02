
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IGroupsMembersRepository : IBaseNetworkRepository<GroupMember, int>
    {
        int Count(int networkId);

        IQueryable<GroupMember> CreateQuery(GroupMemberOptions options);
        IList<GroupMember> GetInvitationsSentToUser(int userId, int networkId, GroupMemberOptions options);
        IList<GroupMember> GetInvitationsAcceptedFromUser(int userId, int networkId, GroupMemberOptions options);

        IList<GroupMember> GetMemberships(int groupId, int userId, GroupMemberOptions options);

        GroupMember GetActualMembership(int networkId, int groupId, int userId);
        IList<GroupMember> GetActualMemberships(int networkId);
        IList<GroupMember> GetActualMemberships(int networkId, GroupMemberStatus status);
        IList<GroupMember> GetActualMembershipsByGroup(int networkId, int groupId);
        IList<GroupMember> GetActualMembershipsByUser(int networkId, int userId);
        IList<GroupMember> GetActualMembershipsByGroup(int groupId, GroupMemberStatus status);
        IList<GroupMember> GetActualMembershipsByUser(int userId, GroupMemberStatus status);

        IList<GroupMember> GetById(int[] ids, GroupMemberOptions groupMemberOptions);
        IList<GroupMember> GetById(int[] ids, int networkId, GroupMemberOptions groupMemberOptions);

        IList<GroupMember> GetAllMembersFromGroupWithNotification(int groupId, NotificationFrequencyType notificationFrequencyType, GroupMemberOptions options = GroupMemberOptions.None);

        int CountActiveMembershipsByUser(int userId, int networkId);
    }

    [Repository]
    public interface ITeamsMembersRepository : IBaseNetworkRepository<TeamMember>
    {
        TeamMember Update(TeamMember item);

        void Delete(TeamMember item);
    }

    [Repository]
    public interface IProjectsMembersRepository : IBaseNetworkRepository<ProjectMember, int>
    {
    }
}
