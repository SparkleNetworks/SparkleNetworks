
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Common;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Subscriptions;
    using Stripe;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Networks.Payments;
    using Sparkle.Helpers;

    public class SubscriptionsService : ServiceBase, ISubscriptionsService
    {
        [DebuggerStepThrough]
        internal SubscriptionsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public SubscriberAccessCacheModel GetSubscribersCache()
        {
            var userSubscriptions = this.Repo.Subscriptions.GetCurrent(this.Services.NetworkId, DateTime.UtcNow)
                .Select(s => new SubscriptionModel(s))
                .ToList();
            var templates = this.Services.SubscriptionTemplates.GetAll();

            var model = new SubscriberAccessCacheModel
            {
                Items = userSubscriptions,
                UserDefaultTemplate = templates.FirstOrDefault(t => t.IsDefaultOnAccountCreate && t.IsSubscriptionEnabled),
            };
            return model;
        }

        public IList<SubscriptionModel> GetByAppliedUser(int userId)
        {
            var items = this.Repo.Subscriptions.Select()
                .Where(s => s.NetworkId == this.Services.NetworkId && s.AppliesToUserId == userId)
                .ToList();
            var models = items.Select(s => new SubscriptionModel(s)).ToList();

            var userIds = new List<int>();
            var companyIds = new List<int>();

            foreach (var item in items)
            {
                if (item.AppliesToUserId != null)
                    userIds.Add(item.AppliesToUserId.Value);
                if (item.AppliesToCompanyId != null)
                    companyIds.Add(item.AppliesToCompanyId.Value);
                if (item.OwnerUserId != null)
                    userIds.Add(item.OwnerUserId.Value);
                if (item.OwnerCompanyId != null)
                    companyIds.Add(item.OwnerCompanyId.Value);
            }

            var users = this.Services.People.GetModelByIdFromAnyNetwork(userIds.ToArray(), PersonOptions.None);
            var companies = this.Services.Company.GetByIdFromAnyNetwork(companyIds, CompanyOptions.None);

            foreach (var item in models)
            {
                if (item.AppliesToUserId != null)
                    item.AppliesToUser = users[item.AppliesToUserId.Value];
                if (item.AppliesToCompanyId != null)
                    item.AppliesToCompany = companies[item.AppliesToCompanyId.Value];
                if (item.OwnerUserId != null)
                    item.OwnerUser = users[item.OwnerUserId.Value];
                if (item.OwnerCompanyId != null)
                    item.OwnerCompany = companies[item.OwnerCompanyId.Value];
            }

            return models;
        }

        public int CountByAppliedUser(int userId)
        {
            var items = this.Repo.Subscriptions.CountByAppliedUser(userId);
            return items;
        }

        public int CountByAppliedUser(int userId, int templateId)
        {
            var items = this.Repo.Subscriptions.CountByAppliedUser(userId, templateId);
            return items;
        }

        public CreateSubscriptionResult CreateDefaultForUser(CreateSubscriptionRequest request)
        {
            const string path = "CreateDefaultForUser";

            if (request == null)
                throw new ArgumentNullException("request");

            var result = new CreateSubscriptionResult(request);

            var user = this.Services.People.SelectWithId(request.UserId);
            if (user == null)
            {
                result.Errors.Add(CreateSubscriptionError.NoSuchUser, NetworksEnumMessages.ResourceManager);
            }

            if (user != null && !this.Services.People.IsActive(user))
            {
                result.Errors.Add(CreateSubscriptionError.InactiveUser, NetworksEnumMessages.ResourceManager);
            }

            var defaultSub = this.Services.SubscriptionTemplates.GetDefaultUserTemplate();
            if (defaultSub == null)
            {
                result.Errors.Add(CreateSubscriptionError.NoDefaultTemplate, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            var count = this.Repo.Subscriptions.CountByAppliedUser(user.Id, defaultSub.Id);
            if (count > 0)
            {
                result.Errors.AddDetail(CreateSubscriptionError.NotEligible, "User " + user.Id + " already has subscriptions for template " + defaultSub.Id + ". ", NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            var now = DateTime.UtcNow;
            var subscription = new Subscription
            {
                AppliesToUserId = user.Id,
                DateCreatedUtc = now,
                OwnerUserId = user.Id,
                IsPaid = true,
            };
            this.ApplyTemplate(defaultSub, subscription, now);

            this.InsertNewSubscription(subscription, new UserModel(user), false);

            subscription = this.Repo.Subscriptions.GetById(subscription.Id);
            result.CreatedSubscription = new SubscriptionModel(subscription);
            result.Succeed = true;

            return this.LogResult(result, path);
        }

        private void ApplyTemplate(SubscriptionTemplateModel template, Subscription item, DateTime dateStartUtc)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (item == null)
                throw new ArgumentNullException("item");

            item.AutoRenew = template.AllowAutoRenew;
            item.DateBeginUtc = dateStartUtc;
            item.DateEndUtc = dateStartUtc.Add(template.DurationValue, template.DurationKind);
            item.DurationKind = (byte)template.DurationKind;
            item.DurationValue = template.DurationValue;
            item.IsForAllCompanyUsers = template.IsForAllCompanyUsers;
            item.Name = template.Name;
            item.NetworkId = template.NetworkId;
            item.PriceEurWithoutVat = template.PriceEurWithoutVat;
            item.PriceEurWithVat = template.PriceEurWithVat;
            item.PriceUsdWithoutVat = template.PriceUsdWithoutVat;
            item.PriceUsdWithVat = template.PriceUsdWithVat;
            item.TemplateId = template.Id;
        }

        public DataPage<SubscriptionModel> GetList(int[] userId, int[] templateId, bool? active, bool? past, Subscription.Columns sortColumn, bool sortAsc, int offset, int pageSize)
        {
            var items = this.Repo.Subscriptions.GetList(this.Services.NetworkId, userId, templateId, active, past, sortColumn, sortAsc, offset, pageSize);
            var models = items.Items.Select(s => new SubscriptionModel(s)).ToList();

            var userIds = items.Items.Where(i => i.AppliesToUserId != null).Select(i => i.AppliesToUserId.Value).ToArray();
            var users = this.Repo.People.GetLiteById(userIds);
            var templateIds = this.Repo.SubscriptionTemplates.GetAll(this.Services.NetworkId);
            foreach (var item in models)
            {
                if (item.AppliesToUserId != null)
                {
                    var user = users.SingleOrDefault(u => u.Id == item.AppliesToUserId.Value);
                    if (user != null)
                    {
                        item.AppliesToUser = new UserModel(user);
                    }
                }
            }

            var count = this.Repo.Subscriptions.CountList(this.Services.NetworkId, userId, templateId, active, past);
            return new DataPage<SubscriptionModel>
            {
                Items = models,
                Offset = offset,
                Size = pageSize,
                Total = count,
            };
        }

        public SubscribeRequest GetSubscribeRequest(int templateId, int userId)
        {
            var request = new SubscribeRequest(userId, templateId);

            request.Template = this.Services.SubscriptionTemplates.GetById(templateId);
            if (request.Template == null)
                return null;

            var user = this.Services.People.SelectWithId(userId);

            request.SubscriptionProvider = SubscriptionProvider.Stripe;
            if (this.Services.AppConfiguration.Tree.Features.Subscriptions.StripeConfig != null)
            {
                request.StripeConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<StripeConfigModel>(
                    this.Services.AppConfiguration.Tree.Features.Subscriptions.StripeConfig);
            }

            request.SiteName = this.Services.Lang.T("AppName");
            request.UserEmail = user.Email;

            return request;
        }

        public SubscribeResult SubscribeUser(SubscribeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new SubscribeResult(request);

            var now = DateTime.UtcNow;
            var transact = new StripeTransaction
            {
                NetworkId = this.Services.NetworkId,
                UserId = request.UserId,
                TokenId = request.stripeToken,
                DateCreatedUtc = now,
                IsChargeCreated = false,
                IsChargeCaptured = false,
            };
            transact = this.Repo.StripeTransactions.Insert(transact);

            request.StripeConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<StripeConfigModel>(this.Services.AppConfiguration.Tree.Features.Subscriptions.StripeConfig);
            var template = this.Services.SubscriptionTemplates.GetById(request.TemplateId);
            if (template == null)
            {
                result.Errors.Add(SubscribeError.NoSuchTemplate, NetworksEnumMessages.ResourceManager);
                return result;
            }
            var user = this.Services.People.GetActiveById(request.UserId, PersonOptions.None);
            if (user == null)
            {
                result.Errors.Add(SubscribeError.NoSuchUserOrInactive, NetworksEnumMessages.ResourceManager);
                return result;
            }

            StripeChargeService stripeService = null;

            // Create the charge
            var chargeOptions = new StripeChargeCreateOptions();
            chargeOptions.Amount = (int)(template.PriceToPay.Value * 100);
            chargeOptions.Currency = template.PriceEurWithoutVat.HasValue || template.PriceEurWithVat.HasValue ? "eur" : "usd";
            chargeOptions.TokenId = request.stripeToken;
            chargeOptions.Description = string.Format("{0} - {1} - {2}", this.Services.Lang.T("AppName"), template.Name, user.Email);
            chargeOptions.Capture = false;

            StripeCharge charge = null;
            try
            {
                stripeService = new StripeChargeService(request.StripeConfig.SecretKey);
                charge = stripeService.Create(chargeOptions);

                this.Repo.StripeTransactions.ChargeIsCreated(transact, charge.Id);
            }
            catch (StripeException ex)
            {
                this.Services.ReportError(ex, "Error when creating the charge: StripeException: " + ex.ToFullString());
                this.Services.Logger.Error(
                    "SubscriptionsService.SubscribeUser",
                    ErrorLevel.ThirdParty,
                    "Error when creating the charge: StripeException: " + ex.ToFullString());

                result.Errors.Add(SubscribeError.CannotCreateCharge, NetworksEnumMessages.ResourceManager);
                return result;
            }
            catch (Exception ex)
            {
                this.Services.ReportError(ex, "Error when creating the charge (Exception: " + ex.ToString() + ")");
                this.Services.Logger.Error(
                    "SubscriptionsService.SubscribeUser",
                    ErrorLevel.ThirdParty,
                    "Error when creating the charge (Exception: " + ex.ToString() + ")");

                result.Errors.Add(SubscribeError.CannotCreateCharge, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // Create subscription entity
            var item = new Subscription
            {
                AppliesToUserId = request.UserId,
                DateCreatedUtc = now,
                OwnerUserId = request.UserId,
                PaymentMethodValue = PaymentMethod.Stripe,
            };

            item = this.AppendToSubscriptions(template, item, now, transact.Id);
            if (item == null)
            {
                this.Services.Logger.Error(
                    "SubscriptionsService.SubscribeUser",
                    ErrorLevel.ThirdParty,
                    "Error with the subscriptions of user " + request.UserId);

                result.Errors.Add(SubscribeError.InternalError, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (template.PriceEurWithoutVat.HasValue || template.PriceEurWithVat.HasValue)
                this.Repo.StripeTransactions.SetAmount(transact, priceEur: template.PriceToPay.Value);
            else
                this.Repo.StripeTransactions.SetAmount(transact, priceUsd: template.PriceToPay.Value);

            // Capture the charge
            try
            {
                stripeService.Capture(charge.Id);

                this.Repo.Subscriptions.SubIsPaid(item);
                this.RenewSubscriptionNotifications(item);
                this.Repo.StripeTransactions.ChargeIsCaptured(transact);
            }
            catch (StripeException ex)
            {
                this.Services.ReportError(ex, "Error when capturing the charge: StripeException: " + ex.ToFullString());
                this.Services.Logger.Error(
                    "SubscriptionsService.SubscribeUser",
                    ErrorLevel.ThirdParty,
                    "Error when capturing the charge: StripeException: " + ex.ToFullString());

                result.Errors.Add(SubscribeError.CannotCaptureCharge, NetworksEnumMessages.ResourceManager);
                return result;
            }
            catch (Exception ex)
            {
                this.Services.ReportError(ex, "Error when capturing the charge (Exception: " + ex.ToString() + ")");
                this.Services.Logger.Error(
                    "SubscriptionsService.SubscribeUser",
                    ErrorLevel.ThirdParty,
                    "Error when capturing the charge (Exception: " + ex.ToString() + ")");

                result.Errors.Add(SubscribeError.CannotCaptureCharge, NetworksEnumMessages.ResourceManager);
                return result;
            }

            item = this.Repo.Subscriptions.GetById(item.Id);
            this.Services.Email.SendSubscriptionActivated(new SubscriptionModel(item), user);
            this.NotifyUserOfActivatedSubscription(user.Id, item.Id);

            result.Succeed = true;
            return result;
        }

        private Subscription AppendToSubscriptions(SubscriptionTemplateModel template, Subscription item, DateTime now, int transactId)
        {
            this.ApplyTemplate(template, item, now);

            // get current or future subs
            var subs = this.Repo.Subscriptions.GetByApplyUserId(item.AppliesToUserId.Value).Where(o => o.DateEndUtc.HasValue && o.DateEndUtc >= now).ToList();
            if (subs.Count > 0)
            {
                var valid = this.AreSubscriptionsValid(subs.OrderBy(o => o.DateBeginUtc).First(), subs);
                if (!valid)
                    return null;

                var lastActiveSub = subs.OrderByDescending(o => o.DateEndUtc).FirstOrDefault();
                if (lastActiveSub == null)
                    return null;
                var newDateBegin = lastActiveSub.DateEndUtc.Value.AddSeconds(1D);
                item.DateBeginUtc = newDateBegin;
                item.DateEndUtc = newDateBegin.Add(template.DurationValue, template.DurationKind);
            }

            item = this.Repo.Subscriptions.Insert(item, transactId);
            return item;
        }

        private bool AreSubscriptionsValid(Subscription item, IList<Subscription> others)
        {
            if (others.Max(o => o.DateEndUtc.Value) == item.DateEndUtc.Value)
                return true;

            var nextDateBegin = item.DateBeginUtc.Value.AddSeconds(1D);
            Subscription nextItem;
            if ((nextItem = others.Where(o => o.DateBeginUtc.Value == nextDateBegin).SingleOrDefault()) == null)
                return false;

            return this.AreSubscriptionsValid(nextItem, others);
        }

        public ManualSubscribeRequest GetManualSubscribeRequest(ManualSubscribeRequest request, int? templateId)
        {
            bool isNew = request == null;
            if (request == null)
            {
                request = new ManualSubscribeRequest();
                request.TemplateId = templateId.Value;
            }

            if (templateId != null)
            {
                request.TemplateId = templateId.Value;
            }

            var template = this.Services.SubscriptionTemplates.GetById(request.TemplateId);
            TimeZoneInfo tz;

            if (request.Timezone == null)
            {
                request.Timezone = this.Services.DefaultTimezone.Id;
            }

            tz = TimeZoneInfo.FindSystemTimeZoneById(request.Timezone);

            if (request.DateStartLocal == DateTime.MinValue)
            {
                request.DateStartLocal = tz.ConvertFromUtc(DateTime.UtcNow.ToPrecision(DateTimePrecision.Hour));
            }

            if (isNew)
            {
                if (template.PriceEurWithVat != null)
                    request.PaidByCashEur = template.PriceEurWithVat;
                else if (template.PriceEurWithoutVat != null)
                    request.PaidByCashEur = template.PriceEurWithoutVat;

                if (template.PriceUsdWithVat != null)
                    request.PaidByCashUsd = template.PriceUsdWithVat;
                else if (template.PriceUsdWithoutVat != null)
                    request.PaidByCashUsd = template.PriceUsdWithoutVat;
            }

            return request;
        }

        public ManualSubscribeResult ManualSubscribe(ManualSubscribeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ManualSubscribeResult(request);

            var actingUser = this.Repo.People.GetById(request.ActingUserId, PersonOptions.Company);
            if (actingUser == null || !this.Services.People.IsActive(actingUser)
             && (actingUser.NetworkId == this.Services.NetworkId && actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManageSubscriptions, NetworkAccessLevel.NetworkAdmin)
                 || actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff)))
            {
                result.Errors.Add(ManualSubscribeError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (!(actingUser.NetworkId == this.Services.NetworkId && actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManageSubscriptions, NetworkAccessLevel.NetworkAdmin)
                 || actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff)))
            {
                result.Errors.Add(ManualSubscribeError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var user = this.Services.People.GetActiveById(request.ApplyUserId, PersonOptions.Company);
            if (user == null)
            {
                result.Errors.Add(ManualSubscribeError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var template = this.Services.SubscriptionTemplates.GetById(request.TemplateId);
            if (template == null || !template.IsValid)
            {
                result.Errors.Add(ManualSubscribeError.InvalidTemplate, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var tz = request.Timezone != null ? TimeZoneInfo.FindSystemTimeZoneById(request.Timezone) : this.Services.DefaultTimezone;
            var dateStartUtc = tz.ConvertToUtc(request.DateStartLocal);

            var item = new Subscription
            {
                AppliesToUserId = request.ApplyUserId,
                OwnerUserId = request.ActingUserId,
                PaymentMethodValue = PaymentMethod.Cash,
                DateCreatedUtc = DateTime.UtcNow,
            };
            this.ApplyTemplate(template, item, dateStartUtc);

            if (template.PriceEurWithVat != null && request.PaidByCashEur >= template.PriceEurWithVat.Value)
                item.IsPaid = true;
            else if (template.PriceEurWithoutVat != null && request.PaidByCashEur >= template.PriceEurWithoutVat.Value)
                item.IsPaid = true;

            if (template.PriceUsdWithVat != null && request.PaidByCashUsd >= template.PriceUsdWithVat.Value)
                item.IsPaid = true;
            else if (template.PriceUsdWithoutVat != null && request.PaidByCashUsd >= template.PriceUsdWithoutVat.Value)
                item.IsPaid = true;

            if (template.PriceEurWithVat == null && template.PriceUsdWithVat == null)
                item.IsPaid = true;

            // manual subscribe => mark as paid anyway
            item.IsPaid = true;

            this.InsertNewSubscription(item, user, request.SendEmail);

            result.NewSubscription = new SubscriptionModel(item);
            result.Succeed = true;

            return result;
        }

        public SubscriptionModel GetById(int id)
        {
            var item = this.Repo.Subscriptions.GetById(id);
            if (item == null)
                return null;

            var model = new SubscriptionModel(item);

            if (item.AppliesToUserId != null)
            {
                var user = this.Services.People.GetEntityByIdInNetwork(item.AppliesToUserId.Value, PersonOptions.Company);
                if (user != null)
                    model.AppliesToUser = new UserModel(user);
            }

            if (item.AppliesToCompanyId != null)
            {
                var user = this.Services.Company.GetById(item.AppliesToCompanyId.Value);
                if (user != null)
                    model.AppliesToCompany = new CompanyModel(user);
            }

            if (item.OwnerUserId != null)
            {
                var user = this.Services.People.GetEntityByIdInNetwork(item.OwnerUserId.Value, PersonOptions.Company);
                if (user != null)
                    model.OwnerUser = new UserModel(user);
            }

            if (item.OwnerCompanyId != null)
            {
                var user = this.Services.Company.GetById(item.OwnerCompanyId.Value);
                if (user != null)
                    model.OwnerCompany = new CompanyModel(user);
            }

            if (item.StripeTransactions.Count > 0)
            {
                model.Transactions = new List<TransactionModel>(item.StripeTransactions.Count);
                foreach (var stripe in item.StripeTransactions)
                {
                    var model1 = new TransactionModel(stripe);
                    model.Transactions.Add(model1);
                    if (model1.UserId != null)
                    {
                        var user = this.Services.People.GetLiteById(new int[] { stripe.UserId, }).SingleOrDefault();
                        if (user != null)
                            model1.User = new UserModel(user);
                    }
                }
            }

            return model;
        }

        public IDictionary<string, Func<SubscriptionEmailModel, string, string>> GetEmailSubstitutionRules()
        {
            var collection = new Dictionary<string, Func<SubscriptionEmailModel, string, string>>
            {
                { "FirstName",          (m, a) => m.User != null ? m.User.FirstName : "{FirstName}" },
                { "LastName",           (m, a) => m.User != null ? m.User.LastName : "{LastName}" },
                { "NetworkName",        (m, a) => m.NetworkName },
                { "NetworkDomain",      (m, a) => m.NetworkDomain },
                { "SubscriptionName",   (m, a) => m.Template != null ? m.Template.Name : "{SubscriptionName}" },
                { "Price",              (m, a) => m.Template != null ? m.Template.PriceToPayTitle : "{Price}" },
                { "DateBeginUtc",       (m, a) => m.Subscription != null ? m.Subscription.DateBeginUtc.Value.ToString(a ?? "") : "{DateBeginUtc " + a + "}" },
                { "DateEndUtc",         (m, a) => m.Subscription != null ? m.Subscription.DateEndUtc.Value.ToString(a ?? "") : "{DateEndUtc " + a + "}" },
                { "RemainingDays",      (m, a) => m.Subscription != null ? m.Subscription.RemainingDays.ToString() : "{RemainingDays}" },
            };

            return collection;
        }

        public void RenewSubscriptionNotifications(Subscription item)
        {
            if (item.DateEndUtc.HasValue)
            {
                this.Repo.SubscriptionNotifications.DeleteNotSentFromSubcriptionId(item.Id);

                //// config vars concernin frequency of the reminder
                int[] reminders = { 14, 3, 0, };
                var config = this.Services.AppConfiguration.Tree.Features.Subscriptions.EndNotificationDays;
                string[] split;
                int test;
                if (string.IsNullOrEmpty(config) || (split = config.Split(new char[] { ';', })).Select(o => int.TryParse(o, out test)).Any(o => !o))
                {
                    this.Services.Logger.Error(
                        "SubscriptionServices.RenewSubscriptionNotifications",
                        ErrorLevel.Data,
                        "SparkleSystem value 'Features.Subscriptions.EndNotificationDays' is null or invalid.");
                }
                else
                {
                    reminders = split.Select(o => int.Parse(o)).ToArray();
                }

                var notifs = reminders
                    .Select(o => new SubscriptionNotification
                    {
                        SubscriptionId = item.Id,
                        DateSendUtc = item.DateEndUtc.Value.AddDays(-1 * o),
                        StatusValue = SubscriptionNotificationStatus.New,
                    })
                    .ToList();

                this.Repo.SubscriptionNotifications.InsertMany(notifs);
            }
        }

        public void RenewSubscriptionNotifications(IList<Subscription> items)
        {
            //// config vars concernin frequency of the reminder
            // TODO: this code is hard to read + double int.Parse
            int[] reminders = { 14, 3, 0, };
            var config = this.Services.AppConfiguration.Tree.Features.Subscriptions.EndNotificationDays;
            string[] split;
            int test;
            if (string.IsNullOrEmpty(config) || (split = config.Split(new char[] { ';', })).Select(o => int.TryParse(o, out test)).Any(o => !o))
            {
                this.Services.Logger.Error(
                    "SubscriptionServices.RenewSubscriptionNotifications",
                    ErrorLevel.Data,
                    "SparkleSystem value 'Features.Subscriptions.EndNotificationDays' is null or invalid.");
                throw new AppConfigurationException("The config key Features.Subscriptions.EndNotificationDays is invalid");
            }
            else
            {
                reminders = split.Select(o => int.Parse(o)).ToArray();
            }

            IList<SubscriptionNotification> notifs = new List<SubscriptionNotification>();
            foreach (var item in items)
            {
                if (item.DateEndUtc != null)
                {
                    this.Repo.SubscriptionNotifications.DeleteNotSentFromSubcriptionId(item.Id);

                    notifs.AddRange(reminders
                        .Select(o => new SubscriptionNotification
                        {
                            SubscriptionId = item.Id,
                            DateSendUtc = item.DateEndUtc.Value.AddDays(-o),
                            StatusValue = SubscriptionNotificationStatus.New,
                        })
                        .ToList());
                }
            }

            this.Repo.SubscriptionNotifications.InsertMany(notifs);
        }

        public void IgnoreNotification(SubscriptionNotification item)
        {
            item.StatusValue = SubscriptionNotificationStatus.Ignored;

            this.Repo.SubscriptionNotifications.Update(item);
        }

        public IList<SubscriptionModel> GetByIds(int[] ids, SubscriptionOptions options = SubscriptionOptions.None)
        {
            return this.Repo.Subscriptions.GetByIds(ids, options).Select(o => new SubscriptionModel(o)).ToList();
        }

        public SubscriptionNotification SentNotification(SubscriptionNotification notif)
        {
            notif.StatusValue = SubscriptionNotificationStatus.Sent;
            notif.DateSentUtc = DateTime.UtcNow;

            return this.Repo.SubscriptionNotifications.Update(notif);
        }

        public IList<SubscriptionModel> GetCurrentAndFutureSubscriptions(DateTime now)
        {
            return this.Repo.Subscriptions.GetCurrentAndFuture(this.Services.NetworkId, now).Select(o => new SubscriptionModel(o)).ToList();
        }

        public IList<SubscriptionNotification> GetNotificationsBySubscriptionIds(int[] ids)
        {
            return this.Repo.SubscriptionNotifications.GetBySubscriptionIds(ids);
        }

        public void IgnoreNotificationsBySubscription(int subscriptionId, DateTime now)
        {
            var items = this.Repo.SubscriptionNotifications.GetBySubscriptionId(subscriptionId);
            foreach (var item in items)
            {
                if (item.DateSendUtc < now)
                {
                    this.IgnoreNotification(item);
                }
            }
        }

        public MassSubscriptionRequest GetMassSubscriptionRequest(MassSubscriptionRequest request = null)
        {
            var model = request ?? new MassSubscriptionRequest();

            var templates = this.Repo.SubscriptionTemplates.GetAll(this.Services.NetworkId).Select(o => new SubscriptionTemplateModel(o)).OrderBy(o => o.PriceToPay).ToList();
            model.Templates = templates;

            return model;
        }

        public MassSubscriptionResult DoMassSubscription(MassSubscriptionRequest request)
        {
            throw new NotSupportedException("This method is not available. ");

            // TODO: CHECK for IsActive users :'(

            if (request == null)
                throw new ArgumentNullException("request");

            var result = new MassSubscriptionResult(request);

            var actingUser = this.Services.People.GetActiveById(request.UserId, PersonOptions.None);
            if (actingUser == null
                || (actingUser.NetworkId == this.Services.NetworkId && !actingUser.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ManageSubscriptions, NetworkAccessLevel.SparkleStaff)))
            {
                result.Errors.Add(MassSubscriptionError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var template = this.Services.SubscriptionTemplates.GetById(request.TemplateId);
            if (template == null)
            {
                result.Errors.Add(MassSubscriptionError.InvalidTemplate, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var usersToSubscribe = this.Services.People.GetNotSubscribedUsers(false);

            var tz = this.Services.People.GetTimezone(actingUser);
            var dateStartUtc = tz.ConvertToUtc(request.StartDateLocal);

            var now = DateTime.UtcNow;
            var subscriptions = usersToSubscribe
                .Select(o =>
                {
                    var item = new Subscription
                    {
                        DateCreatedUtc = now,
                        OwnerUserId = request.UserId,
                        PaymentMethodValue = PaymentMethod.Unknown,
                        IsPaid = true,
                        AppliesToUserId = o.Id,
                    };
                    this.ApplyTemplate(template, item, dateStartUtc);
                    return item;
                })
                .ToList();

            foreach (var item in subscriptions)
            {
                var user = this.Services.People.GetActiveById(item.AppliesToUserId.Value, PersonOptions.None);
                if (user != null)
                    this.InsertNewSubscription(item, user, request.SendConfirmEmail);
            }

            result.Succeed = true;
            return result;
        }

        public IList<SubscriptionModel> GetActiveUserSubscriptions()
        {
            return this.Repo.Subscriptions.GetAllActive(this.Services.NetworkId)
                .Where(o => o.AppliesToUserId.HasValue)
                .Select(o => new SubscriptionModel(o))
                .ToList();
        }

        public bool IsUserSubscribed(int userId)
        {
            var user = this.Services.People.SelectWithId(userId);
            if (user == null)
                return false;

            return this.IsUserSubscribed(user);
        }

        public bool IsUserSubscribed(User user)
        {
            if (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled)
                return true;

            var ids = this.Repo.Subscriptions.GetUserIdsSubscribedAmongIds(this.Services.NetworkId, new int[] { user.Id, }, DateTime.UtcNow);
            return ids.Contains(user.Id);
        }

        public SubscriptionStatusModel GetStatus(bool monthly, bool actual)
        {
            var result = new SubscriptionStatusModel();

            result.Templates = this.Services.SubscriptionTemplates.GetAll().ToDictionary(x => x.Id, x => x);
            DateTime now = DateTime.UtcNow;

            if (monthly)
            {
                // get a begin-end date range
                var firstSub = this.Repo.Subscriptions.GetFirstInTime();
                DateTime begin;
                if (firstSub != null)
                {
                    begin = firstSub.DateBeginUtc.Value.ToPrecision(DateTimePrecision.Month);
                }
                else
                {
                    begin = DateTime.UtcNow.ToPrecision(DateTimePrecision.Month);
                }

                var lastSub = this.Repo.Subscriptions.GetLastInTime();
                DateTime end;
                if (firstSub != null)
                {
                    end = lastSub.DateEndUtc.Value.ToPrecision(DateTimePrecision.Month).AddMonths(1);
                    if (end < now)
                        end = DateTime.UtcNow.ToPrecision(DateTimePrecision.Month).AddMonths(1);
                }
                else
                {
                    end = DateTime.UtcNow.ToPrecision(DateTimePrecision.Month).AddMonths(1);
                }

                if (this.Repo.Subscriptions.HasAnyUnlimited())
                {
                    end.AddMonths(3);
                }

                // weekly report
                DateTime current = begin;
                var weeks = (int)((end - begin).TotalDays / 30 * 1.2);
                IDictionary<DateTime, IDictionary<int, int>> data = new Dictionary<DateTime, IDictionary<int, int>>(weeks);
                result.Monthly = data;
                do
                {
                    var currentData = this.Repo.Subscriptions.CountActiveSubscriptionsInDateRange(current, current.AddMonths(1));
                    data.Add(current, currentData);
                } while ((current = current.AddMonths(1)) < end);
            }

            if (actual)
            {
                var currentData = this.Repo.Subscriptions.CountActiveSubscriptionsInDateRange(now, now);
                result.Actual = new Dictionary<int, int>();
                foreach (var item in result.Templates.Values)
                {
                    result.Actual.Add(item.Id, currentData.ContainsKey(item.Id) ? currentData[item.Id] : 0);
                }

                result.ActiveUsers = this.Repo.People.CountActiveUsers();
                result.InactiveUsers = this.Repo.People.CountInactiveUsers();
            }

            return result;
        }

        public IDictionary<int, IList<SubscriptionModel>> GetUsersAppliedSubscriptions(int[] userIds, DateTime? date = null)
        {
            var now = date ?? DateTime.UtcNow;
            var subs = this.Repo.Subscriptions.GetUsersAppliedSubscriptions(userIds, now);

            return subs
                .GroupBy(o => o.AppliesToUserId.Value)
                .ToDictionary(o => o.Key, o => (IList<SubscriptionModel>)o.Select(i => new SubscriptionModel(i)).ToList());
        }

        public IDictionary<int, bool> GetUserStatus(int[] userIds)
        {
            return this.GetUserStatus(userIds, DateTime.UtcNow);
        }

        public IDictionary<int, bool> GetUserStatus(int[] userIds, DateTime dateTimeUtc)
        {
            if (userIds == null)
                throw new ArgumentNullException("userIds");

            var result = this.Repo.Subscriptions.GetUserIdsSubscribedAmongIds(this.Services.NetworkId, userIds, dateTimeUtc);
            return userIds.ToDictionary(id => id, id => result.Contains(id));
        }

        public int CountSubscribedPeople()
        {
            return this.Repo.Subscriptions.CountSubscribedPeople(this.Services.NetworkId);
        }

        public decimal GetSumOfAllSubscriptionAmounts()
        {
            return this.Repo.Subscriptions.GetSumOfAllSubscriptionAmounts(this.Services.NetworkId);
        }

        public void NotifyUserOfActivatedSubscription(int userId, int subscriptionId)
        {
            var item = new Activity
            {
                Type = (int)ActivityType.NewSubscriptionActivated,
                Message = subscriptionId.ToString(),
                Date = DateTime.UtcNow,
                Displayed = false,
                UserId = userId,
            };
            this.Repo.Activities.Insert(item);
        }

        private Subscription InsertNewSubscription(Subscription item, UserModel applier, bool sendEmail)
        {
            item = this.Repo.Subscriptions.Insert(item);

            item = this.Repo.Subscriptions.GetById(item.Id);
            this.RenewSubscriptionNotifications(item);
            this.NotifyUserOfActivatedSubscription(applier.Id, item.Id);

            if (sendEmail)
            {
                this.Services.Email.SendSubscriptionActivated(new SubscriptionModel(item), applier);
            }

            return item;
        }
    }
}
