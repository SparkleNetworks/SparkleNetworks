
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class EventFilter
    {
        public static IQueryable<Event> WithVisibility(this IQueryable<Event> query, int visibility)
        {
            return query.Where(o => o.Visibility == visibility);
        }

        public static IEnumerable<Event> WithVisibility(this IEnumerable<Event> query, int visibility)
        {
            return query.Where(o => o.Visibility == visibility);
        }
        
        public static IQueryable<Event> WithVisibilityEqualOrLess(this IQueryable<Event> query, int visibility)
        {
            return query.Where(o => o.Visibility <= visibility);
        }

        public static IQueryable<Event> WithVisibility(this IQueryable<Event> query, EventVisibility scope)
        {
            int visibility = (int)scope;
            return query.Where(o => o.Visibility == visibility);
        }

        public static IQueryable<Event> WithId(this IQueryable<Event> query, int Id)
        {
            return query.Where(o => o.Id == Id);
        }

        public static IQueryable<Event> ByCreator(this IQueryable<Event> query, int guid)
        {
            return query.Where(o => o.CreatedByUserId == guid);
        }

        public static IQueryable<Event> ByGroupId(this IQueryable<Event> query, int groupId)
        {
            return query.Where(o => o.GroupId == groupId);
        }

        public static IQueryable<Event> ByGroupsIds(this IQueryable<Event> query, int[] groupId)
        {
            return query.Where(o => o.GroupId.HasValue && groupId.Contains(o.GroupId.Value));
        }

        public static IEnumerable<Event> ByGroupsIds(this IEnumerable<Event> query, int[] groupId)
        {
            return query.Where(o => o.GroupId.HasValue && groupId.Contains(o.GroupId.Value));
        }

        public static IQueryable<Event> ByPlaceId(this IQueryable<Event> query, int placeId)
        {
            return query.Where(o => o.PlaceId == placeId);
        }

        public static IQueryable<Event> TodayAndFuture(this IQueryable<Event> query, DateTime dateTime)
        {
            return query.Where(o => o.DateEndEvent >= dateTime);
        }

        public static IQueryable<Event> Future(this IQueryable<Event> query)
        {
            return query.Where(o => o.DateEndEvent >= DateTime.Now);
        }

        public static IQueryable<Event> Past(this IQueryable<Event> query)
        {
            return query.Where(o => o.DateEndEvent < DateTime.Now);
        }

        /// <summary>
        /// Thises the week.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static IEnumerable<Event> ThisWeek(this IEnumerable<Event> query)
        {
            DateTime dateTemp = DateTime.Now.Date;
            while (dateTemp.DayOfWeek != DayOfWeek.Monday)
            {
                dateTemp = dateTemp.AddDays(-1);
            }
            return query.Where(o => o.DateEvent > dateTemp && o.DateEvent <= DateTime.Now);
        }

        public static IEnumerable<Event> NextWeek(this IEnumerable<Event> query, DateTime start)
        {
            while (start.DayOfWeek != DayOfWeek.Monday)
            {
                start = start.AddDays(1D);
            }
            DateTime end = start.AddDays(1D);
            while (end.DayOfWeek != DayOfWeek.Monday)
            { 
                end = end.AddDays(1D);
            }
            return query.Where(o => o.DateEvent > start && o.DateEvent <= end);
        }

        public static IEnumerable<Event> AfterNextWeek(this IEnumerable<Event> query)
        {
            DateTime start = DateTime.Now.Date;
            while (start.DayOfWeek != DayOfWeek.Monday)
            {
                start = start.AddDays(1D);
            }
            start = start.AddDays(7D);

            DateTime end = start.AddDays(1D);
            while (end.DayOfWeek != DayOfWeek.Monday)
            {
                end = end.AddDays(1D);
            }
            return query.Where(o => o.DateEvent > start && o.DateEvent <= end);
        }

        public static IQueryable<Event> PostedBy(this IQueryable<Event> query, int Id)
        {
            return query.Where(o => o.CreatedByUserId == Id);
        }

        public static IQueryable<Event> Contain(this IQueryable<Event> query, string term)
        {
            return query.Where(o => o.Name.Contains(term) | o.Description.Contains(term));
        }

        public static IQueryable<Event> MeetingFromCompany(this IQueryable<Event> query, int companyId)
        {
            return query.Where(o => o.EventCategoryId == 5 && o.CompanyId == companyId);
        }

        public static IQueryable<Event> Active(this IQueryable<Event> qry)
        {
            return qry.Where(o =>
                o.DeleteDateUtc == null 
                && o.DeletedByUserId == null
                && o.DeleteReason == null);
        }

        public static IEnumerable<Event> Active(this IEnumerable<Event> qry)
        {
            return qry.Where(o =>
                o.DeleteDateUtc == null
                && o.DeletedByUserId == null
                && o.DeleteReason == null);
        }
    }

    public static class EventCategoryFilter
    {
        public static IQueryable<EventCategory> WithId(this IQueryable<EventCategory> query, int Id)
        {
            return query.Where(o => o.Id == Id);
        }
    }

}