
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class GroupMember : IEntityInt32Id
    {
        public NotificationFrequencyType? NotificationFrequencyValue
        {
            get { return this.NotificationFrequency != null ? (NotificationFrequencyType)this.NotificationFrequency : default(NotificationFrequencyType?); }
            set { this.NotificationFrequency = value != null ? (byte)value : default(byte?); }
        }

        public GroupMemberStatus AcceptedStatus
        {
            get { return (GroupMemberStatus)this.Accepted; }
            set { this.Accepted = (short)value; }
        }

        public GroupMemberRight RightStatus
        {
            get { return this.Rights != null ? (GroupMemberRight)this.Rights : GroupMemberRight.Basic; }
            set { this.Rights = (short)value; }
        }

        public override string ToString()
        {
            return this.Id + " User:" + this.UserId + " in group:" + this.GroupId + " Rights:" + this.Rights + " Accepted:" + this.Accepted;
        }
    }

    public enum GroupMemberStatus : short
    {
        /// <summary>
        /// (0) This status is not in use.
        /// </summary>
        None = 0,

        /// <summary>
        /// (1) The current user ask to join the group.
        /// </summary>
        JoinRequest = 1,

        /// <summary>
        /// (2) Invited by another person.
        /// </summary>
        Invited = 2,

        /// <summary>
        /// (3) Accepted : is member.
        /// </summary>
        Accepted = 3,

        /// <summary>
        /// (-1) Join request denied
        /// </summary>
        JoinRejected = -1,

        /// <summary>
        /// (-2) Kicked
        /// </summary>
        Kicked = -2,

        /// <summary>
        /// (-3) Invitation rejected
        /// </summary>
        InvitationRejected = -3,

        /// <summary>
        /// (-4) Left the group
        /// </summary>
        Left = -4,
    }

    public enum GroupMemberRight : int
    {
        /// <summary>
        /// (0) Group member
        /// </summary>
        Basic = 0,

        /// <summary>
        /// (1) Group administrator
        /// </summary>
        Admin = 1,
    }

    public enum GroupMemberOptions : int
    {
        None = 0,
        User       = 0x00001,
        Group      = 0x00002,
        AcceptedBy = 0x00004,
        InvitedBy  = 0x00008,
    }
}
