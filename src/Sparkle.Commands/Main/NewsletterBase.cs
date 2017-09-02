
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.EmailTemplates;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.EmailModels;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Objects;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public abstract class NewsletterBase : BaseSparkleCommand
    {
        private string remoteClient;

        public void Send(SparkleCommandArgs args, NotificationFrequencyType notificationFrequencyType)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "NewsletterBase/" + notificationFrequencyType;
            services.HostingEnvironment.RemoteClient = this.remoteClient;

            DateTime now = args.Now;
            if (notificationFrequencyType == NotificationFrequencyType.Weekly)
            {
                if (now.DayOfWeek == DayOfWeek.Sunday)
                    now = now.AddDays(1D).Date;
            }
            else if (notificationFrequencyType == NotificationFrequencyType.Daily)
            {
            }
            else
            {
                throw new ArgumentException("Frequency '" + notificationFrequencyType + "' is not supported", "notificationFrequencyType");
            }

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("WeeklyNewsletter", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            services.NetworkId = network.Id;

            IList<WeeklyMailSubscriber> people = null;

            // fetch people
            try
            {
                if (args.Arguments.Count < 2 || args.Arguments.Contains("--invited-only") || args.Arguments.Contains("--registered-only"))
                {
                    bool invited = false, registered = false;
                    if (args.Arguments.Contains("--invited-only"))
                    {
                        invited = true;
                    }

                    if (args.Arguments.Contains("--registered-only"))
                    {
                        registered = true;
                    }

                    if (!registered && !invited)
                    {
                        invited = registered = true;
                    }
                    
                    if (notificationFrequencyType == NotificationFrequencyType.Weekly)
                    {
                        people = services.People.SelectForWeeklyMail(invited, registered);
                    }
                    else if (notificationFrequencyType == NotificationFrequencyType.Daily)
                    {
                        people = services.People.SelectForDailyMail(invited, registered);
                    }
                    else
                    {
                        throw new ArgumentException("Frequency '" + notificationFrequencyType + "' is not supported", "notificationFrequencyType");
                    }

                    // filter user subscribed
                    if (services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced)
                    {
                        var subscribedUserIds = services.Repositories.Subscriptions.GetUserIdsSubscribedAmongIds(
                            services.NetworkId,
                            people.Where(o => o.UserId != 0).Select(o => o.UserId).ToArray(),
                            args.Now.ToUniversalTime());

                        int previousCount = people.Count;
                        people = people.Where(o => subscribedUserIds.Contains(o.UserId)).ToList();
                        this.Out.WriteLine("Because Features.Subscriptions.IsEnabled, filtering " + previousCount + " users down to " + people.Count + ".");

                    }
                }
                else
                {
                    people = new List<WeeklyMailSubscriber>();
                    foreach (var arg in args.Arguments.Skip(1))
                    {
                        if (string.IsNullOrWhiteSpace(arg))
                            continue;

                        //var item = services.People.SelectForWeeklyMailByLogin(arg);
                        var person = services.People.SelectWithLogin(arg);
                        if (person != null)
                        {
                            var item = new WeeklyMailSubscriber(person);
                            people.Add(item);
                        }
                        else
                        {
                            var invited = services.Invited.GetByEmail(arg);
                            if (invited != null)
                            {
                                var item = new WeeklyMailSubscriber()
                                {
                                    Email = invited.Email,
                                    UserId = 0,
                                    Registered = false,
                                    InvitedCodeGuid = invited.Code,
                                    InvitedBy = invited.InvitedByUserId,
                                    OptedIn = invited.Unregistred == false,
                                };
                                people.Add(item);
                            }
                            else
                            {
                                this.ErrorWriteLine(services, ErrorLevel.Input, "ERROR: " + arg + " does not exist.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ErrorWriteLine(services, ErrorLevel.Data, ex.ToString());
                return;
            }

            if (people.Count == 0)
            {
                this.Out.WriteLine("Nobody.");
                return;
            }

            int invitedOptedIn = 0, invitedTotal = 0;
            int registeredOptedIn = 0, registeredTotal = 0;
            for (int i = 0; i < people.Count; i++)
            {
                if (people[i].Registered)
                {
                    registeredTotal++;
                    if (people[i].OptedIn)
                        registeredOptedIn++;
                }

                if (!people[i].Registered)
                {
                    invitedTotal++;
                    if (people[i].OptedIn)
                        invitedOptedIn++;
                }
            }

            int invitedNoOptIn = invitedTotal - invitedOptedIn;
            int registeredNoOptIn = registeredTotal - registeredOptedIn;

            this.Out.WriteLine();
            this.Out.WriteLine("==============================================");
            this.Out.WriteLine(" User type  | Opted-in | No optin |    Total  ");
            this.Out.WriteLine("==============================================");
            this.Out.WriteLine(" Registered | " + string.Format("{0,8}", registeredOptedIn) + " | " + string.Format("{0,8}", registeredNoOptIn) + " | " + string.Format("{0,8}", registeredTotal));
            this.Out.WriteLine(" Invited    | " + string.Format("{0,8}", invitedOptedIn)    + " | " + string.Format("{0,8}", invitedNoOptIn)    + " | " + string.Format("{0,8}", invitedTotal));
            this.Out.WriteLine("==============================================");
            this.Out.WriteLine(" Total      | " + string.Format("{0,8}", registeredOptedIn + invitedOptedIn) + " | " + string.Format("{0,8}", registeredNoOptIn + invitedNoOptIn) + " | " + string.Format("{0,8}", invitedTotal + registeredTotal));
            this.Out.WriteLine("==============================================");
            this.Out.WriteLine();

            if (people.Count == 0)
            {
                this.Out.WriteLine("Nothing to send.");
                this.Out.WriteLine();
                return;
            }

            this.Out.WriteLine();
            this.Out.WriteLine("About to send emails to " + people.Count(p => p.Registered) + " registered and " + people.Count(p => !p.Registered) + " invited persons.");
            this.Out.WriteLine("The newsletter start date is " + now.ToShortDateString());
            this.Out.WriteLine(this.Simulate ? "This is a simulation." : "This is for real.");
            this.Out.WriteLine();
            this.Out.Write("Hit Ctrl+C within 5 seconds to cancel.");
            for (int i = 0; i < 5; i++)
            {
                System.Threading.Thread.Sleep(1000);
                this.Out.Write(".");
            }
            this.Out.WriteLine();

            // verify Counters 
            services.StatsCounters.SetupWeeklyNewsletterCounters();
            services.StatsCounters.SetupDailyNewsletterCounters();

            // send email
            int success = 0, failSmtp = 0, failData = 0, failOther = 0;
            foreach (var person in people)
            {
                var status = person.Registered ? "R " : "I ";
                if (this.Simulate)
                {
                    try
                    {
                        var model = new NewsletterEmailModel(person.Email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                        model.Initialize(services);
                        services.Email.MakeNewsletter(person, now, notificationFrequencyType, model);
                        success++;
                        this.Out.WriteLine("OK (simulation): " + status + person.Email);
                    }
                    catch (Exception ex)
                    {
                        failOther++;
                        this.Error.WriteLine("ERROR (simulation): " + status + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.ToString());
                        this.Error.WriteLine(ex.ToString());
                        args.SysLogger.Error("WeeklyNewsletter", remoteClient, Environment.UserName, ErrorLevel.Internal, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        if (ex.IsFatal())
                        {
                            this.Error.WriteLine("FATAL ERROR. Exiting.");
                            throw;
                        }
                    }
                }
                else
                {
                    try
                    {
                        services.Email.SendNewsletter(person, now, notificationFrequencyType);
                        success++;
                        this.Out.WriteLine("OK:    " + status + person.Email);
                        args.SysLogger.Info("WeeklyNewsletter", remoteClient, Environment.UserName, ErrorLevel.Success, person.Email);
                    }
                    catch (DataException ex)
                    {
                        failData++;
                        this.Error.WriteLine("ERROR: " + status + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.ToString());
                        args.SysLogger.Error("WeeklyNewsletter", remoteClient, Environment.UserName, ErrorLevel.Data, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    catch (InvalidOperationException ex)
                    {
                        failSmtp++;
                        this.Error.WriteLine("ERROR: " + status + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.ToString());
                        args.SysLogger.Error("WeeklyNewsletter", remoteClient, Environment.UserName, ErrorLevel.ThirdParty, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    catch (Exception ex)
                    {
                        failOther++;
                        this.Error.WriteLine("ERROR: " + status + person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.ToString());
                        this.Error.WriteLine(ex.ToString());
                        args.SysLogger.Error("WeeklyNewsletter", remoteClient, Environment.UserName, ErrorLevel.Internal, person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        if (ex.IsFatal())
                        {
                            this.Error.WriteLine("FATAL ERROR. Exiting.");
                            throw;
                        }
                    }
                }
            }

            this.Out.WriteLine();
            this.Out.WriteLine("Results: ");
            this.Out.WriteLine("-------- ");
            if (success > 0)
                this.Out.WriteLine("OK:          {0}", success);
            if (failSmtp > 0)
                this.Out.WriteLine("SMTP ERROR:  {0}", failSmtp);
            if (failData > 0)
                this.Out.WriteLine("DATA ERROR:  {0}", failData);
            if (failOther > 0)
                this.Out.WriteLine("OTHER ERROR: {0}", failOther);
            this.Out.WriteLine();
        }

        private void ErrorWriteLine(IServiceFactory services, ErrorLevel level, string message)
        {
            this.Error.WriteLine(message);
            services.Logger.Error("NewsletterBase", level, message);
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle WeeklyNewsletter
    Sends the newsletter to all registered and invited people.

sparkle WeeklyNewsletter <firstname.lastname> ...
    Sends the newsletter to the specified usernames.

sparkle WeeklyNewsletter --invited-only
sparkle WeeklyNewsletter --registered-only
    Sends the newsletter only to the specified target";

            registry.Register(
                "WeeklyNewsletter",
                "Sends the weekly newsletter to all people accepting it.",
                () => new WeeklyNewsletter(),
                longDesc);
        }
    }
}
