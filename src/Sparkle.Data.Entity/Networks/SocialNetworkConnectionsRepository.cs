
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class SocialNetworkConnectionsRepository : BaseNetworkRepositoryInt<SocialNetworkConnection>, ISocialNetworkConnectionsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SocialNetworkConnectionsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SocialNetworkConnections)
        {
        }

        public SocialNetworkConnection GetByUserIdAndType(int userId, SocialNetworkConnectionType type)
        {
            return this.Set
                .Where(s => s.CreatedByUserId == userId && s.Type == (byte)type)
                .SingleOrDefault();
        }

        public int CountByUsernameAndType(string username, SocialNetworkConnectionType type)
        {
            return this.Set
                .Where(o => o.Username == username && o.Type == (byte)type)
                .Count();
        }
    }
}
