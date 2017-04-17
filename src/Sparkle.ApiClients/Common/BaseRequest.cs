
namespace Sparkle.ApiClients.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class BaseRequest
    {
        [DataMember]
        public int? ActingUserId { get; set; }
    }
}
