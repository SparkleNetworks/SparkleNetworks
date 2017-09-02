
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class NetworkModel
    {
        public NetworkModel()
        {
        }

        public Guid Guid { get; set; }

        [Required]
        public string InstanceName { get; set; }

        [Required]
        public string NetworkName { get; set; }

        [Required]
        public string UniverseName { get; set; }

        [Required]
        public string InternalDomainName { get; set; }

        [Required]
        public string MainDomainName { get; set; }

        public IList<string> DomainNames { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public override string ToString()
        {
            return this.InstanceName + "." + this.NetworkName;
        }
    }
}