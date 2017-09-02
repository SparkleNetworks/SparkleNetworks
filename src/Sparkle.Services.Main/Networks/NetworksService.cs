
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NetworksService : ServiceBase, INetworksService
    {
        internal NetworksService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected INetworksRepository networksRepository
        {
            get { return this.Repo.Networks; }
        }

        public Network GetByName(string name)
        {
            return networksRepository
                .Select()
                .Where(d => d.Name == name)
                .FirstOrDefault();
        }

        public IEnumerable<Network> SelectAll()
        {
            return networksRepository.Select();
        }

        public Network GetByNameOrCreate(string name)
        {
            return this.Repo.Networks.GetByNameOrCreate(name);
        }

        public IList<Network> GetAllActive()
        {
            return this.Repo.Networks.Select().ToList();
        }

        public Network GetById(int id)
        {
            return networksRepository
                .Select()
                .Where(d => d.Id == id)
                .FirstOrDefault();
        }

        public NetworkType GetNetworksType(int networkId)
        {
            var network = this.Repo.Networks.GetById(networkId);
            return this.Repo.NetworkTypes.GetById(network.NetworkTypeId);
        }

        public NetworkType GetNetworkType(int networkTypeId)
        {
            return this.Repo.NetworkTypes.GetById(networkTypeId);
        }

        public Network Update(Network network)
        {
            return this.networksRepository.Update(network);
        }
    }
}
