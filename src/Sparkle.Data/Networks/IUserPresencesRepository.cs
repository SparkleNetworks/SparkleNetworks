
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Repository("UserPresences")]
    public interface IUserPresencesRepository : IBaseNetworkRepository<UserPresence, int>
    {
        void UpdateUserPresence(int userId, DateTime day, DateTime now);
        int CountUsersDays(int userId);

        GetUserPresenceStats_Result GetUserPresenceStats(int networkId);

        UserPresence GetLastByUserId(int userId);
    }
}
