
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using SrkToolkit.Common.Validation;
    using System.Data.Objects;

    public class RegisterRequestsRepository : BaseNetworkRepositoryIntNetwork<RegisterRequest>, IRegisterRequestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public RegisterRequestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.RegisterRequests)
        {
        }

        public RegisterRequest GetByEmailAddress(string value, int networkId)
        {
            var item = new EmailAddress(value);
            return this.GetByEmailAddress(item.AccountPart, item.TagPart, item.DomainPart, networkId);
        }

        public RegisterRequest GetByEmailAddress(string accountPart, string tagPart, string domainPart, int networkId)
        {
            if (tagPart == null)
            {
                return this.Set
                    .ByNetwork(networkId)
                    .Where(r => r.EmailAccountPart == accountPart && r.EmailTagPart == null && r.EmailDomain == domainPart)
                    .SingleOrDefault();
            }
            else
            {
                return this.Set
                    .ByNetwork(networkId)
                    .Where(r => r.EmailAccountPart == accountPart && r.EmailTagPart == tagPart && r.EmailDomain == domainPart)
                    .SingleOrDefault();
            }
        }

        public IList<RegisterRequest> GetPendingRequests(int networkId, RegisterRequestOptions options)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(r => (r.Status == 0 || r.Status == 1)
                            && r.Company.IsEnabled)
                .OrderBy(r => r.DateCreatedUtc)
                .ToList();
        }

        public IList<RegisterRequest> GetByStatus(RegisterRequestStatus status, int networkId)
        {
            var intStatus = (int)status;
            return this.Set
                .ByNetwork(networkId)
                .Where(r => r.Status == intStatus
                        && r.Company.IsEnabled)
                .OrderBy(r => r.DateCreatedUtc)
                .ToList();
        }

        public int CountPendingRequests(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(r => r.Status == 0 || r.Status == 1)
                .Count();
        }

        public int CountPendingRequestsByCompany(int companyId)
        {
            return this.Set
                .Where(r => r.CompanyId == companyId)
                .Where(r => r.Status == 0 || r.Status == 1)
                .Count();
        }

        public RegisterRequest GetByCode(Guid id, int networkId)
        {
            return this.Set
                .SingleOrDefault(r => r.Code == id && r.NetworkId == networkId);
        }

        public RegisterRequest GetByCode(Guid id, int networkId, RegisterRequestOptions options)
        {
            return this.NewQuery(options)
                .SingleOrDefault(r => r.Code == id && r.NetworkId == networkId);
        }

        public IQueryable<RegisterRequest> NewQuery(RegisterRequestOptions options)
        {
            ObjectQuery<RegisterRequest> query = this.Set;
            
            if ((options & RegisterRequestOptions.Company) == RegisterRequestOptions.Company)
                query = query.Include("Company");

            if ((options & RegisterRequestOptions.AcceptedInvitation) == RegisterRequestOptions.AcceptedInvitation)
                query = query.Include("AcceptedInvitation");

            if ((options & RegisterRequestOptions.ValidatedBy) == RegisterRequestOptions.ValidatedBy)
                query = query.Include("ValidatedBy");

            return query;
        }

        public IList<RegisterRequest> GetAllByCompany(int companyId, int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(r => r.CompanyId == companyId)
                .OrderBy(r => r.DateCreatedUtc)
                .OrderBy(r => r.Status)
                .ToList();
        }

        protected override void OnUpdateOverride(NetworksEntities model, RegisterRequest itemToUpdate)
        {
            itemToUpdate.DateUpdatedUtc = DateTime.UtcNow;

            base.OnUpdateOverride(model, itemToUpdate);
        }

        protected override void OnInsertOverride(NetworksEntities model, RegisterRequest itemToInsert)
        {
            itemToInsert.DateCreatedUtc = DateTime.UtcNow;

            base.OnInsertOverride(model, itemToInsert);
        }
    }
}
