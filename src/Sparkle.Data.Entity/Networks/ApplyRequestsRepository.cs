
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ApplyRequestsRepository : BaseNetworkRepositoryInt<ApplyRequest>, IApplyRequestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ApplyRequestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ApplyRequests)
        {
        }

        public ApplyRequest GetByKey(Guid key, int networkId)
        {
            return this.Set
                .Where(o => !o.DateDeletedUtc.HasValue)
                .SingleOrDefault(r => r.Key == key && r.NetworkId == networkId);
        }

        public ApplyRequest GetById(int id, int networkId)
        {
            return this.Set
                .Where(o => !o.DateDeletedUtc.HasValue)
                .SingleOrDefault(r => r.Id == id && r.NetworkId == networkId);
        }

        public IList<ApplyRequest> GetPending(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .Where(r => r.DateSubmitedUtc != null && r.DateEmailConfirmedUtc != null
                    && r.DateRefusedUtc == null && r.DateAcceptedUtc == null)
                .OrderBy(r => r.DateSubmitedUtc)
                .ToList();
        }

        public IList<ApplyRequest> GetPendingWithCompanyCreate(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .Where(r => r.DateSubmitedUtc != null && r.DateEmailConfirmedUtc != null
                    && r.DateRefusedUtc == null && r.DateAcceptedUtc == null
                    && r.JoinCompanyId == null)
                .OrderBy(r => r.DateSubmitedUtc)
                .ToList();
        }

        public IList<ApplyRequest> GetPendingWithCompanyJoin(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .Where(r => r.DateSubmitedUtc != null && r.DateEmailConfirmedUtc != null
                    && r.DateRefusedUtc == null && r.DateAcceptedUtc == null
                    && r.JoinCompanyId != null)
                .OrderBy(r => r.DateSubmitedUtc)
                .ToList();
        }

        public IList<ApplyRequest> GetAll(int networkId, int offset, int pageSize)
        {
            var query = this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .OrderByDescending(r => r.DateSubmitedUtc);

            if (offset == 0 && pageSize == 0)
            {
                var items = query
                    .ToList();
                return items;
            }
            else
            {
                var items = query
                    .Skip(offset).Take(pageSize)
                    .ToList();
                return items;
            }
        }

        public int CountAll(int networkId)
        {
            var query = this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .OrderByDescending(r => r.DateSubmitedUtc);
            return query.Count();
        }

        public IList<ApplyRequest> GetByJoinCompanyId(int networkId, int companyId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .Where(o => o.JoinCompanyId.HasValue && o.JoinCompanyId.Value == companyId)
                .OrderByDescending(o => o.DateSubmitedUtc)
                .ToList();
        }

        public int CountByJoinCompanyId(int networkId, int companyId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => !o.DateDeletedUtc.HasValue)
                .Where(o => o.JoinCompanyId.HasValue && o.JoinCompanyId.Value == companyId)
                .Count();
        }

        public void DeleteAllNotSubmitted(int networkId)
        {
            this.Context.DeleteNotSubmittedApplyRequests(networkId);
        }

        public IList<ApplyRequest> GetByUserId(int userId)
        {
            return this.Set
                .Where(o => o.CreatedUserId == userId)
                .ToList();
        }

        public IList<ApplyRequest> GetByEmailAddress(string account, string tag, string domain)
        {
            throw new NotSupportedException("The email address is not yet stored in this table.");
        }

        public IList<ApplyRequest> FindByImportedId(int networkId, string likeQuery)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(x => SqlFunctions.PatIndex(likeQuery, x.ImportedId) >= 0)
                .ToList();
        }


    }
}
