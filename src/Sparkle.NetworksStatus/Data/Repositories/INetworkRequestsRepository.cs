
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial interface INetworkRequestsRepository
    {
        NetworkRequest GetByWebId(Guid webId);

        int Count();
    }
}
