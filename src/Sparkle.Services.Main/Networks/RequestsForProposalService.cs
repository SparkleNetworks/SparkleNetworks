using System.Collections.Generic;
using System.Linq;
using Sparkle.Data.Networks;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;

namespace Sparkle.Services.Main.Networks
{
    public class RequestsForProposalService : ServiceBase, IRequestsForProposalService
    {
        internal RequestsForProposalService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public int Add(RequestForProposal item)
        {
            this.SetNetwork(item);

            return this.Repo.RequestsForProposal.Insert(item).Id;
        }

        public IList<RequestForProposal> GetAll()
        {
            return this.Repo.RequestsForProposal
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .OrderByDescending(o => o.Deadline)
                .ToList();
        }
    }
}
