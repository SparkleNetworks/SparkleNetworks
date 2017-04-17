
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public static class RelationshipFilter
    {
        public static IQueryable<Relationship> WithId(
        this IQueryable<Relationship> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }
    }
}
