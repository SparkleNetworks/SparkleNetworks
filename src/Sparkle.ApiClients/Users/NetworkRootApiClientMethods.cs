
namespace Sparkle.ApiClients.Users
{
    using Sparkle.UnitTests.NetworkRootApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class NetworkRootApiClientMethods
    {
        public static async Task<UserModel> GetFirstUser(this NetworkRootApiClient client)
        {
            var request = client.CreateRequestV1(HttpMethod.Get, "Users/GetFirst", null);
            var response = await client.SendAsync(request);
            var jsonOut = await client.ReadResponseV1(response);
            var obj = client.Deserialize<BaseResponse<UserModel>>(jsonOut);
            return obj.Data;
        }
    }
}
