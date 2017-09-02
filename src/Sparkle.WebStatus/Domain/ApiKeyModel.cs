
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ApiKeyModel
    {
        public ApiKeyModel()
        {
        }

        public Guid Guid { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        [Required, DataType(DataType.Text)]
        public string Name { get; set; }

        /// <summary>
        /// Uniquely identifies the key over the network.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Secret value that SHOULD NOT be exchanged over the network.
        /// </summary>
        public string Secret { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        public bool IsEnabled { get; set; }

        internal void UpdateFrom(ApiKeyModel item)
        {
            this.Name = item.Name;
            this.Remark = item.Remark;
            this.IsEnabled = item.IsEnabled;
        }
    }
}
