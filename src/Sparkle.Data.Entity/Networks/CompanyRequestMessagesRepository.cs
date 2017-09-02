
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class CompanyRequestMessagesRepository : BaseNetworkRepositoryInt<CompanyRequestMessage>, ICompanyRequestMessagesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyRequestMessagesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyRequestMessages)
        {
        }
    }
}