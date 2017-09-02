
namespace Sparkle.Data.Networks.Filters
{
    using Sparkle.Entities.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public static class UserEmailChangeRequestFilter
    {
        public static IQueryable<UserEmailChangeRequest> WithUserId(this IQueryable<UserEmailChangeRequest> query, int userId)
        {
            return query.Where(o => o.UserId == userId);
        }

        public static IQueryable<UserEmailChangeRequest> PendingRequest(this IQueryable<UserEmailChangeRequest> query)
        {
            return query.Where(o => o.Status == (int)UserEmailChangeRequestStatus.Pending);
        }

        public static IQueryable<UserEmailChangeRequest> WithPendingEmail(this IQueryable<UserEmailChangeRequest> query, string accountPart, string domainPart)
        {
            return query.Where(o => o.Status == (int)UserEmailChangeRequestStatus.Pending && 
                o.NewEmailAccountPart == accountPart && 
                o.NewEmailDomainPart == domainPart);
        }

        public static IQueryable<UserEmailChangeRequest> WithForbiddenEmail(this IQueryable<UserEmailChangeRequest> query, string accountPart, string domainPart)
        {
            return query.Where(o => o.Status == (int)UserEmailChangeRequestStatus.Succeed &&
                o.PreviousEmailForbidden == 1 &&
                o.PreviousEmailAccountPart == accountPart &&
                o.PreviousEmailDomainPart == domainPart);
        }
    }
}
