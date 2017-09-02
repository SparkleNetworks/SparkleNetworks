
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
    using Sparkle.Services.Networks.Models;
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
    public class TagDefinitionsController : ApiController
    {
        [Route("TagDefinitions/Search"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadTags")]
        public BaseResponse<IList<Tag2Model>> Search(
            string Keywords = null, string EntityName = null,
            bool CombinedTags = false,
            int Offset = 0, int Count = 20)
        {
            ////var items = this.Services.Tags.GetCategories();

            var categoryIds = new List<int>();
            foreach (var query in this.Request.GetQueryNameValuePairs())
            {
                if (query.Key.Equals("CategoryId"))
                {
                    categoryIds.Add(int.Parse(query.Value));
                }
            }

            ////this.Services.Tags.GetUsedEntityTags("Company");

            var result = this.Services.Tags.GetByNameAndCategory(categoryIds.ToArray(), Keywords);

            return new BaseResponse<IList<Tag2Model>>(result);
        }

        [Route("TagDefinitions/ByCategory"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadTags")]
        public BaseResponse<PagedListModel<Tag2Model>> ByCategory(
            int CategoryId,
            int Offset = 0, int Count = 20)
        {
            var result = this.Services.Tags.GetTagsByCategoryId(CategoryId, Offset, Count);

            return new BaseResponse<PagedListModel<Tag2Model>>(result);
        }
    }
}
