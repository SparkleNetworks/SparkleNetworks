
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class MembershipRepository : BaseNetworkRepository<AspnetMembership>, IMembershipRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public MembershipRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.AspnetMemberships)
        {
        }


        public void LockMemberhipAccount(string applicationName, string username, DateTime dateTime)
        {
            this.Context.aspnet_Membership_LockUser(applicationName, username, dateTime);
        }
    }
}
