
namespace Sparkle.ApiClients.Common
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class JobModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string GroupName { get; set; }
    }
}
