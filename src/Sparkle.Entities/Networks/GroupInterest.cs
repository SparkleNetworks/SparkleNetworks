
namespace Sparkle.Entities.Networks
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class GroupInterest : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " group " + this.GroupId + " has interest " + this.InterestId;
        }
    }
}
