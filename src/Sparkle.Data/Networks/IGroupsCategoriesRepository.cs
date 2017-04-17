
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IGroupsCategoriesRepository : IBaseNetworkRepository<GroupCategory, int>
    {
        int Count();
    }
}