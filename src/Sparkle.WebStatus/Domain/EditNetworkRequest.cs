
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class EditNetworkRequest : BaseRequest
    {
        public Guid Guid { get; set; }
        public string InstanceName { get; set; }
        public string NetworkName { get; set; }
        public string UniverseName { get; set; }
        public string MainDomainName { get; set; }
        public IList<string> DomainNames { get; set; }
        public DateTime DateCreatedUtc { get; set; }

    }
}