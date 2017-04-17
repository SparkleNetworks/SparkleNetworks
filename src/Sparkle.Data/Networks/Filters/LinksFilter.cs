
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class LinksFilter
    {
        public static IQueryable<Link> WithUserId(
       this IQueryable<Link> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }
    }
}
