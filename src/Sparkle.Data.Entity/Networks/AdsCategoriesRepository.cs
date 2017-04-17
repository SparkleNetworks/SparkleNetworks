
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public class AdsCategoriesRepository : BaseNetworkRepositoryInt<AdCategory>, IAdsCategoriesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public AdsCategoriesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.AdCategories)
        {
        }

        public IList<AdCategory> GetAll(int networkId)
        {
            return this.Set.ByNetworkOrCommon(networkId).OrderBy(x => x.Name).ToList();
        }

        public AdCategory GetByIdFromCommonNetwork(int id, int networkId)
        {
            return this.Set.ByNetworkOrCommon(networkId).Where(x => x.Id == id).SingleOrDefault();
        }
    }
}
