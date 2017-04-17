using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IRequestsForProposalService
    {
        int Add(RequestForProposal request);

        IList<RequestForProposal> GetAll();
    }
}
