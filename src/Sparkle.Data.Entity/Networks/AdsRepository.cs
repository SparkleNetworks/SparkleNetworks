
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Filters;
    using Sparkle.Entities.Networks;
    using System.Data.Objects;

    public class AdsRepository : BaseNetworkRepositoryInt<Ad>, IAdsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public AdsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Ads)
        {
        }

        public IQueryable<Ad> NewQuery(AdOptions options)
        {
            ObjectQuery<Ad> query = this.Set;

            if ((options & AdOptions.Owner) == AdOptions.Owner)
                query = query.Include("Owner");

            if ((options & AdOptions.Category) == AdOptions.Category)
                query = query.Include("Category");

            return query;
        }

        public IDictionary<int, Ad> GetById(IList<int> ids, AdOptions options)
        {
            return this.NewQuery(options)
                .Where(a => ids.Contains(a.Id))
                .OrderByDescending(a => a.Date)
                .ToDictionary(a => a.Id, a => a);
        }

        public IDictionary<int, Ad> GetById(int[] ids, int networkId, AdOptions options)
        {
            return this.NewQuery(options)
                .Where(a => ids.Contains(a.Id) && a.NetworkId == networkId)
                .OrderByDescending(a => a.Date)
                .ToDictionary(a => a.Id, a => a);
        }

        public Ad GetById(int id, int networkId, AdOptions options)
        {
            return this.NewQuery(options).Where(x => x.Id == id && x.NetworkId == networkId).SingleOrDefault();
        }

        public Ad GetByAlias(string alias, AdOptions options)
        {
            return this.NewQuery(options).Where(x => x.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        public Ad GetByAlias(string alias, int networkId, AdOptions options)
        {
            return this.NewQuery(options).Where(x => x.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase) && x.NetworkId == networkId).SingleOrDefault();
        }

        public IList<Ad> GetList(Ad.Columns sort, bool desc, int networkId, bool openOnly, bool isValidationRequired, int offset, int pageSize, AdOptions options, int? userId)
        {
            IQueryable<Ad> query = this.GetListQuery(networkId, openOnly, isValidationRequired, options, userId);

            if (sort == Ad.Columns.UpdateDateUtc)
            {
                if (desc)
                    query = query.OrderByDescending(x => x.UpdateDateUtc ?? x.Date);
                else
                    query = query.OrderBy(x => x.UpdateDateUtc ?? x.Date);
            }
            else
            {
                throw new NotSupportedException("Sorting Ads using column '" + sort + "' and direction '" + (desc ? "desc" : "asc") + "'");
            }

            return query.Skip(offset).Take(pageSize).ToList();
        }

        public IList<Ad> GetListByDateRange(DateTime from, DateTime to, Ad.Columns sort, bool desc, int networkId, bool openOnly, bool isValidationRequired, int offset, int pageSize, AdOptions options)
        {
            IQueryable<Ad> query = this.GetListQuery(networkId, openOnly, isValidationRequired, options, null);

            if (sort == Ad.Columns.UpdateDateUtc)
            {
                if (desc)
                    query = query.OrderByDescending(x => x.UpdateDateUtc ?? x.Date);
                else
                    query = query.OrderBy(x => x.UpdateDateUtc ?? x.Date);
            }
            else
            {
                throw new NotSupportedException("Sorting Ads using column '" + sort + "' and direction '" + (desc ? "desc" : "asc") + "'");
            }

            return query
                .Where(x => from <= (x.UpdateDateUtc ?? x.Date) && (x.UpdateDateUtc ?? x.Date) <= to)
                .Skip(offset).Take(pageSize).ToList();
        }

        public int CountByDateRange(DateTime from, DateTime to, int networkId, bool openOnly, bool isValidationRequired)
        {
            IQueryable<Ad> query = this.GetListQuery(networkId, openOnly, isValidationRequired, AdOptions.None, null);

            return query
                .Where(x => from <= (x.UpdateDateUtc ?? x.Date) && (x.UpdateDateUtc ?? x.Date) <= to)
                .Count();
        }

        public int Count(int networkId, bool openOnly, bool isValidationRequired, int? userId)
        {
            IQueryable<Ad> query = GetListQuery(networkId, openOnly, isValidationRequired, AdOptions.None, userId);
            return query.Count();
        }

        public IList<Ad> GetPendingList(int networkId, AdOptions options)
        {
            IQueryable<Ad> query = GetPendingListQuery(networkId, options);
            return query.ToList();
        }

        public int GetPendingCount(int networkId)
        {
            IQueryable<Ad> query = GetPendingListQuery(networkId, AdOptions.None);
            return query.Count();
        }

        public int CountActiveOpenAfter(int networkId, DateTime date, bool isValidationRequired)
        {
            if (isValidationRequired)
            {
                return this.Set
                    .ByNetwork(networkId)
                    .Where(x => x.IsOpen && x.IsValidated == true && x.UpdateDateUtc > date)
                    .Count();
            }
            else
            {
                return this.Set
                    .ByNetwork(networkId)
                    .Where(x => x.IsOpen && x.IsValidated != false && x.UpdateDateUtc > date)
                    .Count();
            }
        }

        public int CountActiveOpen(int networkId, bool isValidationRequired)
        {
            if (isValidationRequired)
            {
                return this.Set
                    .ByNetwork(networkId)
                    .Where(x => x.IsOpen && x.IsValidated == true)
                    .Count();
            }
            else
            {
                return this.Set
                    .ByNetwork(networkId)
                    .Where(x => x.IsOpen && x.IsValidated != false)
                    .Count();
            }
        }

        private IQueryable<Ad> GetListQuery(int networkId, bool openOnly, bool isValidationRequired, AdOptions options, int? userId)
        {
            IQueryable<Ad> query = this.NewQuery(options).ByNetwork(networkId);

            if (openOnly && isValidationRequired)
            {
                query = query.Where(x => x.IsOpen && x.IsValidated == true);
            }
            else if (openOnly && !isValidationRequired)
            {
                query = query.Where(x => x.IsOpen && (x.IsValidated == null || x.IsValidated == true));
            }

            if (userId != null)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            return query;
        }

        private IQueryable<Ad> GetPendingListQuery(int networkId, AdOptions options)
        {
            IQueryable<Ad> query = this.NewQuery(options).ByNetwork(networkId);

            query = query.Where(x => x.IsOpen && (x.IsValidated == null || x.PendingEditDate != null));
            return query;
        }

    }
}
