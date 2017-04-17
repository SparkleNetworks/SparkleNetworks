
namespace Sparkle.Entities.Networks
{
    public partial class TimelineItem : IEntityInt32Id, INetworkEntity
    {
        public bool IsLikeByCurrentId { get; set; }

        public int LikesCount { get; set; }

        public string CommentsCount { get; set; }

        public TimelineItemType TimelineItemType
        {
            get { return (TimelineItemType)this.ItemType; }
            set { this.ItemType = (int)value; }
        }

        public TimelineItemScope TimelineItemScope
        {
            get { return (TimelineItemScope)this.PrivateMode; }
            set { this.PrivateMode = (int)value; }
        }

        public TimelineItemExtraType ExtraTypeValue
        {
            get { return this.ExtraType.HasValue ? (TimelineItemExtraType)this.ExtraType : TimelineItemExtraType.None; }
            set { this.ExtraType = (int)value; }
        }

        public WallItemDeleteReason? DeleteReasonValue
        {
            get { return this.DeleteReason.HasValue ? (WallItemDeleteReason)this.DeleteReason.Value : default(WallItemDeleteReason?); }
            set { this.DeleteReason = (byte)value; }
        }

        public override string ToString()
        {
            return this.Id + " by " + this.PostedBy + " " + (this.PrivateMode == 1 ? "private" : this.PrivateMode == 0 ? "public" : "?");
        }
    }

    public enum TimelineItemExtraType : int
    {
        /// <summary>
        /// 0: No extra type.
        /// </summary>
        None = 0,

        /// <summary>
        /// 1: JSON opengraph data.
        /// </summary>
        OpenGraphDescription = 1,

        /// <summary>
        /// 2: May contain
        /// a deal as JSON if ItemType=16
        /// or a picture URL if ItemType=0
        /// </summary>
        DealOrPicture = 2,

        /// <summary>
        /// Contains a JSON TimelineSocialEntry object.
        /// </summary>
        TimelineSocialEntry = 3,

        GoogleGroupImportedMessage = 4,
    }

    public enum WallItemDeleteReason : byte
    {
        None               = 0,
        AuthorDelete       = 1,
        LinkedEntityDelete = 2,

        Nudity             = 10,
        SexuallySuggestive = 11,

        HateSpeech         = 20,
        Violence           = 21,
        DrugOrAlcoholAbuse = 22,

        Spam               = 30,

        OffTopic           = 40,

        Other              = 50,

        ShouldntHaveBeenImported = 60,
        WrongImportContext =       61,
    }

    public enum TimelineDisplayContext
    {
        Public = 0,
        Private = 1,
        Profile = 2,
        Company = 3,
        Event = 4,
        Ad = 5,
        Group = 6,
        Place = 7,
        CompanyNetwork = 8,
        Project = 9,
        Team = 10,
        CompaniesNews = 11,
        ExternalCompany = 12,
        ExternalCompanies = 13,
        PeopleNews = 14,
        Topic = 15,
        Search = 16,
    }
}
