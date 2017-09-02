
namespace Sparkle.Data.Networks.Options
{
    using System;
    
    [Flags]
    public enum CompanyRelationshipOptions
    {
        None = 0x0000,
        Type = 0x0001,
        Master = 0x0002,
        Slave = 0x0004,
    }
}
