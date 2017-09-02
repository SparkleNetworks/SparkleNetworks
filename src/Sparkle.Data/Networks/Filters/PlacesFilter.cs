
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class PlacesFilter
    {
        public static IQueryable<Place> WithId(this IQueryable<Place> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<Place> WithAlias(this IQueryable<Place> qry, string alias)
        {
            return qry.Where(o => o.Alias == alias);
        }

        public static IQueryable<Place> Contain(this IQueryable<Place> query, string request)
        {
            return query.Where(o => o.Name != null && o.Name.Contains(request) | o.Address != null && o.Address.Contains(request) | o.Alias != null && o.Alias.Contains(request));
        }

        public static IQueryable<Place> PlacesForLunch(this IQueryable<Place> qry)
        {
            return qry.Where(o => o.ParentId == 4 || o.ParentId == 5);
        }
    }

    public static class PlacesCategoriesFilter
    {
        public static IQueryable<PlaceCategory> WithId(this IQueryable<PlaceCategory> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<PlaceCategory> Parents(this IQueryable<PlaceCategory> qry)
        {
            return qry.Where(o => o.ParentId == 0);
        }
    }

    public static class PlacesHistoryFilter
    {
        public static IQueryable<PlaceHistory> WithId(this IQueryable<PlaceHistory> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<PlaceHistory> Today(this IQueryable<PlaceHistory> qry)
        {
            DateTime today = DateTime.Now.Date;
            return qry.Where(o => o.Day == today);
        }

        public static IQueryable<PlaceHistory> Tomorrow(this IQueryable<PlaceHistory> qry)
        {
            DateTime tomorrow = DateTime.Now.Date.AddDays(1);
            return qry.Where(o => o.Day == tomorrow);
        }

        public static IQueryable<PlaceHistory> PlaceId(this IQueryable<PlaceHistory> qry, int placeId)
        {
            return qry.Where(o => o.PlaceId == placeId);
        }

        public static IQueryable<PlaceHistory> UserId(this IQueryable<PlaceHistory> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }

        public static IQueryable<PlaceHistory> ForLunch(this IQueryable<PlaceHistory> qry)
        {
            return qry.Where(o => o.PlaceParentId == 4 || o.PlaceParentId == 5);
        }
    }
}
