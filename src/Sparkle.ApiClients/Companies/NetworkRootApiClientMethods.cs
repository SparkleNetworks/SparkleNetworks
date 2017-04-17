
namespace Sparkle.ApiClients.Companies
{
    using Sparkle.UnitTests.NetworkRootApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class NetworkRootApiClientMethods
    {
        public static async Task<BaseResponse<string[]>> GetCompanies(this NetworkRootApiClient client)
        {
            // WARNING: this method will change or will be removed.
            var request = client.CreateRequestV1(HttpMethod.Get, "Companies/All", null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<string[]>>(jsonOut);
            return obj;
        }

        public static async Task<BaseResponse<CompanyListModel>> SearchCompaniesAsync(
            this NetworkRootApiClient client,
            string keywords = null, string location = null,
            int[] tagIds = null, bool combinedTags = false,
            int offset = 0, int count = 20,
            bool includePlaces = false, bool includeTags = false, bool includeInactive = false, bool includeInvisible = false)
        {
            var args = new List<Tuple<string, string>>();
            args.Add(new Tuple<string, string>("Keywords", keywords));
            args.Add(new Tuple<string, string>("Location", location));
            args.Add(new Tuple<string, string>("Offset", offset.ToString()));
            args.Add(new Tuple<string, string>("Count", count.ToString()));
            args.Add(new Tuple<string, string>("IncludePlaces", includePlaces.ToString()));
            args.Add(new Tuple<string, string>("IncludeTags", includeTags.ToString()));
            args.Add(new Tuple<string, string>("IncludeInactive", includeInactive.ToString()));
            args.Add(new Tuple<string, string>("IncludeInvisible", includeInvisible.ToString()));
            if (tagIds != null)
            {
                args.Add(new Tuple<string, string>("CombinedTags", combinedTags.ToString()));
                for (int i = 0; i < tagIds.Length; i++)
                {
                    args.Add(new Tuple<string, string>("TagId", tagIds[i].ToString()));
                }
            }

            var url = "Companies/Search?" + client.CreateQueryString(args);
            var request = client.CreateRequestV1(HttpMethod.Get, url, null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<CompanyListModel>>(jsonOut);
            return obj;
        }

        public static async Task<BaseResponse<IList<CompanyCategoryApiModel>>> GetCompanyCategoriesAsync(this NetworkRootApiClient client)
        {
            var request = client.CreateRequestV1(HttpMethod.Get, "CompanyCategories/All", null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<IList<CompanyCategoryApiModel>>>(jsonOut);
            return obj;
        }
    }
}
