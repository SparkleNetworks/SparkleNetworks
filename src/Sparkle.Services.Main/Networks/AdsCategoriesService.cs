
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerStepThrough]
    public class AdsCategoriesService : ServiceBase, IAdsCategoriesService
    {
        internal AdsCategoriesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public int Insert(AdCategory item)
        {
            this.VerifyNetwork(item);

            return this.Repo.AdsCategories.Insert(item).Id;
        }

        public void Delete(AdCategory item)
        {
            this.VerifyNetwork(item);

            this.Repo.AdsCategories.Delete(item);
        }

        public AdCategory Update(AdCategory item)
        {
            this.VerifyNetwork(item);

            return this.Repo.AdsCategories.Update(item);
        }

        public IList<AdCategory> SelectAll()
        {
            return this.Repo.AdsCategories.Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                    .ToList();
        }

        public AdCategory SelectById(int adCategoryId)
        {
            return this.Repo.AdsCategories.Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .FirstOrDefault(x => x.Id == adCategoryId);
        }

        public int Count()
        {
            return this.Repo.AdsCategories.Select()
                .Where(c => c.NetworkId == null || c.NetworkId == this.Services.NetworkId)
                .Count();
        }

        public AdCategory GetById(int id)
        {
            return this.Repo.AdsCategories.Select()
                .Where(c => (c.NetworkId == null || c.NetworkId == this.Services.NetworkId) && c.Id == id)
                .SingleOrDefault();
        }
    }
}
