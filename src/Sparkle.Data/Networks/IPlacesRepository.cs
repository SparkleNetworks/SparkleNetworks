
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;
    using System.Data.Spatial;

    [Repository]
    public interface IPlacesRepository : IBaseNetworkRepository<Place, int>
    {
        int Count();

        IQueryable<Place> CreateQuery(PlaceOptions options);
        Place GetById(int placeId, PlaceOptions options);
        Place GetById(int placeId, int networkId, PlaceOptions options);

        IDictionary<int, Place> GetById(int[] placeIds, PlaceOptions options);

        IList<Place> GetAll(int networkId);

        IList<Place> GetAllByEventPopularity(int networkId, PlaceOptions options);

        IList<Place> GetPlacesByRadius(int networkId, string[] geocodes, int radiusKm, bool includeItemsWithoutLocation);
        IList<Place> GetPlacesByNameAndRadius(int networkId, string[] words, string[] geocodes, int radiusKm, bool includeItemsWithoutLocation);

        Place GetByAlias(int networkId, string alias);

        IList<Place> GetByParentId(int id, PlaceOptions options);

        void SetGeography(int placeId, DbGeography geography);

        IList<Place> GetDefaultPlaces(int networkId);
    }

    [Repository]
    public interface IPlacesCategoriesRepository : IBaseNetworkRepository<PlaceCategory, int>
    {
        IDictionary<int, PlaceCategory> GetAll(bool sorted = true);

        IDictionary<int, PlaceCategory> GetById(int[] categoryIds);

        PlaceCategory GetByFoursquareId(string foursquareId);

        PlaceCategory GetDefaultPlaceCategory();
    }

    [Repository]
    public interface IPlacesHistoryRepository : IBaseNetworkRepository<PlaceHistory, int>
    {
    }
}
