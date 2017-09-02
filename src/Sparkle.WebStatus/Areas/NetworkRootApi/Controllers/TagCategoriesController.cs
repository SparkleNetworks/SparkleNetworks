
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
    public class TagCategoriesController : ApiController
    {
        [Route("TagCategories/All"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadTags")]
        public BaseResponse<IList<TagCategoryModel>> GetAll()
        {
            var items = this.Services.Tags.GetCategories();
            return new BaseResponse<IList<TagCategoryModel>>(items);
        }
    }
}