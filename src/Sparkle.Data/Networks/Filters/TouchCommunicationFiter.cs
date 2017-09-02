
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public static class TouchCommunicationFiter
    {
        public static IQueryable<TouchCommunication> ById(
        this IQueryable<TouchCommunication> qry, long id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<TouchCommunicationItem> ById(
        this IQueryable<TouchCommunicationItem> qry, long id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<TouchCommunicationItem> ByParentId(
        this IQueryable<TouchCommunicationItem> qry, long parentId)
        {
            return qry.Where(o => o.ParentId == parentId);
        }
    }
}
