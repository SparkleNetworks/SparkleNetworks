
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HintsRepository : BaseNetworkRepositoryInt<Hint>, IHintsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public HintsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Hints)
        {
        }

        public IList<Hint> GetAll(int networkId)
        {
            return this.Set
                .ByNetworkOrCommon(networkId)
                .ToList();
        }


        public Hint GetByAlias(string alias)
        {
            return this.Set
                .Where(x => x.Alias.Equals(alias))
                .SingleOrDefault();
        }

        public Hint GetByAlias(int networkId, string alias)
        {
            return this.Set
                .ByNetworkOrCommon(networkId)
                .Where(x => x.Alias.Equals(alias))
                .SingleOrDefault();
        }
    }
}
