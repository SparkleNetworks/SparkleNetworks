
namespace Sparkle.NetworksStatus.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.NetworksStatus.Domain.Internals;

    public interface ICacheService
    {
        IpAddressInfo GetIpAddressInfo(string ipAddress);
    }
}
