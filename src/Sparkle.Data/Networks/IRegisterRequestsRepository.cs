
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System;

    [Repository]
    public interface IRegisterRequestsRepository : IBaseNetworkRepositoryNetwork<RegisterRequest, int>
    {
        RegisterRequest GetByEmailAddress(string value, int networkId);
        RegisterRequest GetByEmailAddress(string accountPart, string tagPart, string domainPart, int networkId);

        IList<RegisterRequest> GetPendingRequests(int networkId, RegisterRequestOptions options);

        IList<RegisterRequest> GetByStatus(RegisterRequestStatus status, int networkId);

        int CountPendingRequests(int networkId);

        int CountPendingRequestsByCompany(int companyId);

        RegisterRequest GetByCode(Guid id, int networkId);
        RegisterRequest GetByCode(Guid id, int networkId, RegisterRequestOptions options);

        IList<RegisterRequest> GetAllByCompany(int companyId, int networkId);

        ////IList<RegisterRequest> GetByInvitationId(int invitationId);
    }
}
