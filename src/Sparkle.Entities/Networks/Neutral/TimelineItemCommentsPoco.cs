
namespace Sparkle.Entities.Networks.Neutral
{
    public partial class TimelineItemCommentPoco : IEntityInt32Id
    {
        public bool IsLikeByCurrentId { get; set; }

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
}
