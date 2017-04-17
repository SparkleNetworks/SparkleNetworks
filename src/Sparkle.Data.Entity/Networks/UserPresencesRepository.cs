
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Filters;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class UserPresencesRepository : BaseNetworkRepositoryInt<UserPresence>, IUserPresencesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public UserPresencesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserPresences)
        {
        }

        public void UpdateUserPresence(int userId, DateTime day, DateTime now)
        {
            this.Context.UpdateUserPresence(userId, day, now);
        }

        public int CountUsersDays(int userId)
        {
            return this.Set
                .Where(x => x.UserId == userId)
                .Count();
        }

        public GetUserPresenceStats_Result GetUserPresenceStats(int networkId)
        {
            var line = this.Context.GetUserPresenceStats(networkId).First();
            return line;
        }

        public UserPresence GetLastByUserId(int userId)
        {
            var item = this.Context.UserPresences
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Day)
                .FirstOrDefault();
            return item;
        }
    }
}
