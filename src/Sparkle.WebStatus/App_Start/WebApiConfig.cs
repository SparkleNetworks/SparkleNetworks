
namespace Sparkle.WebStatus
{
    using Sparkle.WebBase.WebApi;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            GlobalConfiguration.Configure(x => x.MapHttpAttributeRoutes());

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            // Handle errors ourselves
            config.Filters.Add(new GlobalApiExceptionFilterAttribute());

#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#else
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;
#endif
        }

    }
}
