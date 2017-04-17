
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

    public class NetworkStatusApiKeyPayload
    {
        public const string ApiKeyHeaderName = "X-SparkleStatus-Key";
        public const string TimeHeaderName = "X-SparkleStatus-Time";
        public const string HashHeaderName = "X-SparkleStatus-Hash";

        public NetworkStatusApiKeyPayload()
        {
        }

        public NetworkStatusApiKeyPayload(string headerKey, string headerTime, string headerHash, string method, string pathAndQuery, string content)
        {
            this.Key = headerKey;
            this.Time = headerTime;
            this.Hash = headerHash;
            this.MethodAndPathAndQuery = method + " " + pathAndQuery;
            this.Content = content;
        }

        public NetworkStatusApiKeyPayload(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            this.Key = httpContext.Request.Headers[ApiKeyHeaderName];
            this.Time = httpContext.Request.Headers[TimeHeaderName];
            this.Hash = httpContext.Request.Headers[HashHeaderName];
            this.MethodAndPathAndQuery = httpContext.Request.HttpMethod + " " + httpContext.Request.Url.PathAndQuery;
            using (var reader = new StreamReader(httpContext.Request.InputStream))
            {
                this.Content = reader.ReadToEnd();
            }
        }

        public bool IsVerified { get; set; }

        public List<KeyValuePair<string, string>> VerificationErrors { get; set; }

        public string Key { get; set; }

        public string Time { get; set; }

        public string Hash { get; set; }

        public string MethodAndPathAndQuery { get; set; }

        public string Content { get; set; }

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