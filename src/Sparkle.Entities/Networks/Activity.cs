
namespace Sparkle.Entities.Networks
{
    using System;

    partial class Activity : IEntityInt32Id
    {
        /*
        [Obsolete]
        public ActityType TypeValue
        {
            get { return (ActityType)this.Type; }
            set { this.Type = (int)value; }
        }
        */
        public override string ToString()
        {
            return this.Id + " " + this.Type + " for " + this.UserId;
        }
    }
/*
    public enum ActityType
    {
        // 
        // WARNING  WARNING  WARNING 
        // 
        // There are 2 enums for activities!
        // This one contains the types stored in the Activities table.
        // Other types exist and are related to other tables.
        // 
        
        Unknown = 0,

        /// <summary>
        /// 1 NewMessage
        /// </summary>
        NewMessage = 1,

        /// <summary>
        /// 2 NewContactRequest
        /// </summary>
        NewContactRequest = 2,

        /// <summary>
        /// 3 ContactRequestAccepted
        /// </summary>
        ContactRequestAccepted = 3,

        /// <summary>
        /// 4 NewCommentOnPublication
        /// </summary>
        NewCommentOnPublication = 4,

        /// <summary>
        /// 5 NewGroupJoinRequest
        /// </summary>
        NewGroupJoinRequest = 5,

        /// <summary>
        /// 6 GroupJoinRequestAccepted
        /// </summary>
        GroupJoinRequestAccepted = 6,

        /// <summary>
        /// 7 NewEventInvitation
        /// </summary>
        NewEventInvitation = 7,

        /// <summary>
        /// 8 NewGroupInvitation
        /// </summary>
        NewGroupInvitation = 8,
        
        /// <summary>
        /// 12 The request you made to join a group was refused.
        /// </summary>
        GroupJoinRequestRefused = 12,
        
        /// <summary>
        /// 13 A user refused the group invitation you sent.
        /// </summary>
        GroupInvitationRefused = 13,

        /// <summary>
        /// 14 A user accepted the group invitation you sent.
        /// </summary>
        GroupInvitationAccepted = 14,
    }
*/
}
