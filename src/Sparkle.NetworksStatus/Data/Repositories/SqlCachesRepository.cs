
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlCachesRepository : ICachesRepository
    {
        public IList<Cache> GetGeocodesByLocation(string location)
        {
            return this.PetaContext.Fetch<Cache>(Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("Type = @0", (byte)CacheType.Geocode)
                .Where("Name = @0", location));
        }
    }
}
