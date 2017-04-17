
namespace Sparkle.Services.Networks
{
    using System;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Entities.Networks.Neutral;

    public interface ISocialNetworkConnectionsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(SocialNetworkConnection item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        SocialNetworkConnection Update(SocialNetworkConnection item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(SocialNetworkConnection item);

        SocialNetworkConnection GetByUserIdAndConnectionType(int userId, SocialNetworkConnectionType type);

        void ClearToken(int userId, SocialNetworkConnectionType socialNetwork);

        void AddManyWithUserId(IList<SocialNetworkConnectionPoco> items, int? userId);

        int CountByUsernameAndType(string username, SocialNetworkConnectionType type);
    }
}
