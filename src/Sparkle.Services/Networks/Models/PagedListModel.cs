
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class PagedListModel<T>
    {
        public PagedListModel()
        {
        }

        public PagedListModel(IList<T> items, long total, long offset, long pageSize)
        {
            this.Items = items;
            this.Total = total;
            this.Offset = offset;
            this.Count = pageSize;
        }

        /// <summary>
        /// The desired offset.
        /// </summary>
        [DataMember(Order = 1)]
        public long Offset { get; set; }

        /// <summary>
        /// The total number of items.
        /// </summary>
        [DataMember(Order = 2)]
        public long Total { get; set; }

        /// <summary>
        /// The desired number of items.
        /// </summary>
        [DataMember(Order = 3)]
        public long Count { get; set; }

        /// <summary>
        /// The requested items.
        /// </summary>
        [DataMember(Order = 4)]
        public IList<T> Items { get; set; }

        public override string ToString()
        {
            return "From " + this.Offset + " with " + (this.Items != null ? this.Items.Count.ToString() : "0") + " items out of " + this.Total;
        }
    }
}
