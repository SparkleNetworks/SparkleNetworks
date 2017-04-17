
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ICompanyRequestsRepository : IBaseNetworkRepository<CompanyRequest, int>
    {
        CompanyRequest GetByUniqueId(Guid id);
        CompanyRequest GetByUniqueId(int networkId, Guid id);

        IList<CompanyRequest> GetPending(int networkId);

        IList<CompanyRequest> GetAllTimeDescending(int networkId);

        int CountPending(int networkId);

        IList<CompanyRequest> GetAll(int networkId);
    }
}