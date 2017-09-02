
namespace Sparkle.ApiClients.Tags
{
    using Sparkle.ApiClients.Common;
    using Sparkle.UnitTests.NetworkRootApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class NetworkRootApiClientMethods
    {
        public static async Task<BaseResponse<IList<TagCategoryModel>>> GetAllTagCategoriesAsync(this NetworkRootApiClient client)
        {
            var request = client.CreateRequestV1(HttpMethod.Get, "TagCategories/All", null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<IList<TagCategoryModel>>>(jsonOut);
            return obj;
        }

        public static async Task<BaseResponse<IList<Tag2Model>>> SearchTagDefinitionsAsync(
            this NetworkRootApiClient client,
            string keywords = null,
            string entityName = null,
            int[] categoryIds = null, bool combinedTags = false,
            int offset = 0, int count = 20)
        {
            var args = new List<Tuple<string, string>>();
            args.Add(new Tuple<string, string>("Keywords", keywords));
            args.Add(new Tuple<string, string>("EntityName", entityName));
            args.Add(new Tuple<string, string>("Offset", offset.ToString()));
            args.Add(new Tuple<string, string>("Count", count.ToString()));
            if (categoryIds != null)
            {
                for (int i = 0; i < categoryIds.Length; i++)
                {
                    args.Add(new Tuple<string, string>("CategoryId", categoryIds[i].ToString()));
                }
            }

            var url = "TagDefinitions/Search?"
                + string.Join("&", args.Where(a => a.Item2 != null).Select(a => a.Item1 + "=" + Uri.EscapeDataString(a.Item2)));
            var request = client.CreateRequestV1(HttpMethod.Get, url, null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<IList<Tag2Model>>>(jsonOut);
            return obj;
        }

        public static async Task<BaseResponse<PagedListModel<Tag2Model>>> GetTagDefinitionsByCategoryPostAsync(
            this NetworkRootApiClient client,
            int categoryId,
            int offset = 0, int count = 20)
        {
            var args = new List<Tuple<string, string>>();
            args.Add(new Tuple<string, string>("CategoryId", categoryId.ToString()));
            args.Add(new Tuple<string, string>("Offset", offset.ToString()));
            args.Add(new Tuple<string, string>("Count", count.ToString()));

            var url = "TagDefinitions/ByCategory?"
                + string.Join("&", args.Where(a => a.Item2 != null).Select(a => a.Item1 + "=" + Uri.EscapeDataString(a.Item2)));
            var request = client.CreateRequestV1(HttpMethod.Get, url, null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<PagedListModel<Tag2Model>>>(jsonOut);
            return obj;
        }

        public static async Task<BaseResponse<PagedListModel<Tag2Model>>> GetTagDefinitionsByCategoryGetAsync(
            this NetworkRootApiClient client,
            int categoryId,
            int offset = 0, int count = 20)
        {
            var args = new List<Tuple<string, string>>();
            args.Add(new Tuple<string, string>("CategoryId", categoryId.ToString()));
            args.Add(new Tuple<string, string>("Offset", offset.ToString()));
            args.Add(new Tuple<string, string>("Count", count.ToString()));

            var url = "TagDefinitions/ByCategory?"
                + string.Join("&", args.Where(a => a.Item2 != null).Select(a => a.Item1 + "=" + Uri.EscapeDataString(a.Item2)));
            var request = client.CreateRequestV1(HttpMethod.Get, url, null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<PagedListModel<Tag2Model>>>(jsonOut);
            return obj;
        }
    }
}
