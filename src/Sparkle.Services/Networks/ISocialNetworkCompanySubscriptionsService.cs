
namespace Sparkle.Services.Networks
{
    using System;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public interface ISocialNetworkCompanySubscriptionsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(SocialNetworkCompanySubscription item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        SocialNetworkCompanySubscription Update(SocialNetworkCompanySubscription item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(SocialNetworkCompanySubscription item);

        IList<SocialNetworkCompanySubscription> GetAllActive();

        IList<SocialNetworkCompanySubscription> GetByNetworkConnectionUsername(string networkConnectionUsername, SocialNetworkConnectionType socialNetworkType);

        IList<SocialNetworkCompanySubscription> GetByCompanyId(int companyId);

        void CreateConnection(
            int userId,
            int companyId,
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
