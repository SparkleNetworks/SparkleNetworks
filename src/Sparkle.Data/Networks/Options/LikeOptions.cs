
namespace Sparkle.Data.Networks.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Flags]
    public enum LikeOptions
    {
        None            = 0x0000,
        User            = 0x0002,
        TimelineItem    = 0x0004,
    }
}
