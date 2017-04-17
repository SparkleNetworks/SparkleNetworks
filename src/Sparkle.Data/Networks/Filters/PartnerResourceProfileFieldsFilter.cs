
namespace Sparkle.Data.Filters
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class PartnerResourceProfileFieldsFilter
    {
        public static PartnerResourceProfileField SingleByType(this IEnumerable<PartnerResourceProfileField> items, ProfileFieldType type)
        {
            return items.Where(o => o.ProfileFieldType == type).SingleOrDefault();
        }
    }
}
