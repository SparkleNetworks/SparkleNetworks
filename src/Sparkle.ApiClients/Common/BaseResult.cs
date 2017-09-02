
namespace Sparkle.ApiClients.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class BaseResult
    {
        [DataMember]
        public IList<ResultError> Errors { get; set; }

        [DataMember]
        public bool Succeed { get; set; }
    }
}
