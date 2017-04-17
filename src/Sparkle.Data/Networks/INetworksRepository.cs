
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface INetworksRepository : IBaseNetworkRepository<Network, int>
    {
        Network GetByNameOrCreate(string name);

        IList<Network> GetAll(NetworkOptions options);
    }
}
