
namespace Sparkle.Entities.Networks
{
    partial class Ad : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Title;
        }
    }

    public enum AdOptions
    {
        None = 0,
        Owner =    0x0001,
        Category = 0x0002,
    }

    partial class AdCategory : IEntityInt32Id,ICommonNetworkEntity
    {
        public AdCategory()
        {
        }

        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    partial class AdTag : IEntityInt32Id, ITagV2Relation
    {
        public TagDeleteReason DeleteReasonValue
        {
            get { return (TagDeleteReason)this.DeleteReason; }
            set { this.DeleteReason = (byte)value; }
        }

        public override string ToString()
        {
            return "AdTag #" + this.Id + " R:" + this.RelationId + " T:" + this.TagId + " By:" + this.CreatedByUserId;
        }
    }
}
