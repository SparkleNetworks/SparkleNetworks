
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class AchievementsFilter
    {
        public static IQueryable<Achievement> ThemeId(this IQueryable<Achievement> query, int id)
        {
            return query.Where(o => o.ThemeId == id);
        }
    }
}
