
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class TimelineItemTag : IEntityInt32Id, ITagV2Relation
    {
        public TagDeleteReason DeleteReasonValue
        {
            get { return (TagDeleteReason)this.DeleteReason; }
            set { this.DeleteReason = (byte)value; }
        }

        public override string ToString()
        {
            return "TimelineItemTag #" + this.Id + " R:" + this.RelationId + " T:" + this.TagId + " By:" + this.CreatedByUserId;
        }
    }
}
