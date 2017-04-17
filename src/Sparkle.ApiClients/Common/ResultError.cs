
namespace Sparkle.ApiClients.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class ResultError
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string DisplayMessage { get; set; }

        [DataMember]
        public string Detail { get; set; }
    }
}
