
namespace Sparkle.Services.Networks.Places
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportFoursquarePlaceRequest : BaseRequest
    {
        public string FoursquareId { get; set; }

        public int ActingUserId { get; set; }
    }

    public class ImportFoursquarePlaceResult : BaseResult<ImportFoursquarePlaceRequest, ImportFoursquarePlaceError>
    {
        public ImportFoursquarePlaceResult(ImportFoursquarePlaceRequest request)
            : base(request)
        {
        }

        public Models.PlaceModel PlaceCreated { get; set; }
    }

    public enum ImportFoursquarePlaceError
    {
        FoursquareNotConfigured,
        FoursquareApiError
    }
}
