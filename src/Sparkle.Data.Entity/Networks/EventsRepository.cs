
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Globalization;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Filters;
    using Sparkle.Entities.Networks;

    public class EventsRepository : BaseNetworkRepositoryInt<Event>, IEventsRepository
    {
        private const string StoreEventDatesTimezoneName = "Romance Standard Time";
        private static TimeZoneInfo storeEventDatesTimezone = TimeZoneInfo.FindSystemTimeZoneById(StoreEventDatesTimezoneName);

        [System.Diagnostics.DebuggerStepThrough]
        public EventsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Events)
        {
        }

        public IQueryable<Event> SelectEventsWichUserIsRegistered(int userId)
        {
            return this.Set
                .Include("EventsMembers")
                .Where(o => o.EventMembers.Any(n => n.UserId == userId));
        }

        public IQueryable<Event> CreateQuery(EventOptions options)
        {
            ObjectQuery<Event> query = this.Context.Events;
            
            if ((options & EventOptions.Category) == EventOptions.Category)
                query = query.Include("EventCategory");
            if ((options & EventOptions.EventsMembers) == EventOptions.EventsMembers)
                query = query.Include("EventMembers");
            if ((options & EventOptions.EventsMembersPeople) == EventOptions.EventsMembersPeople)
                query = query.Include("EventMembers.User");
            if ((options & EventOptions.Owner) == EventOptions.Owner)
                query = query.Include("User");
            if ((options & EventOptions.Place) == EventOptions.Place)
                query = query.Include("Place");

            return query;
        }

        public IList<TimeStatItem> GetStatsPerMonth(int networkId)
        {
            return this.Context.GetEventStatsPerMonth(networkId)
                .Select(r => new TimeStatItem
                {
                    Date = DateTime.ParseExact(r.Month, "yyyy-M-d", CultureInfo.InvariantCulture),
                    Grouping = TimeStatGrouping.Monthly,
                    Count = r.Events.Value,
                    Count1 = r.Members.Value,
                })
                .ToList();
            ////return this.Set
            ////    .Where(e => e.NetworkId == networkId)
            ////    .GroupBy(e => new
            ////    {
            ////        Year = e.DateEvent.Value.Year,
            ////        Month = e.DateEvent.Value.Month,
            ////    })
            ////    .Select(g => new TimeStatItem
            ////    {
            ////        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
            ////        Grouping = TimeStatGrouping.Monthly,
            ////        Count = g.Count(),
            ////    })
            ////    .ToList();
        }

        public IList<Event> GetById(int[] ids, EventOptions options)
        {
            return this.CreateQuery(options)
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Event> GetActiveById(int[] ids, EventOptions options)
        {
            return this.CreateQuery(options)
                .Active()
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Event> GetById(int[] ids, int networkId, EventOptions options)
        {
            return this.CreateQuery(options)
                .ByNetwork(networkId)
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Event> GetActiveById(int[] ids, int networkId, EventOptions options)
        {
            return this.CreateQuery(options)
                .ByNetwork(networkId)
                .Active()
                .Where(g => ids.Contains(g.Id))
                .ToList();
        }

        public IList<Event> GetFutureItems(int networkId, DateTime minDate, int count, bool includeNetworkEvents, int[] includeGroupIds, int[] includeCompanyIds, int? userId)
        {
            var minStoreDate = minDate.AddDays(-1); // the date is stored using a specific tz
            includeGroupIds = includeGroupIds ?? new int[0];
            includeCompanyIds = includeCompanyIds ?? new int[0];

            return this.Set
                .Where(e => e.NetworkId == networkId
                    && e.DateEvent >= minStoreDate
                    && e.DeleteDateUtc == null
                    && (
                        (includeNetworkEvents && e.Visibility <= 0)
                     || (includeGroupIds.Contains(e.GroupId.Value))
                     || (includeCompanyIds.Contains(e.CompanyId.Value))
                     || (userId != null && e.CreatedByUserId == userId)
                    )
                )
                .OrderBy(e => e.DateEvent)
                .Take(count)
                .ToList();
        }

        public int CountAtPlace(int placeId)
        {
            return this.Set.Where(e => e.PlaceId == placeId).Count();
        }

        public IList<Event> GetAtPlace(int networkId, int placeId, DateTime minDate, DateTime maxDate, EventOptions options, bool includeNetworkEvents, int[] includeGroupIds, int[] includeCompanyIds)
        {
            var minStoreDate = storeEventDatesTimezone.ConvertFromUtc(minDate);
            var maxStoreDate = storeEventDatesTimezone.ConvertFromUtc(maxDate);
            includeGroupIds = includeGroupIds ?? new int[0];
            includeCompanyIds = includeCompanyIds ?? new int[0];

            return this.Set
                .Where(e => e.NetworkId == networkId
                    && e.PlaceId == placeId
                    && (
                        e.DateEvent >= minStoreDate && e.DateEvent < maxStoreDate
                        ||
                        e.DateEndEvent >= minStoreDate && e.DateEndEvent < maxStoreDate
                    )
                    && (
                        (includeNetworkEvents && (e.GroupId == null || e.Visibility == 0) && (e.CompanyId == null || e.Visibility == 0))
                     || (includeGroupIds.Contains(e.GroupId.Value))
                     || (includeCompanyIds.Contains(e.CompanyId.Value))
                    )
                )
                .OrderBy(e => e.DateEvent)
                .ToList();
        }

        public int CountCreatedByUser(int userId, int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .ByCreator(userId)
                .Count();
        }
    }

    public class EventsCategoriesRepository : BaseNetworkRepositoryInt<EventCategory>, IEventsCategoriesRepository
    {
        public EventsCategoriesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.EventCategories)
        {
        }

        public int Count()
        {
            return this.Set.Count();
        }

        public IDictionary<int, EventCategory> GetAll(int networkId)
        {
            return this.Set
                .Where(c => c.NetworkId == null || c.NetworkId == networkId)
                .OrderBy(c => c.Name)
                .ToDictionary(c => c.Id, c => c);
        }

        public IDictionary<int, EventCategory> GetById(int[] ids)
        {
            return this.Set
                .Where(c => ids.Contains(c.Id))
                .OrderBy(c => c.Name)
                .ToDictionary(c => c.Id, c => c);
        }
    }
}
