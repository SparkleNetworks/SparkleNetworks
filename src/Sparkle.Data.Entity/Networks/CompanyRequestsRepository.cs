
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public class CompanyRequestsRepository : BaseNetworkRepositoryInt<CompanyRequest>, ICompanyRequestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyRequestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyRequests)
        {
        }

        public CompanyRequest GetByUniqueId(Guid id)
        {
            return this.Set
                .Include("CompanyCategory")
                .SingleOrDefault(r => r.UniqueId == id);
        }

        public CompanyRequest GetByUniqueId(int networkId, Guid id)
        {
            return this.Set
                .Include("CompanyCategory")
                .SingleOrDefault(r => r.NetworkId == networkId && r.UniqueId == id);
        }

        public IList<CompanyRequest> GetPending(int networkId)
        {
            return this.Set
                .Include("CompanyCategory")
                .Where(r => r.NetworkId == networkId && r.Approved == null)
                .OrderBy(r => r.CreatedDateUtc)
                .ToList();
        }

        public IList<CompanyRequest> GetAllTimeDescending(int networkId)
        {
            return this.Set
                .Include("CompanyCategory")
                .ByNetwork(networkId)
                .OrderByDescending(r => r.CreatedDateUtc)
                .ToList();
        }

        public int CountPending(int networkId)
        {
            return this.Set
                .Include("CompanyCategory")
                .Where(r => r.NetworkId == networkId && r.Approved == null)
                .Count();
        }

        public IList<CompanyRequest> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }
    }
}