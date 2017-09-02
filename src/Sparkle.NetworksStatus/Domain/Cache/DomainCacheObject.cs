
namespace Sparkle.NetworksStatus.Domain.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class DomainCacheObject : IDomainCacheObject
    {
        public object Object { get; set; }
        public DateTime DateAdded { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsInvalidated { get; set; }
    }
}
