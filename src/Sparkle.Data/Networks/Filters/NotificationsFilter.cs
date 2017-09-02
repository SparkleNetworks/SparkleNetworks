
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class NotificationsFilter
    {
        public static IQueryable<Notification> WithUserId(this IQueryable<Notification> qry, int guid)
        {
            return qry.Where(o => o.UserId == guid);
        }

        public static IQueryable<Notification> SubscribedToMainTimelineItems(this IQueryable<Notification> qry, bool defaultNetworkValue)
        {
            if (defaultNetworkValue)
                return qry.Where(o => o.MainTimelineItems == null || (o.MainTimelineItems != null && o.MainTimelineItems.Value));
            else
                return qry.Where(o => o.MainTimelineItems != null && o.MainTimelineItems.Value);
        }

        public static IQueryable<Notification> SubscribedToMainTimelineComments(this IQueryable<Notification> qry, bool defaultNetworkValue)
        {
            if (defaultNetworkValue)
                return qry.Where(o => o.MainTimelineComments == null || (o.MainTimelineComments != null && o.MainTimelineComments.Value));
            else
                return qry.Where(o => o.MainTimelineComments != null && o.MainTimelineComments.Value);
        }
    }
}
