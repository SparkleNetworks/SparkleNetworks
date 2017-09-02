
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ConversationModel
    {
        public ConversationModel(Data.Networks.Objects.UsersConversationsGrouping item, Entities.Networks.Neutral.Person myUser, Entities.Networks.Neutral.Person otherUser)
        {
            this.MyUserId = item.MyUserId;
            this.OtherUserId = item.OtherUserId;

            this.MyUser = new UserModel(myUser);
            this.OtherUser = new UserModel(otherUser);

            this.SentDisplayedCount = item.SentDisplayedCount;
            this.SentDisplayedLastDate = item.SentDisplayedLastDate;
            this.SentDisplayedLastId = item.SentDisplayedLastId;

            this.SentUndisplayedCount = item.SentUndisplayedCount;
            this.SentUndisplayedLastDate = item.SentUndisplayedLastDate;
            this.SentUndisplayedLastId = item.SentUndisplayedLastId;

            this.ReceivedDisplayedCount = item.ReceivedDisplayedCount;
            this.ReceivedDisplayedLastDate = item.ReceivedDisplayedLastDate;
            this.ReceivedDisplayedLastId = item.ReceivedDisplayedLastId;

            this.ReceivedUndisplayedCount = item.ReceivedUndisplayedCount;
            this.ReceivedUndisplayedLastDate = item.ReceivedUndisplayedLastDate;
            this.ReceivedUndisplayedLastId = item.ReceivedUndisplayedLastId;
        }

        public ConversationModel(Entities.Networks.Neutral.Person myUser, Entities.Networks.Neutral.Person otherUser)
        {
            this.MyUserId = myUser.Id;
            this.MyUser = new UserModel(myUser);
            this.OtherUserId = otherUser.Id;
            this.OtherUser = new UserModel(otherUser);
        }

        public ConversationModel(Entities.Networks.User myUser, Entities.Networks.User otherUser, IList<Sparkle.Services.Networks.Users.UserProfileFieldModel> fields, IList<Sparkle.Services.Networks.Users.UserProfileFieldModel> otherFields)
        {
            this.MyUserId = myUser.Id;
            this.MyUser = new UserModel(myUser, fields);
            this.OtherUserId = otherUser.Id;
            this.OtherUser = new UserModel(otherUser, otherFields);
        }

        public int MyUserId { get; set; }
        public UserModel MyUser { get; set; }

        public int OtherUserId { get; set; }
        public UserModel OtherUser { get; set; }

        public int? SentDisplayedCount { get; set; }
        public DateTime? SentDisplayedLastDate { get; set; }
        public int? SentDisplayedLastId { get; set; }

        public int? SentUndisplayedCount { get; set; }
        public DateTime? SentUndisplayedLastDate { get; set; }
        public int? SentUndisplayedLastId { get; set; }

        public int? ReceivedDisplayedCount { get; set; }
        public DateTime? ReceivedDisplayedLastDate { get; set; }
        public int? ReceivedDisplayedLastId { get; set; }

        public int? ReceivedUndisplayedCount { get; set; }
        public DateTime? ReceivedUndisplayedLastDate { get; set; }
        public int? ReceivedUndisplayedLastId { get; set; }

        public bool? HasUnreadMessages
        {
            get
            {
                if (this.ReceivedUndisplayedCount != null)
                    return this.ReceivedUndisplayedCount.Value > 0;
                return null;
            }
        }

        public IList<ConversationMessageModel> Messages { get; set; }
    }
}
