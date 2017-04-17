
namespace Sparkle.NetworksStatus.Domain.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BasicDomainCacheProvider : IDomainCacheProvider
    {
        private readonly IDictionary<string, DomainCacheObject> cache;

        public BasicDomainCacheProvider()
        {
            this.cache = new Dictionary<string, DomainCacheObject>();
        }
        
        public void Add(string key, object obj, TimeSpan duration)
        {
            var item = new DomainCacheObject
            {
                DateAdded = DateTime.UtcNow,
                Duration = duration,
                Object = obj,
            };
            this.cache.Add(key, item);
        }

        public void Clear(string key)
        {
            var item = this.GetValue(key);
            if (item != null)
            {
                item.IsInvalidated = true;
            }

            this.cache.Remove(key);
        }

        public IDomainCacheObject Get(string key)
        {
            var item = GetValue(key);
            if (item != null)
                return item;
            return null;
        }

        public object GetObject(string key)
        {
            var item = this.Get(key);
            return item != null ? item.Object : null;
        }

        public T GetObject<T>(string key)
        {
            var item = this.Get(key);
            if (item == null || item.Object == null)
                return default(T);

            if (!(item.Object is T))
            {
                throw new InvalidOperationException("Object with key '" + key + "' is of type '" + item.Object.GetType().FullName + "' and cannot be cast as '" + typeof(T).FullName + "'.");
            }

            return (T)item.Object;
        }

        private DomainCacheObject GetValue(string key)
        {
            if (!this.cache.ContainsKey(key))
                return null;

            var value = (DomainCacheObject)this.cache[key];
            if (value != null && !value.IsInvalidated && DateTime.UtcNow < (value.DateAdded.Add(value.Duration)))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}
