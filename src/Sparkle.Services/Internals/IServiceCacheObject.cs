
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;

    public interface IServiceCacheObject
    {
        object Object { get; }
        DateTime DateAdded { get; }
        TimeSpan Duration { get; }
        bool IsInvalidated { get; }
    }
}
