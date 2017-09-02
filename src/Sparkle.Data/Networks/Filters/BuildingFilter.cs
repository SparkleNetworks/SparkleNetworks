
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class BuildingFilter
    {
        public static IQueryable<Building> ById(this IQueryable<Building> qry, long id)
        {
            return qry.Where(o => o.Id == id);
        }
    }
}
