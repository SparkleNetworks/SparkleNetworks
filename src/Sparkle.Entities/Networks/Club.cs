
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class Club : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }
}
