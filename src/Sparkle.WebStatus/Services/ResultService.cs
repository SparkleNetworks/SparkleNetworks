
namespace Sparkle.WebStatus.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Sparkle.WebStatus.Controllers;
    using SrkToolkit.Web;
    using SrkToolkit.Web.Services;

    /// <summary>
    /// Implementation of https://github.com/sandrock/SrkToolkit/blob/master/Wiki/SrkToolkit.Web-ResultAndErrors.md
    /// </summary>
    public class ResultService : ResultService<ErrorController>
    {
        static ResultService()
        {
            JsonNetResult.Serializer = (obj, response) =>
            {
                Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(response.Output)
                {
#if DEBUG
                    Formatting = Newtonsoft.Json.Formatting.Indented,
#endif
                };
                var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                };
                var serializer = Newtonsoft.Json.JsonSerializer.Create(serializerSettings);
                serializer.Serialize(writer, obj);
                writer.Flush();
            };
        }

        public ResultService(HttpContextBase httpContext)
            : base(httpContext)
        {
        }
    }
}