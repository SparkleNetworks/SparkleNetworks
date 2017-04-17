
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IApiKeysService
    {
        IList<ApiKeyModel> GetAllRelatedToNetwork();

        ApiKeyModel GetByKey(string key);

        EditApiKeyRequest GetEditRequest(int? id, EditApiKeyRequest request);

        EditApiKeyResult Edit(EditApiKeyRequest request);

        ApiKeyModel GetById(int id);
    }
}
