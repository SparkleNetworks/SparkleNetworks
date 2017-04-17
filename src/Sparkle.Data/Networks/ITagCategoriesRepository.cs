
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ITagCategoriesRepository : IBaseNetworkRepository<TagCategory, int>
    {
        IList<TagCategory> GetAll();
        IList<TagCategory> GetAll(int networkId);

        TagCategory GetByAlias(int networkId, string alias);

        IList<TagCategory> GetByNonNullRules(int networkId);
        IList<TagCategory> GetByIds(int[] ids);
    }
}
