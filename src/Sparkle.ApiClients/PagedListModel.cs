
namespace Sparkle.UnitTests.NetworkRootApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class PagedListModel<T>
    {
        [DataMember(Order = 1)]
        public long Offset { get; set; }

        [DataMember(Order = 2)]
        public long Total { get; set; }

        [DataMember(Order = 3)]
        public long Count { get; set; }

        [DataMember(Order = 4)]
        public IList<T> Items { get; set; }

        public override string ToString()
        {
            return "From " + this.Offset + " with " + (this.Items != null ? this.Items.Count.ToString() : "0") + " items out of " + this.Total;
        }
    }
}
