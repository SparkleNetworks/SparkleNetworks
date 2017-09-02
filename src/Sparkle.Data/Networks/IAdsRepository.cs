
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Repository("Ads")]
    public interface IAdsRepository : IBaseNetworkRepository<Ad, int>
    {
        IDictionary<int, Ad> GetById(IList<int> ids, AdOptions options);
        IDictionary<int, Ad> GetById(int[] adIds, int networkId, AdOptions options);
        Ad GetById(int id, int networkId, AdOptions options);
        Ad GetByAlias(string alias, int networkId, AdOptions options);
        Ad GetByAlias(string alias, AdOptions options);

        IList<Ad> GetList(Ad.Columns sort, bool desc, int networkId, bool openOnly, bool isValidationRequired, int offset, int pageSize, AdOptions options, int? userId);
        int Count(int networkId, bool openOnly, bool isValidationRequired, int? userId);

        IList<Ad> GetListByDateRange(DateTime from, DateTime to, Ad.Columns sort, bool desc, int networkId, bool openOnly, bool isValidationRequired, int offset, int pageSize, AdOptions options);
        int CountByDateRange(DateTime from, DateTime to, int networkId, bool openOnly, bool isValidationRequired);

        IList<Ad> GetPendingList(int networkId, AdOptions options);
        int GetPendingCount(int networkId);

        int CountActiveOpenAfter(int networkId, DateTime date, bool isValidationRequired);

        int CountActiveOpen(int networkId, bool isValidationRequired);
    }
}
