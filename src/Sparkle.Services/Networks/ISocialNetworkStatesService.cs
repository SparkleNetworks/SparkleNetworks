
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks.Models;

    public interface ISocialNetworkStatesService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(SocialNetworkState item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        SocialNetworkState Update(SocialNetworkState item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(SocialNetworkState item);

        IList<SocialNetworkState> GetAll();

        IList<SocialNetworkStateModel> GetAllIncludingUnconfigured();

        SocialNetworkStateModel GetState(SocialNetworkConnectionType socialNetworkConnectionType);

        string GetTwitterFollowListName(Network network);
        string GetTwitterFollowListName(NetworkModel network);
    }
}
