
namespace Sparkle.WebBase.WebApi
{
    using Sparkle.Services.StatusApi;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.Controllers;

    public static class ApiKeyPayloadExtensions
    {
        public static NetworkStatusApiKeyPayload LoadWith(this NetworkStatusApiKeyPayload item, HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            item.Key = request.GetSingleHeaderValue(NetworkStatusApiKeyPayload.ApiKeyHeaderName);
            item.Time = request.GetSingleHeaderValue(NetworkStatusApiKeyPayload.TimeHeaderName);
            item.Hash = request.GetSingleHeaderValue(NetworkStatusApiKeyPayload.HashHeaderName);
            item.MethodAndPathAndQuery = request.Method + " " + request.RequestUri.PathAndQuery;
            item.Content = request.Content.ReadAsStringAsync().Result;

            return item;
        }

        public static NetworkApiKeyPayload LoadWith(this NetworkApiKeyPayload item, HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            item.NetworkName = request.GetSingleHeaderValue(NetworkApiKeyPayload.NetworkNameHeaderName);
            item.NetworkDomainName = request.GetSingleHeaderValue(NetworkApiKeyPayload.NetworkDomainNameHeaderName);
            item.Key =  request.GetSingleHeaderValue(NetworkApiKeyPayload.ApiKeyHeaderName);
            item.Time = request.GetSingleHeaderValue(NetworkApiKeyPayload.TimeHeaderName);
            item.Hash = request.GetSingleHeaderValue(NetworkApiKeyPayload.HashHeaderName);
            item.MethodAndPathAndQuery = request.Method + " " + request.RequestUri.PathAndQuery;
            item.Content = request.Content.ReadAsStringAsync().Result;

            Guid clientId;
            if (Guid.TryParse(request.GetSingleHeaderValue(NetworkApiKeyPayload.ClientRequestIdHeaderName), out clientId))
            {
                item.ClientRequestId = clientId;
            }
            else
            {
                item.ClientRequestId = Guid.NewGuid();
            }

            return item;
        }

        public static string GetSingleHeaderValue(this HttpRequestMessage context, string name)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (!context.Headers.Contains(name))
                return null;

            return context.Headers.GetValues(name).Single();
        }
    }
}
