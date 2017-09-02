using System;

namespace Sparkle.Data.Networks
{
    [Flags]
    public enum TimelineItemOptions
    {
        None              = 0x0000,
        PostedBy          = 0x0002,
        User              = 0x0004,
        UserLikes         = 0x0008,
        Company           = 0x0020,
        Group             = 0x0040,
        Place             = 0x0080,
        Event             = 0x0200,
    }
}
