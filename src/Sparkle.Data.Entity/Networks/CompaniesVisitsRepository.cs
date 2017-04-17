
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class CompaniesVisitsRepository : BaseNetworkRepositoryInt<CompaniesVisit>, ICompaniesVisitsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompaniesVisitsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompaniesVisits)
        {
        }
    }
}
