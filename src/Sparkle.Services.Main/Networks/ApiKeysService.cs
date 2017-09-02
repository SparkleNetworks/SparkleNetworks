
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class ApiKeysService : ServiceBase, IApiKeysService
    {
        [DebuggerStepThrough]
        internal ApiKeysService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }


        public IList<ApiKeyModel> GetAllRelatedToNetwork()
        {
            var items = this.Repo.ApiKeys.GetAll();
            var models = new List<ApiKeyModel>(items.Count);
            models.AddRange(items.Select(x => new ApiKeyModel(x)));
            return models;
        }


        public ApiKeyModel GetByKey(string key)
        {
            var item = this.Repo.ApiKeys.GetByKey(key);
            return item != null ? new ApiKeyModel(item) : null;
        }

        public EditApiKeyRequest GetEditRequest(int? id, EditApiKeyRequest request)
        {
            if (request == null)
            {
                request = new EditApiKeyRequest();
            }

            if (id != null)
            {
                var item = this.Repo.ApiKeys.GetById(id.Value);
                if (item == null)
                    return null;

                request.Name = item.Name;
                request.Description = item.Description;
                request.Id = item.Id;
                request.IsEnabled = item.IsEnabled;
                request.Roles = item.Roles;
            }

            return request;
        }

        public EditApiKeyResult Edit(EditApiKeyRequest request)
        {
            const string path = "ApiKeysService.Edit";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditApiKeyResult(request);

            if (!request.IsValid)
            {
                result.Errors.Add(EditApiKeyError.InvalidRequest, NetworksLabels.ResourceManager);
            }

            var tran = this.Repo.NewTransaction();
            using (tran.BeginTransaction())
            {
                ApiKey entity = null;
                var now = DateTime.UtcNow;
                if (request.Id != null)
                {
                    entity = tran.Repositories.ApiKeys.GetById(request.Id.Value);

                    if (entity == null)
                    {
                        result.Errors.Add(EditApiKeyError.NoSuchItem, NetworksLabels.ResourceManager);
                    }
                }
                else
                {
                    entity = new ApiKey();
                    entity.Key = this.GenerateKey();
                    entity.Secret = this.GenerateSecret();
                    entity.DateCreatedUtc = now;
                }

                if (result.Errors.Count > 0)
                {
                    return this.LogResult(result, path);
                }

                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.IsEnabled = request.IsEnabled;
                entity.Roles = request.Roles;

                if (request.Id != null)
                {
                }
                else
                {
                    tran.Repositories.ApiKeys.Attach(entity);
                }

                tran.CompleteTransaction();
                result.Succeed = true;
                result.Item = new ApiKeyModel(entity);

                return this.LogResult(result, path);
            }
        }

        public ApiKeyModel GetById(int id)
        {
            var item = this.Repo.ApiKeys.GetById(id);
            return item != null ? new ApiKeyModel(item) : null;
        }

        private string GenerateKey()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        private string GenerateSecret()
        {
            return GetCryptoRandomHexString(48);
        }

        private static string GetCryptoRandomHexString(int characters)
        {
            var size = (int)Math.Ceiling((double)characters / 2D);
            var array = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(array);
            }

            var builder = new StringBuilder(characters);
            for (int i = 0; i < array.Length; i++)
            {
                builder.Append(array[i].ToString("x2"));
            }

            return builder.ToString();
        }

    }
}
