
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Groups;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks.Neutral;

    public class GroupsMembersService : ServiceBase, IGroupsMembersService
    {
        [DebuggerStepThrough]
        internal GroupsMembersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IGroupsMembersRepository GroupsMembersServiceRepository
        {
            get { return this.Repo.GroupsMembers; }
        }

        /// <summary>
        /// Adds the group member.
        /// </summary>
        /// <param name="me">Me</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public GroupMember AddGroupMember(User me, GroupMember item)
        {
            return this.AddGroupMemberImpl(me, item, false);
        }

        /// <summary>
        /// Adds the group member.
        /// </summary>
        /// <param name="me">Me</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public GroupMember AddFirstGroupMember(User me, GroupMember item)
        {
            return this.AddGroupMemberImpl(me, item, true);
        }

        public JoinGroupResult Join(int groupId, int userId)
        {
            const string path = "Join";
            var result = new JoinGroupResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(JoinGroupError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(JoinGroupError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(JoinGroupError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            if (membership != null)
            {
                // joining a group when the user already has some kind of status related to it
                // conditions apply
                result.MembershipId = membership.Id;
                switch (membership.AcceptedStatus)
                {
                    case GroupMemberStatus.Invited:
                    case GroupMemberStatus.None:
                    case GroupMemberStatus.Kicked:       // right now we allow this but we may change
                    case GroupMemberStatus.JoinRejected: // right now we allow this but we may change
                    case GroupMemberStatus.InvitationRejected:
                    case GroupMemberStatus.Left:
                        membership = null;
                        break;

                    case GroupMemberStatus.Accepted:
                        result.Errors.Add(JoinGroupError.AlreadyAcceptedMember, NetworksEnumMessages.ResourceManager);
                        this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is member of group " + group);
                        break;

                    default:
                        this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                        result.Errors.Add(JoinGroupError.UnexpectedMembership, NetworksEnumMessages.ResourceManager);
                        break;
                }
            }
            else
            {
                // joining a group for the first time (not invited, not kicked, not left, not anything)
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            if (membership == null)
            {
                membership = new GroupMember
                {
                    UserId = user.Id,
                    GroupId = group.Id,
                    DateInvitedUtc = DateTime.UtcNow,
                };
            }

            if (!group.IsPrivate)
            {
                membership.AcceptedStatus = GroupMemberStatus.Accepted;
                membership.DateAcceptedUtc = DateTime.UtcNow;
                membership.DateJoined = DateTime.Now;
                
                if (membership.Id == 0)
                    this.Repo.GroupsMembers.Insert(membership);
                else
                    this.Repo.GroupsMembers.Update(membership);

                result.IsJoined = true;
                result.Succeed = true;

                var wallItem = this.PostJoinInTimeline(groupId, userId);
                result.NewTimelineItemId = wallItem.Id;

                this.Services.Logger.Info(path, ErrorLevel.Success, "handled " + user + " in " + group + "; now group member");
            }
            else
            {
                membership.AcceptedStatus = GroupMemberStatus.JoinRequest;
                if (membership.Id == 0)
                    this.Repo.GroupsMembers.Insert(membership);
                else
                    this.Repo.GroupsMembers.Update(membership);


                result.IsPendingGroupValidation = true;
                result.Succeed = true;

                this.Services.Logger.Info(path, ErrorLevel.Success, "handled " + user + " in " + group + "; now in joinrequest");

                this.NotifyAdminsOfNewJoinRequest(groupId, userId, group);
            }

            return result;
        }

        private GroupMember AddGroupMemberImpl(User me, GroupMember item, bool memberCreatedGroup)
        {
            // this method requires a refactor because it has multiple roles

            GroupMember member;

            if (me == null || me.NetworkId != this.Services.NetworkId && !me.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff))
            {
                throw new ResultErrorsException(new BasicResultError(NetworksEnumMessages.CannotActOnDifferentNetwork));
            }

            if (item.Id > 0 && this.GroupsMembersServiceRepository.GetById(item.Id) != null)
            {
                member = this.GroupsMembersServiceRepository.Update(item);
            }
            else
            {
                member = this.GroupsMembersServiceRepository.Insert(item);
            }

            User user = this.Services.People.GetEntityByIdFromAnyNetwork(item.UserId, PersonOptions.None);

            if (member.Accepted == (short)GroupMemberStatus.Accepted)
            {
                this.PostJoinInTimeline(member.GroupId, user.Id, memberCreatedGroup);
            }

            if (member.Accepted == (short)GroupMemberStatus.Invited)
            {
                // Si l'utilisateur accepte les notifications par email
                Notification notifications = this.Services.Notifications.SelectNotifications(member.UserId);
                if (notifications.EventInvitation)
                {
                    User contact = this.Services.People.SelectWithId(member.UserId);
                    if (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(contact))
                    {
                        Group group = this.Services.Groups.SelectGroupById(member.GroupId);
                        this.Services.Email.SendNotification(me, contact, group);
                    }
                }
            }

            return member;
        }

        public long Insert(GroupMember item)
        {
            return this.GroupsMembersServiceRepository.Insert(item).Id;
        }

        public GroupMember Update(GroupMember item)
        {
            return this.GroupsMembersServiceRepository.Update(item);
        }

        /// <summary>
        /// Determines whether [is group member] [the specified user id].
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>
        ///   <c>true</c> if [is group member] [the specified user id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGroupMember(int userId, int groupId)
        {
            var member = this.GetMembership(groupId, userId);
            if (member != null && member.AcceptedStatus == GroupMemberStatus.Accepted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is group member invited] [the specified user id].
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>
        ///   <c>true</c> if [is group member invited] [the specified user id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGroupMemberInvited(int userId, int groupId)
        {
            var member = this.GetMembership(groupId, userId);
            if (member != null && member.AcceptedStatus == GroupMemberStatus.Invited)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is group member request] [the specified user id].
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="groupId">The group id.</param>
        /// <returns>
        ///   <c>true</c> if [is group member request] [the specified user id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsGroupMemberRequest(int userId, int groupId)
        {
            var member = this.GetMembership(groupId, userId);
            if (member != null && member.AcceptedStatus == GroupMemberStatus.JoinRequest)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Selects the group members.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns></returns>
        public IList<GroupMember> SelectGroupMembers(int groupId, GroupMemberStatus status = GroupMemberStatus.Accepted)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, status);
            return items;
            ////var users = this.Repo.People.GetLiteById(items.Select(i => i.UserId).ToArray(), this.Services.NetworkId);
            ////return this.GroupsMembersServiceRepository.Select(OptionsList)
            ////               .WithGroupId(groupId)
            ////               .Where(gm => status == GroupMemberStatus.None | gm.Accepted == (int)status)
            ////               .Where(o => o.User.CompanyAccessLevel > 0 && o.User.NetworkAccessLevel > 0).ToList();
        }

        public IList<GroupMember> SelectAllMembersFromGroup(int groupId)
        {
            return this.Repo
                .GroupsMembers
                .Select()
                .WithGroupId(groupId)
                .ToList();
        }

        /// <summary>
        /// Selects the admins group members.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns></returns>
        public IList<GroupMember> SelectAdminsGroupMembers(int groupId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.Accepted);
            return items.Where(m => m.RightStatus == GroupMemberRight.Admin).ToList();
            ////return this.GroupsMembersServiceRepository.Select(OptionsList)
            ////    .WithGroupId(groupId)
            ////    .IsAdmin()
            ////    .ToList();
        }

        public IList<GroupMember> SelectGroupRequestMembers(int groupId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.JoinRequest);
            var userIds = items.Select(u => u.UserId).ToArray();
            var users = this.Services.People.KeepActiveUserIds(userIds);
            return items.Where(i => users.Contains(i.UserId)).ToList();
            ////return this.GroupsMembersServiceRepository.Select(OptionsList)
            ////    .WithGroupId(groupId)
            ////    .IsRequest()
            ////    .ToList();
        }

        /// <summary>
        /// Selects my groups for groups service.
        /// </summary>
        /// <param name="userId">The people id.</param>
        /// <returns></returns>
        public IList<GroupMember> SelectMyGroupsForGroupsService(int userId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByUser(this.Services.NetworkId, userId);
            var ids = items.Where(m => m.AcceptedStatus == GroupMemberStatus.Accepted).Select(m => m.Id).ToArray();
            return this.Repo.GroupsMembers.GetById(ids, this.Services.NetworkId, GroupMemberOptions.Group);
        }

        public bool IsAdmin(int userId, int groupId)
        {
            var member = this.GetMembership(groupId, userId);
            if (member == null || !this.IsAdmin(member))
                return false;
            else
                return true;
        }

        private bool IsAdmin(GroupMember member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            return member.AcceptedStatus == GroupMemberStatus.Accepted && member.RightStatus == GroupMemberRight.Admin;
        }

        public int Count()
        {
            var items = this.Repo.GroupsMembers.GetActualMemberships(this.Services.NetworkId, GroupMemberStatus.Accepted);
            return items.Count;
        }

        public IList<GroupMember> SelectInvited(int groupId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.Invited);
            var ids = items/*.Where(m => m.AcceptedStatus == GroupMemberStatus.Invited)*/.Select(m => m.Id).ToArray();
            return this.Repo.GroupsMembers.GetById(ids, this.Services.NetworkId, GroupMemberOptions.User);
            ////return this.GroupsMembersServiceRepository.Select(OptionsList)
            ////    .WithGroupId(groupId)
            ////    .IsInvitation()
            ////    .ToList();
        }

        public int CountGroupRequestMembers(int groupId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.JoinRequest);
            return items.Count;
        }

        public IDictionary<int, GroupMemberModel> GetUsersGroupMemberships(int userId, GroupMemberOptions options)
        {
            return this.GetUsersGroupMemberships(userId, null, options);
        }

        public IDictionary<int, GroupMemberModel> GetUsersGroupMemberships(int userId, int[] groupIds, GroupMemberOptions options)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByUser(this.Services.NetworkId, userId);
            var collection = items
                .Where(i => groupIds == null || groupIds.Contains(i.GroupId))
                .ToDictionary(m => m.GroupId, m => new GroupMemberModel(m));

            if ((options & GroupMemberOptions.Group) == GroupMemberOptions.Group)
            {
                var groups = this.Repo.Groups.GetById(items.Select(i => i.GroupId).ToArray(), this.Services.NetworkId);
                foreach (var group in groups)
                {
                    collection[group.Id].Group = new GroupModel(group);
                }
            }

            return collection;
        }

        public IList<GroupModel> GetUsersGroups(int userId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByUser(userId, GroupMemberStatus.Accepted);
            var groups = this.Repo.Groups.GetById(items.Select(m => m.GroupId).ToArray(), this.Services.NetworkId);
            return groups
                .Select(g => new GroupModel(g, items[g.Id]))
                .OrderBy(g => g.Name)
                .ToList();
        }

        public IList<Group> GetUsersGroupEntities(int userId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByUser(userId, GroupMemberStatus.Accepted);
            var groups = this.Repo.Groups.GetById(items.Select(m => m.GroupId).ToArray(), this.Services.NetworkId);
            return groups;
        }

        public AcceptGroupInvitationResult AcceptInvitation(int groupId, int userId, bool notifyInviter)
        {
            const string path = "AcceptInvitation";
            var result = new AcceptGroupInvitationResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(AcceptGroupInvitationError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(AcceptGroupInvitationError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(AcceptGroupInvitationError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            if (membership == null)
            {
                result.Errors.Add(AcceptGroupInvitationError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not invited to group " + group);
                return result;
            }

            result.MembershipId = membership.Id;
            switch (membership.AcceptedStatus)
            {
                case GroupMemberStatus.Invited:
                    break;

                case GroupMemberStatus.Accepted:
                    result.Errors.Add(AcceptGroupInvitationError.AlreadyAcceptedMember, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is member of group " + group);
                    break;

                case GroupMemberStatus.Kicked:
                    result.Errors.Add(AcceptGroupInvitationError.KickedFromGroup, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is kicked from group " + group);
                    break;

                case GroupMemberStatus.JoinRejected:
                    result.Errors.Add(AcceptGroupInvitationError.KickedFromGroup, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is joinrejected from group " + group);
                    break;

                case GroupMemberStatus.None:
                case GroupMemberStatus.JoinRequest:
                default:
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                    result.Errors.Add(AcceptGroupInvitationError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                    break;
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }
            
            User inviter = null;
            GroupMember inviterMemberhsip = null;
            if (membership.InvitedByUserId != null)
            {
                inviter = this.Repo.People.GetActiveById(membership.InvitedByUserId.Value, Data.Options.PersonOptions.None);
                if (inviter != null)
                {
                    inviterMemberhsip = this.GetMembership(groupId, inviter.Id);
                }
            }

            bool actingUserIsGroupAdmin = inviterMemberhsip != null && this.IsAdmin(inviterMemberhsip);
            
            if (!group.IsPrivate || actingUserIsGroupAdmin)
            {
                membership.AcceptedStatus = GroupMemberStatus.Accepted;
                membership.DateAcceptedUtc = DateTime.UtcNow;
                membership.DateJoined = DateTime.Now;
                this.Repo.GroupsMembers.Update(membership);

                result.IsJoined = true;
                result.Succeed = true;

                var wallItem = this.PostJoinInTimeline(groupId, userId);
                result.NewTimelineItemId = wallItem.Id;
                this.Services.Logger.Info(path, ErrorLevel.Success, "handled " + user + " in " + group + "; now group member");
            }
            else
            {
                membership.AcceptedStatus = GroupMemberStatus.JoinRequest;
                this.Repo.GroupsMembers.Update(membership);
                this.Services.Logger.Info(path, ErrorLevel.Success, "handled " + user + " in " + group + "; converted in joinrequest");

                result.IsPendingGroupValidation = true;
                result.Succeed = true;

                this.NotifyAdminsOfNewJoinRequest(groupId, userId, group);
            }

            if (notifyInviter && inviter != null)
            {
                result.IsInviterNotified = true;
                this.Services.Activities.Create(inviter.Id, ActivityType.GroupInvitationAccepted, groupId: groupId, profileId: userId);
            }

            return result;
        }

        private void NotifyAdminsOfNewJoinRequest(int groupId, int userId, Group group)
        {
            var admins = this.Services.GroupsMembers.SelectAdminsGroupMembers(groupId);
            foreach (var admin in admins)
            {
                if (admin.UserId == userId)
                    continue;

                int nb = admin.Notifications ?? 0;
                GroupMember updated = admin;
                updated.Notifications = ++nb;

                this.Repo.GroupsMembers.Update(updated);

                this.Services.Activities.Create(admin.UserId, ActivityType.NewGroupJoinRequest, groupId: groupId, profileId: userId);

                var notifications = this.Services.Notifications.SelectNotifications(admin.User);
                if (notifications.PrivateGroupJoinRequest && (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(admin.User)))
                {
                    byte frequencyId = updated.NotificationFrequency ?? group.NotificationFrequency;
                    NotificationFrequencyType frequency = (NotificationFrequencyType)frequencyId;

                    if (frequency == NotificationFrequencyType.None || frequency == NotificationFrequencyType.Immediate)
                    {
                        this.Services.Email.SendPrivateGroupJoinRequest(admin.User, group, userId);
                    }
                }
            }
        }

        private TimelineItem PostJoinInTimeline(int groupId, int userId, bool memberCreatedGroup = false)
        {
            var extra = memberCreatedGroup ? "created group" : null;
            var user = this.Services.People.GetEntityByIdFromAnyNetwork(userId, PersonOptions.None);
            var item = this.Services.Wall.Publish(user, null, TimelineItemType.JoinedGroup, string.Empty, TimelineType.Group, groupId, extra: extra);

            return item;
        }

        public AcceptGroupInvitationResult RejectInvitation(int groupId, int userId, bool notifyInviter)
        {
            const string path = "RejectInvitation";
            var result = new AcceptGroupInvitationResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(AcceptGroupInvitationError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(AcceptGroupInvitationError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(AcceptGroupInvitationError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            if (membership == null)
            {
                result.Errors.Add(AcceptGroupInvitationError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not invited to group " + group);
                return result;
            }

            result.MembershipId = membership.Id;
            switch (membership.AcceptedStatus)
            {
                case GroupMemberStatus.Invited:
                    break;

                case GroupMemberStatus.Accepted:
                    result.Errors.Add(AcceptGroupInvitationError.AlreadyAcceptedMember, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is member of group " + group);
                    break;

                case GroupMemberStatus.Kicked:
                    result.Errors.Add(AcceptGroupInvitationError.KickedFromGroup, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is kicked from group " + group);
                    break;

                case GroupMemberStatus.JoinRejected:
                    result.Errors.Add(AcceptGroupInvitationError.KickedFromGroup, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is joinrejected from group " + group);
                    break;

                case GroupMemberStatus.None:
                case GroupMemberStatus.JoinRequest:
                default:
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                    result.Errors.Add(AcceptGroupInvitationError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                    break;
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }
            
            User inviter = null;
            GroupMember inviterMemberhsip = null;
            if (membership.InvitedByUserId != null)
            {
                inviter = this.Repo.People.GetActiveById(membership.InvitedByUserId.Value, Data.Options.PersonOptions.None);
                if (inviter != null)
                {
                    inviterMemberhsip = this.GetMembership(groupId, inviter.Id);
                }
            }

            bool actingUserIsGroupAdmin = inviterMemberhsip != null && this.IsAdmin(inviterMemberhsip);

            membership.AcceptedStatus = GroupMemberStatus.InvitationRejected;
            membership.DateAcceptedUtc = DateTime.UtcNow;
            membership.DateJoined = null;
            this.Repo.GroupsMembers.Update(membership);

            result.Succeed = true;
            this.Services.Logger.Info(path, ErrorLevel.Input, "handled " + user + " in " + group + "; now InvitationRejected");

            if (notifyInviter && inviter != null)
            {
                result.IsInviterNotified = true;
                this.Services.Activities.Create(inviter.Id, ActivityType.GroupInvitationRefused, groupId: groupId, profileId: userId);
            }

            return result;
        }

        public GroupMember GetMembership(int groupId, int userId)
        {
            ////var memberships = this.Repo.GroupsMembers.GetMemberships(groupId, userId, GroupMemberOptions.None);
            ////return memberships.LastOrDefault();
            var membership = this.Repo.GroupsMembers.GetActualMembership(this.Services.NetworkId, groupId, userId);
            return membership;
        }

        public GroupMemberModel GetMembershipModel(int groupId, int userId)
        {
            var item = this.GetMembership(groupId, userId);
            if (item != null)
            {
                return new GroupMemberModel(item);
            }
            else
            {
                return null;
            }
        }

        public InviteToGroupResult InviteUserToGroup(int groupId, int userId, int actingUserId)
        {
            var result = new InviteToGroupResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(InviteToGroupError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(InviteToGroupError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(InviteToGroupError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            var actingUser = this.Repo.People.GetActiveById(actingUserId, Data.Options.PersonOptions.None);
            if (actingUser == null || !this.Services.People.IsActive(actingUser))
            {
                result.Errors.Add(InviteToGroupError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var actingMembership = this.GetMembership(groupId, actingUser.Id);
            if (actingMembership == null)
            {
                result.Errors.Add(InviteToGroupError.ActingUserIsNotGroupMember, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (actingMembership.AcceptedStatus != GroupMemberStatus.Accepted)
            {
                result.Errors.Add(InviteToGroupError.ActingUserIsNotGroupMember, NetworksEnumMessages.ResourceManager);
                return result;
            }

            bool actingUserIsGroupAdmin = actingMembership != null
                && actingMembership.AcceptedStatus == GroupMemberStatus.Accepted
                && actingMembership.RightStatus == GroupMemberRight.Admin;
            var forceInviteAgainAccess = new NetworkAccessLevel[] { NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.SparkleStaff, };
            bool actingUserIsNetworkAdmin = actingUser.NetworkAccess.HasAnyFlag(forceInviteAgainAccess);

            var membership = this.GetMembership(groupId, userId);
            if (membership != null)
            {
                switch (membership.AcceptedStatus)
                {
                    case GroupMemberStatus.JoinRequest:
                        result.Errors.Add(InviteToGroupError.AlreadyJoinRequest, NetworksEnumMessages.ResourceManager);
                        break;

                    case GroupMemberStatus.Invited:
                        result.Errors.Add(InviteToGroupError.AlreadyInvited, NetworksEnumMessages.ResourceManager);
                        break;

                    case GroupMemberStatus.Accepted:
                        result.Errors.Add(InviteToGroupError.AlreadyMember, NetworksEnumMessages.ResourceManager);
                        break;

                    case GroupMemberStatus.Kicked:
                        result.Errors.Add(InviteToGroupError.AlreadyRejected, NetworksEnumMessages.ResourceManager);
                        break;

                    case GroupMemberStatus.None:
                    case GroupMemberStatus.JoinRejected:
                        break;

                    case GroupMemberStatus.Left:
                        if (!actingUserIsNetworkAdmin)
                        {
                            result.Errors.Add(InviteToGroupError.Left, NetworksEnumMessages.ResourceManager);
                        }
                        break;

                    default:
                        result.Errors.Add(InviteToGroupError.UnexpectedMembershipStatis, NetworksEnumMessages.ResourceManager);
                        break;
                }
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            // create membership
            var groupMember = new GroupMember
            {
                GroupId = group.Id,
                UserId = user.Id,
                AcceptedStatus = GroupMemberStatus.Invited,
                DateInvitedUtc = DateTime.UtcNow,
                InvitedByUserId = actingUserId,
            };
            this.Repo.GroupsMembers.Insert(groupMember);

            // notify user
            var notifications = this.Services.Notifications.SelectNotifications(user.Id);
            if (notifications.EventInvitation && (!!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(user)))
            {
                this.Services.Email.SendNotification(actingUser, user, group);
            }

            result.Succeed = true;
            return result;
        }

        public LeaveGroupResult Leave(int groupId, int userId)
        {
            const string path = "Leave";
            var result = new LeaveGroupResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(LeaveGroupError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(LeaveGroupError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(LeaveGroupError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            if (membership == null)
            {
                result.Errors.Add(LeaveGroupError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group);
                return result;
            }

            result.MembershipId = membership.Id;
            switch (membership.AcceptedStatus)
            {
                case GroupMemberStatus.Accepted:
                    // cool
                    break;

                case GroupMemberStatus.Invited:
                case GroupMemberStatus.Kicked:
                case GroupMemberStatus.Left:
                case GroupMemberStatus.JoinRejected:
                case GroupMemberStatus.JoinRequest:
                    result.Errors.Add(LeaveGroupError.NotGroupMember, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group + " " + membership.AcceptedStatus);
                    break;

                case GroupMemberStatus.None:
                default:
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                    result.Errors.Add(LeaveGroupError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                    break;
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            // if the member is admin, there should be other admins in the group
            bool actingUserIsGroupAdmin = this.IsAdmin(membership);
            if (actingUserIsGroupAdmin)
            {
                var admins = this.SelectAdminsGroupMembers(group.Id);
                if (admins.Count == 1)
                {
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is the last admin");
                    result.Errors.Add(LeaveGroupError.LastAdmin, NetworksEnumMessages.ResourceManager);
                    return result;
                }
            }

            membership.AcceptedStatus = GroupMemberStatus.Left;
            this.Repo.GroupsMembers.Update(membership);

            result.Succeed = true;
            this.Services.Logger.Info(path, ErrorLevel.Input, "handled " + user + " in " + group + "; now Left");

            return result;
        }

        public UpdateNotificationTypeResult UpdateNotificationType(int groupId, int userId, NotificationFrequencyType type)
        {
            const string path = "UpdateNotificationType";
            var result = new UpdateNotificationTypeResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(UpdateNotificationError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(UpdateNotificationError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(UpdateNotificationError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            if (membership == null)
            {
                result.Errors.Add(UpdateNotificationError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group);
                return result;
            }

            result.MembershipId = membership.Id;
            switch (membership.AcceptedStatus)
            {
                case GroupMemberStatus.Accepted:
                    // cool
                    break;

                case GroupMemberStatus.Invited:
                case GroupMemberStatus.Kicked:
                case GroupMemberStatus.Left:
                case GroupMemberStatus.JoinRejected:
                case GroupMemberStatus.JoinRequest:
                    result.Errors.Add(UpdateNotificationError.NotGroupMember, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group + " " + membership.AcceptedStatus);
                    break;

                case GroupMemberStatus.None:
                default:
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                    result.Errors.Add(UpdateNotificationError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                    break;
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            membership.NotificationFrequency = (byte)type;
            this.Repo.GroupsMembers.Update(membership);

            result.Succeed = true;
            this.Services.Logger.Info(path, ErrorLevel.Input, "handled " + user + " in " + group + "; now " + type);

            return result;
        }

        public GroupKickResult Kick(int groupId, int userId, int actingUserId, bool notify)
        {
            const string path = "Kick";
            var result = new GroupKickResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(GroupKickError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(GroupKickError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(GroupKickError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            var actingUser = this.Repo.People.GetActiveById(actingUserId, Data.Options.PersonOptions.None);
            if (actingUser == null || !this.Services.People.IsActive(actingUser))
            {
                result.Errors.Add(GroupKickError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            var actingMembership = this.GetMembership(groupId, actingUserId);

            if (membership == null || actingMembership == null)
            {
                result.Errors.Add(GroupKickError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group);
                return result;
            }

            if (!this.IsAdmin(actingMembership))
            {
                result.Errors.Add(GroupKickError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because acting user is not admin of group " + group);
                return result;
            }

            result.MembershipId = membership.Id;
            switch (membership.AcceptedStatus)
            {
                case GroupMemberStatus.Accepted:
                    // cool
                    break;

                case GroupMemberStatus.Invited:
                case GroupMemberStatus.Kicked:
                case GroupMemberStatus.Left:
                case GroupMemberStatus.JoinRejected:
                case GroupMemberStatus.JoinRequest:
                    result.Errors.Add(GroupKickError.NotGroupMember, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group + " " + membership.AcceptedStatus);
                    break;

                case GroupMemberStatus.None:
                default:
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                    result.Errors.Add(GroupKickError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                    break;
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            membership.AcceptedStatus = GroupMemberStatus.Kicked;
            membership = this.Repo.GroupsMembers.Update(membership);
            result.Succeed = true;
            result.Membership = membership;

            if (notify)
            {
                result.IsUserNotified = true;
                this.Services.Activities.Create(userId, ActivityType.KickedFromGroup, groupId: groupId);
            }

            return result;
        }

        public ChangeGroupRightResult ChangeRights(int groupId, int userId, int actingUserId, GroupMemberRight right, bool notify)
        {
            const string path = "ChangeRights";
            var result = new ChangeGroupRightResult();

            var group = this.Services.Groups.SelectGroupById(groupId);
            if (group == null)
            {
                result.Errors.Add(ChangeGroupRightError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
            }

            if (group.IsDeleted)
            {
                result.Errors.Add(ChangeGroupRightError.DeletedGroup, NetworksEnumMessages.ResourceManager);
            }

            var user = this.Repo.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(ChangeGroupRightError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            var actingUser = this.Repo.People.GetActiveById(actingUserId, Data.Options.PersonOptions.Company);
            if (actingUser == null || !this.Services.People.IsActive(actingUser))
            {
                result.Errors.Add(ChangeGroupRightError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var membership = this.GetMembership(groupId, userId);
            var actingMembership = this.GetMembership(groupId, actingUserId);

            if (membership == null || actingMembership == null)
            {
                result.Errors.Add(ChangeGroupRightError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group);
                return result;
            }

            if (!this.IsAdmin(actingMembership))
            {
                result.Errors.Add(ChangeGroupRightError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because acting user is not admin of group " + group);
                return result;
            }

            result.MembershipId = membership.Id;
            switch (membership.AcceptedStatus)
            {
                case GroupMemberStatus.Accepted:
                    // cool
                    break;

                case GroupMemberStatus.Invited:
                case GroupMemberStatus.Kicked:
                case GroupMemberStatus.Left:
                case GroupMemberStatus.JoinRejected:
                case GroupMemberStatus.JoinRequest:
                    result.Errors.Add(ChangeGroupRightError.NotGroupMember, NetworksEnumMessages.ResourceManager);
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because is not member of group " + group + " " + membership.AcceptedStatus);
                    break;

                case GroupMemberStatus.None:
                default:
                    this.Services.Logger.Warning(path, ErrorLevel.Input, "Cannot handle " + user + " because of unexpected membership status " + membership.AcceptedStatus + " in " + group);
                    result.Errors.Add(ChangeGroupRightError.NoSuchMembership, NetworksEnumMessages.ResourceManager);
                    break;
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            membership.RightStatus = right;
            membership = this.Repo.GroupsMembers.Update(membership);
            result.Succeed = true;
            result.Membership = new GroupMemberModel(membership);

            if (notify)
            {
                result.IsUserNotified = true;
                this.Services.Activities.Create(userId, ActivityType.GroupRightsChanged, groupId: groupId, message: right.ToString());
            }

            return result;
        }

        public IList<GroupMember> GetSubscribedToImmediateNotifications(int groupId)
        {
            var items = this.Repo
                .GroupsMembers
                .GetAllMembersFromGroupWithNotification(groupId, NotificationFrequencyType.Immediate, GroupMemberOptions.User);

            if (this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced)
            {
                var userIds = items.Select(x => x.UserId).ToArray();
                var subs = this.Services.Subscriptions.GetUserStatus(userIds);
                var remove = items.Where(i => !subs[i.UserId]).ToArray();
                foreach (var item in remove)
                {
                    items.Remove(item);
                }
            }

            return items;
        }

        public IList<GroupMemberModel> GetActiveGroupMembers(int groupId)
        {
            return this.GetActiveGroupMembers(groupId, false);
        }

        public IList<GroupMemberModel> GetActiveGroupMembers(int groupId, bool includeUnsubscribed)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.Accepted);
            var userIds = items.Select(u => u.UserId).ToArray();
            var users = this.Repo.People.GetActiveById(userIds, this.Services.NetworkId, PersonOptions.None);

            if (this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced && !includeUnsubscribed)
            {
                var subs = this.Services.Subscriptions.GetUserStatus(userIds);
                foreach (var id in users.Keys.ToArray())
                {
                    if (!subs[id])
                    {
                        users.Remove(id);
                    }
                }
            }
            
            return items
                .Where(i => users.ContainsKey(i.UserId))
                .Select(i => new GroupMemberModel(i, users[i.UserId]))
                .ToList();
        }

        public IList<GroupMemberModel> GetActiveGroupRequests(int groupId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.JoinRequest);
            var userIds = items.Select(u => u.UserId).ToArray();
            var users = this.Repo.People.GetActiveById(userIds, this.Services.NetworkId, PersonOptions.Company);

            if (this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced)
            {
                var subs = this.Services.Subscriptions.GetUserStatus(userIds);
                foreach (var id in users.Keys.ToArray())
                {
                    if (!subs[id])
                    {
                        users.Remove(id);
                    }
                }
            }
            
            return items
                .Where(i => users.ContainsKey(i.UserId))
                .Select(i => new GroupMemberModel(i, users[i.UserId]))
                .ToList();
        }

        public IList<GroupMemberModel> GetActiveGroupInvites(int groupId)
        {
            var items = this.Repo.GroupsMembers.GetActualMembershipsByGroup(groupId, GroupMemberStatus.JoinRequest);
            var userIds = items.Select(u => u.UserId).ToArray();
            var users = this.Repo.People.GetActiveById(userIds, this.Services.NetworkId, PersonOptions.None);

            if (this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced)
            {
                var subs = this.Services.Subscriptions.GetUserStatus(userIds);
                foreach (var id in users.Keys.ToArray())
                {
                    if (!subs[id])
                    {
                        users.Remove(id);
                    }
                }
            }
            
            return items
                .Where(i => users.ContainsKey(i.UserId))
                .Select(i => new GroupMemberModel(i, users[i.UserId]))
                .ToList();
        }

        public GroupMemberModel GetMembershipById(int id)
        {
            var item = this.Repo.GroupsMembers.GetById(id);
            if (item == null)
                return null;
            return new GroupMemberModel(item);
        }

        public int CountActiveMembershipsByUser(int userId)
        {
            return this.GroupsMembersServiceRepository.CountActiveMembershipsByUser(userId, this.Services.NetworkId);
        }
    }
}
