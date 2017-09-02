
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Domain layer will need to cache objects. 
    /// This interface will allow this mecanism with basic cache operations.
    /// The Dispose method should not clear the cache: it must only release system resources.
    /// </summary>
    public interface IServiceCache : IDisposable
    {
        /// <summary>
        /// Adds an object to the cache.
        /// </summary>
        void Add(string key, object obj, TimeSpan duration);

        /// <summary>
        /// Clear an object from the cache.
        /// </summary>
        void Clear(string key);

        /// <summary>
        /// Get an object from the cache by its key.
        /// </summary>
        IServiceCacheObject Get(string key);

        /// <summary>
        /// Get an object from the cache by its key.
        /// </summary>
        object GetObject(string key);

        /// <summary>
        /// Get an object from the cache by its key.
        /// </summary>
        T GetObject<T>(string key);
    }
}
