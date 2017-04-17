
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

        [Obsolete("What is that?")]
        public bool Verify(string key, string secret, DateTime now)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The value cannot be empty", "key");
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentException("The value cannot be empty", "secret");
            if (now.Kind != DateTimeKind.Utc)
                throw new ArgumentException("The now DateTime.Kind must be UTC", "now");

            this.VerificationErrors = new List<KeyValuePair<string, string>>();

            DateTime specifiedTime;
            if (!DateTime.TryParseExact(this.Time, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out specifiedTime))
            {
                this.AddVerificationErrors("InvalidTime", "The specified time format is not valid.");
                return false;
            }

            var minTime = now.AddMinutes(-10);
            if (specifiedTime < minTime)
            {
                this.AddVerificationErrors("InvalidTime", "The specified time is too far in the past.");
                return false;
            }

            var maxTime = now.AddMinutes(10);
            if (maxTime < specifiedTime)
            {
                this.AddVerificationErrors("InvalidTime", "The specified time is too far in the future.");
                return false;
            }

            if (string.IsNullOrEmpty(this.Key))
            {
                this.AddVerificationErrors("MissingKey", "The Key is missing in '" + ApiKeyHeaderName + "'.");
                return false;
            }

            if (string.IsNullOrEmpty(this.Hash))
            {
                this.AddVerificationErrors("MissingHash", "The Hash is missing in '" + HashHeaderName + "'.");
                return false;
            }

            var computed = this.ComputeHash(key, secret);
            if (this.IsVerified = computed.Equals(this.Hash, StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                this.AddVerificationErrors("InvalidHash", "The Hash is wrong.");
            }

            return this.IsVerified;
        }

        [Obsolete("What is that?")]
        public string ComputeHash(string secret)
        {
            var value = this.ComputeHash(this.Key, secret);
            return value;
        }

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