
namespace Sparkle.NetworksStatus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Cache
    {
        public CacheType TypeValue
        {
            get { return (CacheType)this.Type; }
            set { this.Type = (byte)value; }
        }
    }

    public enum CacheType
    {
        Geocode = 1,
    }
}
