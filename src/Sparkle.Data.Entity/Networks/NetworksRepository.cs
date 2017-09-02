
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Text;

    public class NetworksRepository : BaseNetworkRepositoryInt<Network>, INetworksRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public NetworksRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Networks)
        {
        }

        protected override Network GetById(NetworksEntities model, int id)
        {
            return base.GetById(model, id);
        }

        public IQueryable<Network> CreateQuery(NetworkOptions options)
        {
            ObjectQuery<Network> query = this.Context.Networks;
            
            if ((options & NetworkOptions.Type) == NetworkOptions.Type)
                query = query.Include("Type");

            return query;
        }

        public Network GetByNameOrCreate(string name)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("NetworksRepository.GetByNameOrCreate: This cannot be used within a transaction.");

            using (var model = this.GetNewContext())
            {
                var network = this.Set.SingleOrDefault(d => d.Name == name);
                if (network == null)
                {
                    network = new Network
                    {
                        Name = name,
                        DisplayName = name,
                    };
                    model.Networks.AddObject(network);
                    model.SaveChanges();
                }

                return network;
            }
        }

        public IList<Network> GetAll(NetworkOptions options)
        {
            return this.CreateQuery(options).OrderBy(n => n.DisplayName).ToList();
        }
    }
}
