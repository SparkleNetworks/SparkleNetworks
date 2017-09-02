
namespace Sparkle.WebStatus.Areas.NetworkRootApi.Controllers
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks.Companies;
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
    public class CompaniesController : ApiController
    {
        [Route("Companies/All"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadCompanies")]
        public BaseResponse<string[]> Get()
        {
            return new BaseResponse<string[]>(new string[] { "value1", "value2", });
        }

        [Route("Companies/Search"), HttpGet, HttpPost]
        [CheckNetworkApiKeyAttribute(Roles = "ReadCompanies")]
        public BaseResponse<CompanyListModel> Search(
            string Keywords = null, string Location = null,
            /*int[] TagIds = null, */bool CombinedTags = false,
            int Offset = 0, int Count = 20,
            bool IncludePlaces = false, bool IncludeTags = false, bool IncludeInactive = false, bool IncludeInvisible = false)
        {
            CompanyOptions options = CompanyOptions.None;
            if (IncludePlaces)
                options = options | CompanyOptions.Places;
            if (IncludeTags)
                options = options | CompanyOptions.Tags;

            var tagIds = new List<int>();
            foreach (var query in this.Request.GetQueryNameValuePairs())
            {
                if (query.Key.Equals("TagId"))
                {
                    tagIds.Add(int.Parse(query.Value));
                }
            }

            var result = this.Services.Company.Search(Keywords, Location, tagIds.Count > 0 ? tagIds.ToArray() : null, CombinedTags, Offset, Count, options);
            return new BaseResponse<CompanyListModel>(result);
        }
    }
}
