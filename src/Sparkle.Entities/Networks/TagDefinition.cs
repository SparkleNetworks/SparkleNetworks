
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class TagDefinition : IEntityInt32Id, INetworkEntity
    {
        public TagType CategoryValue
        {
            get { return (TagType)this.CategoryId; }
            set { this.CategoryId = (int)value; }
        }

        public override string ToString()
        {
            return "Tag " + this.Id + " of type " + this.CategoryValue.ToString() + " named ''" + this.Name + "'";
        }
    }

    public enum TagType
    {
        Unknown         = 0,
        Skill           = 1,
        Interest        = 2,
        Recreation      = 3,
        City            = 4,
        PartnerResource = 5,
    }
}
