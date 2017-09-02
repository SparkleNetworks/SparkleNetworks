
namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;

    public class SocialNetworkUserSubscriptionsService : ServiceBase, ISocialNetworkUserSubscriptionsService
    {
        internal SocialNetworkUserSubscriptionsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ISocialNetworkUserSubscriptionsRepository snRepository
        {
            get { return this.Repo.SocialNetworkUserSubscriptions; }
        }

        public int Insert(SocialNetworkUserSubscription item)
        {
            return this.snRepository.Insert(item).Id;
        }

        public SocialNetworkUserSubscription Update(SocialNetworkUserSubscription item)
        {
            return this.snRepository.Update(item);
        }

        public void Delete(SocialNetworkUserSubscription item)
        {
            this.snRepository.Delete(item);
        }

        public IList<SocialNetworkUserSubscription> GetAllActive()
        {
            return this.Repo.SocialNetworkUserSubscriptions.GetAllActive(this.Services.NetworkId);
        }

        public IList<SocialNetworkUserSubscription> GetByUserId(int userId)
        {
            return this.Services.Repositories.SocialNetworkUserSubscriptions
                .NewQuery(Data.Networks.Options.SocialNetworkUserSubscriptionOptions.Connection)
                .Where(s => s.UserId == userId)
                .ToList();
        }

        public IList<SocialNetworkUserSubscription> GetByNetworkConnectionUsername(string networkConnectionUsername, SocialNetworkConnectionType socialNetworkType)
        {
            return this.Repo.SocialNetworkUserSubscriptions.GetByNetworkConnectionUsername(this.Services.NetworkId, socialNetworkType, networkConnectionUsername);
        }

        public void CreateConnection(
            int createdByUser,
            int userId,
            SocialNetworkConnectionType type,
            string username = "",
            bool autoPublish = true,
            string contentFilter = null,
            string oAuthToken = "",
            string oAuthVerifier = "",
            bool isActive = true,
            DateTime? oAuthTokenDateUtc = null,
            int? oAuthTokenDurationMinutes = null)
        {
            var newConnectionId = this.Services.SocialNetworkConnections.Insert(new SocialNetworkConnection
            {
                CreatedByUserId = createdByUser,
                Type = (byte)type,
                Username = username,
                OAuthToken = oAuthToken,
                OAuthVerifier = oAuthVerifier,
                IsActive = isActive,
                OAuthTokenDateUtc = oAuthTokenDateUtc,
                OAuthTokenDurationMinutes = oAuthTokenDurationMinutes,
            });

            this.Insert(new SocialNetworkUserSubscription
            {
                UserId = userId,
                SocialNetworkConnectionsId = newConnectionId,
                AutoPublish = autoPublish,
                ContentContainsFilter = contentFilter,
            });
        }
    }
}
