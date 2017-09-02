
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using PersonOptions = Sparkle.Data.Options.PersonOptions;

    public class NotificationsService : ServiceBase, INotificationsService
    {
        /// <summary>
        /// Gets the notifications repository.
        /// </summary>
        protected INotificationsRepository notificationsRepository
        {
            get { return this.Repo.Notifications; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        public NotificationsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Initializes the notifications.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Notification InitializeNotifications(User item)
        {
            var entity = SelectNotifications(item);
            if (entity == null)
            {
                entity = this.CreateDefaultNotifications(item.Id);
            }

            return entity;
        }

        public Notification CreateDefaultNotifications(int userId)
        {
            var notifications = this.GetDefaultNotifications();
            notifications.UserId = userId;

            return this.notificationsRepository.Insert(notifications);
        }

        public Notification GetDefaultNotifications()
        {
            string defaults = this.Services.AppConfiguration != null ? this.Services.AppConfiguration.Tree.UserSettings.DefaultNotifications : null;
            string[] items = null;
            if (defaults != null)
            {
                items = defaults
                    .Split(new char[] { ',', }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();
            }

            var notifications = new Notification
            {
                Comment = items != null ? items.Contains("Comment") : true,
                ContactRequest = items != null ? items.Contains("ContactRequest") : true,
                PrivateMessage = items != null ? items.Contains("PrivateMessage") : true,
                EventInvitation = items != null ? items.Contains("EventInvitation") : true,
                Publication = items != null ? items.Contains("Publication") : true,
                Newsletter = items != null ? items.Contains("Newsletter") : true,
                DailyNewsletter = items != null ? items.Contains("DailyNewsletter") : false,
                MailChimp = items != null ? items.Contains("MailChimp") : false,
            };

            return notifications;
        }

        public NotificationModel GetDefaultNotificationsModel()
        {
            string defaults = this.Services.AppConfiguration != null ? this.Services.AppConfiguration.Tree.UserSettings.DefaultNotifications : null;
            string[] items = null;
            if (defaults != null)
            {
                items = defaults
                    .Split(new char[] { ',', }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();
            }

            var notifications = new NotificationModel
            {
                Comment = items != null ? items.Contains("Comment") : true,
                ContactRequest = items != null ? items.Contains("ContactRequest") : true,
                PrivateMessage = items != null ? items.Contains("PrivateMessage") : true,
                EventInvitation = items != null ? items.Contains("EventInvitation") : true,
                Publication = items != null ? items.Contains("Publication") : true,
                Newsletter = items != null ? items.Contains("Newsletter") : true,
                DailyNewsletter = items != null ? items.Contains("DailyNewsletter") : false,
                MailChimp = items != null ? items.Contains("MailChimp") : false,
                MainTimelineItems = items != null ? items.Contains("MainTimelineItems") : false,
                MainTimelineComments = items != null ? items.Contains("MainTimelineComments") : false,
                CompanyTimelineItems = items != null ? items.Contains("CompanyTimelineItems") : false,
                CompanyTimelineComments = items != null ? items.Contains("CompanyTimelineComments") : false,
            };

            return notifications;
        }

        public NotificationModel GetNotifications(int userId)
        {
            var result = this.notificationsRepository.Select()
                .WithUserId(userId)
                .SingleOrDefault();

            var ret = this.GetDefaultNotificationsModel();
            if (result != null)
                ret.Apply(result);

            return ret;
        }

        /// <summary>
        /// Updates the notification.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Notification Update(Notification item)
        {
            int notifications = 0
                .Add(item.Comment)
                .Add(item.ContactRequest)
                .Add(item.DailyNewsletter.GetValueOrDefault())
                .Add(item.EventInvitation)
                .Add(item.MailChimp)
                .Add(item.Newsletter.GetValueOrDefault())
                .Add(item.PrivateGroupJoinRequest)
                .Add(item.PrivateMessage)
                .Add(item.Publication);
            this.Services.Logger.Info("NotificationsService.Update", ErrorLevel.Success, "User {0} changed notifications preferences to {1} options", item.UserId.ToString(), notifications.ToString());
            return this.notificationsRepository.Update(item);
        }

        /// <summary>
        /// Selects the notifications.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Notification SelectNotifications(User item)
        {
            return this.SelectNotifications(item.Id);
        }

        /// <summary>
        /// Selects the notifications.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public Notification SelectNotifications(int userId)
        {
            var result = this.notificationsRepository.Select()
                 .WithUserId(userId)
                 .SingleOrDefault();
            if (result == null)
            {
                result = this.CreateDefaultNotifications(userId);
            }

            return result;
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Notification Insert(Notification item)
        {
            return this.notificationsRepository.Insert(item);
        }

        public IList<Notification> SelectAll()
        {
            return this.Repo.Notifications
                .Select()
                .ToList();
        }

        public void UpdateMailChimpStatus(Notification localNotif, string status, DateTime dateTime)
        {
            if (localNotif.UserId == 0)
            {
                throw new ArgumentException("Missing UserId", "localNotif");
            }

            localNotif.MailChimp = status == "subscribed";
            localNotif.MailChimpStatus = status;
            localNotif.MailChimpStatusDateUtc = dateTime;

            this.notificationsRepository.Update(localNotif);
        }

        public IList<Person> GetSubscribedToContextRequests()
        {
            var byDefault = this.GetDefaultNotificationFronConfig("ContactRequest");

            Expression<Func<User, bool>> optedInPredicate =
                p => p.Notification != null && p.Notification.ContactRequest == true;
            Expression<Func<User, bool>> notOptedInPredicate =
                p => p.Notification == null || p.Notification.ContactRequest == null;

            return this.GetSubscribedUsers(byDefault, optedInPredicate, notOptedInPredicate, null);
        }

        private IList<Person> GetSubscribedUsers(bool byDefault, Expression<Func<User, bool>> optedInPredicate, Expression<Func<User, bool>> notOptedInPredicate, int[] userIds)
        {
            // find registered people willing to receive
            var optedInUserIdsQuery = this.Repo.People.Select()
                .ByNetwork(this.Services.NetworkId)
                .ActiveAccount();

            if (userIds != null)
            {
                optedInUserIdsQuery = optedInUserIdsQuery
                    .Where(u => userIds.Contains(u.Id));
            }

            var optedInUserIds = optedInUserIdsQuery
                .Where(optedInPredicate)
                .Select(o => o.Id)
                .ToArray();

            // find registered people without newsletter preference if default preference exists
            var defaultUserIds = new int[0];
            if (byDefault)
            {
                var defaultUserIdsQuery = this.Repo.People.Select()
                    .ByNetwork(this.Services.NetworkId)
                    .ActiveAccount();

                if (userIds != null)
                {
                    defaultUserIdsQuery = defaultUserIdsQuery
                        .Where(u => userIds.Contains(u.Id));
                }

                defaultUserIds = defaultUserIdsQuery
                    .Where(notOptedInPredicate)
                    .Select(o => o.Id)
                    .ToArray();
            }

            var finalUserIds = optedInUserIds.CombineWith(defaultUserIds);

            return this.Repo.People.GetActiveLiteById(finalUserIds, this.Services.NetworkId);
        }

        public bool GetDefaultNotificationFronConfig(string key)
        {
            var configValue = this.Services.AppConfiguration.Tree.UserSettings.DefaultNotifications;
            if (configValue == null)
                return false;

            var configValueSplitted = configValue.Split(new char[] { ',', }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            return configValueSplitted.Contains(key.ToLowerInvariant());
        }

        public bool IsDefaultNotification(string name)
        {
            return this.GetDefaultNotificationFronConfig(name);
        }

        public bool IsDefaultNotification(NotificationType name)
        {
            return this.GetDefaultNotificationFronConfig(name.ToString());
        }

        public IList<Person> GetSubscribedUsers(NotificationType type)
        {
            return this.GetSubscribedUsers(type, null);
        }

        public IList<Person> GetSubscribedUsers(NotificationType type, int[] userIds)
        {
            var byDefault = this.GetDefaultNotificationFronConfig(type.ToString());

            Expression<Func<User, bool>> optedInPredicate;
            Expression<Func<User, bool>> notOptedInPredicate;
            switch (type)
            {
                case NotificationType.ContactRequest:
                    optedInPredicate = p => p.Notification != null && p.Notification.ContactRequest == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.ContactRequest == null;
                    break;
                case NotificationType.Publication:
                    optedInPredicate = p => p.Notification != null && p.Notification.Publication == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.Publication == null;
                    break;
                case NotificationType.Comment:
                    optedInPredicate = p => p.Notification != null && p.Notification.Comment == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.Comment == null;
                    break;
                case NotificationType.EventInvitation:
                    optedInPredicate = p => p.Notification != null && p.Notification.EventInvitation == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.EventInvitation == null;
                    break;
                case NotificationType.PrivateMessage:
                    optedInPredicate = p => p.Notification != null && p.Notification.PrivateMessage == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.PrivateMessage == null;
                    break;
                case NotificationType.Newsletter:
                    optedInPredicate = p => p.Notification != null && p.Notification.Newsletter == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.Newsletter == null;
                    break;
                case NotificationType.DailyNewsletter:
                    optedInPredicate = p => p.Notification != null && p.Notification.DailyNewsletter == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.DailyNewsletter == null;
                    break;
                case NotificationType.PrivateGroupJoinRequest:
                    optedInPredicate = p => p.Notification != null && p.Notification.PrivateGroupJoinRequest == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.PrivateGroupJoinRequest == null;
                    break;
                case NotificationType.MailChimp:
                    optedInPredicate = p => p.Notification != null && p.Notification.MailChimp == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.MailChimp == null;
                    break;
                case NotificationType.MainTimelineItems:
                    optedInPredicate = p => p.Notification != null && p.Notification.MainTimelineItems == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.MainTimelineItems == null;
                    break;
                case NotificationType.MainTimelineComments:
                    optedInPredicate = p => p.Notification != null && p.Notification.MainTimelineComments == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.MainTimelineComments == null;
                    break;
                case NotificationType.CompanyTimelineItems:
                    optedInPredicate = p => p.Notification != null && p.Notification.CompanyTimelineItems == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.CompanyTimelineItems == null;
                    break;
                case NotificationType.CompanyTimelineComments:
                    optedInPredicate = p => p.Notification != null && p.Notification.CompanyTimelineComments == true;
                    notOptedInPredicate = p => p.Notification == null || p.Notification.CompanyTimelineComments == null;
                    break;
                default:
                    throw new ArgumentException("Notification type " + type + " is not supported");
            }


            return this.GetSubscribedUsers(byDefault, optedInPredicate, notOptedInPredicate, userIds);
        }

        public bool IsUserNotified(int userId, NotificationType notificationType)
        {
            var item = this.SelectNotifications(userId);
            if (item == null)
                return false;

            var optin = IsUserNotified(item, notificationType);
            if (optin != null)
            {
                return optin.Value;
            }
            else
            {
                return this.IsDefaultNotification(notificationType);
            }
        }

        private static bool? IsUserNotified(Notification item, NotificationType type)
        {
            switch (type)
            {
                case NotificationType.ContactRequest:
                    return item.ContactRequest;
                case NotificationType.Publication:
                    return item.Publication;
                case NotificationType.Comment:
                    return item.Comment;
                case NotificationType.EventInvitation:
                    return item.EventInvitation;
                case NotificationType.PrivateMessage:
                    return item.PrivateMessage;
                case NotificationType.Newsletter:
                    return item.Newsletter;
                case NotificationType.DailyNewsletter:
                    return item.DailyNewsletter;
                case NotificationType.PrivateGroupJoinRequest:
                    return item.PrivateGroupJoinRequest;
                case NotificationType.MailChimp:
                    return item.MailChimp;
                case NotificationType.MainTimelineItems:
                    return item.MainTimelineItems;
                case NotificationType.MainTimelineComments:
                    return item.MainTimelineComments;

                default:
                    throw new ArgumentException("Notification type " + type + " is not supported");
            }
        }

        public UnsubscribeFromNotificationResult Unsubscribe(UnsubscribeFromNotificationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new UnsubscribeFromNotificationResult(request);

            var notifs = this.SelectNotifications(request.UserId);
            if (notifs == null)
            {
                result.Errors.Add(UnsubscribeFromNotificationError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            NotificationType type;
            if (!Enum.TryParse(request.Type, out type))
            {
                result.Errors.Add(UnsubscribeFromNotificationError.NoSuchNotification, NetworksEnumMessages.ResourceManager);
                return result;
            }

            switch (type)
            {
                case NotificationType.ContactRequest:
                    notifs.ContactRequest = false;
                    break;
                case NotificationType.Comment:
                    notifs.Comment = false;
                    break;
                case NotificationType.EventInvitation:
                    notifs.EventInvitation = false;
                    break;
                case NotificationType.PrivateMessage:
                    notifs.PrivateMessage = false;
                    break;
                case NotificationType.Newsletter:
                    notifs.Newsletter = false;
                    break;
                case NotificationType.DailyNewsletter:
                    notifs.DailyNewsletter = false;
                    break;
                case NotificationType.PrivateGroupJoinRequest:
                    notifs.PrivateGroupJoinRequest = false;
                    break;
                case NotificationType.MailChimp:
                    notifs.MailChimp = false;
                    notifs.MailChimpStatus = null;
                    notifs.MailChimpStatusDateUtc = null;
                    break;
                case NotificationType.MainTimelineItems:
                    notifs.MainTimelineItems = false;
                    break;
                case NotificationType.MainTimelineComments:
                    notifs.MainTimelineComments = false;
                    break;
                case NotificationType.CompanyTimelineItems:
                    notifs.CompanyTimelineItems = false;
                    break;
                case NotificationType.CompanyTimelineComments:
                    notifs.CompanyTimelineComments = false;
                    break;
                case NotificationType.Publication:
                default:
                    break;
            }

            this.Update(notifs);

            result.Succeed = true;
            return result;
        }

        public string GetUnsubscribeActionUrl(User user, NotificationType? type)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return this.GetUnsubscribeActionUrl(user.Id, user.CreatedDateUtc, type);
        }

        public string GetUnsubscribeActionUrl(UserModel user, NotificationType? type)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return this.GetUnsubscribeActionUrl(user.Id, user.CreateDateUtc, type);
        }

        public string GetUnsubscribeActionUrl(int userId, NotificationType? type)
        {
            var user = this.Services.People.GetById(userId, PersonOptions.None);
            if (user == null)
            {
                throw new ArgumentException("userId", "No such user. ");
            }

            return this.GetUnsubscribeActionUrl(user.Id, user.CreateDateUtc, type);
        }

        private string GetUnsubscribeActionUrl(int userId, DateTime? dateCreatedUtc, NotificationType? type)
        {
            var token = this.GetEditToken(userId, dateCreatedUtc);
            var query = new Dictionary<string, string>
            {
                ////{ "User", SimpleSecrets.GetUsersEmailNotificationActionHash(userId, this.Services.NetworkId) }
                { "User", token }
            };
            if (type.HasValue)
                query.Add("Type", type.Value.ToString());

            return this.Services.GetUrl("Account/Settings", query);
        }

        public string GetEditToken(int userId)
        {
            var user = this.Services.People.GetById(userId, PersonOptions.None);
            if (user == null)
            {
                throw new ArgumentException("userId", "No such user. ");
            }

            return this.GetEditToken(user);
        }

        public string GetEditToken(UserModel user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return GetEditToken(user.Id, user.CreateDateUtc);
        }

        private string GetEditToken(int userId, DateTime? dateCreatedUtc)
        {
            var tokenString = "$0$" + userId + "$" + GetEditTokenValidationDate(dateCreatedUtc) + "$";
            var tokenData = Encoding.UTF8.GetBytes(tokenString);
            var encryptedData = this.Services.Crypto.EncryptWithIV(KnownCryptoPurposes.EmailNotificationUserPreferences, tokenData);

            return encryptedData.ToUrlBase64();
        }

        private static string GetEditTokenValidationDate(DateTime? createDate)
        {
            const string EditTokenDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
            return (createDate ?? DateTime.MinValue).ToString(EditTokenDateFormat, CultureInfo.InvariantCulture);
        }

        public ValidateUserTokenResult ValidateEditToken(ValidateUserTokenRequest request)
        {
            const string path = "ValidateEditToken";
            var result = new ValidateUserTokenResult(request);

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                result.Errors.AddDetail(ValidateUserTokenError.InvalidToken, "The given token is an empty string. ", NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            // decrypt and read token
            byte[] encryptedData;
            try
            {
                encryptedData = request.Token.FromUrlBase64();
            }
            catch (FormatException ex)
            {
                result.Errors.AddDetail(ValidateUserTokenError.InvalidToken, ex.Message, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, path);
            }

            string cryptoPurpose = KnownCryptoPurposes.EmailNotificationUserPreferences;
            byte[] tokenData = this.Services.Crypto.DecryptWithIV(cryptoPurpose, encryptedData);
            var tokenString = Encoding.UTF8.GetString(tokenData);

            int userId = 0;
            string createDateToValidate = null;
            if (tokenString.StartsWith("$0$"))
            {
                var parts = tokenString.Split('$');
                if (parts.Length == 5)
                {
                    var userIdString = parts[2];
                    createDateToValidate = parts[3];
                    if (!int.TryParse(userIdString, out userId))
                    {
                        result.Errors.AddDetail(ValidateUserTokenError.InvalidToken, "Expected int user id in protocol 0.", NetworksEnumMessages.ResourceManager);
                    }
                }
                else
                {
                    result.Errors.AddDetail(ValidateUserTokenError.InvalidToken, "Invalid part count for protocol 0.", NetworksEnumMessages.ResourceManager);
                }
            }
            else
            {
                result.Errors.AddDetail(ValidateUserTokenError.InvalidToken, "Unknown protocol.", NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                // token is wrong
                return this.LogResult(result, path);
            }

            var user = this.Services.People.GetById(userId, PersonOptions.Company);
            if (user == null)
            {
                result.Errors.AddDetail(ValidateUserTokenError.NoSuchUser, "No such user by id " + request.ActingUserId + ". ", NetworksEnumMessages.ResourceManager);
            }
            else
            {
                var referenceDate = GetEditTokenValidationDate(user.CreateDateUtc);
                if (!referenceDate.Equals(createDateToValidate))
                {
                    result.Errors.AddDetail(ValidateUserTokenError.InvalidToken, "User create date verification failed. ", NetworksEnumMessages.ResourceManager);
                }
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            result.Succeed = true;
            result.User = user;

            return this.LogResult(result, path);
        }
    }

    public static class Extensions
    {
        public static int Add(this int value, bool addValue)
        {
            return value + (addValue ? 1 : 0);
        }
    }
}
