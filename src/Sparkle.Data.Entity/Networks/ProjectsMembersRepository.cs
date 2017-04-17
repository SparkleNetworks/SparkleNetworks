
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using Sparkle.Data;

    public class ProjectsMembersRepository : BaseNetworkRepository<ProjectMember, int>, IProjectsMembersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ProjectsMembersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ProjectMembers) {
        }

        protected override ProjectMember GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(ProjectMember item)
        {
            return item.Id;
        }
    }
}
