
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HintsToUsersRepository : BaseNetworkRepository<HintsToUser>, IHintsToUsersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public HintsToUsersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.HintsToUsers)
        {
        }

        public HintsToUser Get(int hintId, int userId)
        {
            return this.Set.SingleOrDefault(x => x.UserId == userId && x.HintId == hintId);
        }
    }
}
