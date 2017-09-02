
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks.Models;
    using Sparkle.WebBase.WebApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;

    [RoutePrefix(NetworkRootApiAreaRegistration.BasePath)]
    public class UsersController : ApiController
    {
        [Route("Users/GetFirst"), HttpGet]
        [CheckNetworkApiKeyAttribute(Roles = "ReadUsers")]
        public BaseResponse<UserModel> GetFirst()
        {
            var result = this.Services.People.GetFirst(PersonOptions.Company);
            return new BaseResponse<UserModel>(result);
        }
    }
}
