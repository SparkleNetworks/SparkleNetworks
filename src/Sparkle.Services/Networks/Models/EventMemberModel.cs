
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EventMemberModel
    {
        public EventMemberModel()
        {
        }

        public EventMemberModel(int eventId, int userId, Entities.Networks.EventMemberState state)
        {
            this.EventId = eventId;
            this.UserId = userId;
            this.State = state;
        }

        public EventMemberModel(Sparkle.Entities.Networks.EventPublicMember item, EventModel model)
        {
            this.Event = model;
            this.Set(item);
        }

        public EventMemberModel(Sparkle.Entities.Networks.EventMember item, EventModel model, UserModel user)
        {
            this.Event = model;
            this.Set(item);
            this.User = user;
        }

        public EventMemberModel(Entities.Networks.Event item, Entities.Networks.EventMember member, IList<Sparkle.Services.Networks.Users.UserProfileFieldModel> fields)
        {
            this.EventId = item.Id;
            this.UserId = member.UserId;

            this.Comment = member.Comment;
            ////this.Notifications = member.Notifications;
            this.Rights = member.RightsValue;
            this.State = member.StateValue;

            if (member.UserReference.IsLoaded)
            {
                this.User = new UserModel(member.User, fields);
            }
            else
            {
                this.User = new UserModel(member.UserId);
            }

            if (item != null)
            {
                this.Event = new EventModel(item);
            }
            else if (member.EventReference.IsLoaded)
            {
                this.Event = new EventModel(member.Event);
            }
            else
            {
                this.Event = new EventModel(member.EventId);
            }
        }

        private void Set(Sparkle.Entities.Networks.EventMember item)
        {
            this.EventId = item.EventId;
            this.UserId = item.UserId;
            this.Comment = item.Comment;
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateUpdatedUtc = item.DateUpdatedUtc.AsUtc();
            this.Notifications = item.Notifications;
            this.Rights = item.Rights != null ? (Entities.Networks.EventMemberRight)item.Rights.Value : Entities.Networks.EventMemberRight.None;
            this.State = item.StateValue;
        }

        private void Set(Sparkle.Entities.Networks.EventPublicMember item)
        {
            this.PublicUser = new PublicUserModel(item);
            this.EventId = item.EventId;
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateUpdatedUtc = item.DateUpdatedUtc.AsUtc();
            this.State = item.StateValue;
        }

        public int EventId { get; set; }

        public int? UserId { get; set; }

        public UserModel User { get; set; }

        public EventModel Event { get; set; }

        public string Comment { get; set; }

        public Entities.Networks.EventMemberRight Rights { get; set; }

        public Entities.Networks.EventMemberState State { get; set; }

        public PublicUserModel PublicUser { get; set; }

        public DateTime? DateCreatedUtc { get; set; }

        public DateTime? DateUpdatedUtc { get; set; }

        public int? Notifications { get; set; }
    }
}
