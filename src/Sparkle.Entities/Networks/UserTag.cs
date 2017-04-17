
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class UserTag : IEntityInt32Id, ITagV2Relation
    {
        public TagDeleteReason DeleteReasonValue
        {
            get { return (TagDeleteReason)this.DeleteReason; }
            set { this.DeleteReason = (byte)value; }
        }

        public int RelationId
        {
            get { return this.UserId; }
            set { this.UserId = value; }
        }

        public override string ToString()
        {
            return "UserTag #" + this.Id + " R:" + this.UserId + " T:" + this.TagId + " By:" + this.CreatedByUserId;
        }
    }
}
