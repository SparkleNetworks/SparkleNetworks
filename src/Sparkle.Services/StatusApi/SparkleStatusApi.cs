
namespace Sparkle.Services.StatusApi
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Client for the STATUS website's API.
    /// </summary>
    public class SparkleStatusApi
    {
        private Guid? oldApiKey;
        private string baseUrl;
        private string apiKey;
        private string apiSecret;
        private const string contentTypeUrlEncoded = "application/x-www-form-urlencoded";

        public SparkleStatusApi(Guid oldApiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("The value cannot be empty", "baseUrl");
            if (oldApiKey == Guid.Empty)
                throw new ArgumentException("The value cannot be empty", "apiKey");

            this.baseUrl = baseUrl;
            this.oldApiKey = oldApiKey;
        }

        public SparkleStatusApi(Guid oldApiKey, string apiKey, string apiSecret, string baseUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("The value cannot be empty", "apiKey");
            if (string.IsNullOrEmpty(apiSecret))
                throw new ArgumentException("The value cannot be empty", "apiSecret");
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("The value cannot be empty", "baseUrl");

            this.baseUrl = baseUrl;
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.oldApiKey = oldApiKey;
        }

        public SparkleStatusApi(string apiKey, string apiSecret, string baseUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("The value cannot be empty", "apiKey");
            if (string.IsNullOrEmpty(apiSecret))
                throw new ArgumentException("The value cannot be empty", "apiSecret");
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("The value cannot be empty", "baseUrl");

            this.baseUrl = baseUrl;
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
        }

        public string Echo(string message)
        {
            var url = string.Format(
                "{0}/Home/AuthorizeEcho",
                this.baseUrl);

            var request = CreateRequest("POST", url);
            string postContent = "message=" + Uri.EscapeDataString(message);
            this.PutHash(request, postContent);

            this.Post(request, postContent, contentTypeUrlEncoded);

            return GetResponse<string>(request);
        }

        /// <summary>
        /// Create a redirection for linkedin authentication.
        /// </summary>
        /// <param name="userId">the sparkle user id</param>
        /// <param name="scope">the linkedin access scope</param>
        /// <param name="apiKey">the linkedin oauth access token</param>
        /// <param name="returnUrl">the (network) url to redirect the user to</param>
        /// <returns></returns>
        public LinkedInAuthorizationRedirectionCreate CreateLinkedInAuthorizationRedirection(int userId, int scope, string apiKey, string returnUrl)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("The value cannot be empty", "apiKey");
            if (string.IsNullOrEmpty(returnUrl))
                throw new ArgumentNullException("The value cannot be empty", "returnUrl");

            var url = string.Format(
                "{0}/Redirect/CreateLinkedInAuth?userId={1}&scope={2}&apiKey={3}&returnUrl={4}",
                this.baseUrl,
                userId,
                scope,
                Uri.EscapeDataString(apiKey),
                Uri.EscapeDataString(returnUrl));

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "WebBase.SparkleStatusApi";
            request.Headers.Add("X-SparkleStatus-ApiKey", this.oldApiKey.ToString());
            request.ContentLength = 0L;

            string postContent = null;
            this.PutHash(request, postContent);

            return GetResponse<LinkedInAuthorizationRedirectionCreate>(request);
        }

        /// <summary>
        /// Gets an existing redirection for linkedin authentication.
        /// </summary>
        /// <param name="id">the redirection (status) id</param>
        /// <returns></returns>
        public LinkedInAuthorizationRedirectionData GetRedirectionById(Guid id)
        {
            if (id == null)
                throw new ArgumentNullException("The value cannot be null", "id");

            var url = string.Format(
                "{0}/Redirect/GetLinkedInAuth?guid={1}",
                this.baseUrl,
                id);

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "WebBase.SparkleStatusApi";
            request.Headers.Add("X-SparkleStatus-ApiKey", this.oldApiKey.ToString());
            request.ContentLength = 0L;

            string postContent = null;
            this.PutHash(request, postContent);

            return GetResponse<LinkedInAuthorizationRedirectionData>(request);
        }

        /// <summary>
        /// Make a forward geocode request.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public LocationGeolocData GetLocationGeolocFromCache(string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException("location");

            var url = string.Format(
                "{0}/Cache/GetLocationGeoloc?id={1}",
                this.baseUrl,
                Uri.EscapeDataString(location));

            var request = CreateRequest("POST", url);
            request.ContentLength = 0L;
            string postContent = null;
            this.PutHash(request, postContent);

            return GetResponse<LocationGeolocData>(request);
        }

        public object GetNetworksStatus()
        {
            var url = string.Format(
                "{0}/Networks/Status",
                this.baseUrl);

            var request = CreateRequest("GET", url);
            this.PutHash(request);

            var result = GetResponse<object>(request);
            return result;
        }

        private HttpWebRequest CreateRequest(string method, string url)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method ?? "GET";
            request.UserAgent = "WebBase.SparkleStatusApi";
            request.Accept = "application/json";
            request.Headers.Add("X-SparkleStatus-ApiKey", this.oldApiKey.ToString());
            return request;
        }

        private void Post(HttpWebRequest request, string postContent, string contentType)
        {
            var data = new MemoryStream(Encoding.UTF8.GetBytes(postContent));
            data.Seek(0L, SeekOrigin.Begin);
            request.ContentType = contentType + "; charset=utf-8";
            request.ContentLength = data.Length;

            var requestStream = request.GetRequestStream();
            data.CopyTo(requestStream);
        }

        private void PutHash(HttpWebRequest request)
        {
            this.PutHash(request, null);
        }

        private void PutHash(HttpWebRequest request, string postContent)
        {
            var time = DateTime.UtcNow.ToString("o");
            var payload = new NetworkStatusApiKeyPayload(this.apiKey, time, null, request.Method, request.RequestUri.PathAndQuery, postContent);
            var hash = payload.ComputeHash(this.apiSecret);
            request.Headers.Add(NetworkStatusApiKeyPayload.ApiKeyHeaderName, this.apiKey);
            request.Headers.Add(NetworkStatusApiKeyPayload.TimeHeaderName, time);
            request.Headers.Add(NetworkStatusApiKeyPayload.HashHeaderName, hash);
        }

        private static T GetResponse<T>(HttpWebRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            HttpWebResponse response;
            string json;
            BaseResponse<T> result = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var readStream = response.GetResponseStream();
                var reader = new StreamReader(readStream, Encoding.UTF8);
                json = reader.ReadToEnd();

                try
                {
                    result = JsonConvert.DeserializeObject<BaseResponse<T>>(json);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to read API response", ex);
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Error from API (HTTP " + (int)response.StatusCode + ")");
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;

                if (response == null)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }

                var headerErrors = response.GetResponseHeader("X-SparkleStatus-ErrorCodes");
                ex.Data["X-SparkleStatus-ErrorCodes"] = headerErrors;
                ex.Data["HttpCode"] = ((int)response.StatusCode) + " " + response.StatusCode;

                string message = "Error from API: " + ex.Message;
                if (!string.IsNullOrEmpty(headerErrors))
                {
                    message = "API request authentication errors: " + headerErrors;
                }

                throw new InvalidOperationException(message, ex);
            }

            if (result == null)
            {
                throw new InvalidOperationException("API responded with an empty reason");
            }

            return result.Data;
        }
    }
}
