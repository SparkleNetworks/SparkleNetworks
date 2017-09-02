
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;

    public abstract class GroupNewsletterBase : BaseSparkleCommand
    {
        private string remoteClient;

        public void Send(SparkleCommandArgs args, DateTime start, DateTime end, NotificationFrequencyType notificationFrequencyType)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            var usernames = new List<string>(args.Arguments.Count);
            {
                bool foundCommandName = false;
                for (int i = 0; i < args.Arguments.Count; i++)
                {
                    if (foundCommandName)
                    {
                        usernames.Add(args.Arguments[i].Trim().ToLowerInvariant());
                    }
                    else
                    {
                        foundCommandName = args.Arguments[i].ToLowerInvariant().EndsWith("groupnewsletter");
                    }
                }
            }

            string subject;
            switch (notificationFrequencyType)
            {
                case NotificationFrequencyType.Daily:
                case NotificationFrequencyType.DailyTest:
                    subject = "Hier dans vos groupes";
                    break;

                case NotificationFrequencyType.Weekly:
                case NotificationFrequencyType.WeeklyTest:
                    subject = "Cette semaine dans vos groupes";
                    break;

                case NotificationFrequencyType.Monthly:
                case NotificationFrequencyType.MonthlyTest:
                    subject = "Le mois dernier dans vos groupes";
                    break;

                default:
                    subject = "Récemment dans vos groupes";
                    break;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "GroupNewsletterBase";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            this.Out.WriteLine();
            this.Out.WriteLine("Running group newsletter with notification type: " + notificationFrequencyType);
            args.SysLogger.Info("Running group newsletter with notification type: " + notificationFrequencyType, remoteClient, Environment.UserName, ErrorLevel.Success);

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("GroupNewsletterBase", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            services.NetworkId = network.Id;

            IList<MemberGroupNewsletter> people = new List<MemberGroupNewsletter>();

            // step through each group
            this.Out.WriteLine("Preparing data...");
            var allGroups = services.Groups.SelectAllGroups();
            foreach (var group in allGroups)
            {
                if (group.IsDeleted)
                    continue;

                services.Wall.OptionsList.Add("PostedBy");
                services.Wall.OptionsList.Add("Comments.PostedBy");

                // fetch timeline
                var news = services.Wall.SelectFromGroup(group.Id, start, end);
                foreach (var item in news)
                {
                    item.LikesCount = item.Likes.Count();
                    var commentsCount = item.Comments.Count();
                    item.CommentsCount = (commentsCount == 0 ? "Aucun commentaire" : commentsCount + " commentaire") + (commentsCount > 1 ? "s" : "");
                }

                // find members and requests
                services.GroupsMembers.OptionsList.Add("PostedBy");
                var members = services.GroupsMembers.SelectGroupMembers(group.Id);
                var groupMembersAdmins = services.GroupsMembers.SelectAdminsGroupMembers(group.Id);
                var groupRequest = services.GroupsMembers.CountGroupRequestMembers(group.Id);
                foreach (var member in members)
                {
                    if (!services.People.IsActive(member.User))
                        continue;

                    // has valid frequency?
                    if (member.NotificationFrequencyValue == notificationFrequencyType
                    || !member.NotificationFrequency.HasValue && group.NotificationFrequencyValue == notificationFrequencyType)
                    {
                        // add group to user mailing
                        MemberGroupNewsletter person = people.SingleOrDefault(p => p.Person.Id == member.UserId);
                        if (person == null)
                        {
                            person = new MemberGroupNewsletter
                            {
                                Person = member.User,
                                Groups = new List<MemberGroupNewsletterGroup>(),
                            };
                            people.Add(person);
                        }

                        var theGroup = new MemberGroupNewsletterGroup
                        {
                            Group = group,
                            Walls = news,
                            PendingRequestsCount = (groupMembersAdmins.Select(o => o.Id).Contains(member.Id) ? groupRequest : 0),
                        };
                        if (theGroup.Walls.Count > 0 || theGroup.PendingRequestsCount != 0)
                        {
                            person.Groups.Add(theGroup);
                        }
                    }
                }
            }

            IList<MemberGroupNewsletter> sendToPeople = new List<MemberGroupNewsletter>();

            foreach (MemberGroupNewsletter person in people)
            {
                if (person.Groups.Count > 0 && services.People.IsActive(person.Person))
                    sendToPeople.Add(person);
            }

            if (services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled && services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced)
            {
                var subscribedUserIds = services.Repositories.Subscriptions.GetUserIdsSubscribedAmongIds(
                    services.NetworkId,
                    sendToPeople.Select(o => o.Person.Id).ToArray(),
                    args.Now.ToUniversalTime());

                sendToPeople = sendToPeople.Where(o => subscribedUserIds.Contains(o.Person.Id)).ToList();
            }

            if (sendToPeople.Count == 0)
            {
                this.Out.WriteLine("Nothing to send.");
                this.Out.WriteLine();
                return;
            }

            this.Out.WriteLine();
            this.Out.WriteLine("About to send emails to " + sendToPeople.Count + " persons.");
            this.Out.WriteLine("The newsletter start date is " + start.ToString());
            this.Out.WriteLine("The newsletter end   date is " + end.ToString());
            this.Out.WriteLine(this.Simulate ? "This is a simulation." : "This is for real.");
            this.Out.WriteLine();
            this.Out.Write("Hit Ctrl+C within 5 seconds to cancel.");
            for (int i = 0; i < 8; i++)
            {
                System.Threading.Thread.Sleep(1000);
                this.Out.Write(".");
            }
            this.Out.WriteLine();

            // send email
            int success = 0, failSmtp = 0, failData = 0, failOther = 0, skipped = 0;
            foreach (var person in sendToPeople)
            {
                if (usernames.Count > 0 && !usernames.Contains(person.Person.Login.ToLowerInvariant()))
                {
                    skipped++;
                    this.Out.WriteLine("SKIP:  " + person.Person.Email);
                    continue;
                }

                if (!this.Simulate)
                {
                    try
                    {
                        services.Email.SendWeeklyGroupNewsletter(person, subject);
                        success++;
                        this.Out.WriteLine("OK:    " + person.Person.Email);
                        args.SysLogger.Info(this.ToString(), remoteClient, Environment.UserName, ErrorLevel.Success, person.Person.Email);
                    }
                    catch (DataException ex)
                    {
                        failData++;
                        this.Error.WriteLine("ERROR: " + person.Person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.Message);
                        args.SysLogger.Error(this.ToString(), remoteClient, Environment.UserName, ErrorLevel.Data, person.Person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        Trace.TraceError("GroupNewsletterBase.Send[" + person + "] SendWeeklyGroupNewsletter error " + ex.ToString());
                    }
                    catch (InvalidOperationException ex)
                    {
                        failSmtp++;
                        this.Error.WriteLine("ERROR: " + person.Person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.Message);
                        args.SysLogger.Error(this.ToString(), remoteClient, Environment.UserName, ErrorLevel.ThirdParty, person.Person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        Trace.TraceError("GroupNewsletterBase.Send[" + person + "] SendWeeklyGroupNewsletter error " + ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        failOther++;
                        this.Error.WriteLine("ERROR: " + person.Person.Email + " -> " + ex.GetType().Name + Environment.NewLine + "  " + ex.Message);
                        args.SysLogger.Error(this.ToString(), remoteClient, Environment.UserName, ErrorLevel.Internal, person.Person.Email + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                        Trace.TraceError("GroupNewsletterBase.Send[" + person + "] SendWeeklyGroupNewsletter error " + ex.ToString());
                        if (ex.IsFatal())
                        {
                            this.Error.WriteLine("FATAL ERROR. Exiting.");
                            throw;
                        }
                    }
                }
                else
                {
                    success++;
                    this.Out.WriteLine("OK (simulation): " + person.Person.Email);
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
            if (skipped > 0)
                this.Out.WriteLine("SKIP:        {0}", skipped);
            this.Out.WriteLine();
        }
    }
}
