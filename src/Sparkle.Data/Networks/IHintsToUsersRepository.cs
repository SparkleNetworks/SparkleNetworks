
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Repository]
    public interface IHintsToUsersRepository : IBaseNetworkRepository<HintsToUser>
    {
        HintsToUser Get(int hintId, int userId);
    }
}
