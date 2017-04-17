
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class InvitedFilter
    {
        public static IQueryable<Invited> EmailDomain(this IQueryable<Invited> qry, string emailDomain)
        {
            return qry.Where(o => o.Email.Contains(emailDomain));
        }

        public static IQueryable<Invited> ByCompany(this IQueryable<Invited> qry, int companyId)
        {
            return qry.Where(o => o.CompanyId == companyId);
        }

        public static IQueryable<Invited> Registered(this IQueryable<Invited> qry)
        {
            return qry.Where(o => o.UserId.HasValue);
        }

        public static IQueryable<Invited> Pending(this IQueryable<Invited> qry)
        {
            return qry.Where(o => !o.UserId.HasValue);
        }

        public static IQueryable<Invited> ById(this IQueryable<Invited> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<Invited> ByInvitationKey(this IQueryable<Invited> qry, Guid key)
        {
            return qry.Where(o => o.Code == key);
        }

        public static IQueryable<Invited> CheckCode(this IQueryable<Invited> qry, Guid code)
        {
            return qry.Where(o => o.Code == code && o.UserId == null);
        }

        public static IQueryable<Invited> CheckEmail(this IQueryable<Invited> qry, string email)
        {
            return qry.Where(o => o.Email == email);
        }

        public static IQueryable<Invited> NotUnregistered(this IQueryable<Invited> qry)
        {
            return qry.Where(o => o.Unregistred == false && o.UserId == null);
        }

        public static IQueryable<Invited> Unregistered(this IQueryable<Invited> qry)
        {
            return qry.Where(o => o.Unregistred == true && o.UserId == null);
        }
    }
}
