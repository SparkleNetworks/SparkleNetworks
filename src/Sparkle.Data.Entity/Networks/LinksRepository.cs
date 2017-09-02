
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class LinksRepository : BaseNetworkRepository<Link, int>, ILinksRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public LinksRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Links)
        {
        }

        protected override Link GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(Link item)
        {
            return item.Id;
        }
    }
}
