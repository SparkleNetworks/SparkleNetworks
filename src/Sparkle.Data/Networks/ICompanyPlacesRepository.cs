
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Repository]
    public interface ICompanyPlacesRepository : IBaseNetworkRepository<CompanyPlace, int>
    {
        IList<CompanyPlace> GetByCompanyId(int companyId);
        IList<CompanyPlace> GetByCompanyId(int[] companyId);

        IList<CompanyPlace> GetByPlaceId(int placeId);
        IList<CompanyPlace> GetByPlaceId(int[] placeIds);

        IList<CompanyPlace> GetAll();

        IDictionary<string, IList<Place>> GetUsedPlacesGroupedByLocationString(int networkId);
    }
}
