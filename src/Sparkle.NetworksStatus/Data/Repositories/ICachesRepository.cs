﻿
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial interface ICachesRepository
    {
        IList<Cache> GetGeocodesByLocation(string location);
    }
}
