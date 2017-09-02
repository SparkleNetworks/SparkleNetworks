
namespace Sparkle.ApiClients.Common
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class Tag2Model
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public int CategoryId { get; set; }

        [DataMember]
        public string CategoryValue { get; set; }

        [DataMember]
        public DateTime CreatedDateUtc { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int Payload { get; set; }

        [DataMember]
        public JToken Data { get; set; }
    }
}
