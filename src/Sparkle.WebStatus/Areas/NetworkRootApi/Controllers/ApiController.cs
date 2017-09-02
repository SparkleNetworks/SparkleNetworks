
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
    using Sparkle.Services.Networks;
    using Sparkle.WebBase.WebApi;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;

    public class ApiController : System.Web.Http.ApiController
    {
        protected IServiceFactory Services
        {
            get
            {
                if (this.Request.Properties.ContainsKey("IServiceFactory"))
                {
                    var obj = this.Request.Properties["IServiceFactory"] as IServiceFactory;
                    return obj;
                }

                return null;
            }
        }

        public override async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            var result = await base.ExecuteAsync(controllerContext, cancellationToken);

            if (controllerContext.Request.Properties.ContainsKey("IServiceFactory"))
            {
                var obj = controllerContext.Request.Properties["IServiceFactory"];
                controllerContext.Request.Properties.Remove("IServiceFactory");
                if (obj is IDisposable)
                {
                    ((IDisposable)obj).Dispose();
                }
            }

            return result;
        }

        /// <summary>Gets the client's IP address</summary>
        /// <exception cref="NotSupportedException"></exception>
        protected string GetRemoteAddress()
        {
            // http://stackoverflow.com/questions/9565889/get-the-ip-address-of-the-remote-host
            string address = null;
            if (this.Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var remote = this.Request.Properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (remote != null)
                {
                    address = remote.Address;
                }
            }
            else if (this.Request.Properties.ContainsKey("MS_HttpContext"))
            {
                var context = this.Request.Properties["MS_HttpContext"] as HttpContextWrapper;
                if (context != null)
                {
                    address = context.Request.UserHostAddress;
                }
            }
            else
            {
                throw new NotSupportedException("Cannot resolve remote address");
            }

            return address;
        }

        /// <summary>Gets the server's IP address</summary>
        /// <exception cref="NotSupportedException"></exception>
        protected string GetLocalAddress()
        {
            // http://stackoverflow.com/questions/1345676/how-to-get-the-server-ip-address
            string address = null;
            ////if (this.Request.Properties.ContainsKey(LocalEndpointMessageProperty.Name))
            ////{
            ////    var remote = this.Request.Properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            ////    if (remote != null)
            ////    {
            ////        address = remote.Address;
            ////    }
            ////}
            ////else 
            if (this.Request.Properties.ContainsKey("MS_HttpContext"))
            {
                var context = this.Request.Properties["MS_HttpContext"] as HttpContextWrapper;
                if (context != null)
                {
                    address = context.Request.ServerVariables["LOCAL_ADDR"];
                }
            }
            else
            {
                throw new NotSupportedException("Cannot resolve remote address");
            }

            return address;
        }

        /// <summary>
        /// Handles returning a domain request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected T DomainRequest<T>(T result)
            where T : BaseRequest
        {
            HttpResponseMessage response;
            if (result != null)
            {
                return result;
            }
            else
            {
                response = this.Request.CreateResponse<T>(HttpStatusCode.BadRequest, result);
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        /// Handles returning a domain response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected T DomainResult<T>(T result)
            where T : IBaseResult
        {
            HttpResponseMessage response;
            if (result.Succeed)
            {
                // success: just return the thing.
                return result;
            }
            else
            {
                // domain error: create a 400 response and throw.
                response = this.Request.CreateResponse<T>(HttpStatusCode.BadRequest, result);
                throw new HttpResponseException(response);
            }
        }
        /// <summary>
        /// Handles returning a domain response and wraps the response in a BaseResponse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected BaseResponse<T> DomainResultWrapped<T>(T result)
            where T : IBaseResult
        {
            var message = new BaseResponse<T>(result);
            HttpResponseMessage response;
            if (result.Succeed)
            {
                // success: just return the thing. wrapped.
                return message;
            }
            else
            {
                // domain error: create a 400 response and throw. wrapped.
                response = this.Request.CreateResponse<BaseResponse<T>>(HttpStatusCode.BadRequest, message);
                throw new HttpResponseException(response);
            }
        }

        /*
         * The following code comes from SB, it must be changed in order to be used. @sandrock
         * 
        /// <summary>
        /// Handles returning a domain response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T DomainResultError<T>(object errorResult)
            where T : IBaseResult
        {
            return this.DomainResultError<T>(errorResult, HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Handles returning a domain response.
        /// </summary>
        protected T DomainResultError<T>(object errorResult, HttpStatusCode code)
            where T : IBaseResult
        {
            HttpResponseMessage response;
            response = this.Request.CreateResponse(code, errorResult);
            throw new HttpResponseException(response);
        }
        */

        protected bool ValidateResult<T>(T result)
            where T : IBaseResult
        {
            if (result == null)
            {
                var response = this.Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }

            if (!result.Succeed)
            {
                var response = this.Request.CreateResponse<T>(HttpStatusCode.BadRequest, result);
                throw new HttpResponseException(response);
            }

            return true;
        }

        /// <summary>
        /// Runs the ModelState validation and return a HTTP 400 when validation does not pass.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">the request to validate.</param>
        protected void ValidateRequestWrapped<T>(T request)
        {
            if (!this.ModelState.IsValid)
            {
                // domain error: create a 400 response and throw. wrapped.
                var message = new BaseResponse<object>()
                {
                    ModelState = this.ModelState.ToDictionary(x => x.Key.Substring("request.".Length), x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()),
                    ErrorCode = "InvalidMessage",
                    ErrorHelp = "Validation errors occured, check the ModelState property. ",
                };
                var response = this.Request.CreateResponse<BaseResponse<object>>(HttpStatusCode.BadRequest, message);
                throw new HttpResponseException(response);
            }
        }
    }
}
