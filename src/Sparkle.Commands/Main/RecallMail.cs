
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.UI;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class RecallMail : BaseSparkleCommand
    {
        private string remoteClient;

        public override void RunUniverse(SparkleCommandArgs args)
        {
            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "RecallMail";
            services.HostingEnvironment.RemoteClient = this.remoteClient;

            var context = new Context
            {
                Args = args,
                Services = services,
            };

            if (!this.Run(args, context))
            {
                services.Logger.Error("RecallMail failed", ErrorLevel.Business);
            }

        }

        private bool Run(SparkleCommandArgs args, Context context)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return false;
            }

            // network
            var services = context.Services;
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("RecallMail", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return false;
            }

            services.NetworkId = network.Id;

            return   ! this.ParseArgs(context)
                   ||! this.CollectElements(context)
                   ||! this.Confirm(context)
                   ||  this.Execute(context);
        }

        private bool ParseArgs(Context context)
        {
            const string showParam = "show", invitedParam = "invited", emainConfirmParam = "emailconfirm",
                noConfirmParam = "NoConfirm", lockedoutParam = "lockedout", messageFileParam = "message",
                subjectParam = "subject", delayParam = "delay", userParam = "user", actionParam = "action";

            foreach (var arg in context.Args.Arguments)
            {
                if (arg.ToLowerInvariant() == "RecallMail".ToLowerInvariant())
                {
                    // ignore command name
                }
                else if (arg.StartsWith("--"))
                {
                    var eqIndex = arg.IndexOf('=');
                    var carg = arg.Substring(2, eqIndex > 0 ? eqIndex - 2 : arg.Length - 2);
                    var carglo = carg.ToLowerInvariant();
                    var cargValue = eqIndex > 0 ? arg.Substring(eqIndex + 1) : null;
                    var cargValueLo = cargValue != null ? cargValue.ToLowerInvariant() : null;

                    if (carglo == showParam.ToLowerInvariant())
                    {
                        context.Show = true;
                    }
                    else if (carglo == lockedoutParam.ToLowerInvariant())
                    {
                        context.CollectLockedout = true;
                    }
                    else if (carglo == invitedParam.ToLowerInvariant())
                    {
                        context.CollectInvited = true;
                    }
                    else if (carglo == emainConfirmParam.ToLowerInvariant())
                    {
                        context.CollectEmailNotConfirmed = true;
                    }
                    else if (carglo == noConfirmParam.ToLowerInvariant())
                    {
                        context.NoConfirm = true;
                    }
                    else if (carglo == messageFileParam.ToLowerInvariant())
                    {
                        if (!File.Exists(cargValue))
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "No such message file found '" + cargValue + "'.");
                        }

                        using (var fileStream = new FileStream(cargValue, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var reader = new StreamReader(fileStream);
                            context.Message = reader.ReadToEnd();
                        }
                    }
                    else if (carglo == subjectParam.ToLowerInvariant())
                    {
                        context.Subject = cargValue;
                    }
                    else if (carglo == delayParam.ToLowerInvariant())
                    {
                        int delay;
                        if (!int.TryParse(cargValue, out delay))
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Delay value is not a number.", true);
                        }

                        context.Delay = delay;
                    }
                    else if (carglo == userParam.ToLowerInvariant())
                    {
                        var user = context.Services.People.SelectWithLogin(cargValue)
                                ?? context.Services.People.SelectWithProMail(cargValue);
                        if (user == null)
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "No such user '" + cargValue + "'.", true);
                        }

                        var profileFields = context.Services.ProfileFields.GetUserProfileFieldsByUserId(user.Id);
                        context.UserRecalls.Add(user.Id, new RecallModel(new UserModel(user, profileFields)));
                    }
                    else if (carglo == actionParam.ToLowerInvariant())
                    {
                        var actions = new string[] { "passwordreset", };

                        if (!actions.Contains(cargValueLo))
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "No such action '" + cargValue + "'. Valid values: " + string.Join(", ", actions) + ".", true);
                        }

                        context.Action = cargValueLo;
                    }
                    else if (!carg.StartsWith("--"))
                    {
                        return this.EndWithError(context, ErrorLevel.Input, "Invalid parameter '" + arg + "'.", true);
                    }
                }
                else
                {
                    return this.EndWithError(context, ErrorLevel.Input, "File not found '" + arg + "'.");
                }
            }

            return true;
        }

        private bool CollectElements(Context context)
        {
            // registerrequests
            //
            // actions are pending for the company administrators
            // there is no email to send
            // or maybe later; make some kind of a remainder for all admins

            // pending company validation
            //
            // actions are pending for the company administrators
            // there is no email to send
            // or maybe later; make some kind of a remainder for all admins

            // invited
            if (context.CollectInvited)
            {
                context.CollectedInvited = context.Services.Invited.GetPending();
            }

            // IsEmailConfirmed = false
            if (context.CollectEmailNotConfirmed)
            {
                context.CollectedEmailNotConfirmed = context.Services.People.GetPendingEmailAddressConfirmation();
            }

            ////if (context.CollectLockedout)
            {
                context.CollectedLockedout = context.Services.People.GetMembershipLockedOutUsers();
            }

            // merge invited
            if (context.CollectedInvited != null)
            {
                context.InvitedRecalls.AddRange<Invited, int, RecallModel>(context.CollectedInvited, i => i.Id, i => new RecallModel(i)); 
            }

            // merge users
            if (context.CollectedEmailNotConfirmed != null)
            {
                context.UserRecalls.AddRange(context.CollectedEmailNotConfirmed, u => u.Id, u => new RecallModel(u) { IsEmailUnconfirmed = true, }); 
            }

            if (context.CollectLockedout)
            {
                context.UserRecalls.Merge(
                    context.CollectedLockedout,
                    u => u.Id,
                    u => new RecallModel(u) { IsLockedOut = true, },
                    (key, oldValue, newValue) =>
                    {
                        oldValue.IsLockedOut = true;
                        return oldValue;
                    });
            }
            else
            {
                foreach (var item in context.UserRecalls)
                {
                    var locked = context.CollectedLockedout.SingleOrDefault(u => u.Id == item.Key);
                    if (locked != null)
                    {
                        item.Value.IsLockedOut = locked.IsLockedOut;
                    }
                }
            }

            return true;
        }

        private bool Confirm(Context context)
        {
            // show summary
            var c = new string[5];
            c[1] = " ";
            c[2] = " "; 
            c[3] = context.CollectInvited ? "Y" : " ";
            c[4] = context.CollectEmailNotConfirmed ? "Y" : " ";
            var ct = c.Count(x => x == "Y");

            var l = new int[5];
            l[1] = 0;
            l[2] = 0;
            l[3] = context.CollectedInvited != null ? context.CollectedInvited.Count : 0;
            l[4] = context.CollectedEmailNotConfirmed != null ? context.CollectedEmailNotConfirmed.Count : 0;
            var ll = l.Select(x => x.ToString().PadLeft(5)).ToArray();
            var lt = l.Sum().ToString().PadLeft(5);

            this.Out.WriteLine("");
            this.Out.WriteLine("+-----------------------+------+-------+");
            this.Out.WriteLine("| Kind                  | Send | Count |");
            this.Out.WriteLine("+-----------------------+------+-------+");
            this.Out.WriteLine("| Register requests     | " + c[1] + "    | " + ll[1] + " |");
            this.Out.WriteLine("| Pending C. validation | " + c[2] + "    | " + ll[2] + " |");
            this.Out.WriteLine("| Invited               | " + c[3] + "    | " + ll[3] + " |");
            this.Out.WriteLine("| Email addr not conf.  | " + c[4] + "    | " + ll[4] + " |");
            this.Out.WriteLine("+-----------------------+------+-------+");
            this.Out.WriteLine("| Total                 | " + ct + "    | " + lt + " |");
            this.Out.WriteLine("+-----------------------+------+-------+");
            this.Out.WriteLine("");

            if (context.Show)
            {
                this.Out.WriteLine("");
                this.Out.WriteLine("Register requests");
                this.Out.WriteLine("================================");
                this.Out.WriteLine("");
                this.Out.WriteLine("Not supported.");
                this.Out.WriteLine("");
                this.Out.WriteLine("");
                this.Out.WriteLine("Pending C. validation");
                this.Out.WriteLine("================================");
                this.Out.WriteLine("");
                this.Out.WriteLine("Not supported.");
                this.Out.WriteLine("");
                this.Out.WriteLine("");
                this.Out.WriteLine("Invited");
                this.Out.WriteLine("================================");
                this.Out.WriteLine("");
                if (context.CollectedInvited != null && context.CollectedInvited.Count == 0)
                {
                    int i = 0;
                    foreach (var item in context.CollectedInvited)
                    {
                        this.Out.Write((i++ + ".").PadRight(6));
                        this.Out.WriteLine(item.Email);
                    }
                }
                else
                {
                    this.Out.WriteLine("No entries.");
                }

                this.Out.WriteLine("");
                this.Out.WriteLine("");
                this.Out.WriteLine("Email addr not conf.");
                this.Out.WriteLine("================================");
                this.Out.WriteLine("");
                if (context.CollectedEmailNotConfirmed != null && context.CollectedEmailNotConfirmed.Count == 0)
                {
                    int i = 0;
                    foreach (var item in context.CollectedEmailNotConfirmed)
                    {
                        this.Out.Write((i++ + ".").PadRight(6));
                        this.Out.WriteLine(item.Email);
                    }
                }
                else
                {
                    this.Out.WriteLine("No entries.");
                }

                this.Out.WriteLine("");

            }

            this.Out.WriteLine("");
            this.Out.WriteLine("Total users:                        " + context.UserRecalls.Count);
            this.Out.WriteLine("Users with email addr. unconfirmed: " + context.UserRecalls.Count(u => u.Value.IsEmailUnconfirmed));
            this.Out.WriteLine("Locked-out users:                   " + context.UserRecalls.Count(u => u.Value.IsLockedOut));
            this.Out.WriteLine("");
            this.Out.WriteLine("Total invited:                      " + context.InvitedRecalls.Count);
            this.Out.WriteLine("");
            this.Out.WriteLine("Total register requests:            " + context.RegisterRequestRecalls.Count);
            this.Out.WriteLine("");
            this.Out.WriteLine("");
            this.Out.WriteLine("Send delay:                         " + (context.Delay != null ? TimeSpan.FromSeconds(context.Delay.Value).ToString() : "none"));


            int items = context.UserRecalls.Count + context.InvitedRecalls.Count + context.RegisterRequestRecalls.Count;
            int sendTime = 500;
            if (context.Delay != null)
                this.Out.WriteLine("Estimated send duration:            " + TimeSpan.FromMilliseconds(items * (sendTime + context.Delay.Value * 1000)));
            else
                this.Out.WriteLine("Estimated send duration:            " + TimeSpan.FromMilliseconds(items * sendTime));

            if (context.Args.Simulation)
            {
                this.Out.WriteLine("");
                this.Out.WriteLine("Simulation:                         YES");
            }

            this.Out.WriteLine("");
            this.Out.WriteLine("Message subject:                        " + (context.Subject != null ? context.Subject : "<default>"));
            this.Out.WriteLine("");
            this.Out.WriteLine("Message content: " + (context.Message != null ? "" : "<default>"));
            if (context.Message != null)
                this.Out.WriteLine(context.Message);
            this.Out.WriteLine("");

            if (!context.NoConfirm)
            {
                if (!this.AskConfirmation("Confirm send?"))
                {
                    this.Out.WriteLine("User aborted.");
                    return false;
                }
            }

            return true;
        }

        private bool Execute(Context context)
        {
            var userRecallsQuery = context.UserRecalls
                .OrderBy(u => u.Value.User.CompanyId);
            foreach (var item in userRecallsQuery)
            {
                this.Out.Write("User ");
                this.Out.Write(item.Value.User.Email.OriginalString.TrimTextRight(20).PadRight(20));
                this.Out.Write(": ");
                if (item.Value.IsLockedOut || context.Action == "passwordreset")
                {
                    if (context.Args.Simulation)
                    {
                        this.Out.WriteLine("sending password reset email (simulation)");
                    }
                    else
                    {
                        this.Out.Write("sending password reset email: ");
                        var result = context.Services.People.SendPasswordRecoveryEmail(item.Key, message: context.Message, subject: context.Subject);
                        if (result.Succeed)
                        {
                            this.Out.WriteLine("succeed");
                        }
                        else
                        {
                            this.Out.WriteLine("failed (" + result.Errors.First().DisplayMessage + ")");
                        }
                    }
                }
                else if (item.Value.IsEmailUnconfirmed)
                {
                    if (context.Args.Simulation)
                    {
                        this.Out.Write("sending activation email (simulation)");
                    }
                    else
                    {
                        this.Out.Write("sending activation email");
                        var result = context.Services.People.SendActivationEmail(item.Value.User.Email.OriginalString, context.Message);
                        if (result.Succeed)
                        {
                            this.Out.WriteLine("succeed");
                        }
                        else
                        {
                            this.Out.WriteLine("failed (" + result.Errors.First().Code + ")");
                        }
                    }
                }
                else
                {
                    if (context.Args.Simulation)
                    {
                        this.Out.Write("sending welcome email (simulation)");
                    }
                    else
                    {
                        this.Out.Write("sending welcome email: ");
                        context.Services.Email.SendRegistred(item.Value.User, context.Message);
                        this.Out.WriteLine("may have succeeded");
                    }
                }

                if (context.Delay != null)
                    System.Threading.Thread.Sleep(context.Delay.Value * 1000);
            }

            foreach (var item in context.InvitedRecalls)
            {
                throw new NotImplementedException("Recalls for invited are not supported");
            }

            foreach (var item in context.RegisterRequestRecalls)
            {
                throw new NotImplementedException("Recalls for register requests are not supported");
            }

            return true;
        }

        private bool EndWithError(Context context, ErrorLevel errorLevel, string message, bool showUsage = false)
        {
            this.Out.WriteLine(message);
            context.Args.SysLogger.Info("RecallMail", remoteClient, Environment.UserName, errorLevel, message);

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

        private bool AskConfirmation(string message)
        {
            var yesChars = new char[] { 'y', 'Y', };
            var noChars = new char[] { 'n', 'N', };
            for (;;)
            {
                this.Out.WriteLine(message);
                char[] inputBuffer = new char[1];
                //this.In.Read(inputBuffer, 0, 1);
                inputBuffer[0] = (char)this.In.Read();
                if (yesChars.Contains(inputBuffer[0]))
                {
                    return true;
                }

                if (noChars.Contains(inputBuffer[0]))
                {
                    return false;
                }
            }
        }

        public class Context
        {
            public Context()
            {
                this.UserRecalls = new Dictionary<int, RecallModel>();
                this.InvitedRecalls = new Dictionary<int, RecallModel>();
                this.RegisterRequestRecalls = new Dictionary<int, RecallModel>();
            }

            public IServiceFactory Services { get; set; }

            public SparkleCommandArgs Args { get; set; }

            public bool Show { get; set; }

            public bool CollectInvited { get; set; }
            public IList<Invited> CollectedInvited { get; set; }

            public bool CollectEmailNotConfirmed { get; set; }
            public IList<UserModel> CollectedEmailNotConfirmed { get; set; }

            public bool NoConfirm { get; set; }

            public bool CollectLockedout { get; set; }
            public IList<UserModel> CollectedLockedout { get; set; }

            public IDictionary<int, RecallModel> UserRecalls { get; set; }
            public IDictionary<int, RecallModel> InvitedRecalls { get; set; }
            public IDictionary<int, RecallModel> RegisterRequestRecalls { get; set; }

            public string Message { get; set; }

            public string Subject { get; set; }

            public int? Delay { get; set; }

            public string Action { get; set; }
        }

        public class RecallModel
        {
            public RecallModel(Sparkle.Entities.Networks.Invited item)
            {
                this.Id = item.Id;
                this.IsInvited = true;
                this.Invited = item;
            }

            public RecallModel(UserModel item)
            {
                this.Id = item.Id;
                this.User = item;
                if (item.IsEmailConfirmed != null)
                    this.IsEmailUnconfirmed = !item.IsEmailConfirmed.Value;
            }

            public int Id { get; set; }
            public bool IsLockedOut { get; set; }
            public bool IsEmailUnconfirmed { get; set; }
            public bool IsInvited { get; set; }

            public UserModel User { get; set; }
            public RegisterRequest RegisterRequest { get; set; }
            public Invited Invited { get; set; }
        }
    }
}
