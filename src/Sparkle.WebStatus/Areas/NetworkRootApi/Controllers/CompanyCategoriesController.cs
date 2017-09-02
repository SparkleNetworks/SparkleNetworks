
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
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
    public class CompanyCategoriesController : ApiController
    {
        [Route("CompanyCategories/All"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadCompanies")]
        public BaseResponse<IList<CompanyCategoryModel>> Get()
        {
            var items = this.Services.Company.GetAllCategories();
            return new BaseResponse<IList<CompanyCategoryModel>>(items);
        }
    }
}
