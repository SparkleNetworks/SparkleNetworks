
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ICompaniesVisitsRepository : IBaseNetworkRepository<CompaniesVisit, int>
    {
    }
}
