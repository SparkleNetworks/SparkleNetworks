
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class BuildingService : ServiceBase, IBuildingService
    {
        [DebuggerStepThrough]
        internal BuildingService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IBuildingRepository buildingRepository
        {
            get { return this.Repo.Building; }
        }

        public int Insert(Building item)
        {
            this.SetNetwork(item);

            return this.buildingRepository.Insert(item).Id;
        }

        public Building Update(Building item)
        {
            this.VerifyNetwork(item);

            return this.buildingRepository.Update(item);
        }

        public void Delete(Building item)
        {
            this.VerifyNetwork(item);

            this.buildingRepository.Delete(item);
        }

        public IList<Building> SelectAll()
        {
            return this.buildingRepository
                    .Select()
                .ByNetwork(this.Services.NetworkId)
                    .OrderBy(o => o.Name)
                    .ToList();
        }

        public Building GetById(int buildingId)
        {
            return this.buildingRepository
                    .Select()
                .ByNetwork(this.Services.NetworkId)
                    .ById(buildingId)
                    .FirstOrDefault();
        }
    }
}
