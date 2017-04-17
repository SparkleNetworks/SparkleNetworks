
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Newtonsoft.Json;
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Domain.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;

    public partial class CachesService : ICachesService
    {
        /// <summary>
        /// Queries geocode.farm for a forward geocode.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public IList<CacheModel> GetGeolocCollectionFromLocation(string location)
        {
            // when world weather api will be available, make insert of new geocodes in cache db
            var geocodes = this.Repos.Caches.GetGeocodesByLocation(location);
            if (geocodes.Count > 0)
            {
                return geocodes.Select(o => new CacheModel(o)).ToList();
            }

            var geocodeResult = this.GetGeocodingFromGeocodeFarm(location);
            if (geocodeResult != null && geocodeResult.Result != null && geocodeResult.Result.Results != null)
            {
                var numberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

                foreach (var item in geocodeResult.Result.Results)
                {
                    if (item.Coordinates != null)
                    {
                        this.Repos.Caches.Insert(new Cache
                        {
                            TypeValue = CacheType.Geocode,
                            Name = location,
                            Value = item.Coordinates.Latitude.ToString(numberFormat) + " " + item.Coordinates.Longitude.ToString(numberFormat),
                            DateCreatedUtc = DateTime.UtcNow,
                        });
                    }
                }
            }

            geocodes = this.Repos.Caches.GetGeocodesByLocation(location);
            return geocodes.Select(o => new CacheModel(o)).ToList();
        }

        private GeocodeFarmModel GetGeocodingFromGeocodeFarm(string location)
        {
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException("location");

            var geocodeFarmRequestAddress = "https://www.geocode.farm/v3/json/forward/?addr=" + Uri.EscapeUriString(location);

            var httpRequest = (HttpWebRequest)WebRequest.Create(geocodeFarmRequestAddress);
            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                var responseStream = new StreamReader(httpResponse.GetResponseStream());
                return JsonConvert.DeserializeObject<GeocodeFarmModel>(responseStream.ReadToEnd());
            }
            catch (WebException ex)
            {
                Trace.TraceError("CachesService.GetGeocodingFromGeocodeFarm: web exception for location '" + location + "' " + ex.Message);
                throw;
            }
        }
    }
}
