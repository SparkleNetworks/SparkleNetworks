using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparkle.Data.Networks.Options
{
    [Flags]
    public enum SocialNetworkUserSubscriptionOptions
    {
        None = 0x0000,
        Connection = 0x0002,
    }

    [Flags]
    public enum SocialNetworkCompanySubscriptionOptions
    {
        None = 0x0000,
        Connection = 0x0002,
    }
}
