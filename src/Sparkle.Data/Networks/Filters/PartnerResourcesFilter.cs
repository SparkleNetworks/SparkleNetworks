
namespace Sparkle.Data.Filters
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class PartnerResourcesFilter
    {
        public static IQueryable<PartnerResource> ByAlias(this IQueryable<PartnerResource> qry, string alias)
        {
            return qry.Where(o => o.Alias == alias);
        }

        public static IQueryable<PartnerResource> ByIds(this IQueryable<PartnerResource> qry, int[] ids)
        {
            return qry.Where(o => ids.Contains(o.Id));
        }

        public static IQueryable<PartnerResource> Approved(this IQueryable<PartnerResource> qry)
        {
            return qry.Where(o => o.IsApproved);
        }

        public static IQueryable<PartnerResource> ToApprove(this IQueryable<PartnerResource> qry)
        {
            return qry
                .NotDeleted()
                .Where(o => !o.IsApproved);
        }

        public static IQueryable<PartnerResource> NotDeleted(this IQueryable<PartnerResource> qry)
        {
            return qry.Where(o => o.DateDeletedUtc == null);
        }

        public static IQueryable<PartnerResource> Active(this IQueryable<PartnerResource> qry)
        {
            return qry
                .Approved()
                .NotDeleted();
        }

        public static IQueryable<PartnerResource> ById(this IQueryable<PartnerResource> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }
    }
}
