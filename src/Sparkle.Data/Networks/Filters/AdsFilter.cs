
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public static class AdsFilter
    {
        public static IQueryable<Ad> ById(this IQueryable<Ad> qry, int adId)
        {
            return qry.Where(o => o.Id == adId);
        }
    }
}
