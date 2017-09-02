
namespace Sparkle.WebBase.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Http.Filters;

    public class GlobalApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        const string ERROR_CODE_KEY = "ErrorCode";
        const string ERROR_REFERENCE_KEY = "ErrorReference";

        private static readonly object typeId = new object();

        private bool log = true;

        public override object TypeId
        {
            get { return typeId; }
        }

        public bool Log
        {
            get { return this.log; }
            set { this.log = value; }
        }

        public GlobalApiExceptionFilterAttribute()
        {
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            string message;
            BaseResponse<object> content = null;
            if (exception is HttpResponseException)
            {
                // this special exception occurs when:
                // - Content-Type is not set in the request
                message = "HttpResponseException " + ((HttpResponseException)exception).Response.ReasonPhrase;
                statusCode = ((HttpResponseException)exception).Response.StatusCode;
            }
            else if (exception is SparkleApiException)
            {
                var apiException = (SparkleApiException)exception;
                content = apiException.Response;
                message = exception.Message;
                statusCode = apiException.StatusCode ?? statusCode;
            }
            else if (exception is Exception)
            {
                message = exception.Message;
            }
            else
            {
                message = "Error is not an exception.";
            }

            bool includeErrorDetailPolicy = GlobalConfiguration.Configuration.IncludeErrorDetailPolicy == IncludeErrorDetailPolicy.Always;

            if (content == null)
            {
                content = new BaseResponse<object>
                {
                    ErrorCode = statusCode.ToString(),
                    ErrorMessage = message,
                    Exception = exception != null && includeErrorDetailPolicy ? exception.ToString() : null,
                    ////ErrorDetails = exception != null ? exception.ToString() : null,
                    ////ErrorType = exception != null ? exception.GetType().Name : null,
                    ModelState = context.ActionContext.ModelState.ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()),
                };
            }

            context.Response = context.Request.CreateResponse(statusCode, content);

            var now = DateTime.Now.ToString("o");
            Trace.WriteLine("HTTP request " + now + " at '" + context.Request.RequestUri.PathAndQuery + "' error: " + message);

            // flow through to the base
            base.OnException(context);
        }
    }
}
