
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class TagCategory : IEntityInt32Id
    {
        public override string ToString()
        {
            return "TagCategory " + this.Id + " " + this.Name + " N:" + this.NetworkId;
        }
    }
}
