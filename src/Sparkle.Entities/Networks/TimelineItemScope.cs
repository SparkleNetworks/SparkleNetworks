
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum TimelineItemScope
    {
        /// <summary>
        /// Devices scope (-2).
        /// </summary>
        Devices = -2,

        /// <summary>
        /// Public scope (-1).
        /// </summary>
        Public = -1,

        /// <summary>
        /// Network scope (0).
        /// </summary>
        Network = 0,

        /// <summary>
        /// (1).
        /// </summary>
        Self = 1,
    }
}
