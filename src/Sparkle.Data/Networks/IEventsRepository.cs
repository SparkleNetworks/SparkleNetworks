
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IEventsRepository : IBaseNetworkRepository<Event, int>
    {
        IQueryable<Event> CreateQuery(EventOptions options);
        IQueryable<Event> SelectEventsWichUserIsRegistered(int userId);

        IList<TimeStatItem> GetStatsPerMonth(int networkId);

        IList<Event> GetById(int[] ids, EventOptions options);
        IList<Event> GetActiveById(int[] ids, EventOptions options);
        IList<Event> GetById(int[] ids, int networkId, EventOptions options);
        IList<Event> GetActiveById(int[] ids, int networkId, EventOptions options);

        IList<Event> GetFutureItems(int networkId, DateTime minDate, int count, bool includeNetworkEvents, int[] includeGroupIds, int[] includeCompanyIds, int? userId);

        int CountAtPlace(int placeId);

        IList<Event> GetAtPlace(int networkId, int placeId, DateTime minDate, DateTime maxDate, EventOptions options, bool includeNetworkEvents, int[] includeGroupIds, int[] includeCompanyIds);

        int CountCreatedByUser(int userId, int networkId);
    }

    [Repository]
    public interface IEventsCategoriesRepository : IBaseNetworkRepository<EventCategory, int>
    {
        int Count();

        IDictionary<int, EventCategory> GetAll(int networkId);

        IDictionary<int, EventCategory> GetById(int[] ids);
    }
}
