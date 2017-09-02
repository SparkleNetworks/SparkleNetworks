
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Newtonsoft.Json;
    using Sparkle.NetworksStatus.Domain.Cache;
    using Sparkle.NetworksStatus.Domain.Internals;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    public class CacheService : BaseService, IDisposable, ICacheService
    {
        private readonly IDomainCacheProvider cache;

        private HttpClient httpClient;

        [DebuggerStepThrough]
        internal CacheService(IServiceFactoryEx serviceFactory, IDomainCacheProvider cache)
            : base(serviceFactory)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            this.cache = cache;
        }

        public IpAddressInfo GetIpAddressInfo(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentException("The value cannot be empty", "ipAddress");

            const string cacheKeyBase = "StatusDomain.CacheService.GetIpAddressInfo/";
            var cacheKey = cacheKeyBase+ipAddress;

            {
                var item = this.cache.GetObject<IpAddressInfo>(cacheKey);
                if (item != null)
                    return item;
            }

            var httpClient = this.httpClient ?? (this.httpClient = new HttpClient());
            var request = new HttpRequestMessage(HttpMethod.Get, "http://www.freegeoip.net/json/" + Uri.EscapeDataString(ipAddress));
            var response = httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                var stream = response.Content.ReadAsStreamAsync().Result;
                var json = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                var obj = JsonConvert.DeserializeObject<IpAddressInfo>(json);

                this.cache.Add(cacheKey, obj, TimeSpan.FromHours(1D));

                return obj;
            }
            else
            {
                throw new InvalidOperationException("API returned HTTP code " + (int)response.StatusCode + " " + response.StatusCode);
            }
        }
    }
}
