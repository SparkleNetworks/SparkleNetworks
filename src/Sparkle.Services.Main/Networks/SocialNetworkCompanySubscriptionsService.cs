
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SocialNetworkCompanySubscriptionsService : ServiceBase, ISocialNetworkCompanySubscriptionsService
    {
        internal SocialNetworkCompanySubscriptionsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ISocialNetworkCompanySubscriptionsRepository snRepository
        {
            get { return this.Repo.SocialNetworkCompanySubscriptions; }
        }

        public int Insert(SocialNetworkCompanySubscription item)
        {
            return this.snRepository.Insert(item).Id;
        }

        public SocialNetworkCompanySubscription Update(SocialNetworkCompanySubscription item)
        {
            return this.snRepository.Update(item);
        }

        public void Delete(SocialNetworkCompanySubscription item)
        {
            this.snRepository.Delete(item);
        }

        public IList<SocialNetworkCompanySubscription> GetAllActive()
        {
            return this.Repo.SocialNetworkCompanySubscriptions.GetAllActive(this.Services.NetworkId);
        }

        public IList<SocialNetworkCompanySubscription> GetByNetworkConnectionUsername(string networkConnectionUsername, SocialNetworkConnectionType socialNetworkType)
        {
            return this.Repo.SocialNetworkCompanySubscriptions.GetByNetworkConnectionUsername(this.Services.NetworkId, socialNetworkType, networkConnectionUsername);
        }

        public System.Collections.Generic.IList<SocialNetworkCompanySubscription> GetByCompanyId(int companyId)
        {
            return this.Services.Repositories.SocialNetworkCompanySubscriptions
                .NewQuery(Data.Networks.Options.SocialNetworkCompanySubscriptionOptions.Connection)
                .Where(s => s.CompanyId == companyId)
                .ToList();
        }

        public void CreateConnection(
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
            int? oAuthTokenDurationMinutes = null)
        {
            var newConnectionId = this.Services.SocialNetworkConnections.Insert(new SocialNetworkConnection
            {
                CreatedByUserId = userId,
                Type = (byte)type,
                Username = username,
                OAuthToken = oAuthToken,
                OAuthVerifier = oAuthVerifier,
                IsActive = isActive,
                OAuthTokenDateUtc = oAuthTokenDateUtc,
                OAuthTokenDurationMinutes = oAuthTokenDurationMinutes,
            });

            this.Insert(new SocialNetworkCompanySubscription
            {
                CompanyId = companyId,
                SocialNetworkConnectionsId = newConnectionId,
                AutoPublish = autoPublish,
                ContentContainsFilter = contentFilter,
            });
        }
    }
}
