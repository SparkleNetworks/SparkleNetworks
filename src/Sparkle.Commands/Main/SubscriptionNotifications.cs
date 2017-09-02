
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Subscriptions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SubscriptionNotifications : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;

            ////// simulation: not supported
            ////if (args.Simulation)
            ////{
            ////    this.Out.WriteLine("Simulation is not supported; not runnin.");
            ////    args.SysLogger.Error("SubscriptionNotifications", remoteClient, Environment.UserName, ErrorLevel.Input, "Simulation is not supported; not running.");
            ////    return;
            ////}

            var context = new Context
            {
                Args = args,
                UtcNow = args.Now.ToUniversalTime(),
            };

            if (!this.Run(context))
            {
                context.Services.Logger.Error("SubscriptionNotifications failed", ErrorLevel.Business);
            }
        }

        private bool Run(Context context)
        {
            return !this.ParseArgs(context)
                || !this.InitializeServices(context)
                || !this.InitializeNetwork(context)
                || !this.GetPendingNotifications(context)
                || !this.GetRelatedSubscriptions(context)
                || this.SendEmails(context);
        }

        private bool ParseArgs(Context context)
        {
            return true;
        }

        private bool InitializeServices(Context context)
        {
            var services = context.Args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "SubscriptionNotifications";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            context.Services = services;

            return true;
        }

        private bool InitializeNetwork(Context context)
        {
            var networkName = context.Args.ApplicationConfiguration.Tree.NetworkName ?? context.Args.Application.UniverseName;
            var network = context.Services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                context.Args.SysLogger.Info("SubscriptionNotifications", this.remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return false;
            }

            context.Services.NetworkId = network.Id;

            if (!context.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled)
            {
                this.Out.WriteLine("Subscriptions are not enabled on network \"" + networkName + "\"; not running.");
                context.Args.SysLogger.Info("SubscriptionNotifications", this.remoteClient, Environment.UserName, ErrorLevel.Success, "Subscriptions are not enabled on network \"" + networkName + "\"; not running.");
                return false;
            }

            {
                var message = "Ready to run SubscriptionNotifications for network '" + context.Services.Network.ToString() + "' (universe " + context.Args.Application.UniverseName + ") " + (context.Args.Simulation ? "as simulation" : "for real");
                this.Out.WriteLine(message);
                context.Args.SysLogger.Info("SubscriptionNotifications", this.remoteClient, Environment.UserName, ErrorLevel.Success, message);
            }

            return true;
        }

        private SubscriptionModel GetSubToNotify(SubscriptionModel item, IList<SubscriptionModel> others)
        {
            if (!item.DateEndUtc.HasValue)
                return null;

            SubscriptionModel next;
            if ((next = others.Where(o => o.DateBeginUtc.Value == item.DateEndUtc.Value.AddSeconds(1D)).SingleOrDefault()) == null)
                return item;

            return this.GetSubToNotify(next, others);
        }

        private bool GetPendingNotifications(Context context)
        {
            var dateExpired = context.UtcNow.AddDays(-4D);

            this.Out.WriteLine();
            this.Out.WriteLine("Retrieving subscriptions to notify by user...");
            IList<SubscriptionModel> itemsToNotify = new List<SubscriptionModel>();
            var subscriptions = context.Services.Subscriptions.GetCurrentAndFutureSubscriptions(dateExpired);
            foreach (var item in subscriptions.GroupBy(o => o.AppliesToUserId))
            {
                var userSubs = item.OrderBy(o => o.DateBeginUtc.Value).ToList();
                var toNotify = this.GetSubToNotify(userSubs.First(), userSubs);
                if (toNotify != null)
                {
                    itemsToNotify.Add(toNotify);
                    userSubs.Remove(toNotify);
                }

                userSubs.ForEach(o =>
                {
                    if (context.Args.Simulation)
                    {
                        this.Out.WriteLine("SIMULATION: Subscriptions.IgnoreNotificationsBySubscription(" + o.Id + ") because it's too late to notify");
                    }
                    else
                    {
                        context.Services.Subscriptions.IgnoreNotificationsBySubscription(o.Id, context.UtcNow);
                    }
                });
            }

            this.Out.WriteLine("Found " + itemsToNotify.Count + " subscriptions ready to be notified.");

            // remove expired and multiple occurence
            var notifications = context.Services.Subscriptions.GetNotificationsBySubscriptionIds(itemsToNotify.Select(o => o.Id).ToArray())
                .Where(o => o.StatusValue == SubscriptionNotificationStatus.New && o.DateSendUtc <= context.UtcNow)
                .OrderBy(o => o.SubscriptionId)
                .ThenByDescending(o => o.DateSendUtc)
                .ToList();

            int ignored = 0;

            foreach (var item in notifications)
            {
                if (item.DateSendUtc < dateExpired || context.Notifications.ContainsKey(item.SubscriptionId))
                {
                    ignored++;
                    if (context.Args.Simulation)
                    {
                        this.Out.WriteLine("SIMULATION: Subscriptions.IgnoreNotification(" + item.Id + ")");
                    }
                    else
                    {
                        context.Services.Subscriptions.IgnoreNotification(item);
                    }
                }
                else
                {
                    context.Notifications.Add(item.SubscriptionId, item);
                }
            }

            this.Out.WriteLine("Among them " + ignored + " notifications have been definitively ignored and " + context.Notifications.Count + " will be sent.");
            this.Out.WriteLine();

            return true;
        }

        private bool GetRelatedSubscriptions(Context context)
        {
            context.Subscriptions = context.Services.Subscriptions
                .GetByIds(context.Notifications.Keys.ToArray(), SubscriptionOptions.AppliesToUser)
                .Where(o => o.NetworkId == context.Services.NetworkId)
                .ToDictionary(o => o.Id, o => o);

            return true;
        }

        private bool SendEmails(Context context)
        {
            if (context.Subscriptions.Count == 0)
            {
                context.Args.SysLogger.Info("SubscriptionNotifications.SendEmails", this.remoteClient, Environment.UserName, ErrorLevel.Success, "No emails to send");
                return true;
            }

            this.Out.WriteLine("Ready to send emails.");
            this.Out.Write("Press Ctrl+C within 5 seconds to cancel");
            for (int i = 0; i < 5; i++)
            {
                System.Threading.Thread.Sleep(1000);
                this.Out.Write(".");
            }

            this.Out.Write(this.Out.NewLine);

            int failure = 0;
            int success = 0;
            foreach (var item in context.Notifications)
            {
                var sub = context.Subscriptions[item.Key];
                sub.Now = context.UtcNow;

                var notif = item.Value;
                var user = sub.AppliesToUser;

                bool isUserActive = context.Services.People.GetActiveById(user.Id, Data.Options.PersonOptions.None) != null;
                if (context.Args.Simulation)
                {
                    this.Out.WriteLine(
                        "SIMULATION: Notification {0} related to subscription {1} " + (isUserActive ? "" : "not ") + "sent to {2}",
                        user.Login,
                        sub.Id,
                        notif.Id);
                }
                else
                {
                    if (isUserActive)
                    {
                        try
                        {
                            context.Services.Email.SendSubscriptionEnded(sub, user);

                            context.Services.Subscriptions.SentNotification(notif);
                            context.Args.SysLogger.Info(
                                "SubscriptionNotifications.SendEmails",
                                this.remoteClient,
                                Environment.UserName,
                                ErrorLevel.Success,
                                "Notification {0} related to subscription {1} sent to {2}",
                                user.Login,
                                sub.Id,
                                notif.Id);
                            success++;
                        }
                        catch (Exception ex)
                        {
                            context.Args.SysLogger.Error("SubscriptionNotifications.SendEmails", this.remoteClient, Environment.UserName, ErrorLevel.Business, ex);
                            failure++;
                        }
                    }
                    else
                    {
                        context.Args.SysLogger.Info("SubscriptionNotifications.SendEmails", this.remoteClient, Environment.UserName, ErrorLevel.Success, "Not sending notification {0} to user {1} because the user is not active", notif.Id, user.Id);
                    }
                }
            }

            this.Out.WriteLine();
            this.Out.WriteLine("Done! There have been " + success + " success and " + failure + " failure(s)");
            this.Out.WriteLine();

            return true;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle SubscriptionNotifications
    Sends the notifications to all subscribed people.";

            registry.Register(
                "SubscriptionNotifications",
                "Sends an email that warns users of the end of their subscription based on the SusbcriptionNotifications table.",
                () => new SubscriptionNotifications(),
                longDesc);
        }

        public class Context
        {
            public Context()
            {
                this.Notifications = new Dictionary<int, SubscriptionNotification>();
                this.Subscriptions = new Dictionary<int, SubscriptionModel>();
            }

            public SparkleCommandArgs Args { get; set; }

            public DateTime UtcNow { get; set; }

            public IServiceFactory Services { get; set; }

            public IDictionary<int, SubscriptionNotification> Notifications { get; set; }

            public IDictionary<int, SubscriptionModel> Subscriptions { get; set; }
        }
    }
}
