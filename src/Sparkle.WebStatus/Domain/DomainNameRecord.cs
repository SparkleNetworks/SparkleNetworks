
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class DomainNameRecord
    {
        public Guid Guid { get; set; }
        public DateTime DateCreatedUtc { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        public string[] ExpectedValues { get; set; }

        public override string ToString()
        {
            return this.Name + " " + this.Type + " '" + string.Join(",", (this.ExpectedValues ?? new string[0])) + "'";
        }
    }
}
