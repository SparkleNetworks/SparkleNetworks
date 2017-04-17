
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Hint : IEntityInt32Id
    {
        public HintType HintType
        {
            get { return (HintType)this.HintTypeId; }
            set { this.HintTypeId = (int)value; }
        }

        public override string ToString()
        {
            return "Hint #" + this.Id + " '" + this.Alias + "' (" + this.HintType + ")";
        }
    }

    partial class HintsToUser
    {
    }

    public enum HintType
    {
        Unknown = 0,

        /// <summary>
        /// 1: help message located somewhere in the web UI.
        /// </summary>
        WebTip = 1,

        /// <summary>
        /// 2: Internal hint that is not visible anywhere.
        /// </summary>
        Internal = 2,
    }
}
