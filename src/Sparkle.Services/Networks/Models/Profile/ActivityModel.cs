
namespace Sparkle.Services.Networks.Models.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ActivityModel
    {
        public ActivityModel()
        {
        }

        public ActivityModel(Sparkle.Entities.Networks.Activity item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Set(item);
        }

        public ActivityType Type { get; set; }

        public DateTime Date { get; set; }

        public int Id { get; set; }

        public int? CompanyId { get; set; }

        public bool Displayed { get; set; }

        public int? EventId { get; set; }

        public int? GroupId { get; set; }

        public string Message { get; set; }

        public int? ProfileId { get; set; }

        public int UserId { get; set; }

        public UserModel User { get; set; }

        public CompanyModel Company { get; set; }

        public GroupModel Group { get; set; }

        public EventModel Event { get; set; }

        public UserModel Profile { get; set; }

        public bool IsActivity { get; set; }

        public int? AdId { get; set; }

        public override string ToString()
        {
            return this.Id + " " + this.Type + " " + this.Date;
        }

        private void Set(Entities.Networks.Activity item)
        {
            var romanceToUtcDate = new DateTime(2016, 3, 1);
            this.Type = (ActivityType)item.Type;
            this.Date = item.Date > romanceToUtcDate ? item.Date.AsUtc() : item.Date.AsLocal().ToUniversalTime();
            this.Id = item.Id;
            this.CompanyId = item.CompanyId;
            this.Displayed = item.Displayed;
            this.EventId = item.EventId;
            this.GroupId = item.GroupId;
            this.Message = item.Message;
            this.ProfileId = item.ProfileID;
            this.UserId = item.UserId;
            this.IsActivity = true;
            this.AdId = item.AdId;
        }
    }

    public enum ActivityType
    {
        // 
        // WARNING  WARNING  WARNING 
        // 
        // There are 2 enums for activities!
        // This one contains all types stored in various tables.
        // 

        Undefined = 0,

        /// <summary>
        /// 1 NewMessage
        /// </summary>
        Message = 1,

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
        NewComment = 4,

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
        EventInvitation = 7,

        /// <summary>
        /// 8 NewGroupInvitation
        /// </summary>
        NewGroupInvitation = 8,

        /// <summary>
        /// 9 timeline item like
        /// </summary>
        TimelineItemLike = 9,

        /// <summary>
        /// 10 timeline item comment like
        /// </summary>
        TimelineItemCommentLike = 10,

        /// <summary>
        /// 11 group publication
        /// </summary>
        GroupPublication = 11,

        /// <summary>
        /// 12 The request you made to join a group was refused.
        /// </summary>
        GroupJoinRequestRefused = 12,

        /// <summary>
        /// 13 A user accepted the group invitation you sent.
        /// </summary>
        GroupInvitationAccepted = 13,

        /// <summary>
        /// 14 A user refused the group invitation you sent.
        /// </summary>
        GroupInvitationRefused = 14,

        /// <summary>
        /// 15 kicked from group
        /// </summary>
        KickedFromGroup = 15,

        /// <summary>
        /// 16 The group rights changed
        /// </summary>
        GroupRightsChanged = 16,

        /// <summary>
        /// 17 A user you invited has joined
        /// </summary>
        InvitedUserHasJoined = 17,

        /// <summary>
        /// 18 A pending user is waiting to join the company
        /// </summary>
        PendingCompanyApplyRequest = 18,

        /// <summary>
        /// 19 An ad pending validation has been validated.
        /// </summary>
        AdCreateValidated = 19,

        /// <summary>
        /// 20 An ad pending edit validation has been validated.
        /// </summary>
        AdEditValidated = 20,

        /// <summary>
        /// 19 An ad pending validation has been validated.
        /// </summary>
        AdCreateRefused = 21,

        /// <summary>
        /// 20 An ad pending edit validation has been validated.
        /// </summary>
        AdEditRefused = 22,

        /// <summary>
        /// 181 The (next) new subscription has been activated
        /// </summary>
        NewSubscriptionActivated = 181,
        
        Relation = short.MaxValue + 1, // contact request from seekfriends
    }
}
