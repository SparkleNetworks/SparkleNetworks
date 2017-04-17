
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class LostItemsRepository : BaseNetworkRepository<LostItem, int>, ILostItemsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public LostItemsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.LostItems)
        {
        }

        protected override LostItem GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(LostItem item)
        {
            return item.Id;
        }
    }
}
