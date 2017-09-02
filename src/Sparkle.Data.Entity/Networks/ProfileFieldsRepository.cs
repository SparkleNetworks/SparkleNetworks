
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProfileFieldsRepository : BaseNetworkRepositoryInt<ProfileField>, IProfileFieldsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ProfileFieldsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory,  m => m.ProfileFields)
        {
        }

        public int Count()
        {
            return this.Set.Count();
        }

        public IList<ProfileField> GetUserFields()
        {
            return this.Set
                .Where(f => f.ApplyToUsers)
                .OrderBy(f => f.Name)
                .ToList();
        }

        public IList<ProfileField> GetAll()
        {
            return this.Set
                .ToList();
        }

        public IList<ProfileField> GetCompanyFields()
        {
            return this.Set
                .Where(f => f.ApplyToCompanies)
                .OrderBy(f => f.Name)
                .ToList();
        }
    }
}
