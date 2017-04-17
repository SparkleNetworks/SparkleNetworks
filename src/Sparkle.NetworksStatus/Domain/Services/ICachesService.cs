
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Domain.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial interface ICachesService
    {
        IList<CacheModel> GetGeolocCollectionFromLocation(string location);
    }
}
