
namespace Sparkle.ApiClients.InformationNotes
{
    using Sparkle.UnitTests.NetworkRootApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class NetworkRootApiClientMethods
    {
        public static async Task<PagedListModel<InformationNoteModel>> GetInformationNotes(this NetworkRootApiClient client, int offset, int count)
        {
            var args = new List<Tuple<string, string>>();
            args.Add(new Tuple<string, string>("Offset", offset.ToString()));
            args.Add(new Tuple<string, string>("Count", count.ToString()));
            var request = client.CreateRequestV1(HttpMethod.Get, "InformationNotes/List?" + client.CreateQueryString(args), null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<PagedListModel<InformationNoteModel>>>(jsonOut);
            return obj.Data;
        }

        public static async Task<InformationNoteModel> GetInformationNoteById(this NetworkRootApiClient client, int id)
        {
            var request = client.CreateRequestV1(HttpMethod.Get, "InformationNotes/ById/" + id, null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<InformationNoteModel>>(jsonOut);
            return obj.Data;
        }

        public static async Task<EditInformationNoteRequest> PrepareEditInformationNote(this NetworkRootApiClient client, int? id)
        {
            var request = client.CreateRequestV1(HttpMethod.Get, "InformationNotes/PrepareEdit" + (id != null ? "/" + id : null), null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<EditInformationNoteRequest>>(jsonOut);
            return obj.Data;
        }

        public static async Task<EditInformationNoteResult> EditInformationNote(this NetworkRootApiClient client, EditInformationNoteRequest model)
        {
            var jsonIn = client.Serialize(model);
            var request = client.CreateRequestV1(HttpMethod.Post, "InformationNotes/Edit", jsonIn);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<EditInformationNoteResult>>(jsonOut);
            return obj.Data;
        }
    }
}
