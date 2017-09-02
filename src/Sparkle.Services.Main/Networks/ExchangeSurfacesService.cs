
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class ExchangeSurfacesService : ServiceBase, IExchangeSurfacesService
    {
        [DebuggerStepThrough]
        internal ExchangeSurfacesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory) 
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IExchangeSurfacesRepository exchangeSurfacesRepository
        {
            get { return this.Repo.ExchangeSurfaces; }
        }

        public IList<ExchangeSurface> GetActiveExchanges()
        {
            return this.exchangeSurfacesRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ToList();
        }

        public int Add(ExchangeSurface item)
        {
            this.SetNetwork(item);

            return this.exchangeSurfacesRepository.Insert(item).Id;
        }
    }
}
