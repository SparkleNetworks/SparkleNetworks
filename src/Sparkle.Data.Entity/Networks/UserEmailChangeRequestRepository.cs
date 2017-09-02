
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Text;

    public class UserEmailChangeRequestRepository : BaseNetworkRepositoryInt<UserEmailChangeRequest>, IUserEmailChangeRequestRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public UserEmailChangeRequestRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserEmailChangeRequests)
        {
        }

        public IQueryable<UserEmailChangeRequest> NewQuery(UserEmailChangeRequestOptions options)
        {
            ObjectQuery<UserEmailChangeRequest> query = this.Set;

            if ((options & UserEmailChangeRequestOptions.CreatedByUser) == UserEmailChangeRequestOptions.CreatedByUser)
                query = query.Include("CreatedByUser");

            if ((options & UserEmailChangeRequestOptions.User) == UserEmailChangeRequestOptions.User)
                query = query.Include("User");

            return query;
        }

        public UserEmailChangeRequest GetById(int id, UserEmailChangeRequestOptions options)
        {
            return this.NewQuery(options)
                .Where(o => o.Id == id)
                .SingleOrDefault();
        }

        public IList<UserEmailChangeRequest> GetAll(UserEmailChangeRequestOptions options)
        {
            return this.NewQuery(options).OrderByDescending(x => x.CreateDateUtc).ToList();
        }
    }
}
