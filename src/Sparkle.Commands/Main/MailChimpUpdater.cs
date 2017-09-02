
namespace Sparkle.Commands.Main
{
    using MailChimp;
    using MailChimp.Errors;
    using MailChimp.Helper;
    using MailChimp.Lists;
    using Sparkle.Commands.Lang;
    using Sparkle.Common.CommandLine;
    using Sparkle.Data.Filters;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Objects;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Synchronizes members email addresses with a mailchimp service.
    /// </summary>
    public class MailChimpUpdater : BaseSparkleCommand
    {
        const string MC_subscribed = "subscribed";
        const string MC_unsubscribed = "unsubscribed";
        const string MC_cleaned = "cleaned";
        const string MC_banned = "banned";
        const string MC_pending = "pending";
        const string MC_invalidEmail = "invalidEmail";
        const string MC_resubscribe = "resubscribe"; // this is not a MC status

        private string remoteClient;

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;

            var context = new Context
            {
                Args = args,
            };
            if (!this.Run(args, context))
            {
                context.Services.Logger.Error("RecallMail failed", ErrorLevel.Business);
            }
        }

        private bool Run(SparkleCommandArgs args, Context context)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return false;
            }

            return !this.ParseArgs(context)
                || !this.InitializeServices(context)
                || !this.InitializeMailchimp(context)
                || !this.GetMailchimpList(context)
                || !this.GetOrCreateMergeVars(context)
                || !this.GetOrCreateSegments(context)
                || !this.GetMailchimpUsers(context)
                || !this.UpdateLocalUsers(context)
                || !this.UpdateMailchimpUsers(context)
                || this.UpdateSegments(context);
        }

        private bool ParseArgs(Context context)
        {
            return true;
        }

        private bool InitializeServices(Context context)
        {
            var services = context.Args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "MailChimpUpdater";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            context.Services = services;

            return true;
        }

        private bool InitializeMailchimp(Context context)
        {
            var args = context.Args;

            var apiKey = context.Services.AppConfiguration.Tree.Externals.MailChimp.ApiKey.NullIfEmpty();
            var listName = context.Services.AppConfiguration.Tree.Externals.MailChimp.ListName.NullIfEmpty();

            if (apiKey == null || listName == null)
            {
                var message = "MailChimp is not configured for this universe. Not running.";
                context.Services.Logger.Warning("MailChimpUpdater", ErrorLevel.Success, message);
                this.Out.WriteLine(message);
                this.Out.WriteLine();
                return this.EndWithError(context, ErrorLevel.Input, message, false);
            }

            {
                var message = "Starting with list: " + listName + (args.Simulation ? " (SIMULATION)" : "");
                context.Services.Logger.Info("MailChimpUpdater", ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }

            var mc = new MailChimpManager(apiKey);
            context.MailchimpClient = mc;

            {
                var account = mc.GetAccountDetails();
                if (account != null)
                {
                    this.Out.WriteLine("MC UserId:   " + account.UserId);
                    this.Out.WriteLine("MC Username: " + account.Username);
                    this.Out.WriteLine("MC PlanType: " + account.PlanType);
                }
                else
                {
                    this.Out.WriteLine("Could not fetch MC account details. ");
                }
            }

            return true;
        }

        private bool GetMailchimpList(Context context)
        {
            var mc = context.MailchimpClient;
            var listName = context.Services.AppConfiguration.Tree.Externals.MailChimp.ListName.NullIfEmpty();
            if (listName == null)
                return this.EndWithError(context, ErrorLevel.Input, "No MailChimp list name specified.");

            // find the desired list
            var lists = mc.GetLists();
            var list = lists.Data.Where(o => o.Name == listName || o.Id == listName).SingleOrDefault();
            if (list == null)
            {
                return this.EndWithError(context, ErrorLevel.Input, "The list '" + listName + "' does not exists in MailChimp.");
            }

            context.LocalUsers = context.Services.People.SelectAll().ToDictionary(u => u.Email, u => u);
            context.LocalNotifs = context.Services.Notifications.SelectAll().ToDictionary(u => u.UserId, u => u);
            context.LocalCompanies = context.Services.Company.GetAllLight().ToDictionary(c => c.ID, c => c);
            context.LocalLastActivities = context.Services.Live.GetAllLastActivityDate();

            context.MailchimpList = list;

            return true;
        }

        private bool GetOrCreateMergeVars(Context context)
        {
            var mc = context.MailchimpClient;
            var list = context.MailchimpList;

            var vars = mc.GetMergeVars(new string[] { list.Id, });
            var listVars = vars.Data.Single().MergeVars;

            foreach (var varDef in context.MergeVars.All)
            {
                varDef.Definition = listVars.SingleOrDefault(v => v.Tag == varDef.Options.Tag);
                if (varDef.Definition == null)
                {
                    varDef.Definition = mc.AddMergeVar(list.Id, varDef.Options.Tag, varDef.Options.Name, varDef.Options);
                    var message = "Create merge var '" + varDef.Options.Tag + "'.";
                    context.Services.Logger.Info("GetOrCreateMergeVars", ErrorLevel.Success, message);
                    this.Out.WriteLine(message);
                }
            }

            return true;
        }

        private bool GetOrCreateSegments(Context context)
        {
            // segment configuration
            var createdSegments = new List<int>();
            string segmentNameFormat = "SN-{0}"; // Part1: SN-Key-Display name
            string segmentFullNameFormat = "{0}-{1}"; // Full: Part1-Display name
            var networksDefaultCulture = context.Services.DefaultCulture;
            string networksDefaultCultureName = networksDefaultCulture.Name;
            var emailTypes = new string[] { "Other", "Active", "Inactive", "ActiveSubscribed", "ActiveUnsubscribed", };

            // compute segment name parts
            var emailTypesList = emailTypes
                .Select(emailType =>
                {
                    var segmentKeyPart = string.Format(segmentNameFormat, emailType);
                    var segmentDisplayName = CommandStrings.ResourceManager.GetString("MailChimpUpdater_SegmentNames_" + emailType, networksDefaultCulture);
                    var segmentFullName = string.Format(segmentFullNameFormat, segmentKeyPart, segmentDisplayName);

                    return new
                    {
                        EmailType = emailType,                           // internal name
                        SegmentKeyPart = segmentKeyPart,                 // beginning of the MC name to match
                        segmentDisplayName = segmentDisplayName,         // display name in network's language
                        SegmentFullName = segmentFullName,               // full name for create operation
                        MailchimpSegment = new Mutable<StaticSegmentResult>(), // the MC segment object
                    };
                })
                .ToDictionary(x => x.EmailType, x => x);

            // first segments pass (create missing ones)
            var segments = context.MailchimpClient.GetStaticSegmentsForList(context.MailchimpList.Id);
            foreach (var kvp in emailTypesList)
            {
                var emailType = kvp.Value;

                var matches = segments
                    .Where(s => s.SegmentName.StartsWith(emailType.SegmentKeyPart))
                    .ToArray();
                if (matches.Length == 0)
                {
                    var segmentId = context.MailchimpClient.AddStaticSegment(context.MailchimpList.Id, emailType.SegmentFullName);
                    createdSegments.Add(segmentId.NewStaticSegmentID);

                    context.Services.Logger.Info("GetOrCreateSegments", ErrorLevel.Success, "Created MC segment '" + emailType.SegmentFullName + "' with id " + segmentId);
                }
            }

            // second segments pass to get them all
            segments = context.MailchimpClient.GetStaticSegmentsForList(context.MailchimpList.Id);
            foreach (var kvp in emailTypesList)
            {
                var emailType = kvp.Value;

                var matches = segments
                    .Where(s => s.SegmentName.StartsWith(emailType.SegmentKeyPart + "-"))
                    .ToArray();
                if (matches.Length == 0)
                {
                    context.Services.Logger.Info("GetOrCreateSegments", ErrorLevel.Success, "Missing segment '" + emailType.SegmentKeyPart + "' (create should have succeeded but did not)");
                }
                else if (matches.Length == 1)
                {
                    emailType.MailchimpSegment.Object = matches[0];
                }
                else
                {
                    emailType.MailchimpSegment.Object = matches[0];
                    context.Services.Logger.Warning("GetOrCreateSegments", ErrorLevel.Integrity, "Multiple segments for key '" + emailType.SegmentKeyPart + "' ('" + string.Join("', '", matches.Select(x => x.SegmentName)) + "')");
                }
            }

            context.MailchimpAllSegments = emailTypesList.Select(x => x.Value.MailchimpSegment).Where(x => x != null && x.Object != null).Select(x => x.Object).ToArray();
            context.MailchimpOtherSegment.MailchimpSegment = emailTypesList.ContainsKey("Other") ? emailTypesList["Other"].MailchimpSegment.Object : null;
            context.MailchimpActiveSegment.MailchimpSegment = emailTypesList.ContainsKey("Active") ? emailTypesList["Active"].MailchimpSegment.Object : null;
            context.MailchimpInactiveSegment.MailchimpSegment = emailTypesList.ContainsKey("Inactive") ? emailTypesList["Inactive"].MailchimpSegment.Object : null;
            context.MailchimpActiveSubscribedSegment.MailchimpSegment = emailTypesList.ContainsKey("ActiveSubscribed") ? emailTypesList["ActiveSubscribed"].MailchimpSegment.Object : null;
            context.MailchimpActiveUnsubscribedSegment.MailchimpSegment = emailTypesList.ContainsKey("ActiveUnsubscribed") ? emailTypesList["ActiveUnsubscribed"].MailchimpSegment.Object : null;

            return true;
        }

        private bool GetMailchimpUsers(Context context)
        {
            var mc = context.MailchimpClient;
            var list = context.MailchimpList;
            var localUsers = context.LocalUsers;
            
            // fetch users from MailChimp
            this.Out.WriteLine("Fetching MailChimp users...");
            var mailChimpUsers = new List<MemberInfo>();
            context.MailchimpUsers = mailChimpUsers;
            string[] statuses = new string[] { MC_subscribed, MC_unsubscribed, MC_cleaned, };
            foreach (var status in statuses)
            {
                for (int i = 0; ; i++)
                {
                    var result = mc.GetAllMembersForList(list.Id, status, i, 100, "id");
                    if (result == null || result.Data == null || result.Data.Count == 0)
                        break;

                    mailChimpUsers.AddRange(result.Data);
                }
            }

            {
                var message = "Retreived " + mailChimpUsers.Count + " users from MC list '" + list.Name + "' and there are " + localUsers.Count + " users in the local database";
                context.Services.Logger.Verbose("GetMailchimpUsers", ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }

            return true;
        }

        private bool UpdateLocalUsers(Context context)
        {
            var localUsers = context.LocalUsers;
            var localNotifs = context.LocalNotifs;
            const string logPath = "UpdateLocalUsers";

            // update local from MC
            this.Out.WriteLine();
            this.Out.WriteLine("Updating local users from MailChimp users...");
            var statusChanges = new List<string>();
            int unknownMcUsers = 0;
            int j = 0;
            foreach (var mcUser in context.MailchimpUsers)
            {
                if (++j % 50 == 0)
                    this.Out.WriteLine("  MailChimp user: " + j + " / " + context.MailchimpUsers.Count);

                string message = "";
                var localUser = localUsers.ContainsKey(mcUser.Email) ? localUsers[mcUser.Email] : null;
                if (localUser != null)
                {
                    message += "  User " + localUser + " ";
                    var localNotif = localNotifs.ContainsKey(localUser.Id) ? localNotifs[localUser.Id] : null;
                    bool resubscribed = false;
                    switch (mcUser.Status)
                    {
                        case MC_pending:
                            break;

                        case MC_cleaned:
                            break;

                        case MC_subscribed:
                            break;

                        case MC_unsubscribed:
                            break;

                        default:
                            message += ": invalid MC status '" + mcUser.Status + "' ";
                            context.Services.Logger.Info(logPath, ErrorLevel.ThirdParty, "User '" + mcUser.Email + "' has a unknown status in MailChimp: '" + mcUser.Status + "'");
                            this.Out.WriteLine("User '" + mcUser.Email + "' has a unknown status in MailChimp: '" + mcUser.Status + "'");
                            break;
                    }

                    if (localNotif.MailChimpStatus == MC_resubscribe)
                    {
                        message += ": has pending resubscribe";
                        this.Out.WriteLine(message);
                    }
                    else if (localNotif.MailChimpStatus != mcUser.Status || resubscribed)
                    {
                        if (context.Args.Simulation)
                        {
                            message += ": MC status changed to '" + mcUser.Status + "' (simulation)";
                            this.Out.WriteLine(message);
                        }
                        else
                        {
                            message += ": MC status changed to '" + mcUser.Status + "'";
                            statusChanges.Add(mcUser.Status);
                            localNotif.MailChimpStatus = mcUser.Status;
                            localNotif.MailChimpStatusDateUtc = DateTime.UtcNow;
                            context.Services.Notifications.Update(localNotif);
                            message += " (saved)";

                            context.Services.Logger.Info(logPath, ErrorLevel.Success, message);
                            this.Out.WriteLine(message);
                        }
                    }
                }
                else
                {
                    unknownMcUsers++;
                    context.UnknownMailchimpUsers.Add(mcUser.Email, mcUser);
                    message = "  MC entry '" + mcUser.Email + "' does not match a local user";
                    context.Services.Logger.Info(logPath, ErrorLevel.Success, message);
                    this.Out.WriteLine(message);
                }
            }

            this.Out.WriteLine();
            if (statusChanges.Count > 0)
            {
                var message = "When syncing with MailChimp, local users changed: " + Environment.NewLine + string.Join(Environment.NewLine, statusChanges.GroupBy(s => s).Select(g => g.Key + ": " + g.Count()));
                context.Services.Logger.Info(logPath, ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }
            else
            {
                var message = "When syncing with MailChimp, none of the local users changed";
                context.Services.Logger.Info(logPath, ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }

            {
                var message = "MailChimp has  " + unknownMcUsers + " users that are not in the local database";
                context.Services.Logger.Verbose(logPath, ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }

            return true;
        }

        private bool UpdateMailchimpUsers(Context context)
        {
            const string logPath = "UpdateMailchimpUsers";

            // update MC users from local
            this.Out.WriteLine();
            this.Out.WriteLine("Updating MailChimp users from locals...");
            var subscribedUsers = new List<EmailParameter>();
            var unsubscribedUsers = new List<EmailParameter>();
            int skippedBannedUsers = 0;
            var defaultNotif = context.Services.Notifications.GetDefaultNotifications();
            var list = context.MailchimpList;
            int j = 0;
            foreach (var localUser in context.LocalUsers.Values)
            {
                if (++j % 100 == 0)
                    this.Out.WriteLine("  Local user: " + j + " / " + context.LocalUsers.Count);

                var match = context.MailchimpUsers.Where(o => o.Email == localUser.Email).SingleOrDefault();
                var localNotif = context.LocalNotifs.ContainsKey(localUser.Id) ? context.LocalNotifs[localUser.Id] : context.Services.Notifications.SelectNotifications(localUser.Id);
                var company = context.LocalCompanies.ContainsKey(localUser.CompanyID) ? context.LocalCompanies[localUser.CompanyID] : null;
                var lastActivity = context.LocalLastActivities.ContainsKey(localUser.Id) ? context.LocalLastActivities[localUser.Id] : default(DateTime?);
                bool isActive = context.Services.People.IsActive(localUser);

                bool resub = localNotif.MailChimpStatus == MC_resubscribe && isActive;
                bool subscribe = resub || localNotif.MailChimp && localNotif.MailChimpStatus == null && isActive;
                bool unsubscribe = (!localNotif.MailChimp || !isActive) && localNotif.MailChimpStatus == MC_subscribed;

                if (match != null && this.IsMailchimpUserOutdated(context, match, localUser, company, lastActivity))
                {
                    var result = this.McUpdate(context, localUser, localNotif, company, lastActivity, match);
                    if (result == McUpdateFlag.SkipUser)
                    {
                        continue;
                    }
                }

                if (!subscribe && !unsubscribe)
                    continue;

                var message = "  User " + localUser + " " + (subscribe ? "S+" : "") + (subscribe ? "S-" : "") + ": ";
                if (match != null)
                {
                    if (match.Status == MC_banned)
                    {
                        message += "is banned from MC";
                        skippedBannedUsers++;
                    }
                    else if (subscribe && resub)
                    {
                        // re-sub
                        if (!context.Args.Simulation)
                        {
                            this.McSubscribe(context, subscribedUsers, localUser, localNotif, company, lastActivity, true);
                            message += "resubscribed (double opt-in)";
                        }
                        else
                        {
                            message += "resubscribed (double opt-in) (simulation)";
                        }
                    }
                    else if (subscribe && !resub)
                    {
                        message += "no action " + match.Status;
                    }
                    else if (unsubscribe && match.Status == MC_subscribed)
                    {
                        // un-sub
                        if (!context.Args.Simulation)
                        {
                            this.McUnsubscribe(context, unsubscribedUsers, localUser, localNotif);
                            message += "unsubscribed";
                        }
                        else
                        {
                            message += "unsubscribed (simulation)";
                        }
                    }
                    else
                    {
                        message += "no action";
                    }
                }
                else
                {
                    if (subscribe && !resub)
                    {
                        // first sub
                        if (!context.Args.Simulation)
                        {
                            this.McSubscribe(context, subscribedUsers, localUser, localNotif, company, lastActivity, false);
                            message += "subscribed";
                        }
                        else
                        {
                            message += "subscribed (simulation)";
                        }
                    }
                    else if (subscribe && resub)
                    {
                        var user = context.MailchimpClient.GetMemberInfo(list.Id, new List<EmailParameter>() { new EmailParameter { Email = localUser.Email, }, }).Data.SingleOrDefault();
                        if (user != null)
                        {
                            if (user.Status == MC_pending)
                            {
                                // waiting for user opt-in
                                message += "resubscribing, waiting for opt-in";
                            }
                            else
                            {
                                message += "resubscribing, unexpected state " + user.Status;
                            }

                            if (!context.Args.Simulation)
                            {
                                localNotif.MailChimpStatus = user.Status;
                                localNotif.MailChimpStatusDateUtc = DateTime.UtcNow;
                                context.Services.Notifications.Update(localNotif);
                            }
                        }
                        else
                        {
                            message += "resubscribing, unexpected state";
                        }
                    }
                    else
                    {
                        message += "no action";
                    }
                }

                context.Services.Logger.Info(logPath, ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }

            {
                this.Out.WriteLine();
                var message = "Subscribed " + subscribedUsers.Count + ", Unsubscribed " + unsubscribedUsers.Count + " into/from MailChimp";
                context.Services.Logger.Info(logPath, ErrorLevel.Success, message);
                this.Out.WriteLine(message);

                message = "There are " + skippedBannedUsers + " users in the local database that are banned from MailChimp";
                context.Services.Logger.Verbose(logPath, ErrorLevel.Success, message);
                this.Out.WriteLine(message);
            }

            return true;
        }

        private bool UpdateSegments(Context context)
        {
            // clear and update segments
            this.UpdateSegment(context,
                context.MailchimpOtherSegment,
                context.UnknownMailchimpUsers.Select(x => new UserContext(x.Value)).ToList());
            this.UpdateSegment(context,
                context.MailchimpActiveSegment,
                context.LocalUsers.Where(u => context.Services.People.IsActive(u.Value)).Select(x => new UserContext(x.Value)).ToList());
            this.UpdateSegment(context,
                context.MailchimpInactiveSegment,
                context.LocalUsers.Where(u => !context.Services.People.IsActive(u.Value)).Select(x => new UserContext(x.Value)).ToList());

            var subscribedModels = context.Services.Subscriptions.GetActiveUserSubscriptions();
            var subscribed = subscribedModels.Where(x => x.AppliesToUserId != null).Select(x => x.AppliesToUserId.Value).ToArray();

            this.UpdateSegment(context,
                context.MailchimpActiveSubscribedSegment,
                context.LocalUsers.Where(u => context.Services.People.IsActive(u.Value) && subscribed.Contains(u.Value.Id)).Select(x => new UserContext(x.Value)).ToList());
            this.UpdateSegment(context,
                context.MailchimpActiveUnsubscribedSegment,
                context.LocalUsers.Where(u => context.Services.People.IsActive(u.Value) && !subscribed.Contains(u.Value.Id)).Select(x => new UserContext(x.Value)).ToList());

            return true;
        }

        private bool UpdateSegment(Context context, SegmentContext segment, IList<UserContext> users)
        {
            this.Out.WriteLine("Updating MailChimp segment '" + segment.MailchimpSegment.SegmentName + "'...");

            var list = context.MailchimpList;
            var mc = context.MailchimpClient;

            var emails = users.Select(u => new EmailParameter { Email = u.Email, }).ToList();

            var resetResult = mc.ResetStaticSegment(list.Id, segment.MailchimpSegment.StaticSegmentId);

            string message;
            if (users.Count > 0)
            {
                var updateResult = mc.AddStaticSegmentMembers(
                    list.Id,
                    segment.MailchimpSegment.StaticSegmentId,
                    emails);
                message = "MailChimp segment '" + segment.MailchimpSegment.SegmentName + "' re-populated with " + emails.Count + "";
                context.Services.Logger.Info("UpdateSegment", ErrorLevel.Success, message);
            }
            else
            {
                message = "MailChimp segment '" + segment.MailchimpSegment.SegmentName + "' re-populated with " + emails.Count + "";
                context.Services.Logger.Info("UpdateSegment", ErrorLevel.Success, message);
            }

            this.Out.WriteLine(message);
            return true;
        }

        private bool EndWithError(Context context, ErrorLevel errorLevel, string message, bool showUsage = false)
        {
            this.Out.WriteLine(message);
            context.Args.SysLogger.Info("Run", remoteClient, Environment.UserName, errorLevel, message);

            if (showUsage)
            {
                this.Out.WriteLine("  ");
                this.Out.WriteLine("Usage: sparkle [-arg[=value]]* RecallMail [--arg[=value]]*");
                this.Out.WriteLine("  ");
                this.Out.WriteLine("  User selection");
                this.Out.WriteLine("    --invited:        invited users");
                this.Out.WriteLine("    --emailconfirm:   users with email address unconfirmed");
                this.Out.WriteLine("    --lockedout:      users without valid password");
                this.Out.WriteLine("    --user=<login>:   specified user (login or email)");
                this.Out.WriteLine("  ");
                this.Out.WriteLine("  Emails");
                this.Out.WriteLine("    --message=<file>: MD file containing a custom message for emails");
                this.Out.WriteLine("    --subject=<msg>:  custom email subject");
                this.Out.WriteLine("  ");
                this.Out.WriteLine("  Execution");
                this.Out.WriteLine("    --show:           display all elements before execution");
                this.Out.WriteLine("    --noconfirm:      executes without confirmation");
                this.Out.WriteLine("    --delay=<secs>:   add a delay between emails (in seconds)");
                this.Out.WriteLine("    --action=<opt>:   forces an action (passwordreset)");
                this.Out.WriteLine("  ");
                this.Out.WriteLine("");
            }

            return false;
        }

        private void McUnsubscribe(Context context, List<EmailParameter> unsubscribedUsers, Entities.Networks.User localUser, Entities.Networks.Notification localNotif)
        {
            const string logPath = "McUnsubscribe";
            MailChimpManager mc = context.MailchimpClient;
            ListInfo list = context.MailchimpList;

            try
            {
                var email = new EmailParameter { Email = localUser.Email, };
                var result = mc.Unsubscribe(list.Id, email, deleteMember: false, sendGoodbye: false, sendNotify: false);
                unsubscribedUsers.Add(email);
            }
            catch (MailChimpAPIException ex)
            {
                var error = ex.MailChimpAPIError;
                if (error != null)
                {
                    var code = error.Code;
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get unsubscribed to MailChimp with code '" + code + "' (" + error.Error + ")";
                        context.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        throw;
                    }
                }
                else
                {
                    var message = "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get unsubscribed to MailChimp (" + ex.GetType().Name + ")";
                    context.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, message);
                    this.Error.WriteLine(message);
                    throw;
                }
            }
        }

        private void McSubscribe(Context context, List<EmailParameter> subscribedUsers, Entities.Networks.User localUser, Entities.Networks.Notification localNotif, Sparkle.Entities.Networks.Neutral.CompanyPoco company, DateTime? lastActivity, bool doubleOptIn)
        {
            const string logPath = "McSubscribe";
            MailChimpManager mc = context.MailchimpClient;
            ListInfo list = context.MailchimpList;

            try
            {
                var email = new EmailParameter { Email = localUser.Email, };
                var vars = GetMergeVars(localUser, company, lastActivity);
                var result = mc.Subscribe(list.Id, email, mergeVars: vars, updateExisting: false, doubleOptIn: doubleOptIn);
                if (result != null)
                {
                    subscribedUsers.Add(result);
                }
            }
            catch (MailChimpAPIException ex)
            {
                var error = ex.MailChimpAPIError;
                if (error != null)
                {
                    var code = error.Code;
                    if (code == "220") ////&& error.Error != null && error.Error.EndsWith("has been banned"))
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' was banned from MailChimp (220)";
                        context.Services.Logger.Warning(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        context.Services.Notifications.UpdateMailChimpStatus(localNotif, MC_banned, DateTime.UtcNow);
                    }
                    else if (code == "214") // User with email '' failed to get subscribed to MailChimp with code '214' ('' is already subscribed to the list.)
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' is already subscribed (214)";
                        context.Services.Logger.Warning(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        context.Services.Notifications.UpdateMailChimpStatus(localNotif, MC_banned, DateTime.UtcNow);
                    }
                    else
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get subscribed to MailChimp with code '" + code + "' (" + error.Error + ")";
                        context.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        throw;
                    }
                }
                else
                {
                    var message = "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get subscribed to MailChimp (" + ex.GetType().Name + ")";
                    context.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get subscribed to MailChimp (" + ex.GetType().Name + ")");
                    this.Error.WriteLine(message);
                    throw;
                }
            }
        }

        private static MyMergeVar GetMergeVars(Entities.Networks.User localUser, Sparkle.Entities.Networks.Neutral.CompanyPoco company, DateTime? lastActivity)
        {
            // TODO: Language should be set from user profile and network configuration
            var vars = new MyMergeVar
            {
                FirstName = localUser.FirstName,
                Language = "fr",
                LastName = localUser.LastName,
                Company = company != null ? company.Name : null,
                City = localUser.UserProfileFields.City(),
                DateRegistered = localUser.CreatedDateUtc,
                LastActivity = lastActivity,
                Zip = localUser.UserProfileFields.ZipCode(),
            };
            return vars;
        }

        private McUpdateFlag McUpdate(Context context, Entities.Networks.User localUser, Entities.Networks.Notification localNotif, Entities.Networks.Neutral.CompanyPoco company, DateTime? lastActivity, MemberInfo listMember)
        {
            const string logPath = "McUpdate";
            var mc = context.MailchimpClient;
            var listId = context.MailchimpList.Id;

            var vars = GetMergeVars(localUser, company, lastActivity);
            
            try
            {
                var result = mc.UpdateMember(context.MailchimpList.Id, GetEmailParameter(listMember), vars);
                return McUpdateFlag.None;
            }
            catch (MailChimpAPIException ex)
            {
                var error = ex.MailChimpAPIError;
                if (error != null)
                {
                    var code = error.Code;
                    if (code == "220") // has been banned
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' was banned from MailChimp (220)";
                        context.Services.Logger.Warning(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        context.Services.Notifications.UpdateMailChimpStatus(localNotif, MC_banned, DateTime.UtcNow);
                        return McUpdateFlag.SkipUser;
                    }
                    else if (code == "215") // The email address "..." does not belong to this list
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' does not belong to the MailChimp list (215)";
                        context.Services.Logger.Warning(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        context.Services.Notifications.UpdateMailChimpStatus(localNotif, MC_banned, DateTime.UtcNow);
                        return McUpdateFlag.SkipUser;
                    }
                    else
                    {
                        var message = "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get updated to MailChimp with code '" + code + "' (" + error.Error + ")";
                        context.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, message);
                        this.Error.WriteLine(message);
                        throw;
                    }
                }
                else
                {
                    var message = "User " + localUser.Id + " with email '" + localUser.Email + "' failed to get updated to MailChimp (" + ex.GetType().Name + ")";
                    context.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, message);
                    this.Error.WriteLine(message);
                    throw;
                }
            }
        }

        private EmailParameter GetEmailParameter(Entities.Networks.User localUser)
        {
            return new EmailParameter
            {
                Email = localUser.Email,
            };
        }

        private EmailParameter GetEmailParameter(MemberInfo listMember)
        {
            return new EmailParameter
            {
                Email = listMember.Email,
            };
        }

        private bool IsMailchimpUserOutdated(Context context, MemberInfo match, Entities.Networks.User localUser, Entities.Networks.Neutral.CompanyPoco company, DateTime? lastActivity)
        {
            if (match.InfoChanged == null)
                return false;

            if (localUser.PersonalDataUpdateDateUtc == null)
                return false;

            DateTime dateMcChanged = match.InfoChanged.Value; // 2014-07-16 14:27:44

            var diff = localUser.PersonalDataUpdateDateUtc.Value - dateMcChanged;
            return diff > TimeSpan.FromHours(20);
        }

        [System.Runtime.Serialization.DataContract]
        public class MyMergeVar : MergeVar
        {
            [System.Runtime.Serialization.DataMember(Name = "FNAME")]
            public string FirstName { get; set; }

            [System.Runtime.Serialization.DataMember(Name = "LNAME")]
            public string LastName { get; set; }

            [System.Runtime.Serialization.DataMember(Name = "COMPANY")]
            public string Company { get; set; }

            [System.Runtime.Serialization.DataMember(Name = "DATEREG")]
            public DateTime? DateRegistered { get; set; }

            [System.Runtime.Serialization.DataMember(Name = "DATEACT")]
            public DateTime? LastActivity { get; set; }

            [System.Runtime.Serialization.DataMember(Name = "CITY")]
            public string City { get; set; }

            [System.Runtime.Serialization.DataMember(Name = "ZIP")]
            public string Zip { get; set; }

            ////[System.Runtime.Serialization.DataMember(Name = "GENDER")]
            ////public string Gender { get; set; }
        }

        class Mutable<T>
        {
            public T Object { get; set; }
        }

        public class Context
        {
            public Context()
            {
                this.UnknownMailchimpUsers = new Dictionary<string, MemberInfo>();
                this.MailchimpOtherSegment = new SegmentContext("Other");
                this.MailchimpActiveSegment = new SegmentContext("Active");
                this.MailchimpInactiveSegment = new SegmentContext("Inactive");
                this.MailchimpActiveSubscribedSegment = new SegmentContext("ActiveSubscribed");
                this.MailchimpActiveUnsubscribedSegment = new SegmentContext("ActiveUnsubscribed");
                this.MergeVars = new MergeVars();
            }

            public SparkleCommandArgs Args { get; set; }

            public IServiceFactory Services { get; set; }

            public MailChimpManager MailchimpClient { get; set; }

            public ListInfo MailchimpList { get; set; }

            public SegmentContext MailchimpOtherSegment { get; set; }

            public SegmentContext MailchimpActiveSegment { get; set; }

            public SegmentContext MailchimpInactiveSegment { get; set; }

            public SegmentContext MailchimpActiveSubscribedSegment { get; set; }

            public SegmentContext MailchimpActiveUnsubscribedSegment { get; set; }

            public StaticSegmentResult[] MailchimpAllSegments { get; set; }

            public Dictionary<string, Entities.Networks.User> LocalUsers { get; set; }

            public List<MemberInfo> MailchimpUsers { get; set; }

            public Dictionary<int, Entities.Networks.Notification> LocalNotifs { get; set; }

            public Dictionary<int, Entities.Networks.Neutral.CompanyPoco> LocalCompanies { get; set; }

            public Dictionary<string, MemberInfo> UnknownMailchimpUsers { get; set; }

            public MergeVars MergeVars { get; set; }

            public Dictionary<int, DateTime> LocalLastActivities { get; set; }
        }

        public class MergeVars
        {
            public MergeVarContext Company
            {
                get
                {
                    return new MergeVarContext(new MergeVarOptions
                    {
                        Tag = "COMPANY",
                        Name = "Company",
                    });
                }
            }

            public MergeVarContext DateRegistered
            {
                get
                {
                    return new MergeVarContext(new MergeVarOptions
                    {
                        Tag = "DATEREG",
                        Name = "Date registered",
                        FieldType = "date",
                        DateFormat = "DD/MM/YYYY",
                    });
                }
            }

            public MergeVarContext LastActivity
            {
                get
                {
                    return new MergeVarContext(new MergeVarOptions
                    {
                        Tag = "DATEACT",
                        Name = "Last activity",
                        FieldType = "date",
                        DateFormat = "DD/MM/YYYY",
                    });
                }
            }

            public MergeVarContext City
            {
                get
                {
                    return new MergeVarContext(new MergeVarOptions
                    {
                        Tag = "CITY",
                        Name = "City",
                    });
                }
            }

            public MergeVarContext Zip
            {
                get
                {
                    return new MergeVarContext(new MergeVarOptions
                    {
                        Tag = "ZIP",
                        Name = "Zip",
                        FieldType = "zip",
                    });
                }
            }

            public IEnumerable<MergeVarContext> All
            {
                get
                {
                    yield return this.Company;
                    yield return this.DateRegistered;
                    yield return this.LastActivity;
                    yield return this.City;
                    yield return this.Zip;
                }
            }
        }

        public class MergeVarContext
        {
            public MergeVarContext(MergeVarOptions options)
            {
                this.Options = options;
            }

            public MergeVarOptions Options { get; set; }

            public MergeVarItemResult Definition { get; set; }
        }

        public class SegmentContext
        {
            public SegmentContext(string type)
            {
                this.Type = type;
            }

            public string Type { get; set; }
            public StaticSegmentResult MailchimpSegment { get; set; }
            public Dictionary<string, MemberInfo> MailchimpUsers { get; set; }
        }

        public class UserContext
        {
            public UserContext(MemberInfo item)
            {
                this.Email = item.Email;
            }

            public UserContext(Entities.Networks.User item)
            {
                this.Email = item.Email;
            }

            public string Email { get; set; }
        }

        public enum McUpdateFlag
        {
            None = 0,
            SkipUser,
        }
    }
}
