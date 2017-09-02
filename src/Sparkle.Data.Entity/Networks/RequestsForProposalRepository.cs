
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public class RequestsForProposalRepository : BaseNetworkRepositoryInt<RequestForProposal>, IRequestsForProposalRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public RequestsForProposalRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.RequestsForProposal)
        {
        }

        public System.Collections.Generic.IList<RequestForProposal> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }
    }
}
