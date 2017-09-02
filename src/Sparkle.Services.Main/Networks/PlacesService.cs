
namespace Sparkle.Services.Main.Networks
{
    using FourSquare.SharpSquare;
    using FourSquare.SharpSquare.Core;
    using Newtonsoft.Json;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Places;
    using Sparkle.Services.StatusApi;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Spatial;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;

    public class PlacesService : ServiceBase, IPlacesService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacesService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal PlacesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IPlacesRepository PlacesRepository
        {
            get { return this.Repo.Places; }
        }

        public IList<Place> SelectAll()
        {
            return PlacesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .ToList();
        }

        public Place SelectPlaceById(int id)
        {
            return this.Repo.Places
                .GetById(id, this.Services.NetworkId, PlaceOptions.Category);
        }

        public IList<Place> SelectLunchPlaces()
        {
            return PlacesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .PlacesForLunch()
                .OrderBy(o => o.Name)
                .ToList();
        }

        public Place SelectPlaceByAlias(string alias)
        {
            return PlacesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .WithAlias(alias)
                .FirstOrDefault();
        }

        public PlaceModel GetByAlias(string alias)
        {
            var item = this.Repo.Places.GetByAlias(this.Services.NetworkId, alias);
            if (item == null)
                return null;

            var model = new PlaceModel(item);

            var category = this.Repo.PlacesCategories.GetById(item.CategoryId);
            if (category != null)
            {
                model.Category = new PlaceCategoryModel(category);
            }

            return model;
        }

        public long Insert(Place item)
        {
            this.SetNetwork(item);

            return this.PlacesRepository.Insert(item).Id;
        }

        public long Update(Place item)
        {
            this.VerifyNetwork(item);

            return this.PlacesRepository.Update(item).Id;
        }

        public IList<Place> Search(string request)
        {
            return this.PlacesRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Contain(request)
                .Take(5)
                .ToList();
        }

        public IList<Place> SelectAllMainPlaces()
        {
            return PlacesRepository
                .Select()
                .Where(place => place.Main)
                .ToList();
        }

        public IList<Place> SelectMainPlacesByNetworkId(int networkId)
        {
            return PlacesRepository
                .Select()
                .Where(place => place.NetworkId == networkId && place.Main)
                .ToList();
        }

        public int Count()
        {
            return this.PlacesRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Count();
        }

        public IList<Place> GetBySearch(string search)
        {
            return this.PlacesRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Contain(search)
                .Take(5)
                .ToList();
        }

        public EditPlaceRequest GetAddRequest(EditPlaceRequest model)
        {
            if (model != null)
            {
                var user = this.Services.Cache.GetUser(model.ActingUserId);
                var company = this.Services.Company.GetByAlias(model.CompanyAlias);
                if (company != null)
                {
                    model.CompanyName = company.Name;

                    if (string.IsNullOrEmpty(model.Name))
                    {
                        model.Name = company.Name;
                    }

                    var isUserInCompany = user.CompanyId == company.ID;
                    var isUserAdmin = user.NetworkAccessLevel.HasValue && user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.ManageCompany, NetworkAccessLevel.SparkleStaff);
                    if (isUserAdmin || isUserInCompany)
                        model.IsAuthorized = true;
                }
            }
            else
            {
                model = new EditPlaceRequest();
                model.IsAuthorized = true;
            }

            this.FillEditRequest(model);

            return model;
        }

        public EditPlaceRequest GetEditRequest(int placeId, EditPlaceRequest model)
        {
            var place = this.Repo.Places.GetById(placeId, this.Services.NetworkId, PlaceOptions.None);
            if (place == null)
                return null;

            var request = model ?? new EditPlaceRequest(place);
            request.Place = new PlaceModel(place);

            this.FillEditRequest(request);
            return request;
        }

        private void FillPlaceCategories(IList<PlaceCategoryModel> models, IDictionary<int, IList<PlaceCategory>> categories, int currentParentId, int depth)
        {
            var modelsToAdd = new List<PlaceCategoryModel>();
            foreach (var item in categories[currentParentId])
            {
                PlaceCategoryModel model = new PlaceCategoryModel(item);
                model.Depth = depth;

                if (categories.ContainsKey(model.Id))
                {
                    model.Children = new List<PlaceCategoryModel>();
                    this.FillPlaceCategories(model.Children, categories, model.Id, depth + 1);
                }
                modelsToAdd.Add(model);
            }

            models.AddRange(modelsToAdd.OrderBy(o => o.Name));
        }

        private void FillEditRequest(EditPlaceRequest model)
        {
            // Catégories
            IList<PlaceCategory> categories = this.Services.PlacesCategories.SelectAll();
            model.Categories = new List<PlaceCategoryModel>();
            this.FillPlaceCategories(
                model.Categories,
                categories.GroupBy(o => o.ParentId).ToDictionary(o => o.Key, o => (IList<PlaceCategory>)o.ToList()),
                0,
                0);
            if (model.CategoryId == 0)
                model.CategoryId = this.Repo.PlacesCategories.GetDefaultPlaceCategory().Id;

            // Lieux
            IList<Place> places = this.Services.Places.SelectAll();
            model.Places = new List<PlaceModel>();
            foreach (var place in places.Where(p => p.ParentId == null))
            {
                var placeModel = new PlaceModel(place);
                IList<PlaceModel> children = places
                    .Where(p => p.ParentId.HasValue && p.ParentId.Value == place.Id)
                    .Select(p => new PlaceModel(p))
                    .OrderBy(c => c.Name)
                    .ToList();
                foreach (var child in children)
                {
                    IList<Place> children2 = places
                        .Where(p => p.ParentId.HasValue && p.ParentId.Value == child.Id)
                        .ToList();
                    child.Children = children2
                        .Select(p => new PlaceModel(p))
                        .OrderBy(c => c.Name)
                        .ToList();
                }

                placeModel.Children = children;
                model.Places.Add(placeModel);
            }

            model.Places = model.Places.OrderBy(c => c.Name).ToList();
        }

        public PlaceModel GetById(int placeId, PlaceOptions options)
        {
            var place = this.Repo.Places.GetById(placeId, this.Services.NetworkId, options);
            if (place == null)
                return null;

            return new PlaceModel(place);
        }

        public IDictionary<int, PlaceModel> GetById(int[] placeIds, PlaceOptions options)
        {
            var places = this.Repo.Places.GetById(placeIds, options);
            var models = new Dictionary<int, PlaceModel>(places.Count);
            foreach (var place in places.Values)
            {
                models.Add(place.Id, new PlaceModel(place));
            }

            return models;
        }

        public EditPlaceResult Create(EditPlaceRequest request)
        {
            const string logPath = "PlacesService.Create";
            if (request == null)
                throw new ArgumentException("request");

            var result = new EditPlaceResult(request);

            if (!request.IsValid)
            {
                return this.LogResult(result, logPath);
            }

            if (request.CategoryId == 0)
            {
                result.Errors.Add(AddPlaceError.MissingCategory, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var actingUser = this.Repo.People.GetActiveById(request.ActingUserId, Data.Options.PersonOptions.Company);
            if (actingUser == null || !this.Services.People.IsActive(actingUser))
            {
                result.Errors.Add(AddPlaceError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            Company company = null;
            if (request.CompanyAlias != null)
            {
                company = this.Services.Company.GetByAlias(request.CompanyAlias);
                if (company == null)
                {
                    result.Errors.Add(AddPlaceError.NoSuchCompany, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }

                var isUserCompany = company.ID == actingUser.CompanyID;
                var isUserAdmin = actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManageCompany, NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);
                if (!isUserCompany && !isUserAdmin)
                {
                    result.Errors.Add(AddPlaceError.CannotAddPlaceForCompany, NetworksEnumMessages.ResourceManager);
                    return this.LogResult(result, logPath);
                }
            }

            request.Alias = request.Name.MakeUrlFriendly(true);

            var place = new Place
            {
                ParentId = request.ParentId <= 0 ? null : request.ParentId,
                Name = request.Name,
                Alias = this.MakeAlias(request.Name),
                Description = request.Description,
                CategoryId = request.CategoryId,
                Address = request.Address,
                ZipCode = request.ZipCode,
                City = request.City,
                Country = request.Country,
                Building = request.Building,
                Access = request.Access,
                Floor = request.Floor,
                Door = request.Door,
                PeopleOwner = actingUser.UserId,
                CreatedByUserId = actingUser.Id,
                FoursquareId = request.FoursquareId,
                lat = request.Latitude != null ? Convert.ToDouble(request.Latitude.Value) : default(double?),
                lon = request.Longitude != null ? Convert.ToDouble(request.Longitude.Value) : default(double?),
                ////ImportedId = request.GetImportedId(), // implement later
            };

            this.Services.Places.Insert(place);

            DbGeography geography = null;
            if (request.Latitude != null && request.Longitude != null)
            {
                var invariantCulture = CultureInfo.InvariantCulture;
                var numberFormat = invariantCulture.NumberFormat;
                var geoResultValue = "POINT(" + request.Longitude.Value.ToString(numberFormat) + " " + request.Latitude.Value.ToString(numberFormat) + ")";
                geography = DbGeography.FromText(geoResultValue, 4326);
                this.Repo.Places.SetGeography(place.Id, geography);

                place.Geography = geography;
            }

            if (company != null)
            {
                var companyPlace = new CompanyPlace
                {
                    CompanyId = company.ID,
                    PlaceId = place.Id,
                    DateCreatedUtc = DateTime.UtcNow,
                    CreatedByUserId = actingUser.Id,
                };

                try
                {
                    this.Repo.CompanyPlaces.Insert(companyPlace);
                }
                catch (DbException ex)
                {
                    result.Errors.Add(new ResultError<AddPlaceError>(AddPlaceError.InternalError, NetworksEnumMessages.ResourceManager)
                    {
                        ////Detail = ex.Message,
                    });
                    return this.LogResult(result, logPath, ex.Message);
                }
            }

            place = this.Repo.Places.GetById(place.Id, PlaceOptions.Category);
            var model = new PlaceModel(place);
            result.Place = model;

            var hasAddress = !string.IsNullOrEmpty(place.Address) && !string.IsNullOrEmpty(place.City);
            if (hasAddress && geography == null)
            {
                result.GeocodeSucceed = this.TryGeocodePlace(logPath, model);
            }

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public string MakeAlias(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (this.Repo.Places.GetByAlias(this.Services.NetworkId, alias) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.Places.GetByAlias(this.Services.NetworkId, a) == null);
            }

            return alias;
        }

        public EditPlaceResult Edit(EditPlaceRequest request)
        {
            const string logPath = "PlacesService.Edit";
            if (request == null)
                throw new ArgumentException("request");

            var result = new EditPlaceResult(request);

            if (!request.IsValid)
            {
                return result;
            }

            if (request.CategoryId == 0)
            {
                result.Errors.Add(AddPlaceError.MissingCategory, NetworksEnumMessages.ResourceManager);
                return result;
            }

            request.Alias = request.Name.MakeUrlFriendly(true);

            var place = this.Repo.Places.GetById(request.Id, this.Services.NetworkId, PlaceOptions.None);
            if (place == null)
            {
                result.Errors.Add(AddPlaceError.NoSuchPlace, NetworksEnumMessages.ResourceManager);
                return result;
            }

            place.ParentId = request.ParentId <= 0 ? null : request.ParentId;
            place.Name = request.Name;
            place.Alias = request.Alias;
            place.Description = request.Description;
            place.CategoryId = request.CategoryId;

            bool hasAddressChanged = false, hasAddress = false;
            if (!string.Equals(place.Address, request.Address))
            {
                place.Address = request.Address;
                hasAddressChanged |= true;
            }

            if (!string.Equals(place.ZipCode, request.ZipCode))
            {
                place.ZipCode = request.ZipCode;
                hasAddressChanged |= true;
            }

            if (!string.Equals(place.City, request.City))
            {
                place.City = request.City;
                hasAddressChanged |= true;
            }

            if (!string.Equals(place.Country, request.Country))
            {
                place.Country = request.Country;
                hasAddressChanged |= true;
            }

            if (!string.Equals(place.Address, request.Address))
            {
                place.Address = request.Address;
                hasAddressChanged |= true;
            }

            hasAddress = !string.IsNullOrEmpty(place.Address) && !string.IsNullOrEmpty(place.City);

            if (hasAddressChanged)
            {
                place.Geography = null;
                place.lat = null;
                place.lon = null;
            }

            place.Building = request.Building;
            place.Floor = request.Floor;
            place.Door = request.Door;
            place.FoursquareId = request.FoursquareId;

            this.Services.Places.Update(place);
            result.Succeed = true;

            place = this.Repo.Places.GetById(place.Id, PlaceOptions.Category);
            var model = new PlaceModel(place);
            result.Place = model;

            if (hasAddressChanged && hasAddress)
            {
                result.GeocodeSucceed = this.TryGeocodePlace(logPath, model);
            }

            return result;
        }

        public string GetProfileUrl(Place place, UriKind uriKind)
        {
            var path = "Places/Place/" + Uri.EscapeUriString(place.Alias);
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public IDictionary<int, Place> GetEntityById(int[] placeIds, PlaceOptions options)
        {
            return this.Repo.Places.GetById(placeIds, options);
        }

        public IList<PlaceModel> GetPlacesByEventPopularity()
        {
            return this.Repo
                .Places
                .GetAllByEventPopularity(this.Services.NetworkId, PlaceOptions.Category)
                .Select(o => new PlaceModel(o) { Category = new PlaceCategoryModel(o.PlaceCategory) })
                .ToList();
        }

        public IList<PlaceModel> GetAll()
        {
            return this.Repo.Places.GetAll(this.Services.NetworkId)
                .Select(o => new PlaceModel(o))
                .ToList();
        }

        public PlacePickerModel GetPlacePickerModel(PlacePickerModel model)
        {
            if (model == null)
                model = new PlacePickerModel();

            var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            FreeGeoIpModel freegoipModel = null;
            if (!string.IsNullOrEmpty(model.UserRemoteAddress))
            {
                freegoipModel = this.Services.Cache.GetLocationViaFreegeoip(model.UserRemoteAddress);
            }

            double? lat = null;
            double? lon = null;
            if (freegoipModel != null
                && (lat = freegoipModel.Latitude).HasValue && lat.Value != 0D
                && (lon = freegoipModel.Longitude).HasValue && lon.Value != 0D)
            {
                var city = freegoipModel.City;
                var country = freegoipModel.CountryName;
                model.Geography = lat.Value.ToString(numberFormat) + " " + lon.Value.ToString(numberFormat);
                if (!string.IsNullOrEmpty(city))
                {
                    model.Location = city;
                    if (!string.IsNullOrEmpty(country))
                        model.Location += ", " + country;
                }
                else
                {
                    model.Location = model.Geography;
                }
            }
            else
            {
                var defaultPlace = this.Repo.Places.GetDefaultPlaces(this.Services.NetworkId).FirstOrDefault();

                if (defaultPlace != null)
                {
                    var placeModel = new PlaceModel(defaultPlace);
                    if (placeModel.Location != null)
                    {
                        var latitude = placeModel.Location.Latitude;
                        var longitude = placeModel.Location.Longitude;
                        model.Geography = placeModel.Location != null && latitude.HasValue && longitude.HasValue ? latitude.Value.ToString(numberFormat) + " " + longitude.Value.ToString(numberFormat) : null;
                    }

                    model.Location = placeModel.City;
                    if (!string.IsNullOrEmpty(placeModel.Country))
                    {
                        model.Location += ", " + placeModel.Country;
                    }
                }
            }

            return model;
        }

        public FreeGeoIpModel GetLocationViaFreegeoip(string ip)
        {
            // PREFER this.Services.Cache.GetLocationViaFreegeoip(ip)

            const string logPath = "PlacesService.GetLocationViaFreegoip";
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("ip");

            var watch = Stopwatch.StartNew();
            var freegoipRequestAddress = "http://freegeoip.net/json/" + ip;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(freegoipRequestAddress);
                httpRequest.Timeout = 5000;
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                TimeSpan httpRequestTime = watch.Elapsed;
                var responseStream = new StreamReader(httpResponse.GetResponseStream());
                var json = responseStream.ReadToEnd();
                TimeSpan httpResponseTime = watch.Elapsed;
                var result = JsonConvert.DeserializeObject<FreeGeoIpModel>(json);
                TimeSpan fullTime = watch.Elapsed;

                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "Query timing: " + httpRequestTime + ";" + httpResponseTime + ";" + fullTime + " (http req, resp, deser).");
                return result;
            }
            catch (Exception ex)
            {
                TimeSpan fullTime = watch.Elapsed;
                this.Services.Logger.Error(
                    "PlacesServices.GetLocationViaFreegoip",
                    ErrorLevel.ThirdParty,
                    "Query timing:" + fullTime + " " + ex);
                return null;
            }
        }

        private IList<PlaceModel> SearchPlacesFromFoursquare(string name, string location, bool isCoordinates)
        {
            var clientId = this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientId;
            var clientSecret = this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientSecret;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                this.Services.Logger.Error("PlacesService.SearchPlacesFromFoursquare", ErrorLevel.Internal, "Foursquare credentials are not set");
                return new List<PlaceModel>();
            }

            if (isCoordinates && location.Contains(" "))
                location = location.Replace(" ", ",");

            IList<FourSquare.SharpSquare.Entities.Venue> results = null;
            try
            {
                var api = new SharpSquare(clientId, clientSecret);
                var searchParams = new Dictionary<string, string>
                {
                    { "query", name },
                    { (isCoordinates ? "ll" : "near"), location },
                };

                results = api.SearchVenues(searchParams);
            }
            catch (Exception ex)
            {
                this.Services.Logger.Error("PlacesService.SearchPlacesFromFoursquare", ErrorLevel.ThirdParty, ex);
                return new List<PlaceModel>();
            }

            return results
                .Select(o =>
                {
                    var primaryCategory = o.categories != null && o.categories.SingleOrDefault(c => c.primary) != null ? o.categories.SingleOrDefault(c => c.primary) : o.categories.FirstOrDefault();
                    var category = new PlaceCategoryModel
                    {
                        Symbol = primaryCategory != null ? this.Services.PlacesCategories.GetSymbolFromFoursquareUrlPrefix(primaryCategory.icon.prefix) : null
                    };
                    return new PlaceModel(o, m => this.Services.Lang.T(m)) { Category = category };
                })
                .ToList();
        }

        public IList<PlaceModel> SearchByNameAndLocation(string[] words, string[] geocodes, string location)
        {
            var numberFormat = CultureInfo.InvariantCulture.NumberFormat;

            // pre-treatement of geocodes if necessary (required format: lat lon)
            var places = this.Repo.Places.GetPlacesByNameAndRadius(this.Services.NetworkId, words, geocodes, 30, true);
            var geographyCodes = geocodes
                .Select(p =>
                {
                    var split = p.Split(new char[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);
                    return DbGeography.PointFromText("POINT(" + split[1].Replace(",", ".") + " " + split[0].Replace(",", ".") + ")", 4326);
                })
                .ToList();

            // getting place categories for categories symbol
            var usedCategoriesIds = places.Select(o => o.CategoryId).Distinct().ToArray();
            var categories = this.Repo.PlacesCategories.GetById(usedCategoriesIds);

            // make a search on foursquare with the coordinates if exists and unique, otherwise with location
            var exactLocation = false;
            var locationToSearch = location;
            if (geocodes.Length == 1)
            {
                exactLocation = true;
                locationToSearch = geocodes.First();
            }

            var foursquarePlaces = this.SearchPlacesFromFoursquare(string.Join(" ", words), locationToSearch, exactLocation);
            // setting the GeographicDistance to the foursquare items
            foreach (var item in foursquarePlaces)
            {
                if (item.Location.Latitude.HasValue && item.Location.Longitude.HasValue)
                {
                    var geographyItem = DbGeography.PointFromText("POINT(" + item.Location.Longitude.Value.ToString(numberFormat) + " " + item.Location.Latitude.Value.ToString(numberFormat) + ")", 4326);
                    item.GeographicDistance = geographyCodes.Count > 0 ? geographyCodes.Min(o => geographyItem.Distance(o)) : default(double?);
                }
            }

            var localFoursquarePlacesIds = places.Where(o => !string.IsNullOrEmpty(o.FoursquareId)).Select(o => o.FoursquareId).ToArray();

            // setting the GeographicDistance and Category of local items and merge with foursquare items
            var results = places
                .Select(p => new PlaceModel(p)
                {
                    GeographicDistance = p.Geography != null && geographyCodes.Count > 0 ? geographyCodes.Min(o => p.Geography.Distance(o)) : default(double?),
                    Category = categories.ContainsKey(p.CategoryId) ? new PlaceCategoryModel { Symbol = categories[p.CategoryId].Symbol } : null,
                })
                .OrderBy(o => o.GeographicDistance ?? double.MaxValue)
                .ToList();
            results
                .AddRange(foursquarePlaces
                            .Where(o => o.GeographicDistance < 30000 && !localFoursquarePlacesIds.Contains(o.FoursquareId))
                            .Take(50));

            return results;
        }

        public IDictionary<int, PlaceModel> GetAllForCache()
        {
            var items = this.Repo.Places.GetAll(this.Services.NetworkId);
            var categories = this.Repo
                .PlacesCategories
                .GetAll()
                .Select(o => new PlaceCategoryModel(o.Value))
                .ToDictionary(o => o.Id, o => o);
            return this.PrepareCacheItems(items, categories);
        }

        public IDictionary<int, PlaceModel> GetForCache(int[] ids)
        {
            var items = this.Repo.Places.GetById(ids, PlaceOptions.None);
            var categories = this.Repo
                .PlacesCategories
                .GetAll()
                .Select(o => new PlaceCategoryModel(o.Value))
                .ToDictionary(o => o.Id, o => o);
            return this.PrepareCacheItems(items.Values, categories);
        }

        private IDictionary<int, PlaceModel> PrepareCacheItems(ICollection<Place> items, IDictionary<int, PlaceCategoryModel> categories)
        {
            var result = items.ToDictionary(u => u.Id, u => new PlaceModel(u) { Category = categories[u.CategoryId], });

            foreach (var item in result.Values)
            {
                item.SetAspect<PlaceModel, StringSearchAspect>(new StringSearchAspect(item.Name, item.City));
            }

            return result;
        }

        public IList<PlaceModel> GetByParentId(int id, PlaceOptions options)
        {
            var items = this.Repo.Places.GetByParentId(id, options);
            var models = new List<PlaceModel>(items.Count);
            models.AddRange(items.Select(p => new PlaceModel(p)));

            var categoryIds = items.Select(p => p.CategoryId).ToArray();
            var categories = this.Repo.PlacesCategories.GetById(categoryIds)
                .ToDictionary(c => c.Key, c => new PlaceCategoryModel(c.Value));
            foreach (var item in models)
            {
                item.Category = categories[item.CategoryId];
            }

            return models;
        }

        public IList<PlaceModel> GetParents(int id, PlaceOptions options)
        {
            var items = new List<PlaceModel>();

            Place item = this.Repo.Places.GetById(id, options);
            ////items.Add(new PlaceModel(item));
            while (item.ParentId != null)
            {
                item = this.Repo.Places.GetById(item.ParentId.Value, options);
                items.Insert(0, new PlaceModel(item));
            }

            return items;
        }

        public bool TryGeocodePlace(string logPath, PlaceModel place)
        {
            logPath += "..TryGeocodePlace";

            var addressString = place.AddressString;
            if (addressString != null && this.Services.AppConfiguration.Tree.Externals.SparkleStatus.ApiKey != null)
            {
                var sparkleApi = new SparkleStatusApi(
                    this.Services.AppConfiguration.Tree.Externals.SparkleStatus.ApiKey,
                    this.Services.AppConfiguration.Tree.Externals.SparkleStatus.Api2Key,
                    this.Services.AppConfiguration.Tree.Externals.SparkleStatus.Api2Secret,
                    this.Services.AppConfiguration.Tree.Externals.SparkleStatus.BaseUrl);
                var watch = Stopwatch.StartNew();
                try
                {
                    var geoResult = sparkleApi.GetLocationGeolocFromCache(addressString);
                    watch.Stop();
                    this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "API call to SparkleStatus GetLocationGeolocFromCache succeeded in " + watch.Elapsed + " with query '" + addressString + "' returned '" + string.Join("', '", geoResult.Geocodes) + "'");

                    var geoResult1 = geoResult != null && geoResult.Geocodes != null ? geoResult.Geocodes.FirstOrDefault() : null;
                    if (geoResult1 != null)
                    {
                        var numberStyleFloat = NumberStyles.Float;
                        var invariantCulture = CultureInfo.InvariantCulture;
                        var numberFormat = invariantCulture.NumberFormat;
                        var geoSplit = geoResult1.Split(new char[] { ' ', });
                        double lat;
                        double lon;
                        if (geoSplit.Length == 2
                            && double.TryParse(geoSplit[0], numberStyleFloat, invariantCulture, out lat)
                            && double.TryParse(geoSplit[1], numberStyleFloat, invariantCulture, out lon))
                        {
                            var geoResultValue = "POINT(" + lon.ToString(numberFormat) + " " + lat.ToString(numberFormat) + ")";

                            var geography = DbGeography.FromText(geoResultValue, 4326);
                            this.Repo.Places.SetGeography(place.Id, geography);

                            place.Geography = geography;
                            place.Location = new GeographyModel(geography);

                            return true;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    watch.Stop();
                    this.Services.Logger.Warning(logPath, ErrorLevel.ThirdParty, "API call to SparkleStatus GetLocationGeolocFromCache succeeded in " + watch.Elapsed + " with query '" + addressString + "' returned error '" + ex.Message + "'");
                }
            }

            return false;
        }

        public ImportFoursquarePlaceResult ImportFoursquarePlace(ImportFoursquarePlaceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "The value cannot be null");

            var result = new ImportFoursquarePlaceResult(request);
            var logPath = "PlacesService.ImportFoursquarePlace";

            var clientId = this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientId;
            var clientsecret = this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientSecret;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientsecret))
            {
                result.Errors.Add(ImportFoursquarePlaceError.FoursquareNotConfigured, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            FourSquare.SharpSquare.Entities.Venue item = null;
            try
            {
                var api = new SharpSquare(clientId, clientsecret);
                item = api.GetVenue(request.FoursquareId);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ImportFoursquarePlaceError.FoursquareApiError, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var transaction = this.Services.Repositories.NewTransaction();
            using (transaction.BeginTransaction())
            {
                // category
                PlaceCategory localCategory = null;
                var primaryCategory = item.categories != null ? item.categories.SingleOrDefault(o => o.primary) : item.categories.FirstOrDefault();
                if (primaryCategory == null || (localCategory = transaction.Repositories.PlacesCategories.GetByFoursquareId(primaryCategory.id)) == null)
                {
                    localCategory = transaction.Repositories.PlacesCategories.GetDefaultPlaceCategory();
                }

                string address = null;
                string city = null;
                string zipcode = null;
                string country = null;
                double? lat = null;
                double? lon = null;
                DbGeography geo = null;
                if (item.location != null)
                {
                    address = item.location.address;
                    city = item.location.city;
                    zipcode = item.location.postalCode;
                    country = item.location.country;

                    var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                    lat = item.location.lat;
                    lon = item.location.lng;

                    geo = DbGeography.PointFromText("POINT(" + lon.Value.ToString(numberFormat) + " " + lat.Value.ToString(numberFormat) + ")", 4326);
                }

                var about = string.IsNullOrEmpty(item.description) ? null : item.description;

                var place = new Place
                {
                    CategoryId = localCategory.Id,
                    Name = item.name.TrimTextRight(50),
                    Alias = this.MakeAlias(item.name.TrimTextRight(50)),
                    Description = about.TrimTextRight(4000),
                    Address = address.TrimTextRight(50),
                    ZipCode = zipcode.TrimTextRight(10),
                    City = city.TrimTextRight(50),
                    Country = country.TrimTextRight(50),
                    CreatedByUserId = request.ActingUserId,
                    lat = lat,
                    lon = lon,
                    NetworkId = this.Services.NetworkId,
                    FoursquareId = item.id,
                };

                place = transaction.Repositories.Places.Insert(place);
                transaction.Repositories.Places.SetGeography(place.Id, geo);

                transaction.CompleteTransaction();
                result.PlaceCreated = new PlaceModel(place);
            }

            this.Services.Cache.InvalidatePlaces();

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public IList<CompanyPlaceModel> GetCompanyPlaces(int placeId)
        {
            return this.Repo.CompanyPlaces.GetByPlaceId(placeId).Select(o => new CompanyPlaceModel(o)).ToList();
        }

        public string GetLocationStringFromIp(string ip)
        {
            var location = this.Services.Cache.GetLocationViaFreegeoip(ip);
            if (location != null)
            {
                if (!string.IsNullOrEmpty(location.City) && !string.IsNullOrEmpty(location.CountryCode))
                {
                    try
                    {
                        var country = new RegionInfo(location.CountryCode);
                        return location.City + ", " + country.DisplayName;
                    }
                    catch (Exception ex)
                    {
                        this.Services.Logger.Error("PlacesService.GetLocationStringFromIp", ErrorLevel.ThirdParty, ex);
                    }
                }
            }

            return null;
        }

        public IDictionary<string, IList<PlaceModel>> GetPlacesUsedByCompanies()
        {
            return this.Repo.CompanyPlaces.GetUsedPlacesGroupedByLocationString(this.Services.NetworkId).ToDictionary(o => o.Key, o => (IList<PlaceModel>)o.Value.Select(p => new PlaceModel(p)).ToList());
        }
    }
}
