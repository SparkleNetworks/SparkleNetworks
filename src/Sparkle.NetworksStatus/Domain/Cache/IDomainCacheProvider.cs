
namespace Sparkle.NetworksStatus.Domain.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IDomainCacheProvider
    {
        void Add(string key, object obj, TimeSpan duration);

        void Clear(string key);

        IDomainCacheObject Get(string key);

        object GetObject(string key);

        T GetObject<T>(string key);
    }
}
