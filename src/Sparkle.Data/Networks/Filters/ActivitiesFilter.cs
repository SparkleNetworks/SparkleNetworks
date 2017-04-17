
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class ActivitiesFilter
    {
        public static IQueryable<Activity> UserId(this IQueryable<Activity> query, int userId)
        {
            return query.Where(o => o.UserId == userId);
        }

        public static IQueryable<Activity> Displayed(this IQueryable<Activity> query, bool displayed)
        {
            return query.Where(o => o.Displayed == displayed);
        }
    }
}
