
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface IUserEmailChangeRequestRepository : IBaseNetworkRepository<UserEmailChangeRequest, int>
    {
        IQueryable<UserEmailChangeRequest> NewQuery(UserEmailChangeRequestOptions options);

        UserEmailChangeRequest GetById(int id, UserEmailChangeRequestOptions options);

        IList<UserEmailChangeRequest> GetAll(UserEmailChangeRequestOptions options);
    }
}
