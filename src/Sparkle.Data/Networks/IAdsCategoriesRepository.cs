
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository]
    public interface IAdsCategoriesRepository : IBaseNetworkRepository<AdCategory, int>
    {
        IList<AdCategory> GetAll(int networkId);

        AdCategory GetByIdFromCommonNetwork(int id, int networkId);
    }
}
