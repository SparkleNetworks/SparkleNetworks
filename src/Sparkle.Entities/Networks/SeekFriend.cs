
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class SeekFriend
    {
        
    }

    public enum SeekFriendOptions
    {
        None           = 0x00000,
        Seeker         = 0x00001,
        SeekerCompany  = 0x00002,
        SeekerJob      = 0x00004,
        Target         = 0x00008,
        TargetCompany  = 0x00010,
        TargetJob      = 0x00011,
    }
}
