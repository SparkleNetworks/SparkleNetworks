
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TagCategoriesRepository : BaseNetworkRepositoryInt<TagCategory>, ITagCategoriesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TagCategoriesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TagCategories)
        {
        }

        public IList<TagCategory> GetAll()
        {
            return this.Set
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        public IList<TagCategory> GetAll(int networkId)
        {
            return this.Set
                .ByNetworkOrCommon(networkId)
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        public TagCategory GetByAlias(int networkId, string alias)
        {
            return this.Set
                .ByNetworkOrCommon(networkId)
                .Where(o => o.Alias == alias)
                .OrderBy(x => x.SortOrder)
                .SingleOrDefault();
        }

        public IList<TagCategory> GetByNonNullRules(int networkId)
        {
            return this.Set
                .ByNetworkOrCommon(networkId)
                .Where(o => o.Rules != null)
                .OrderBy(x => x.SortOrder)
                .ToList();
        }

        public IList<TagCategory> GetByIds(int[] ids)
        {
            return this.Set
                .Where(o => ids.Contains(o.Id))
                .ToList();
        }
    }
}
