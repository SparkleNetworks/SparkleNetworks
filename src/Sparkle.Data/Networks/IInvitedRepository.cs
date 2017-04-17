
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IInvitedRepository : IBaseNetworkRepository<Invited, int>
    {
        Invited GetByCode(Guid code);
        Invited GetByCode(Guid code, int networkId);

        /// <summary>
        /// Gets the people count by inviter.
        /// </summary>
        /// <returns></returns>
        IList<PeopleCountByInviter> GetPeopleCountByInviter();

        Invited GetByEmail(string email, bool includeUserDetails);

        IList<Invited> GetByUserId(int userId);
    }
}
