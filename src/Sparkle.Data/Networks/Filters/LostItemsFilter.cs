
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public static class LostItemsFilter
    {
        public static IQueryable<LostItem> ById(this IQueryable<LostItem> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<LostItem> Visible(this IQueryable<LostItem> qry)
        {
            return qry.Where(o => o.State == true);
        }
    }
}
