
namespace Sparkle.WebBase.WebApi
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class BaseResponse<T> : IBaseResponse
    {
        public BaseResponse()
        {
        }

        public BaseResponse(T data)
        {
            this.Data = data;
        }

        [DataMember]
        public T Data { get; set; }

        [DataMember(IsRequired = false), JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode { get; set; }

        [DataMember(IsRequired = false), JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [DataMember(IsRequired = false), JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Exception { get; set; }

        [DataMember(IsRequired = false), JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string[]> ModelState { get; set; }

        [DataMember(IsRequired = false), JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorHelp { get; set; }
    }
}
