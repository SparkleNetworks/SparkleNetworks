
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class ProjectsFilter
    {
        public static IQueryable<Project> WithCompanyId(this IQueryable<Project> qry, int companyId)
        {
            return qry.Where(o => o.OwnerType == 1 && o.OwnerValue == companyId);
        }
    }
}
