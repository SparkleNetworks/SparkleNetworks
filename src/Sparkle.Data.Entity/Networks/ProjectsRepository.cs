
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class ProjectsRepository : BaseNetworkRepositoryInt<Project>, IProjectsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ProjectsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Projects)
        {
        }
    }
}
