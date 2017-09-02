
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface IClubsRepository : IBaseNetworkRepository<Club, int>
    {
        int? GetIdByAlias(string alias);

        IList<Club> GetAll(int networkId);
    }
}
