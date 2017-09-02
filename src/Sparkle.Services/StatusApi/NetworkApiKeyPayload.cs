
namespace Sparkle.Services.StatusApi
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    /// <summary>
    /// ApiKeyPayload for the NetworkRootApi and ClientRootApi.
    /// </summary>
    public class NetworkApiKeyPayload
    {
        public const string NetworkNameHeaderName = "X-SparkleNetworksApi-NetworkName";
        public const string NetworkDomainNameHeaderName = "X-SparkleNetworksApi-NetworkDomainName";
        public const string ApiKeyHeaderName = "X-SparkleNetworksApi-Key";
        public const string TimeHeaderName = "X-SparkleNetworksApi-Time";
        public const string HashHeaderName = "X-SparkleNetworksApi-Hash";
        public const string ClientRequestIdHeaderName = "X-SparkleNetworksApi-ClientRequestId";

        public NetworkApiKeyPayload()
        {
        }

        public NetworkApiKeyPayload(string networkName, string headerKey, string headerTime, string headerHash, string method, string pathAndQuery, string content)
        {
            this.NetworkName = networkName;
            this.Key = headerKey;
            this.Time = headerTime;
            this.Hash = headerHash;
            this.MethodAndPathAndQuery = method + " " + pathAndQuery;
            this.Content = content;
        }

        public NetworkApiKeyPayload(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            this.NetworkName = httpContext.Request.Headers[NetworkNameHeaderName];
            this.NetworkDomainName = httpContext.Request.Headers[NetworkDomainNameHeaderName];
            this.Key = httpContext.Request.Headers[ApiKeyHeaderName];
            this.Time = httpContext.Request.Headers[TimeHeaderName];
            this.Hash = httpContext.Request.Headers[HashHeaderName];
            this.MethodAndPathAndQuery = httpContext.Request.HttpMethod + " " + httpContext.Request.Url.PathAndQuery;

            Guid clientId;
            if (Guid.TryParse(httpContext.Request.Headers[ClientRequestIdHeaderName], out clientId))
            {
                this.ClientRequestId = clientId;
            }
            else
            {
                this.ClientRequestId = Guid.NewGuid();
            }

            using (var reader = new StreamReader(httpContext.Request.InputStream))
            {
                this.Content = reader.ReadToEnd();
            }
        }

        public bool IsVerified { get; set; }

        public List<KeyValuePair<string, string>> VerificationErrors { get; set; }

        public string NetworkDomainName { get; set; }

        public string NetworkName { get; set; }

        public string Key { get; set; }

        public string Time { get; set; }

        public string Hash { get; set; }

        public string MethodAndPathAndQuery { get; set; }

        public string Content { get; set; }

        public Guid? ClientRequestId { get; set; }

        private void AddVerificationErrors(string p1, string p2)
        {
            this.VerificationErrors.Add(new KeyValuePair<string, string>(p1, p2));
        }

        private string ComputeHash(string key, string secret)
        {
            var sb = new StringBuilder();
            sb.Append(key);
            sb.Append('\n');
            sb.Append(this.MethodAndPathAndQuery);
            sb.Append('\n');
            sb.Append(this.Content);
            sb.Append('\n');
            sb.Append(secret);
            sb.Append('\n');
            sb.Append(this.Time);
            sb.Append('\n');
            var prehash = sb.ToString();

            byte[] prehashBytes, hashedBytes;
            prehashBytes = Encoding.UTF8.GetBytes(prehash);
            using (var hasher = new SHA256Managed())
            {
                hashedBytes = hasher.ComputeHash(prehashBytes);
            }

            var hexHash = BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToLowerInvariant();
            return hexHash;
        }
    }
}