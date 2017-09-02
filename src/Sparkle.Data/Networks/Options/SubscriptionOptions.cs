
namespace Sparkle.Data.Networks.Options
{
    using System;

    [Flags]
    public enum SubscriptionOptions
    {
        None = 0x0000,
        Notifications = 0x0002,
        OwnerUser = 0x0004,
        OwnerCompany = 0x0008,
        AppliesToCompany = 0x0020,
        AppliesToUser = 0x0040,
    }
}
