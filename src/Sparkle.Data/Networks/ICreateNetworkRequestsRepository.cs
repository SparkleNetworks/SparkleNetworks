using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparkle.Entities.Networks;

namespace Sparkle.Data.Networks
{
    [Repository]
    public interface ICreateNetworkRequestsRepository : IBaseNetworkRepository<CreateNetworkRequest, int>
    {
        IList<CreateNetworkRequest> GetAll(int networkId);
    }
}
