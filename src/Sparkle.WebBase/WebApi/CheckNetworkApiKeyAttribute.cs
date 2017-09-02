
namespace Sparkle.WebBase.WebApi
{
    using Sparkle.EmailTemplates;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.StatusApi;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using AppConfiguration = Sparkle.Infrastructure.AppConfiguration;
    using IServiceFactory = Sparkle.Services.Networks.IServiceFactory;
    
    /// <summary>
    /// Authorization filter for the NetworkRootApi and ClientRootApi.
    /// </summary>
    public class CheckNetworkApiKeyAttribute : AuthorizationFilterAttribute
    {
        public const string NewClientRequestIdHeaderName = "X-SparkleNetworks-ClientRequestId";
        public const string TimeFormat = "yyyyMMdd'T'HHmmssffff'Z'";
        private static readonly TimeSpan TimeDelay = TimeSpan.FromMinutes(15D);

        private readonly object _typeId = new object();
        private static readonly string[] _emptyArray = new string[0];
        private string _roles;
        private string[] _rolesSplit = CheckNetworkApiKeyAttribute._emptyArray;

        public string Roles
        {
            get
            {
                return this._roles ?? string.Empty;
            }
            set
            {
                this._roles = value;
                this._rolesSplit = SplitString(value);
            }
        }

        public override object TypeId
        {
            get { return this.GetType().FullName; }
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            if (SkipAuthorization(actionContext))
                return;

            var result = this.IsAuthorized(actionContext);
            if (!result.IsAuthorized)
            {
                this.HandleUnauthorizedRequest(actionContext, result);
            }
        }

        protected virtual AuthorizeResult IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            var givenApiKey = new NetworkApiKeyPayload().LoadWith(actionContext);
            actionContext.Request.Properties["CheckApiKey.GivenApiKey"] = givenApiKey;

            if (string.IsNullOrEmpty(givenApiKey.Key))
            {
                return this.Log(actionContext, new AuthorizeResult("MissingApplicationKey", "The Application Key should be specified.", "The header '" + NetworkApiKeyPayload.ApiKeyHeaderName + "' is missing."), null);
            }

            if (string.IsNullOrEmpty(givenApiKey.Time))
            {
                return this.Log(actionContext, new AuthorizeResult("MissingTime", "The Time should be specified.", "The header '" + NetworkApiKeyPayload.TimeHeaderName + "' is missing."), givenApiKey.Key);
            }

            if (string.IsNullOrEmpty(givenApiKey.Hash))
            {
                return this.Log(actionContext, new AuthorizeResult("MissingHash", "The Hash should be specified.", "The header '" + NetworkApiKeyPayload.HashHeaderName + "' is missing."), givenApiKey.Key);
            }

            IServiceFactory services;
            try
            {
                services = GetServices(givenApiKey);
            }
            catch (UnknownApplicationException ex)
            {
                return this.Log(actionContext, new AuthorizeResult("InvalidNetworkSpecification", "The specified network does not exist.", "The header '" + NetworkApiKeyPayload.NetworkNameHeaderName + "' or '" + NetworkApiKeyPayload.NetworkDomainNameHeaderName + "' is invalid."), givenApiKey.Key);
            }
            catch (InvalidOperationException ex)
            {
                return this.Log(actionContext, new AuthorizeResult("InvalidNetworkSpecification", "The specified network does not exist.", "The header '" + NetworkApiKeyPayload.NetworkNameHeaderName + "' or '" + NetworkApiKeyPayload.NetworkDomainNameHeaderName + "' is invalid."), givenApiKey.Key);
            }

            actionContext.Request.Properties["IServiceFactory"] = services;

            var key = this.GetKey(givenApiKey.Key, services);
            if (key == null)
            {
                return this.Log(actionContext, new AuthorizeResult("UnknownApplicationKey", "The header '" + NetworkApiKeyPayload.ApiKeyHeaderName + "' is invalid."), givenApiKey.Key);
            }

            string verifyTimeErrorDetail;
            if (!this.VerifyTime(givenApiKey.Time, out verifyTimeErrorDetail))
            {
                return this.Log(actionContext, new AuthorizeResult("InvalidTime", verifyTimeErrorDetail, "The header '" + NetworkApiKeyPayload.TimeHeaderName + "' value '" + givenApiKey.Time.TrimToLength(TimeFormat.Length) + "' is invalid. It should look like '" + this.GetUtcNow().ToString(TimeFormat) + "', the format is '" + TimeFormat + "'. The time zone is UTC."), key.Id.ToString());
            }

            AccessTokenModel accessToken = null;
            /* 
             * we can later use this code to validate the user identity
             * 
            var identity = givenApiKey.IdentityKey;
            if (!string.IsNullOrEmpty(identity))
            {
                accessToken = this.GetAccessToken(identity, services);
                if (accessToken != null)
                {
                    if (accessToken.ValidationState != ValidateAnonymousTokenState.Ok)
                    {
                        return new AuthorizeResult("DisabledIdentityKey", "The provided identity has expired or has been disabled. You should renew the identity with the user.");
                    }
                }
                else
                {
                    return this.Log(actionContext, new AuthorizeResult("UnknownIdentityKey", "The header '" + NewIdentityKey + "' is invalid."));
                }
            }
            else if (this.IsAccessTokenRequired)
            {
                return new AuthorizeResult("MissingIdentityKey", "An identity key was required but none was provided.");
            }
            */
            string protocol;
            if (!this.VerifyHash(key, accessToken, actionContext.Request, givenApiKey.Hash, givenApiKey.Time, out protocol))
            {
                return this.Log(actionContext, new AuthorizeResult("InvalidHash", "The locally computed hash is not equal to the provided hash.", "The hash verification failed. Make sure you followed the documentation."), key.Id.ToString());
            }

            actionContext.Request.Properties["CheckApiKey.ValidatedApiKey"] = key;
            /*actionContext.Request.Properties["CheckApiKey.ValidatedIdentityKey"] = accessToken;*/
            actionContext.Request.Properties["CheckApiKey.Protocol"] = protocol;

            if (this._rolesSplit == null || this._rolesSplit.Length == 0)
            {
                return this.Log(actionContext, AuthorizeResult.Success, key.Id.ToString());
            }

            if (key.Roles == null)
            {
                return this.Log(actionContext, new AuthorizeResult("ApplicationKeyIsMissingPermission", "The application key you have passed does not have the permission to call this method."), key.Id.ToString());
            }
            else
            {
                var keyRoles = key.Roles.Split(new char[] { ',', ';', }, StringSplitOptions.None).Select(x => x.Trim()).ToArray();
                var roles = this._rolesSplit;
                if (keyRoles.Any(x => this._rolesSplit.Contains(x)))
                {
                    return this.Log(actionContext, AuthorizeResult.Success, key.Id.ToString());
                }
                else
                {
                    return this.Log(actionContext, new AuthorizeResult("ApplicationKeyIsMissingPermission", "The application key you have passed does not have the permission to call this method."), key.Id.ToString());
                }
            }
        }

        private IServiceFactory GetServices(NetworkApiKeyPayload payload)
        {
            bool useSparkleSystems = true; // after removing sparklesystems, this should come from configuration 
            if (useSparkleSystems)
            {
                AppConfiguration config = null;
                SparkleNetworksApplication app = null;
                IServiceFactory services = null;

                if (!string.IsNullOrEmpty(payload.NetworkDomainName))
                {
                    config = AppConfiguration.CreateSingleFromWebConfiguration(payload.NetworkDomainName);
                }
                
                if (!string.IsNullOrEmpty(payload.NetworkName) && config == null)
                {
                    config = AppConfiguration.CreateSingleFromConfiguration(payload.NetworkName);
                }

                if (config == null)
                {
                    throw new InvalidOperationException("Cannot determine the network to use.");
                }

                app = new SparkleNetworksApplication(config, () => new DefaultEmailTemplateProvider());
                services = app.GetNewServiceFactory(new BasicServiceCache()); // TODO: use AspnetServiceCache
                return services;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected virtual ApiKeyModel GetKey(string key, IServiceFactory services)
        {
            var apiKey = services.ApiKeys.GetByKey(key);
            return apiKey;
        }

        protected virtual DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext, AuthorizeResult result)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            var message = result != null ? result.Error : null;
            message = message ?? "Authorization denied.";

            var response = new BaseResponse<object>();
            response.ErrorCode = result.Error;
            response.ErrorMessage = result.ErrorDetail;
            response.ErrorHelp = result.HelpMessage;
            throw new SparkleApiException(response, HttpStatusCode.Unauthorized);

            actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor == null)
                return false;

            if (actionContext.ControllerContext.ControllerDescriptor == null)
                return false;

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>()
                || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>();
        }

        private static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return _emptyArray;
            }

            IEnumerable<string> source =
                from piece in original.Split(new char[]
                {
                    ','
                })
                let trimmed = piece.Trim()
                where !string.IsNullOrEmpty(trimmed)
                select trimmed;
            return source.ToArray<string>();
        }

        private AuthorizeResult Log(HttpActionContext actionContext, AuthorizeResult item, string keyId)
        {
            string host = ((dynamic)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            string prefix = "CheckNetworkApiKeyAttribute: " + host + "@" + DateTime.UtcNow.ToString("o") + "/" + keyId.TrimToLength(12) + " '" + actionContext.Request.RequestUri.PathAndQuery + "' ";
            Trace.WriteLine(prefix + item.Error + " " + item.ErrorDetail);
            return item;
        }

        private bool VerifyTime(string timeString, out string errorDetail)
        {
            DateTime time;
            if (!DateTime.TryParseExact(timeString, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out time))
            {
                errorDetail = "The DateTime format is now valid.";
                return false;
            }

            var now = this.GetUtcNow();
            if ((now - TimeDelay) < time && time < (now + TimeDelay))
            {
                errorDetail = null;
                return true;
            }
            else
            {
                errorDetail = "Time is out of allowed range by " + (now.Subtract(time).Duration().Subtract(TimeDelay).ToString());
                return false;
            }
        }

        private bool VerifyHash(ApiKeyModel key, AccessTokenModel accessToken, HttpRequestMessage httpRequestMessage, string clientHash, string time, out string protocol)
        {
            if (clientHash[0] != '$')
            {
                protocol = null;
                return false;
            }

            var formatPosition = clientHash.IndexOf('$', 1);
            if (formatPosition < 2)
            {
                protocol = null;
                return false;
            }

            var format = clientHash.Substring(1, formatPosition - 1);
            protocol = format;
            switch (format)
            {
                case "1":
                    return this.VerifyHashFormat1(key, accessToken, httpRequestMessage, clientHash.Substring(formatPosition + 1), time);
            }

            return false;
        }

        private bool VerifyHashFormat1(ApiKeyModel key, AccessTokenModel accessToken, HttpRequestMessage httpRequestMessage, string clientHash, string time)
        {
            var serverHash = ComputeHashFormat1(key, accessToken, httpRequestMessage, time, clientHash ?? string.Empty, null);
            return serverHash.Equals(clientHash, StringComparison.InvariantCultureIgnoreCase);
        }

        internal static string ComputeHashFormat1(ApiKeyModel key, AccessTokenModel accessToken, HttpRequestMessage httpRequestMessage, string time, string clientHash, string responseBody)
        {
            var b = new StringBuilder();
            b.Append(key.Key);
            b.Append('\n');
            b.Append(key.GetSecret());
            b.Append('\n');
            ////if (accessToken != null)
            ////    b.Append(accessToken.Token);
            b.Append('\n');
            ////if (accessToken != null)
            ////    b.Append(accessToken.GetSecret());
            b.Append('\n');
            b.Append(httpRequestMessage.Method.Method.ToUpperInvariant());
            b.Append('\n');
            b.Append(httpRequestMessage.RequestUri.PathAndQuery);
            b.Append('\n');

            if (clientHash != null)
            {
                if (httpRequestMessage.Content != null)
                {
                    b.Append(httpRequestMessage.Content.ReadAsStringAsync().Result);
                }
            }
            else
            {
                if (responseBody != null)
                {
                    b.Append(responseBody);
                }
            }

            b.Append('\n');
            b.Append(time);
            var stringToHash = b.ToString();
            var bytesToHash = Encoding.UTF8.GetBytes(stringToHash);
            byte[] hash;
            using (var sha256 = new SHA256Managed())
            {
                hash = sha256.ComputeHash(bytesToHash);
            }

            var hashAsString = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            return hashAsString;
        }
    }
}
