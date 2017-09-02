
namespace Sparkle.WebBase.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;

    [Serializable]
    public class SparkleApiException : Exception
    {
        private BaseResponse<object> response;

        public SparkleApiException()
        {
        }

        public SparkleApiException(string message) : base(message)
        {
        }
        
        public SparkleApiException(string message, Exception inner) : base(message, inner)
        {
        }
        
        protected SparkleApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SparkleApiException(BaseResponse<object> response)
            : this(response.ErrorCode + ": " + response.ErrorMessage)
        {
            if (response == null)
                throw new ArgumentNullException("response");
            this.Response = response;
        }

        public SparkleApiException(BaseResponse<object> response, HttpStatusCode httpCode)
            : this(response.ErrorCode + ": " + response.ErrorMessage)
        {
            if (response == null)
                throw new ArgumentNullException("response");
            this.Response = response;
            this.StatusCode = httpCode;
        }

        public BaseResponse<object> Response { get; set; }

        public HttpStatusCode? StatusCode { get; set; }
    }
}
