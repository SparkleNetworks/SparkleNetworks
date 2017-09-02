
namespace Sparkle.NetworksStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.NetworksStatus.Domain.Services;

    partial interface IServiceFactory
    {
        void Verify();
        ICacheService Cache { get; }
    }
}
