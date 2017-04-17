
namespace Sparkle.Services.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Subscriptions;

    public interface ISubscriptionsService
    {
        SubscriberAccessCacheModel GetSubscribersCache();

        IList<SubscriptionModel> GetByAppliedUser(int userId);

        int CountByAppliedUser(int userId);
        int CountByAppliedUser(int userId, int templateId);

        CreateSubscriptionResult CreateDefaultForUser(CreateSubscriptionRequest request);

        DataPage<SubscriptionModel> GetList(int[] userId, int[] templateId, bool? active, bool? past, Subscription.Columns sortColumn, bool sortAsc, int offset, int pageSize);

        SubscribeRequest GetSubscribeRequest(int templateId, int userId);

        SubscribeResult SubscribeUser(SubscribeRequest request);

        ManualSubscribeRequest GetManualSubscribeRequest(ManualSubscribeRequest request, int? templateId);
        ManualSubscribeResult ManualSubscribe(ManualSubscribeRequest request);

        SubscriptionModel GetById(int id);
        IList<SubscriptionModel> GetByIds(int[] ids, SubscriptionOptions options = SubscriptionOptions.None);

        IDictionary<string, Func<SubscriptionEmailModel, string, string>> GetEmailSubstitutionRules();
        void RenewSubscriptionNotifications(Subscription item);
        void RenewSubscriptionNotifications(IList<Subscription> items);

        IList<SubscriptionNotification> GetNotificationsBySubscriptionIds(int[] ids);
        void IgnoreNotification(SubscriptionNotification item);
        void IgnoreNotificationsBySubscription(int subscriptionId, DateTime now);

        SubscriptionNotification SentNotification(SubscriptionNotification notif);

        IList<SubscriptionModel> GetCurrentAndFutureSubscriptions(DateTime now);

        MassSubscriptionRequest GetMassSubscriptionRequest(MassSubscriptionRequest request = null);
        MassSubscriptionResult DoMassSubscription(MassSubscriptionRequest request);

        IList<SubscriptionModel> GetActiveUserSubscriptions();

        bool IsUserSubscribed(int userId);
        bool IsUserSubscribed(User user);

        SubscriptionStatusModel GetStatus(bool weekly, bool actual);

        IDictionary<int, IList<SubscriptionModel>> GetUsersAppliedSubscriptions(int[] userIds, DateTime? date = null);

        int CountSubscribedPeople();

        decimal GetSumOfAllSubscriptionAmounts();

        void NotifyUserOfActivatedSubscription(int userId, int subscriptionId);

        IDictionary<int, bool> GetUserStatus(int[] userIds);
        IDictionary<int, bool> GetUserStatus(int[] userIds, DateTime dateTimeUtc);
    }
}
