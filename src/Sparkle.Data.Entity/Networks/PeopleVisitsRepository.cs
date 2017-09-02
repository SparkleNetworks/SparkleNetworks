
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class PeopleVisitsRepository : BaseNetworkRepositoryInt<UsersVisit>, IPeopleVisitsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PeopleVisitsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UsersVisits)
        {
        }
    }
}
