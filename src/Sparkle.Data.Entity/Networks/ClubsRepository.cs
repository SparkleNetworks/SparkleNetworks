
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ClubsRepository : BaseNetworkRepositoryInt<Club>, IClubsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ClubsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Clubs)
        {
        }

        public int? GetIdByAlias(string alias)
        {
            var obj = this.Set
                .Where(c => c.Alias == alias)
                .Select(u => new { Id = u.Id, })
                .SingleOrDefault();

            return obj != null ? obj.Id : default(int?);
        }

        public IList<Club> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }
    }
}
