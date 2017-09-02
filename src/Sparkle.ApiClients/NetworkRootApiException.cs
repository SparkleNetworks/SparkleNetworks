
namespace Sparkle.ApiClients
{
    using Sparkle.ApiClients.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    public class NetworkRootApiException : Exception
    {
        public NetworkRootApiException()
        {
        }

        public NetworkRootApiException(string message)
            : base(message)
        {
        }

        public NetworkRootApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NetworkRootApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public BaseResponse<BaseResult> Error { get; set; }

        public string TransportError { get; set; }

        public string ServiceError { get; set; }

        public string LocalError { get; set; }
    }
}
