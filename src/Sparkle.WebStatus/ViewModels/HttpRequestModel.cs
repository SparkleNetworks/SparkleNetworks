
namespace Sparkle.WebStatus.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class HttpRequestModel
    {
        private List<string> messages;
        private List<string> files;

        public static HttpRequestModel Create(HttpRequestBase httpRequest)
        {
            return new HttpRequestModel
            {
                Id = Guid.NewGuid(),
                DateUtc = DateTime.UtcNow,
                Method = httpRequest.HttpMethod,
                Path = httpRequest.RawUrl,
                Headers = string.Join(
                    Environment.NewLine,
                    httpRequest.Headers.AllKeys.Select(k => k + ": " + httpRequest.Headers[k])),
            };
        }

        public DateTime DateUtc { get; set; }

        public string Method { get; set; }

        public string Path { get; set; }

        public string Headers { get; set; }

        public string PostContent { get; set; }

        public Guid Id { get; set; }

        public byte[] PostBytes { get; set; }

        public List<string> Messages
        {
            get { return this.messages ?? (this.messages = new List<string>()); }
        }

        public List<string> Files
        {
            get { return this.files ?? (this.files = new List<string>()); }
        }
    }
}