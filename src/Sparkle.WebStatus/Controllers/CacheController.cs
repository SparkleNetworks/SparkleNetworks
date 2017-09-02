
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.Services.StatusApi;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [Authorize]
    public class CacheController : Controller
    {
        /// <summary>
        /// Network websites call this method to do geocodong requests. The results are cached in the status database to limit queries to this limited API.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetLocationGeoloc(string id)
        {
            var location = id;
            if (string.IsNullOrEmpty(location))
                return this.ResultService.JsonError("EmptyLocation");

            ////if (!this.ApiServices.IsKeyAuthorized(this.Request))
            ////    return this.ResultService.JsonError("UnauthorizedApiKey");

            var geolocCollection = this.Domain.Caches.GetGeolocCollectionFromLocation(location);
            var model = new LocationGeolocData
            {
                Geocodes = geolocCollection.Select(o => o.Value).ToArray(),
            };

            return this.ResultService.JsonSuccess(model);
        }
    }
}
