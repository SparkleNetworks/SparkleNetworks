
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class CompanyContactsRepository : BaseNetworkRepositoryInt<CompanyContact>, ICompanyContactsRepository
    {
        public CompanyContactsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyContacts)
        {
        }
    }
}
