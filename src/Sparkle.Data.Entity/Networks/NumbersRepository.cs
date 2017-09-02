
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using Sparkle.Data;
    using Sparkle.Data.Networks;

    public class NumbersRepository : BaseNetworkRepositoryInt<Number>, INumbersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public NumbersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Numbers)
        {
        }
    }
}
