
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class GroupsFilter
    {
        public static IQueryable<Group> WithId(this IQueryable<Group> query, int groupId)
        {
            return query.Where(o => o.Id == groupId);
        }

        public static IQueryable<Group> NewGroups(this IQueryable<Group> query, DateTime start)
        {
            while (start.DayOfWeek != DayOfWeek.Monday)
            {
                start = start.AddDays(-1);

            }

            return query.Where(o => o.Date > start);
        }

        public static IQueryable<Group> Active(this IQueryable<Group> query)
        {
            return query.Where(g => !g.IsDeleted);
        }

        public static IQueryable<Group> CreatedInRange(this IQueryable<Group> qry, DateTime start, DateTime end)
        {
            return qry.Where(g => g.Date >= start && g.Date <= end);
        }

        /// <summary>
        /// Contains the specified qry.
        /// </summary>
        /// <param name="qry">The company query.</param>
        /// <param name="request">The contain request.</param>
        /// <returns>list of companies</returns>
        public static IQueryable<Group> Contain(this IQueryable<Group> qry, string request)
        {
            return qry.Where(o => o.Name.ToLower().Contains(request));
        }
    }

    public static class TeamsFilter
    {
        public static IQueryable<Team> WithId(this IQueryable<Team> query, int teamId)
        {
            return query.Where(o => o.Id == teamId);
        }

        public static IQueryable<Team> WithCompanyId(this IQueryable<Team> qry, int companyId)
        {
            return qry.Where(o => o.OwnerType == 1 && o.OwnerValue == companyId);
        }
    }
}
