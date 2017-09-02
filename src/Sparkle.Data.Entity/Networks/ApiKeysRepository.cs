
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Filters;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class ApiKeysRepository : BaseNetworkRepositoryInt<ApiKey>, IApiKeysRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ApiKeysRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ApiKeys)
        {
        }


        public IList<ApiKey> GetAll()
        {
            return this.Set.ToList();
        }


        public ApiKey GetByKey(string key)
        {
            return this.Set
                .Where(x => x.Key == key)
                .SingleOrDefault();
        }
    }
}
