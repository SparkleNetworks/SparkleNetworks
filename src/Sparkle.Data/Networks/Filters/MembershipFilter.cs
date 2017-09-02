
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class MembershipFilter
    {
        public static IQueryable<AspnetMembership> ThisWeek(this IQueryable<AspnetMembership> qry)
        {
            DateTime start = DateTime.Now.Date;
            while (start.DayOfWeek != DayOfWeek.Monday)
            {
                start = start.AddDays(-1);
            }
            DateTime end = start;
            while (end.DayOfWeek != DayOfWeek.Friday)
            {
                end = end.AddDays(1);
            }
            return qry.Where(o => o.CreateDate >= start && o.CreateDate <= end);
        }
    }
}
