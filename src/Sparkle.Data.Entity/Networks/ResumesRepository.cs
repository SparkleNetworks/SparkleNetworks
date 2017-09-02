
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Linq;

    public class ResumesRepository : BaseNetworkRepositoryInt<Resume>, IResumesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ResumesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Resumes)
        {
        }

        public System.Collections.Generic.IList<Resume> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }
    }
}
