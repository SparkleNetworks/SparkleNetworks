
namespace Sparkle.Entities.Networks
{
    using System;

    public partial class TimelineItemComment : IEntityInt32Id
    {
        public bool IsLikeByCurrentId { get; set; }

        public int LikesCount { get; set; }

        public DateTime? DateReadUtc { get; set; }

        public TimelineItemExtraType ExtraTypeValue
        {
            get { return this.ExtraType.HasValue ? (TimelineItemExtraType)this.ExtraType : TimelineItemExtraType.None; }
            set { this.ExtraType = (int)value; }
        }

        public override string ToString()
        {
            return this.Id + " by " + this.PostedBy + " on " + this.TimelineItemId;
        }
    }

    public enum TimelineItemCommentOptions
    {
        None = 0,
        TimelineItem =            0x0001,
        TimelineItemUser =        0x0002,
        User =                    0x0004,
        PostedBy =                0x0008,
    }
}
