using System.Collections.Generic;
using System.Linq;
using Sparkle.Data.Filters;
using Sparkle.Data.Networks;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;
using System.Diagnostics;

namespace Sparkle.Services.Main.Networks
{
    public class LostItemsService : ServiceBase, ILostItemsService
    {
        [DebuggerStepThrough]
        internal LostItemsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public int Insert(LostItem item)
        {
            this.SetNetwork(item);

            return this.Repo.LostItems.Insert(item).Id;
        }

        public void Delete(LostItem item)
        {
            this.VerifyNetwork(item);

            this.Repo.LostItems.Delete(item);
        }

        public LostItem Update(LostItem item)
        {
            this.VerifyNetwork(item);

            return this.Repo.LostItems.Update(item);
        }

        public IList<LostItem> SelectAll()
        {
            return this.Repo.LostItems.Select()
                .ByNetwork(this.Services.NetworkId)
                    .OrderByDescending(o => o.Date)
                    .Visible()
                    .ToList();
        }

        public LostItem SelectById(int lostItemId)
        {
            return this.Repo.LostItems.Select()
                .ByNetwork(this.Services.NetworkId)
                    .ById(lostItemId)
                    .FirstOrDefault();
        }

        public int Count()
        {
            return this.Repo.LostItems.Select()
                .ByNetwork(this.Services.NetworkId)
                   .Count();
        }
    }
}
