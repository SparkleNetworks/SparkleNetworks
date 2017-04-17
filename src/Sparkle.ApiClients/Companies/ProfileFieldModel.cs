
namespace Sparkle.ApiClients.Companies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class ProfileFieldModel
    {
        [DataMember]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string Data { get; set; }
    }
}
