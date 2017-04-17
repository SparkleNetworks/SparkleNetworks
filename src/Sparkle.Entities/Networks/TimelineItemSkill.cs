
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class TimelineItemSkill : IEntityInt32Id
    {
        public WallItemDeleteReason? DeleteReasonValue
        {
            get { return this.DeleteReason != null ? (WallItemDeleteReason)this.DeleteReason : default(WallItemDeleteReason?); }
            set { this.DeleteReason = value != null ? (byte)value : default(byte?); }
        }

        public override string ToString()
        {
            return this.Id + " Skill:" + this.SkillId + " TimelineItem:" + this.TimelineItemId;
        }
    }

    public enum TimelineItemSkillOptions
    {
        None = 0x0000,
        Skill = 0x0001,
        TimelineItem = 0x0002,
    }   
}
