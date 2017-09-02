
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
    using Sparkle.Services.Networks.Tags;
    using Sparkle.WebBase.WebApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;

    [RoutePrefix(NetworkRootApiAreaRegistration.BasePath)]
    public class UtilController : ApiController
    {
        [HttpGet, Route("Util/Time")]
        public BaseResponse<string> Time()
        {
            return new BaseResponse<string>(DateTime.UtcNow.ToString("o"));
        }

        [HttpGet, Route("Util/CheckApiKey")]
        [CheckNetworkApiKeyAttribute]
        public BaseResponse<string> CheckApiKey()
        {
            return new BaseResponse<string>("If you see this message, then you have passed the API key check.");
        }

        [HttpGet, Route("Util/Sample500")]
        public BaseResponse<string> Sample500()
        {
            throw new InvalidOperationException("Sample exception");
        }
    }
}
