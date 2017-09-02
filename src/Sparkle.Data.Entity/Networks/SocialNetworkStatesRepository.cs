
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class SocialNetworkStatesRepository : BaseNetworkRepositoryInt<SocialNetworkState>, ISocialNetworkStatesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SocialNetworkStatesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.SocialNetworkStates)
        {
        }

        public IList<SocialNetworkState> GetAll(int networkId)
        {
            return this.Set.Where(s => s.NetworkId == networkId).ToList();
        }

        public SocialNetworkState GetByType(int networkId, SocialNetworkConnectionType socialNetworkConnectionType)
        {
            byte type = (byte)socialNetworkConnectionType;
            return this.Set.Where(s => s.NetworkId == networkId && s.SocialNetworkType == type).SingleOrDefault();
        }
    }
}
