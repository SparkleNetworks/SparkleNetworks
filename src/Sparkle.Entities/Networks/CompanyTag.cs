
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CompanyTag : IEntityInt32Id, ITagV2Relation
    {
        public TagDeleteReason DeleteReasonValue
        {
            get { return (TagDeleteReason)this.DeleteReason; }
            set { this.DeleteReason = (byte)value; }
        }

        public override string ToString()
        {
            return "CompanyTag #" + this.Id + " R:" + this.CompanyId + " T:" + this.TagId + " By:" + this.CreatedByUserId;
        }

        public int RelationId
        {
            get { return this.CompanyId; }
            set { this.CompanyId = value; }
        }
    }

    public enum TagDeleteReason : byte
    {
        None = 0,
        AuthorDelete = 1,
    }
}
