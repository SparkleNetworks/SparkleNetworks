
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class CreateNetworkRequestsService : ServiceBase, ICreateNetworkRequestsService
    {
        [DebuggerStepThrough]
        internal CreateNetworkRequestsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ICreateNetworkRequestsRepository cnrRepository
        {
            get { return this.Repo.CreateNetworkRequests; }
        }

        public CreateNetworkRequest Insert(CreateNetworkRequest item)
        {
            this.SetNetwork(item);

            return this.cnrRepository.Insert(item);
        }

        public CreateNetworkRequest Create(CreateNetworkRequest item)
        {
            this.SetNetwork(item);

            item = this.cnrRepository.Insert(item);
            
            try
            {
                var messages = new List<string>()
                {
                    "new CreateNetworkRequest of id " + item.Id,
                };
                this.Services.Email.SendTechnicalMessages(messages, new NetworkAccessLevel[] { NetworkAccessLevel.SparkleStaff, });
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to send email in CreateNetworkRequestsService.Create" + Environment.NewLine + ex.Message);
            }

            return item;
        }

        public CreateNetworkRequest Update(CreateNetworkRequest item)
        {
            this.VerifyNetwork(item);

            return this.cnrRepository.Update(item);
        }
    }
}
