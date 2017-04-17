
namespace Sparkle.NetworksStatus.Domain.Models
{
    using Sparkle.NetworksStatus.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CacheModel
    {
        public CacheModel(Cache item)
        {
            this.Set(item);
        }

        public void Set(Cache item)
        {
            this.Id = item.Id;
            this.Type = item.TypeValue;
            this.Name = item.Name;
            this.Value = item.Value;
            this.DateCreatedUtc = item.DateCreatedUtc;
        }

        public int Id { get; set; }

        public CacheType Type { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime DateCreatedUtc { get; set; }
    }
}
