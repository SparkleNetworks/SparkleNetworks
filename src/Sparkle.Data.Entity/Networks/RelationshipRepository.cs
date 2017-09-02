
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class RelationshipRepository : BaseNetworkRepositoryInt<Relationship>, IRelationshipRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public RelationshipRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Relationships)
        {
        }
    }
}
