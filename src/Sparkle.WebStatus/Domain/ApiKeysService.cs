
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class ApiKeysService : BaseService<ApiKeyModel>
    {
        public ApiKeysService(ServiceConfiguration configuration)
            : base(configuration, "ApiKeys")
        {
        }

        public IList<ApiKeyModel> GetAll()
        {
            return this.Read().Values.ToList();
        }


        internal ApiKeyModel Get(Guid guid)
        {
            var items = this.Read();
            return items.ContainsKey(guid) ? items[guid] : null;
        }

        internal ApiKeyModel Update(ApiKeyModel request)
        {
            using (var tran = this.Write())
            {
                ApiKeyModel item;
                if (tran.Items.ContainsKey(request.Guid))
                {
                    item = tran.Items[request.Guid];
                }
                else
                {
                    item = new ApiKeyModel();
                    item.DateCreatedUtc = DateTime.UtcNow;
                    item.Guid = Guid.NewGuid();
                    item.Key = Guid.NewGuid().ToString().Replace("-", "");
                    item.Secret = Guid.NewGuid().ToString().Replace("-", "");
                    item.IsEnabled = true;
                    tran.Items.Add(item.Guid, item);
                }

                item.UpdateFrom(request);

                tran.Save();
                return item;
            }
        }

        internal ApiKeyModel GetByKey(string key)
        {
            return this.Read().Values.SingleOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }
    }
}
