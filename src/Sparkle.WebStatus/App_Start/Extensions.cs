
namespace Sparkle.WebStatus
{
    using Status = Sparkle.NetworksStatus.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Sparkle.NetworksStatus.Domain.Cache;

    public static class Extensions
    {
        public static Status.ServiceFactory GetStatusServices(this HttpContextBase context)
        {
            const string key = "StatusServices";
            if (context == null)
                throw new ArgumentNullException("context");

            var item = (Status.ServiceFactory)context.Items[key];
            if (item == null)
            {
                ////var configuration = Status.ServicesConfiguration.FromAppConfig("WebStatus");
                var jsonFilePath = context.Server.MapPath("~/App_Data/ServicesConfiguration.json");
                var configuration = Status.ServicesConfiguration.FromJson(System.IO.File.ReadAllText(jsonFilePath));
                item = new Status.ServiceFactory(configuration, new AspnetDomainCacheProvider(context.Cache));
                context.Items[key] = item;
            }

            return item;
        }
    }
}