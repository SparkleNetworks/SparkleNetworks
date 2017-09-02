
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Data.Objects.SqlClient;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Filters;

    public class InvitedRepository : BaseNetworkRepositoryInt<Invited>, IInvitedRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public InvitedRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Inviteds)
        {
        }

        public Invited GetByCode(Guid code)
        {
            return this.Set.SingleOrDefault(x => x.Code == code);
        }

        public Invited GetByCode(Guid code, int networkId)
        {
            return this.Set.SingleOrDefault(x => x.Code == code && x.Company.NetworkId == networkId);
        }

        public IList<PeopleCountByInviter> GetPeopleCountByInviter()
        {
            ObjectQuery<PeopleCountByInviter> query = this.Set
                ////.Where(o => o.InvitedBy.HasValue)
                .GroupBy(o => o.InvitedByUserId)
                .OrderByDescending(g => g.Count())
                .Take(20)
                .Select(g => new PeopleCountByInviter
                {
                    Inviter = g.Key,
                    InvitedCount = g.Count(i => !i.UserId.HasValue),
                    PeopleCount = g.Count(i => i.UserId.HasValue),
                    FullPeopleCount = g
                        .Where(i => i.UserId.HasValue)
                        .Count(i => i.User.Picture != null && /*i.User.NetworkAccessLevel != null &&*/ SqlFunctions.DataLength(i.User.UserProfileFields.About()) > 400)
                }) as ObjectQuery<PeopleCountByInviter>;
            return query.ToList();
        }

        public Invited GetByEmail(string email, bool includeUserDetails)
        {
            email = email.ToLowerInvariant();

            if (includeUserDetails)
            {
                return this.Set
                    .Include("User")
                    .SingleOrDefault(i => i.Email == email);
            }
            else
            {
                return this.Set
                    .SingleOrDefault(i => i.Email == email);
            }
        }

        public IList<Invited> GetByUserId(int userId)
        {
            return this.Set.Where(x => x.UserId == userId).ToList();
        }

        protected override void OnInsertOverride(NetworksEntities model, Invited itemToInsert)
        {
            itemToInsert.Email = itemToInsert.Email.ToLowerInvariant();

            base.OnInsertOverride(model, itemToInsert);
        }
    }
}
