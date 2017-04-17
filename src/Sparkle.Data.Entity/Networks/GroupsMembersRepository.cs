
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using Sparkle.Data.Filters;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class GroupsMembersRepository : BaseNetworkRepositoryInt<GroupMember>, IGroupsMembersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupsMembersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.GroupMembers)
        {
        }

        public int Count(int networkId)
        {
            return this.Set.Where(e => e.Group.NetworkId == networkId).Count();
        }

        public IQueryable<GroupMember> CreateQuery(GroupMemberOptions options)
        {
            ObjectQuery<GroupMember> query = this.Context.GroupMembers;

            if ((options & GroupMemberOptions.User) == GroupMemberOptions.User)
                query = query.Include("User");

            if ((options & GroupMemberOptions.Group) == GroupMemberOptions.Group)
                query = query.Include("Group");

            if ((options & GroupMemberOptions.AcceptedBy) == GroupMemberOptions.AcceptedBy)
                query = query.Include("AcceptedBy");

            if ((options & GroupMemberOptions.InvitedBy) == GroupMemberOptions.InvitedBy)
                query = query.Include("InvitedBy");

            return query;
        }

        public IList<GroupMember> GetInvitationsSentToUser(int userId, int networkId, GroupMemberOptions options)
        {
            var status = (short)GroupMemberStatus.Invited;
            return this.CreateQuery(options)
                .Where(m => m.UserId == userId
                         && m.Group.NetworkId == networkId
                         && m.Accepted == status)
                .OrderByDescending(m => m.DateInvitedUtc)
                .ToList();
        }

        public IList<GroupMember> GetInvitationsAcceptedFromUser(int userId, int networkId, GroupMemberOptions options)
        {
            var status = (short)GroupMemberStatus.Accepted;
            return this.CreateQuery(options)
                .Where(m => m.InvitedByUserId == userId
                         && m.Group.NetworkId == networkId
                         && m.Accepted == status)
                .OrderByDescending(m => m.DateInvitedUtc)
                .ToList();
        }

        public IList<GroupMember> GetMemberships(int groupId, int userId, GroupMemberOptions options)
        {
            return this.CreateQuery(options)
                .Where(m => m.UserId == userId && m.GroupId == groupId)
                .OrderBy(m => m.Id)
                .ToList();
        }

        public GroupMember GetActualMembership(int networkId, int groupId, int userId)
        {
            return this.Context.GetActualGroupMembers(groupId, userId, networkId).SingleOrDefault();
        }

        public IList<GroupMember> GetActualMemberships(int networkId)
        {
            return this.Context.GetActualGroupMembers(null, null, networkId).ToList();
        }

        public IList<GroupMember> GetActualMemberships(int networkId, GroupMemberStatus status)
        {
            return this.Context.GetActualGroupMembers(null, null, networkId).ToList();
        }

        public IList<GroupMember> GetActualMembershipsByGroup(int networkId, int groupId)
        {
            return this.Context.GetActualGroupMembers(groupId, null, networkId).ToList();
        }

        public IList<GroupMember> GetActualMembershipsByUser(int networkId, int userId)
        {
            return this.Context.GetActualGroupMembers(null, userId, networkId).ToList();
        }

        public IList<GroupMember> GetActualMembershipsByGroup(int groupId, GroupMemberStatus status)
        {
            return this.Context.GetActualGroupMembersByStatus(groupId, null, (int)status).ToList();
        }

        public IList<GroupMember> GetActualMembershipsByUser(int userId, GroupMemberStatus status)
        {
            return this.Context.GetActualGroupMembersByStatus(null, userId, (int)status).ToList();
        }

        public IList<GroupMember> GetById(int[] ids, GroupMemberOptions options)
        {
            return this.CreateQuery(options)
                .Where(m => ids.Contains(m.Id))
                .OrderBy(m => m.Group.Name)
                .ToList();
        }

        public IList<GroupMember> GetById(int[] ids, int networkId, GroupMemberOptions options)
        {
            return this.CreateQuery(options)
                .Where(m => ids.Contains(m.Id) && m.Group.NetworkId == networkId)
                .OrderBy(m => m.Group.Name)
                .ToList();
        }

        public IList<GroupMember> GetAllMembersFromGroupWithNotification(int groupId, NotificationFrequencyType notificationFrequencyType, GroupMemberOptions options = GroupMemberOptions.None)
        {
            return this.CreateQuery(options)
                .WithGroupId(groupId)
                .WithNotification(notificationFrequencyType)
                .ToList();
        }

        public int CountActiveMembershipsByUser(int userId, int networkId)
        {
            var status = (short)GroupMemberStatus.Accepted;
            var items = this.Context.GetActualGroupMembers(null, userId, networkId);
            return items.Where(x => x.Accepted == status).Count();
        }
    }

    public class TeamsMembersRepository : BaseNetworkRepository<TeamMember>, ITeamsMembersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TeamsMembersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TeamMembers)
        {
        }

        public new int Insert(TeamMember item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("TeamsMembersRepository.Insert: This cannot be used within a transaction.");

            using (var dc = this.GetNewContext())
            {
                dc.AddToTeamMembers(item);
                dc.SaveChanges();
            }
            return item.TeamId;
        }

        public TeamMember Update(TeamMember item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("TeamsMembersRepository.Update: This cannot be used within a transaction.");

            using (var DC = this.GetNewContext())
            {
                EntityKey key = DC.CreateEntityKey(DC.TeamMembers.EntitySet.Name, item);
                Object OutItem;
                if (DC.TryGetObjectByKey(key, out OutItem))
                {
                    DC.TeamMembers.ApplyCurrentValues(item);
                    DC.SaveChanges();
                }
            }
            return item;
        }

        public void Delete(TeamMember item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("TeamsMembersRepository.Delete: This cannot be used within a transaction.");
            
            using (var DC = this.GetNewContext())
            {
                EntityKey key = DC.CreateEntityKey(DC.TeamMembers.EntitySet.Name, item);
                Object OutItem;
                if (DC.TryGetObjectByKey(key, out OutItem))
                {
                    DC.DeleteObject(OutItem);
                    DC.SaveChanges();
                }
            }
        }
    }
}
