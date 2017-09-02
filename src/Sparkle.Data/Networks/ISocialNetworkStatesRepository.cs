
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository]
    public interface ISocialNetworkStatesRepository : IBaseNetworkRepository<SocialNetworkState, int>
    {
        IList<SocialNetworkState> GetAll(int networkId);

        SocialNetworkState GetByType(int networkId, SocialNetworkConnectionType socialNetworkConnectionType);
    }
}
