
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public static class ContactsFilter
    {
        public static IQueryable<User> WithCompanyId(this IQueryable<User> qry, int companyId)
        {
            return qry.Where(o => o.CompanyID == companyId);
        }
    }
}
