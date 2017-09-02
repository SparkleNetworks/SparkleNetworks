
namespace Sparkle.Data.Networks.Options
{
    using System;

    [Flags]
    public enum CompanyOptions : int
    {
        None = 0x0000,
        CompanyAdmins = 0x0001,
        CompanyNews   = 0x0002,
        Users         = 0x0004,
        Network       = 0x0008,
        Events        = 0x0010,
        CompanySkills = 0x0020,
        Places        = 0x0040,
        Tags          = 0x0080,
    }
}
