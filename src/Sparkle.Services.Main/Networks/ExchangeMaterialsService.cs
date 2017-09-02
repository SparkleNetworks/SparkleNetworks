
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class ExchangeMaterialsService : ServiceBase, IExchangeMaterialsService
    {
        [DebuggerStepThrough]
        internal ExchangeMaterialsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory) 
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IExchangeMaterialsRepository exchangeMaterialsRepository
        {
            get { return this.Repo.ExchangeMaterials; }
        }

        public IList<ExchangeMaterial> GetActiveExchanges()
        {
            return this.exchangeMaterialsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ToList();
        }

        public int Add(ExchangeMaterial item)
        {
            this.SetNetwork(item);

            return this.exchangeMaterialsRepository.Insert(item).Id;
        }
    }
}
