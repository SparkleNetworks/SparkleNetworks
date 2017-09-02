
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IExchangeMaterialsRepository : IBaseNetworkRepository<ExchangeMaterial, int>
    {
    }
}
