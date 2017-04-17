
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class MenusFilter
    {
        public static IQueryable<MenuPlanning> ThisWeek(this IQueryable<MenuPlanning> query)
        {
            DateTime start = DateTime.Now.Date;
            int s = (int)start.DayOfWeek;
            if (s > 1)
            {
                start = start.AddDays(-s + 1).Date;
            }
            DateTime end = start.AddDays(5D).Date;
            return query.Where(o => o.Date >= start && o.Date <= end);
        }

        public static IQueryable<MenuPlanning> ByDate(this IQueryable<MenuPlanning> qry, DateTime dateTime)
        {
            DateTime start = dateTime.Date;
            int s = (int)start.DayOfWeek;
            if (s > 1)
            {
                start = start.AddDays(-s + 1).Date;
            }
            DateTime end = start.AddDays(5D).Date;
            return qry.Where(o => o.Date >= start && o.Date <= end);
        }

        public static IQueryable<Menu> ById(this IQueryable<Menu> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<MenuPlanning> ById(this IQueryable<MenuPlanning> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }
    }
}
