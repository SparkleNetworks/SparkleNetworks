
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.PartnerResources;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Normalized stored phone numbers in the database. 
    /// </summary>
    public class UpdatePhoneNumbers : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private readonly Regex regexCaptureBracket = new Regex(@"\(.*\)");
        private readonly Regex regexMultipleNumber = new Regex(@"^(\+?[0-9\.\-]+)/\+?[0-9\.\-]+$");

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null && !args.Arguments.Any(o => o == "--test"))
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;

            var context = new Context
            {
                Args = args,
            };
            if (!this.Run(context))
            {
                context.Services.Logger.Error("UpdatePhoneNumbers failed", ErrorLevel.Business);
            }
        }

        private bool Run(Context context)
        {
            return !this.ParseArgs(context)
                || !(context.TestMode ? !this.DoTests(context) : true)
                || !this.InitializeServices(context)
                || !this.InitializeNetwork(context)
                || this.DoUpdateDatabase(context);
        }

        private bool ParseArgs(Context context)
        {
            const string testMode = "--test", clubs = "--clubs", companies = "--companies", companyRequests = "--companyrequests",
                         createNetworkRequests = "--createnetworkrequests", eventPublicMembers = "--eventpublicmembers", places = "--places",
                         requestsforproposal = "--requestsforproposal", resumes = "--resumes",
                         partnerresourceprofilefields = "--partnerresourceprofilefields", userprofilefields = "--userprofilefields", companyfrofilefields = "--companyprofilefields";

            foreach (var arg in context.Args.Arguments)
            {
                if (arg.ToLowerInvariant() == "UpdatePhoneNumbers".ToLowerInvariant())
                {
                    continue;
                }
                else if (arg.StartsWith("--"))
                {
                    if (arg.ToLowerInvariant() == testMode)
                    {
                        context.TestMode = true;
                    }
                    else if (arg.ToLowerInvariant() == clubs)
                    {
                        context.All = false;
                        context.Clubs = true;
                    }
                    else if (arg.ToLowerInvariant() == companies)
                    {
                        context.All = false;
                        context.Companies = true;
                    }
                    else if (arg.ToLowerInvariant() == companyRequests)
                    {
                        context.All = false;
                        context.CompanyRequests = true;
                    }
                    else if (arg.ToLowerInvariant() == createNetworkRequests)
                    {
                        context.All = false;
                        context.CreateNetworkRequests = true;
                    }
                    else if (arg.ToLowerInvariant() == eventPublicMembers)
                    {
                        context.All = false;
                        context.EventPublicMembers = true;
                    }
                    else if (arg.ToLowerInvariant() == places)
                    {
                        context.All = false;
                        context.Places = true;
                    }
                    else if (arg.ToLowerInvariant() == requestsforproposal)
                    {
                        context.All = false;
                        context.RequestsForProposal = true;
                    }
                    else if (arg.ToLowerInvariant() == resumes)
                    {
                        context.All = false;
                        context.Resumes = true;
                    }
                    else if (arg.ToLowerInvariant() == partnerresourceprofilefields)
                    {
                        context.All = false;
                        context.PartnerResourceProfileFields = true;
                    }
                    else if (arg.ToLowerInvariant() == userprofilefields)
                    {
                        context.All = false;
                        context.UserProfileFields = true;
                    }
                    else if (arg.ToLowerInvariant() == companyfrofilefields)
                    {
                        context.All = false;
                        context.CompanyProfileFields = true;
                    }
                }
            }

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

            return true;
        }

        private string TransformPhoneNumber(Context context, string number)
        {
            string[] separators = new string[] { ".", "/", "-", };
            var result = number;

            // remove spaces & brackets
            result = result.RemoveSpaces();
            result = regexCaptureBracket.Replace(result, "");

            // if there are mutiple number, take the first
            var multipleMatch = regexMultipleNumber.Match(result);
            if (multipleMatch.Groups.Count > 1 && multipleMatch.Groups[1].Success)
                result = multipleMatch.Groups[1].Value;

            // remove separators
            foreach (var sep in separators)
            {
                result = result.Replace(sep, "");
            }

            // already international?
            if (result.StartsWith("+"))
                return result;

            // international the other way (0033)?
            if (result.StartsWith("0033"))
            {
                result = result.Remove(0, 4);
                result = "+33" + result;
            }
            else if (result.StartsWith("33"))
            {
                result = "+" + result;
            }
            else
            {
                // number is local
                if (result.StartsWith("0"))
                {
                    result = result.Remove(0, 1);
                    result = "+33" + result;
                }
                else
                {
                    if (!context.TestMode)
                        return null;
                }
            }

            return result;
        }

        private bool DoTests(Context context)
        {
            string[] phoneTests = @"
+33 123456789
02 53 55 11 92
0637574867
05-56-79-29-74
33651059299
+33647557259
0627509524
09.50.56.00.57
+33 6 30 56 47 46
0 361 760 792 / 0 686 574 512
03 20 00 80 37 (fixe d'Ineat Conseil)
0547 50 01 31
05 57 77 59 60 / 01 42 65 27 16
05/56/33/29/05
+ 33 (0) 6 88 89 28 03
+33(0)613625204
05 5 87 72 21
0033 6 68 60 02 99
 +33 (0)3 55 87 01 96
+1 917 615 2673
".Split(new char[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);

            this.Out.WriteLine("Entering test mode...");
            this.Out.WriteLine();
            foreach (var item in phoneTests)
            {
                this.Out.WriteLine("Value:  " + item);
                this.Out.WriteLine("Result: " + this.TransformPhoneNumber(context, item));
                this.Out.WriteLine();
            }

            return true;
        }

        private bool DoUpdateDatabase(Context context)
        {
            this.Out.WriteLine("Entering database update mode..");
            this.Out.WriteLine();

            if (context.All || context.Clubs)
            {
                int done = 0;
                var clubs = context.Services.Repositories.Clubs.GetAll(context.Services.NetworkId).Where(o => !string.IsNullOrEmpty(o.Phone)).ToList();
                this.Out.WriteLine("Found " + clubs.Count + " clubs to update..");
                foreach (var item in clubs)
                {
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        var result = this.TransformPhoneNumber(context, item.Phone);
                        if (result == null)
                        {
                            this.Out.WriteLine("Club " + item.Id + " has an unrecognized phone number format: " + item.Phone);
                        }
                        else
                        {
                            done++;
                            item.Phone = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.Clubs.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " clubs updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.CompanyRequests)
            {
                int done = 0;
                var companyRequests = context.Services.Repositories.CompanyRequests.GetAll(context.Services.NetworkId).Where(o => !string.IsNullOrEmpty(o.Phone)).ToList();
                this.Out.WriteLine("Found " + companyRequests.Count + " companyrequests to update..");
                foreach (var item in companyRequests)
                {
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        var result = this.TransformPhoneNumber(context, item.Phone);
                        if (result == null)
                        {
                            this.Out.WriteLine("Companyrequest " + item.Id + " has an unrecognized phone number format: " + item.Phone);
                        }
                        else
                        {
                            done++;
                            item.Phone = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.CompanyRequests.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " companyrequests updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.CreateNetworkRequests)
            {
                int done = 0;
                var createNetworkRequests = context.Services.Repositories.CreateNetworkRequests.GetAll(context.Services.NetworkId).Where(o => !string.IsNullOrEmpty(o.ContactPhone)).ToList();
                this.Out.WriteLine("Found " + createNetworkRequests.Count + " createNetworkRequests to update..");
                foreach (var item in createNetworkRequests)
                {
                    if (!string.IsNullOrEmpty(item.ContactPhone))
                    {
                        var result = this.TransformPhoneNumber(context, item.ContactPhone);
                        if (result == null)
                        {
                            this.Out.WriteLine("Createnetworkrequest " + item.Id + " has an unrecognized phone number format: " + item.ContactPhone);
                        }
                        else
                        {
                            done++;
                            item.ContactPhone = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.CreateNetworkRequests.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " createNetworkRequests updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.EventPublicMembers)
            {
                int done = 0;
                var eventPublicMembers = context.Services.Repositories.EventPublicMembers.GetAll().Where(o => !string.IsNullOrEmpty(o.Phone)).ToList();
                this.Out.WriteLine("Found " + eventPublicMembers.Count + " eventPublicMembers to update..");
                foreach (var item in eventPublicMembers)
                {
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        var result = this.TransformPhoneNumber(context, item.Phone);
                        if (result == null)
                        {
                            this.Out.WriteLine("EventPublicMember " + item.Id + " has an unrecognized phone number format: " + item.Phone);
                        }
                        else
                        {
                            done++;
                            item.Phone = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.EventPublicMembers.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " eventPublicMembers updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.Places)
            {
                int done = 0;
                // TODO: paginate this query
                var places = context.Services.Repositories.Places.GetAll(context.Services.NetworkId).Where(o => !string.IsNullOrEmpty(o.Phone)).ToList();
                this.Out.WriteLine("Found " + places.Count + " places to update..");
                foreach (var item in places)
                {
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        var result = this.TransformPhoneNumber(context, item.Phone);
                        if (result == null)
                        {
                            this.Out.WriteLine("Place " + item.Id + " has an unrecognized phone number format: " + item.Phone);
                        }
                        else
                        {
                            done++;
                            item.Phone = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.Places.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " places updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.RequestsForProposal)
            {
                int done = 0;
                var requestsForProposal = context.Services.Repositories.RequestsForProposal.GetAll(context.Services.NetworkId).Where(o => !string.IsNullOrEmpty(o.CompanyPhoneNumber)).ToList();
                this.Out.WriteLine("Found " + requestsForProposal.Count + " requestsForProposal to update..");
                foreach (var item in requestsForProposal)
                {
                    if (!string.IsNullOrEmpty(item.CompanyPhoneNumber))
                    {
                        var result = this.TransformPhoneNumber(context, item.CompanyPhoneNumber);
                        if (result == null)
                        {
                            this.Out.WriteLine("RequestForProposal " + item.Id + " has an unrecognized phone number format: " + item.CompanyPhoneNumber);
                        }
                        else
                        {
                            done++;
                            item.CompanyPhoneNumber = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.RequestsForProposal.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " requestsForProposal updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.Resumes)
            {
                int done = 0;
                var resumes = context.Services.Repositories.Resumes.GetAll(context.Services.NetworkId).Where(o => !string.IsNullOrEmpty(o.Phone)).ToList();
                this.Out.WriteLine("Found " + resumes.Count + " resumes to update..");
                foreach (var item in resumes)
                {
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        var result = this.TransformPhoneNumber(context, item.Phone);
                        if (result == null)
                        {
                            this.Out.WriteLine("Resume " + item.Id + " has an unrecognized phone number format: " + item.Phone);
                        }
                        else
                        {
                            done++;
                            item.Phone = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.Resumes.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " resumes updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.UserProfileFields)
            {
                int done = 0;
                // TODO: paginate this query
                var userProfileFields = context.Services.Repositories.UserProfileFields.GetAll()
                    .Where(o => o.ProfileFieldId == (int)Entities.Networks.ProfileFieldType.Phone && !string.IsNullOrEmpty(o.Value))
                    .ToList();
                this.Out.WriteLine("Found " + userProfileFields.Count + " userProfileFields to update..");
                foreach (var item in userProfileFields)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        var result = this.TransformPhoneNumber(context, item.Value);
                        if (result == null)
                        {
                            this.Out.WriteLine("UserProfileField " + item.Id + " has an unrecognized phone number format: " + item.Value);
                        }
                        else
                        {
                            done++;
                            item.Value = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.UserProfileFields.Update(item);
                        }
                    }
                }

                this.Out.WriteLine(done + " userProfileFields updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.CompanyProfileFields)
            {
                int done = 0;
                // TODO: paginate this query
                var companyProfileFields = context.Services.Repositories.CompanyProfileFields.GetAll(null)
                    .Where(o => o.ProfileFieldId == (int)Entities.Networks.ProfileFieldType.Phone && !string.IsNullOrEmpty(o.Value))
                    .ToList();
                this.Out.WriteLine("Found " + companyProfileFields.Count + " companyProfileFields to update..");
                foreach (var item in companyProfileFields)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        var result = this.TransformPhoneNumber(context, item.Value);
                        if (result == null)
                        {
                            this.Out.WriteLine("CompanyProfileField " + item.Id + " has an unrecognized phone number format: " + item.Value);
                        }
                        else
                        {
                            done++;
                            item.Value = result;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.CompanyProfileFields.Update(item);
                        }
                    }
                }
                this.Out.WriteLine(done + " companyProfileFields updated!");
                this.Out.WriteLine();
            }

            if (context.All || context.PartnerResourceProfileFields)
            {
                int done = 0;
                var partnerResourceProfileField = context.Services.Repositories.PartnerResourceProfileFields.GetAll()
                    .Where(o => o.ProfileFieldId == (int)ProfileFieldType.Contact)
                    .ToDictionary(o => o, o => new PartnerResourceProfileFieldModel(o));
                this.Out.WriteLine("Found " + partnerResourceProfileField.Count + " partnerResourceProfileField to update..");
                foreach (var item in partnerResourceProfileField)
                {
                    if (!string.IsNullOrEmpty(item.Value.ContactModel.Phone))
                    {
                        var result = this.TransformPhoneNumber(context, item.Value.ContactModel.Phone);
                        if (result == null)
                        {
                            this.Out.WriteLine("PartnerResourceProfileField " + item.Key.Id + " has an unrecognized phone number format: " + item.Value.ContactModel.Phone);
                        }
                        else
                        {
                            done++;
                            item.Value.ContactModel.Phone = result;
                            item.Key.Data = item.Value.Data;
                            if (!context.Args.Simulation)
                                context.Services.Repositories.PartnerResourceProfileFields.Update(item.Key);
                        }
                    }
                }
                this.Out.WriteLine(done + " partnerResourceProfileField updated!");
                this.Out.WriteLine();
            }

            this.Out.WriteLine("Exiting database update mode.");
            this.Out.WriteLine();

            return true;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle UpdatePhoneNumbers
    Update phone numbers to international format (+33..).

sparkle UpdatePhoneNumbers  --test
    Launch a set of test numbers not affecting the database and display the result.

sparkle UpdatePhoneNumbers  --clubs --companies --companyrequests --createnetworkrequests
                            --eventpublicmembers --places --requestsforproposal --resumes
                            --userprofilefields --partnerresourceprofilefields
    Update phone numbers only for the specified entities.
";

            registry.Register(
                "UpdatePhoneNumbers",
                "Update the phone numbers to match the international format",
                () => new UpdatePhoneNumbers(),
                longDesc);
        }

        public class Context
        {
            public SparkleCommandArgs Args { get; set; }

            public IServiceFactory Services { get; set; }

            public bool TestMode { get; set; }

            public bool All { get; set; }

            public bool Clubs { get; set; }

            public bool Companies { get; set; }

            public bool CompanyRequests { get; set; }

            public bool CreateNetworkRequests { get; set; }

            public bool EventPublicMembers { get; set; }

            public bool Places { get; set; }

            public bool RequestsForProposal { get; set; }

            public bool Resumes { get; set; }

            public bool UserProfileFields { get; set; }

            public bool PartnerResourceProfileFields { get; set; }

            public Context()
            {
                All = true;
            }

            public bool CompanyProfileFields { get; set; }
        }
    }
}
