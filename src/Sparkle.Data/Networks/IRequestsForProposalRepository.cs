using Sparkle.Entities.Networks;
using System.Collections.Generic;

namespace Sparkle.Data.Networks
{
    [Repository]
    public interface IRequestsForProposalRepository : IBaseNetworkRepository<RequestForProposal, int>
    {
        IList<RequestForProposal> GetAll(int networkId);
    }
}
