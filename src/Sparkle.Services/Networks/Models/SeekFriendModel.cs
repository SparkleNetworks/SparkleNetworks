
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class SeekFriendModel
    {
        private static readonly TimeZoneInfo databaseTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

        public SeekFriendModel()
        {
        }

        public SeekFriendModel(Sparkle.Entities.Networks.SeekFriend item, UserModel seeker, UserModel target)
        {
            this.Set(item);
            this.Seeker = seeker ?? new UserModel(item.SeekerId);
            this.Target = target ?? new UserModel(item.TargetId);
        }

        private void Set(Entities.Networks.SeekFriend item)
        {
            this.DateCreatedUtc = databaseTimeZone.ConvertToUtc(item.CreateDate ?? databaseTimeZone.ConvertFromUtc(DateTime.UtcNow));
            this.DateExpiresUtc = databaseTimeZone.ConvertToUtc(item.ExpirationDate ?? databaseTimeZone.ConvertFromUtc(DateTime.UtcNow));
            this.HasAccepted = item.HasAccepted;
            this.SeekerId = item.SeekerId;
            this.TargetId = item.TargetId;
        }

        [DataMember]
        public int SeekerId { get; set; }

        [DataMember]
        public int TargetId { get; set; }

        [DataMember]
        public DateTime DateCreatedUtc { get; set; }

        [DataMember]
        public DateTime DateExpiresUtc { get; set; }

        [DataMember]
        public bool? HasAccepted { get; set; }

        [DataMember]
        public bool IsPendingAccept
        {
            get { return this.HasAccepted == null; }
        }

        [DataMember]
        public UserModel Seeker { get; set; }

        [DataMember]
        public UserModel Target { get; set; }
    }
}
