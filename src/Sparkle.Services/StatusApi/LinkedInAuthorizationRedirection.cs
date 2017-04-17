
namespace Sparkle.Services.StatusApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class LinkedInAuthorizationRedirectionCreate
    {
        [DataMember(IsRequired = true)]
        public string GoUrl { get; set; }

        [DataMember(IsRequired = true)]
        public string LinkedInReturnUrl { get; set; }
    }

    [DataContract]
    public class LinkedInAuthorizationRedirectionData
    {
        [DataMember(IsRequired = true)]
        public int UserId { get; set; }

        [DataMember(IsRequired = true)]
        public string PreviousReturnUrl { get; set; }
    }

    [DataContract]
    public class BaseResponse<T>
    {
        [DataMember]
        public T Data { get; set; }

        [DataMember]
        public ResponseError[] Errors { get; set; }
    }

    [DataContract]
    public class ResponseError
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
