
namespace Sparkle.WebStatus.Domain
{
    using Sparkle.NetworksStatus.Domain.Cache;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public static class DomainHttpContextExtensions
    {
        private const string ConfigKey = "SparkleStatusConfiguration";
        private const string InstanceKey = "SparkleStatusServices";

        public static ServiceFactory GetStatusLocalServices(this HttpContext http, bool forceNew)
        {
            var instance = GetStatusServices(http.Items[InstanceKey] as ServiceFactory, forceNew, http.Server.MapPath("~/App_Data/Services"), http.Cache);
            http.Items[InstanceKey] = instance;
            var protocol = http.Request.IsSecureConnection ? "https" : "http";
            instance.Context.WebsitePath = string.Format("{0}://{1}:{2}", protocol, http.Request.Url.Host, http.Request.Url.Port);
            ////instance.Context.WebsiteAuthTokenPath = http.Url.Action("AuthToken", new { id = "ID", }).Replace("ID", "{0}");
            return instance;
        }

        public static ServiceFactory GetStatusLocalServices(this HttpContextBase http, bool forceNew)
        {
            var instance = GetStatusServices(http.Items[InstanceKey] as ServiceFactory, forceNew, http.Server.MapPath("~/App_Data/Services"), http.Cache);
            http.Items[InstanceKey] = instance;
            var protocol = http.Request.IsSecureConnection ? "https" : "http";
            instance.Context.WebsitePath = string.Format("{0}://{1}:{2}", protocol, http.Request.Url.Host, http.Request.Url.Port);
            ////instance.Context.WebsiteAuthTokenPath = http.Url.Action("AuthToken", new { id = "ID", }).Replace("ID", "{0}");
            return instance;
        }

        private static ServiceFactory GetStatusServices(ServiceFactory instance, bool forceNew, string dataPath, System.Web.Caching.Cache cache)
        {
            if (instance != null && !forceNew)
            {
                return instance;
            }

            var config = new ServiceConfiguration()
            {
                DataDirectory = dataPath,
                SmtpConfiguration = ConfigurationManager.AppSettings["SmtpConfiguration"],
            };

            instance = new ServiceFactory(config);////, new AspnetDomainCacheProvider(cache));
            return instance;
        }
    }
}
