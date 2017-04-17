
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Basic in-memory cache. 
    /// Passive cache cleanup. 
    /// Not thread-safe!
    /// </summary>
    public class BasicServiceCache : IServiceCache
    {
        private readonly IDictionary<string, CacheObject> cache;

        public BasicServiceCache()
        {
            this.cache = new Dictionary<string, CacheObject>();
        }
        
        public void Add(string key, object obj, TimeSpan duration)
        {
            var item = new CacheObject
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

        public IServiceCacheObject Get(string key)
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

        public void Dispose()
        {
        }

        private CacheObject GetValue(string key)
        {
            if (!this.cache.ContainsKey(key))
                return null;

            var value = (CacheObject)this.cache[key];
            if (value != null && !value.IsInvalidated && DateTime.UtcNow < (value.DateAdded.Add(value.Duration)))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        private class CacheObject : IServiceCacheObject
        {
            public object Object { get; set; }
            public DateTime DateAdded { get; set; }
            public TimeSpan Duration { get; set; }
            public bool IsInvalidated { get; set; }
        }
    }
}
