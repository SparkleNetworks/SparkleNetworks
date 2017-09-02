
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models.Profile;

    public interface IActivitiesService
    {
        void Delete(Activity item);
        int Insert(Activity item);
        void MarkAsDisplayed(int userId);
        IList<Activity> SelectAllActivitiesByUserId(int userId);
        IList<Activity> SelectFiveRecentActivitiesByUserId(int userId);
        IList<Activity> SelectRecentActivitiesByUserId(int userId);
        Activity Update(Activity item);

        IList<ActivityModel> GetUsersNotifications(int userId, bool markAsRead, int pageSize, int pageId);

        ActivityModel Create(int userId, ActivityType type, int? groupId = null, int? profileId = null, int? companyId = null, int? eventId = null, string message = null, int? adId = null);
    }
}
