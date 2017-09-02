
namespace Sparkle.Data.Options
{
    using System;

    [Flags]
    public enum PersonOptions
    {
        None              = 0x0000,
        Job               = 0x0002,
        Company           = 0x0004,
        Skills            = 0x0008,
        Interests         = 0x0020,
        Recreations       = 0x0040,
        SkillsValues      = 0x0080,
        InterestsValues   = 0x0200,
        RecreationsValues = 0x0400,
        Notification      = 0x0800,
        Contacts          = 0x2000,
        ContactsOf        = 0x4000,
        ProfileFields     = 0x8000,
    }
}
