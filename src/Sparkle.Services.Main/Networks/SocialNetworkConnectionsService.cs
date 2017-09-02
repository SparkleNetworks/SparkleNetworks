
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SocialNetworkConnectionsService : ServiceBase, ISocialNetworkConnectionsService
    {
        internal SocialNetworkConnectionsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ISocialNetworkConnectionsRepository snRepository
        {
            get { return this.Repo.SocialNetworkConnections; }
        }

        public int Insert(SocialNetworkConnection item)
        {
            return this.snRepository.Insert(item).Id;
        }

        public SocialNetworkConnection Update(SocialNetworkConnection item)
        {
            return this.snRepository.Update(item);
        }

        public void Delete(SocialNetworkConnection item)
        {
            this.snRepository.Delete(item);
        }

        public SocialNetworkConnection GetByUserIdAndConnectionType(int userId, SocialNetworkConnectionType type)
        {
            return this.Repo.SocialNetworkConnections.GetByUserIdAndType(userId, type);
        }

        public void ClearToken(int userId, SocialNetworkConnectionType socialNetwork)
        {
            var item = this.Repo.SocialNetworkConnections.GetByUserIdAndType(userId, socialNetwork);
            if (item != null)
            {
                item.OAuthToken = string.Empty;
                this.Repo.SocialNetworkConnections.Update(item);
            }
        }

        public void AddManyWithUserId(IList<SocialNetworkConnectionPoco> items, int? userId)
        {
            var toAdd = items
                .Select(o => new SocialNetworkConnection
                {
                    CreatedByUserId = userId ?? o.CreatedByUserId,
                    Type = o.Type,
                    Username = o.Username,
                    OAuthToken = o.OAuthToken,
                    OAuthVerifier = o.OAuthVerifier,
                    IsActive = o.IsActive,
                    OAuthTokenDateUtc = o.OAuthTokenDateUtc,
                    OAuthTokenDurationMinutes = o.OAuthTokenDurationMinutes,
                })
                .ToList();

            this.Repo.SocialNetworkConnections.InsertMany(toAdd);
        }

        public int CountByUsernameAndType(string username, SocialNetworkConnectionType type)
        {
            return this.Repo
                .SocialNetworkConnections
                .CountByUsernameAndType(username, type);
        }
    }
}
