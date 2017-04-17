
namespace Sparkle.Services.Networks
{
    using System;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public interface ISocialNetworkUserSubscriptionsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(SocialNetworkUserSubscription item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        SocialNetworkUserSubscription Update(SocialNetworkUserSubscription item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(SocialNetworkUserSubscription item);

        IList<SocialNetworkUserSubscription> GetAllActive();

        /// <summary>
        /// Gets the by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        IList<SocialNetworkUserSubscription> GetByUserId(int userId);

        IList<SocialNetworkUserSubscription> GetByNetworkConnectionUsername(string networkConnectionUsername, SocialNetworkConnectionType socialNetworkType);

        void CreateConnection(
            int createdByUserId,
            int userId,
            SocialNetworkConnectionType type,
            string username = "",
            bool autoPublish = true,
            string contentFilter = null,
            string oAuthToken = "",
            string oAuthVerifier = "",
            bool isActive = true,
            DateTime? oAuthTokenDateUtc = null,
            int? oAuthTokenDurationMinutes = null);
    }
}
