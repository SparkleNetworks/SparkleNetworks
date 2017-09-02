
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Places;

    public interface IPlacesService
    {
        long Insert(Place item);

        IList<Place> Search(string request);

        IList<Place> SelectAll();

        IList<Place> SelectLunchPlaces();

        [Obsolete("Use GetByAlias")]
        Place SelectPlaceByAlias(string alias);
        PlaceModel GetByAlias(string alias);

        [Obsolete("Use GetById")]
        Place SelectPlaceById(int id);
        PlaceModel GetById(int placeId, PlaceOptions options);
        IDictionary<int, PlaceModel> GetById(int[] placeIds, PlaceOptions options);

        long Update(Place item);

        int Count();
        string MakeAlias(string name);

        IList<Place> GetBySearch(string search);

        IList<Place> SelectAllMainPlaces();

        IList<Place> SelectMainPlacesByNetworkId(int networkId);

        EditPlaceRequest GetAddRequest(EditPlaceRequest model);
        EditPlaceRequest GetEditRequest(int placeId, EditPlaceRequest model);
        EditPlaceResult Create(EditPlaceRequest model);
        EditPlaceResult Edit(EditPlaceRequest request);

        string GetProfileUrl(Place place, UriKind uriKind);

        IDictionary<int, Place> GetEntityById(int[] placeIds, PlaceOptions options);

        IList<PlaceModel> GetAll();
        IList<PlaceModel> GetPlacesByEventPopularity();

        PlacePickerModel GetPlacePickerModel(PlacePickerModel model);

        IList<PlaceModel> SearchByNameAndLocation(string[] words, string[] geocodes, string location);

        IDictionary<int, PlaceModel> GetAllForCache();
        IDictionary<int, PlaceModel> GetForCache(int[] ids);

        IList<PlaceModel> GetByParentId(int id, PlaceOptions options);

        IList<PlaceModel> GetParents(int id, PlaceOptions placeOptions);

        ImportFoursquarePlaceResult ImportFoursquarePlace(ImportFoursquarePlaceRequest request);

        IList<CompanyPlaceModel> GetCompanyPlaces(int placeId);

        FreeGeoIpModel GetLocationViaFreegeoip(string ip);

        bool TryGeocodePlace(string logPath, PlaceModel place);

        string GetLocationStringFromIp(string ip);
        IDictionary<string, IList<PlaceModel>> GetPlacesUsedByCompanies();
    }
}
