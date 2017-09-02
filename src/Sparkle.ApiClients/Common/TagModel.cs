
namespace Sparkle.ApiClients.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class TagModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public int CreatedByUserId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Weight { get; set; }

        [DataMember]
        public bool? Activated { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public Dictionary<string, int> Numbers { get; set; }
    }
}
