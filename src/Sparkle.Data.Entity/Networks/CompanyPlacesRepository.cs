
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompanyPlacesRepository : BaseNetworkRepositoryInt<CompanyPlace>, ICompanyPlacesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyPlacesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyPlaces)
        {
        }

        public IList<CompanyPlace> GetByCompanyId(int companyId)
        {
            return this.Set
                .Where(o => o.CompanyId == companyId)
                .ToList();
        }

        public IList<CompanyPlace> GetByCompanyId(int[] companyId)
        {
            return this.Set
                .Where(o => companyId.Contains(o.CompanyId))
                .ToList();
        }

        public IList<CompanyPlace> GetByPlaceId(int placeId)
        {
            return this.Set
                .Where(o => o.PlaceId == placeId)
                .ToList();
        }

        public IList<CompanyPlace> GetByPlaceId(int[] placeIds)
        {
            return this.Set
                .Where(o => placeIds.Contains(o.PlaceId))
                .ToList();
        }

        public IList<CompanyPlace> GetAll()
        {
            return this.Set
                .ToList();
        }

        public IDictionary<string, IList<Place>> GetUsedPlacesGroupedByLocationString(int networkId)
        {
            return this.Set
                .Where(o => o.Place.NetworkId == networkId && o.Place.City != null || o.Place.Country != null)
                .Select(o => o.Place)
                .GroupBy(o => new { o.City, o.Country, })
                .ToDictionary(o => string.Join(", ", o.Key.City.NullIfEmpty(), o.Key.Country.NullIfEmpty()), o => (IList<Place>)o.ToList());
        }
    }
}
