
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GroupMemberModel
    {
        private GroupModel group;

        public GroupMemberModel()
        {
        }

        public GroupMemberModel(Entities.Networks.GroupMember item, Entities.Networks.User user = null, Sparkle.Entities.Networks.Group group = null)
        {
            this.Id = item.Id;
            this.GroupId = item.GroupId;
            
            this.Accepted = item.Accepted;
            this.Status = item.AcceptedStatus;

            this.DateAcceptedUtc = item.DateAcceptedUtc.AsUtc();
            this.DateInvitedUtc = item.DateInvitedUtc.AsUtc();
            this.DateJoinedUtc = item.DateJoined != null
                ? Sparkle.Entities.Networks.DataEntities.DatabaseTimezone.ConvertToUtc(item.DateJoined.Value)
                : default(DateTime?);


            this.Notifications = item.Notifications;
            this.Rights = item.Rights ?? 0;
            this.NotificationFrequency = item.NotificationFrequency;
            this.UserId = item.UserId;
            this.RightStatus = item.RightStatus;
            this.RightStatusString = item.RightStatus.ToString();

            if (item.UserReference != null && item.UserReference.IsLoaded)
            {
                this.Login = item.UserReference.Value.Login;
                this.FirstName = item.UserReference.Value.FirstName;
                this.LastName = item.UserReference.Value.LastName;
            }
            else if (user != null)
            {
                this.Login = user.Login;
                this.FirstName = user.FirstName;
                this.LastName = user.LastName;
                this.PictureName = user.Picture;
            }

            if (item.GroupReference != null && item.GroupReference.IsLoaded)
            {
                this.Group = new GroupModel(item.Group);
            }
            else if (group != null)
            {
                this.Group = new GroupModel(group);
            }
            else
            {
                this.group = new GroupModel(item.GroupId);
            }
        }

        public int Id { get; set; }

        public int GroupId { get; set; }

        public short Accepted { get; set; }

        public int? Notifications { get; set; }

        public byte? NotificationFrequency { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Login { get; set; }

        public string Picture { get; set; }

        public int? Rights { get; set; }

        public Entities.Networks.GroupMemberRight RightStatus { get; set; }

        public string RightStatusString { get; set; }

        public DateTime? DateAcceptedUtc { get; set; }

        public DateTime? DateInvitedUtc { get; set; }

        public DateTime? DateJoinedUtc { get; set; }

        public DateTime DateCreatedUtc
        {
            get { return (this.DateInvitedUtc ?? this.DateAcceptedUtc ?? this.DateJoinedUtc.Value).AsUtc(); }
        }

        public Entities.Networks.GroupMemberStatus Status { get; set; }

        public GroupModel Group
        {
            get { return this.group; }
            set { this.group = value; }
        }

        public string PictureName { get; set; }
    }
}
