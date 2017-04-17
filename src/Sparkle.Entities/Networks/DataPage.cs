
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataPage<T>
    {
        public IList<T> Items { get; set; }

        public int Offset { get; set; }

        public int Total { get; set; }

        public int Size { get; set; }
    }
}
