
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Repository]
    public interface IHintsRepository : IBaseNetworkRepository<Hint, int>
    {
        IList<Hint> GetAll(int networkId);

        Hint GetByAlias(string alias);
        Hint GetByAlias(int networkId, string alias);

    }
}
