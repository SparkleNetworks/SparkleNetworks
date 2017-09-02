
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class TagDefinitionPoco
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
}
