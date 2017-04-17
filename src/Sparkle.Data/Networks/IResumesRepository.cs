
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository]
    public interface IResumesRepository : IBaseNetworkRepository<Resume, int>
    {
        IList<Resume> GetAll(int networkId);
    }
}
