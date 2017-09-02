
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class TimelineItemPoco
    {
        public TimelineType TimelineItemType
        {
            get { return (TimelineType)this.ItemType; }
            set { this.ItemType = (int)value; }
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
}
