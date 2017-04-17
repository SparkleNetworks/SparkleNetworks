
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UserActionKeysRepository : BaseNetworkRepositoryInt<UserActionKey>, IUserActionKeysRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public UserActionKeysRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserActionKeys)
        {
        }

        public UserActionKey GetLatestAction(int userID, string key)
        {
            return this.Set
                .Where(a => a.UserId == userID && a.Action == key)
                .OrderByDescending(a => a.DateCreatedUtc)
                .FirstOrDefault();
        }
    }
}
