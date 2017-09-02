
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface INetworksService
    {
        Network GetByName(string name);

        IEnumerable<Network> SelectAll();

        Network GetByNameOrCreate(string name);

        IList<Network> GetAllActive();

        Network GetById(int networkId);

        NetworkType GetNetworksType(int networkId);

        NetworkType GetNetworkType(int networkTypeId);

        Network Update(Network network);
    }
}
