
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Groups;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;

    public interface IGroupsMembersService
    {
        [Obsolete("There should be domain methods to get a list with eager loading")]
        IList<string> OptionsList { get; set; }

        GroupMember AddGroupMember(User me, GroupMember item);
        GroupMember AddFirstGroupMember(User user, GroupMember member);

        /// <summary>
        /// Inserts the specified item in the context of an import process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        long Insert(GroupMember item);

        [Obsolete("There should be domain methods to update a membership")]
        GroupMember Update(GroupMember item);

        ////void DeleteGroupMember(GroupMember item); // don't ever do that!

        bool IsGroupMember(int userId, int groupId);
        bool IsGroupMemberInvited(int userId, int groupId);
        bool IsGroupMemberRequest(int userId, int groupId);
        bool IsAdmin(int userId, int groupId);

        IList<GroupMember> SelectAdminsGroupMembers(int groupId);
        IList<GroupMember> SelectGroupMembers(int groupId, GroupMemberStatus status = GroupMemberStatus.Accepted);
        IList<GroupMember> SelectGroupRequestMembers(int groupId);
        IList<GroupMember> SelectInvited(int groupId);
        IList<GroupMember> SelectAllMembersFromGroup(int groupId);
        IList<GroupMember> GetSubscribedToImmediateNotifications(int groupId);

        IList<GroupMember> SelectMyGroupsForGroupsService(int userId);
        IDictionary<int, GroupMemberModel> GetUsersGroupMemberships(int userId, GroupMemberOptions options);
        IDictionary<int, GroupMemberModel> GetUsersGroupMemberships(int userId, int[] groupIds, GroupMemberOptions options);
        IList<GroupModel> GetUsersGroups(int userId);
        IList<Group> GetUsersGroupEntities(int userId);

        GroupMember GetMembership(int groupId, int userId);
        GroupMemberModel GetMembershipModel(int groupId, int userId);

        int Count();
        int CountGroupRequestMembers(int groupId);

        AcceptGroupInvitationResult AcceptInvitation(int groupId, int userId, bool notifyInviter);
        AcceptGroupInvitationResult RejectInvitation(int groupId, int userId, bool notifyInviter);

        InviteToGroupResult InviteUserToGroup(int groupId, int userId, int actingUserId);

        LeaveGroupResult Leave(int groupId, int userId);

        UpdateNotificationTypeResult UpdateNotificationType(int groupId, int userId, NotificationFrequencyType type);

        GroupKickResult Kick(int groupId, int userId, int actingUserId, bool notify);

        ChangeGroupRightResult ChangeRights(int groupId, int userId, int actingUserId, GroupMemberRight right, bool notify);

        JoinGroupResult Join(int groupId, int userId);

        IList<GroupMemberModel> GetActiveGroupMembers(int groupId);
        IList<GroupMemberModel> GetActiveGroupMembers(int groupId, bool includeUnsubscribed);

        IList<GroupMemberModel> GetActiveGroupRequests(int groupId);

        IList<GroupMemberModel> GetActiveGroupInvites(int groupId);

        GroupMemberModel GetMembershipById(int id);

        int CountActiveMembershipsByUser(int userId);
    }
}
