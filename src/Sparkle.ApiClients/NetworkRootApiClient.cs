
namespace Sparkle.ApiClients
{
    using Newtonsoft.Json;
    using Sparkle.ApiClients.Common;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class NetworkRootApiClient
    {
        private const string TimeFormat = "yyyyMMdd'T'HHmmssffff'Z'";
        private readonly HttpClient client = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(5D),
        };
        private readonly SHA256Managed sha256 = new SHA256Managed();
        private readonly string baseUrl;
        private bool verifyResponseSignature;
        private bool isDisposed;
        private IList<CultureInfo> acceptLanguages;
        private string networkDomainName;
        private string networkName;

        public NetworkRootApiClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string NetworkName
        {
            get { return this.networkName; }
            set
            {
                this.networkName = value;
                if (value != null)
                {
                    this.networkDomainName = null;
                }
            }
        }

        public string NetworkDomainName
        {
            get { return this.networkDomainName; }
            set
            {
                this.networkDomainName = value;
                if (value != null)
                {
                    this.networkName = null;
                }
            }
        }

        public bool VerifyResponseSignature
        {
            get { return this.verifyResponseSignature; }
            set { this.verifyResponseSignature = value; }
        }

        public DateTime? Time { get; set; }

        public IList<CultureInfo> AcceptLanguages
        {
            get { return this.acceptLanguages; }
            set { this.acceptLanguages = value; }
        }

        public async Task<BaseResponse<string>> GetTimeAsync()
        {
            // prepare a HTTP request. add the headers and sign it.
            var request = this.CreateRequestV1(HttpMethod.Get, "Util/Time", null);

            // send the request and get a response from the server.
            var response = await this.SendAsync(request);

            // try to read the content as string. throw an exception if the code != 200.
            var jsonOut = await this.ReadResponseV1(response);

            // deserialize the JSON text content into an object.
            var obj = JsonConvert.DeserializeObject<BaseResponse<string>>(jsonOut);

            // and we're done.
            return obj;
        }

        public async Task<BaseResponse<string>> CheckApiKeyAsync()
        {
            var request = this.CreateRequestV1(HttpMethod.Get, "Util/CheckApiKey", null);
            var response = await this.SendAsync(request);
            var jsonOut = await this.ReadResponseV1(response);
            var obj = JsonConvert.DeserializeObject<BaseResponse<string>>(jsonOut);
            return obj;
        }

        public async Task Sample500Async()
        {
            var request = this.CreateRequestV1(HttpMethod.Get, "Util/Sample500", null);
            var response = await this.SendAsync(request);
            var jsonOut = await this.ReadResponseV1(response);
            var obj = JsonConvert.DeserializeObject<BaseResponse<string>>(jsonOut);
        }

        public async Task<string> RawQueryP1(HttpMethod method, string path, string json)
        {
            return await this.RawQueryP1(method, path, json, null, null);
        }

        public async Task<string> RawQueryP1(HttpMethod method, string path, string json, string identityToken, string identitySecret)
        {
            var request = this.CreateRequestV1(method, path, json, null, null);
            try
            {
                var authResponse = await this.SendAsync(request);
                var jsonOut = await this.ReadResponseV1(authResponse);
                return jsonOut;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetType().Name + " " + ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Stuff

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.client.Dispose();
                }

                this.isDisposed = true;
            }
        }

        internal string CreateQueryString(IEnumerable<Tuple<string, string>> collection)
        {
            return string.Join("&", collection.Where(a => a.Item2 != null).Select(a => a.Item1 + "=" + Uri.EscapeDataString(a.Item2)));
        }

        /// <summary>
        /// Creates an anonymous request using protocol $1$.
        /// </summary>
        internal HttpRequestMessage CreateRequestV1(HttpMethod method, string path, string content)
        {
            return this.CreateRequestV1(method, path, content, null, null);
        }

        /// <summary>
        /// Creates a user-authenticated request using protocol $1$.
        /// </summary>
        internal HttpRequestMessage CreateRequestV1(HttpMethod method, string path, string content, string identityToken, string identitySecret)
        {
            var request = new HttpRequestMessage(method, this.baseUrl + path);
            var now = this.Time ?? DateTime.UtcNow;
            string time = now.ToString(TimeFormat);
            string prehash = string.Join(
                "\n",
                this.ApiKey,         // "ak_..."
                this.ApiSecret,      // "as_..."
                identityToken,       // empty or "ik_..."
                identitySecret,      // empty or "is_..."
                request.Method.ToString().ToUpperInvariant(), // "GET", "POST"...
                request.RequestUri.PathAndQuery,              // "/api/Do/Something..."
                content,                                      // the POST content
                time);                                        // "20160223T1234560000Z"
            byte[] bytes = Encoding.UTF8.GetBytes(prehash);   // the string is encoded as UTF-8, returns the corresponding bytes
            byte[] hash = this.sha256.ComputeHash(bytes);     // hash using SHA-256

            // BitConverter.ToString(hash) converts bytes to hexadecimal string representation (84-D2-E5-02-F4-...)
            // the dashes are not desired. theyr are removed using the Replace method
            // the hex string is prefixed with $1$
            string signature = "$1$" + BitConverter.ToString(hash).Replace("-", "");

            request.Headers.Add("Accept", "application/json");
            request.Headers.UserAgent.ParseAdd("Sparkle.ApiClients.NetworkRootApiClient/1");
            request.Headers.Add("X-SparkleNetworksApi-NetworkName", this.NetworkName);
            request.Headers.Add("X-SparkleNetworksApi-NetworkDomainName", this.NetworkDomainName);
            request.Headers.Add("X-SparkleNetworksApi-Key", this.ApiKey);
            request.Headers.Add("X-SparkleNetworksApi-Time", time);
            request.Headers.Add("X-SparkleNetworksApi-Hash", signature);

            if (identityToken != null)
            {
                // IGNORE this part (this is for the authenticated API)
                request.Headers.Add("X-SparkleNetworksApi-Identity", identityToken);
            }

            if (this.acceptLanguages != null)
            {
                // IGNORE this part (this is not implemented yet)
                foreach (var item in this.acceptLanguages)
                {
                    request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(item.Name));
                }
            }

            if (content != null)
            {
                // if there is HTTP content to be posted, put it in the request
                var baContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
                baContent.Headers.Add("Content-Type", "application/json; charset=utf-8");
                request.Content = baContent;
            }

            return request;
        }

        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            try
            {
                var response = await this.client.SendAsync(request);
                return response;
            }
            catch (Exception exception)
            {
                var ex = new NetworkRootApiException("Failed to serialize request: " + exception.Message, exception);
                ex.TransportError = exception.Message;
                throw ex;
            }
        }

        internal async Task<string> ReadResponseV1(HttpResponseMessage response)
        {
            // handle server errors
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                BaseResponse<BaseResult> error = null;
                if (errorContent != null && errorContent.Length > 2)
                {
                    // there is a high probability we have a json error content here. parse it.
                    try
                    {
                        error = JsonConvert.DeserializeObject<BaseResponse<BaseResult>>(errorContent);
                    }
                    catch (Exception jsonException)
                    {
                        // TODO: catch the right exception 
                        Trace.WriteLine("Sparkle.ApiClients.NetworkRootApiClient.ReadResponse: jsonException = " + jsonException.ToString());
                    }
                }

                string message = "Request failed with error code " + ((int)response.StatusCode) + " " + response.StatusCode + ". ";
                if (response.StatusCode == HttpStatusCode.BadRequest && error != null)
                {
                    if (error.ModelState != null && error.ModelState.Count > 0)
                    {
                        error.ErrorCode = error.ErrorCode ?? "InvalidMessage";
                        message = "The request has validation errors. (" + message + ")";
                    }
                    else if (error.Data != null && error.Data.Errors != null && error.Data.Errors.Count > 0)
                    {
                        message = "Service errors: " + string.Join("; ", error.Data.Errors.Select(x => x.Code + " " + x.Detail));
                    }
                    else
                    {
                        message = "Service error: " + (error.Message ?? error.ErrorCode);
                    }
                }
                else if (error != null)
                {
                    message += "Service error: " + (error.Message ?? error.ErrorCode);
                }

                var ex = new NetworkRootApiException(message);
                ex.TransportError = "HTTP " + (int)response.StatusCode + " " + response.StatusCode.ToString();
                ex.Data.Add("HttpCode", (int)response.StatusCode);
                ex.Data.Add("Content", errorContent);
                ex.Error = error;

                if (error != null && error.ErrorMessage != null)
                {
                    ex.ServiceError = error.ErrorCode + ": " + error.ErrorMessage;
                }

                throw ex;
            }

            var contentBytes = await response.Content.ReadAsByteArrayAsync();
            var contentString = Encoding.UTF8.GetString(contentBytes, 0, contentBytes.Length);

            // the server is supposed to sign its response
            // the client is supposed to verify the server's signature
            // but this is not implemented now
            if (this.verifyResponseSignature)
            {
                // IGNORE this part
                var time = response.Headers.GetValues("X-SparkleNetworksApi-Time").Single();
                string prehash = string.Join(
                    "\n",
                    this.ApiKey,         // "ak_..."
                    this.ApiSecret,      // "as_..."
                    string.Empty,        // empty or "ik_..."
                    string.Empty,        // empty or "is_..."
                    response.RequestMessage.Method.ToString().ToUpperInvariant(), // "GET", "POST"...
                    response.RequestMessage.RequestUri.PathAndQuery,              // "/api/Do/Something..."
                    contentString,                                      // the request content
                    time);                                        // "20160223T1234560000Z"
                var bytes = Encoding.UTF8.GetBytes(prehash);
                var hash = this.sha256.ComputeHash(bytes);
                var signature = "$1$" + BitConverter.ToString(hash).Replace("-", "");
                var serverSignature = response.Headers.GetValues("X-SparkleNetworksApi-Hash").Single();

                if (!signature.Equals(serverSignature, StringComparison.OrdinalIgnoreCase))
                {
                    var ex = new NetworkRootApiException("Server provided signature '" + serverSignature + "' does not match computed signature '" + signature + "'.");
                    throw ex;
                }
            }

            return contentString;
        }

        internal string Serialize<T>(T obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (JsonException jsonException)
            {
                var ex = new NetworkRootApiException("Failed to serialize request: " + jsonException.Message, jsonException);
                ex.LocalError = jsonException.Message;
                throw ex;
            }
        }

        internal T Deserialize<T>(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(json);
                return obj;
            }
            catch (JsonException jsonException)
            {
                var ex = new NetworkRootApiException("Failed to unserialize result: " + jsonException.Message, jsonException);
                ex.LocalError = jsonException.Message;
                ex.Data.Add("Content", json);
                throw ex;
            }
        }

        #endregion
    }
}
