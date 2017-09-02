
namespace Sparkle.Data.Filters
{
    using Sparkle.Entities.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public static class CompanyProfileFieldsFilter
    {
        public static CompanyProfileField SingleByType(this IEnumerable<CompanyProfileField> query, ProfileFieldType type)
        {
            return query.Where(o => o.ProfileFieldType == type).SingleOrDefault();
        }

        public static string About(this IEnumerable<CompanyProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.About).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string Site(this IEnumerable<CompanyProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Site).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string Twitter(this IEnumerable<CompanyProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Twitter).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }
    }
}
