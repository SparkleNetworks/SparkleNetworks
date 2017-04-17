
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Entities.Networks.Neutral;

    public interface INotificationsService
    {
        Notification GetDefaultNotifications();
        Notification InitializeNotifications(User item);
        Notification SelectNotifications(User item);
        Notification SelectNotifications(int userId);
        NotificationModel GetNotifications(int userId);
        Notification Update(Notification item);
        Notification Insert(Notification item);

        IList<Notification> SelectAll();

        void UpdateMailChimpStatus(Notification localNotif, string status, DateTime dateTime);
        bool GetDefaultNotificationFronConfig(string key);

        IList<Person> GetSubscribedToContextRequests();

        bool IsDefaultNotification(string name);
        bool IsDefaultNotification(NotificationType name);
        IList<Person> GetSubscribedUsers(NotificationType type);
        IList<Person> GetSubscribedUsers(NotificationType type, int[] userIds);

        bool IsUserNotified(int userId, NotificationType notificationType);

        Users.UnsubscribeFromNotificationResult Unsubscribe(Users.UnsubscribeFromNotificationRequest request);

        string GetUnsubscribeActionUrl(int userId, NotificationType? type);
    }
}
