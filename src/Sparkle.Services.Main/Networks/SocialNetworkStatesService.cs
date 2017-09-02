
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SocialNetworkStatesService : ServiceBase, ISocialNetworkStatesService
    {
        internal SocialNetworkStatesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ISocialNetworkStatesRepository snRepository
        {
            get { return this.Repo.SocialNetworkStates; }
        }

        public int Insert(SocialNetworkState item)
        {
            item.NetworkId = this.Services.NetworkId;
            return this.snRepository.Insert(item).Id;
        }

        public SocialNetworkState Update(SocialNetworkState item)
        {
            return this.snRepository.Update(item);
        }

        public void Delete(SocialNetworkState item)
        {
            this.snRepository.Delete(item);
        }

        public IList<SocialNetworkState> GetAll()
        {
            return this.snRepository.GetAll(this.Services.NetworkId);
        }

        public IList<SocialNetworkStateModel> GetAllIncludingUnconfigured()
        {
            var list = new List<SocialNetworkStateModel>();
            var configured = this.snRepository.GetAll(this.Services.NetworkId);

            foreach (var type in Enum.GetValues(typeof(SocialNetworkConnectionType)))
            {
                var model = new SocialNetworkStateModel
                {
                    NetworkId = this.Services.NetworkId,
                    Type = (SocialNetworkConnectionType)type,
                    Entity = configured.SingleOrDefault(s => s.SocialNetworkConnectionType == (SocialNetworkConnectionType)type),
                };
                list.Add(model);
            }

            return list;
        }

        public SocialNetworkStateModel GetState(SocialNetworkConnectionType socialNetworkConnectionType)
        {
            return new SocialNetworkStateModel
            {
                NetworkId = this.Services.NetworkId,
                Type = socialNetworkConnectionType,
                Entity = this.snRepository.GetByType(this.Services.NetworkId, socialNetworkConnectionType),
            };
        }

        public string GetTwitterFollowListName(Network network)
        {
            if (network == null)
                throw new ArgumentNullException("network");

            return this.GetTwitterFollowListName(network.Name);
        }

        public string GetTwitterFollowListName(NetworkModel network)
        {
            if (network == null)
                throw new ArgumentNullException("network");

            return this.GetTwitterFollowListName(network.Name);
        }

        private string GetTwitterFollowListName(string networkName)
        {
#if DEBUG
            var listName = "DbgNwkFlw" + networkName;
#else
            var listName = "PrdNwkFlw" + networkName;
#endif

            return listName;
        }
    }
}
