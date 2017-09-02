
namespace Sparkle.NetworksStatus.Domain.Cache
{
    using System;
    using System.Collections.Generic;

    public interface IDomainCacheObject
    {
        object Object { get; }
        DateTime DateAdded { get; }
        TimeSpan Duration { get; }
        bool IsInvalidated { get; }
    }
}
