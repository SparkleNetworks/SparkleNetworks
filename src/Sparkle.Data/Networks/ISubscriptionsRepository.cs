
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Data.Networks.Options;

    [Repository]
    public interface ISubscriptionsRepository : IBaseNetworkRepository<Subscription, int>
    {
        Subscription Insert(Subscription item, int transactId);
        IList<Subscription> GetByIds(int[] ids, SubscriptionOptions options = SubscriptionOptions.None);

        IList<SubscriptionPoco> GetCurrent(int networkId, DateTime dateTime);

        DataPage<Subscription> GetList(int networkId, int[] userId, int[] templateId, bool? active, bool? past, Subscription.Columns sortColumn, bool sortAsc, int offset, int pageSize);
        int CountList(int networkId, int[] userId, int[] templateId, bool? active, bool? past);

        void SubIsPaid(Subscription item);

        IQueryable<Subscription> NewQuery(SubscriptionOptions options);

        IList<Subscription> GetByApplyUserId(int applyUserId);

        IList<Subscription> GetCurrentAndFuture(int networkId, DateTime now);

        IList<Subscription> GetAllActive(int networkId);

        int[] GetUserIdsSubscribedAmongIds(int networkId, int[] applyingUserIds, DateTime now);

        Subscription GetFirstInTime();

        Subscription GetLastInTime();

        bool HasAnyUnlimited();

        IDictionary<int, int> CountActiveSubscriptionsInDateRange(DateTime begin, DateTime dateTime);

        IList<Subscription> GetUsersAppliedSubscriptions(int[] userIds, DateTime date);

        int CountSubscribedPeople(int networkId);

        decimal GetSumOfAllSubscriptionAmounts(int networkId);

        int CountByAppliedUser(int userId);
        int CountByAppliedUser(int userId, int templateId);
    }
}
