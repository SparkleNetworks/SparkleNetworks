
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Caching;

    /// <summary>
    /// Allows usage of the ASP.NET cache objet as global service cache.
    /// </summary>
    public sealed class AspnetServiceCache : IServiceCache, IDisposable
    {
        /// <summary>
        /// The cache provider (ASP.NET).
        /// </summary>
        private readonly Cache cache;

        public AspnetServiceCache(Cache cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            this.cache = cache;
        }

        public void Add(string key, object obj, TimeSpan duration)
        {
            var item = new CacheObject
            {
                DateAdded = DateTime.UtcNow,
                Duration = duration,
                Object = obj,
            };
            this.cache.Add(key, item, null, item.DateAdded.Add(item.Duration), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
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
                return (IServiceCacheObject)item;
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
            var watch = new Stopwatch();
            watch.Start();
            var value = (CacheObject)this.cache[key];
            watch.Stop();

            if (watch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine("AspnetServiceCache: access to an aspnet cache object took " + watch.Elapsed);
            }

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
