
namespace Sparkle.Commands.Main
{
    using Newtonsoft.Json;
    using Sparkle.Commands.Main.Import;
    using Sparkle.Common.CommandLine;
    using Sparkle.Data.Stub;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Security;

    public class Flood : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.None,
        };

        private Random random = new Random();

        public override void RunUniverse(SparkleCommandArgs args)
        {
            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "Flood";
            services.HostingEnvironment.RemoteClient = this.remoteClient;

            var context = new Context
            {
                Args = args,
                BatchId = random.Next(999).ToString().PadLeft(3, '0'),
            };
            context.Services = services;

            if (!this.Run(args, context))
            {
                services.Logger.Error("Flood failed", ErrorLevel.Business);
            }
        }

        private bool Run(SparkleCommandArgs args, Context context)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return false;
            }

            this.Out.WriteLine();

            var services = context.Services;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("Flood", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return false;
            }

            // simulation: not supported
            if (args.Simulation)
            {
                this.Out.WriteLine("Simulation is not supported; not runnin'");
                args.SysLogger.Info("Flood", remoteClient, Environment.UserName, ErrorLevel.Success, "Simulation is not supported; not running.");
                return false;
            }

            services.NetworkId = network.Id;

            if (!this.ParseArgs(context))
                return false;

            if (!this.DoWork(context))
                return false;

            return true;
        }

        protected bool ParseArgs(Context context)
        {
            WorkType? entity = null;
            context.Works = new List<Work>();
            foreach (var arg in context.Args.Arguments)
            {
                var argLo = arg.ToLowerInvariant();
                if (argLo == "flood")
                {
                    continue;
                }

                if (entity != null)
                {
                    long count;
                    if (long.TryParse(arg, out count))
                    {
                        context.Works.Add(new Work
                        {
                            Entity = entity.Value,
                            Count = count,
                        });
                    }
                    else
                    {
                        return this.EndWithError(context, ErrorLevel.Input, "Expected integer value after argument '" + entity + "' but got '" + arg + "'");
                    }
                    
                    entity = null;
                }
                else
                {
                    WorkType e;
                    if (Enum.TryParse(arg, true, out e))
                    {
                        entity = e;
                    }
                    else
                    {
                        return this.EndWithError(context, ErrorLevel.Input, "Unknown entity type '" + arg + "'");
                    }
                }
            }

            if (entity != null)
            {
                return this.EndWithError(context, ErrorLevel.Input, "Expected integer value after argument '" + entity + "' but got nothing");
            }

            return true;
        }

        protected bool DoWork(Context context)
        {
            var membership = new System.Web.Security.SqlMembershipProvider();
            var membershipConfig = new System.Collections.Specialized.NameValueCollection();
            membershipConfig.Add("requiresQuestionAndAnswer", "False");
            membershipConfig.Add("minRequiredNonalphanumericCharacters", "0");
            membershipConfig.Add("passwordStrengthRegularExpression", null);
            membershipConfig.Add("connectionStringName", null);
            membershipConfig.Add("connectionString", context.Args.ApplicationConfiguration.Tree.ConnectionStrings.NetworkApplicationServices);
            membershipConfig.Add("enablePasswordRetrieval", null);
            membershipConfig.Add("enablePasswordReset", null);
            membershipConfig.Add("applicationName", "/");
            membershipConfig.Add("requiresUniqueEmail", null);
            membershipConfig.Add("maxInvalidPasswordAttempts", null);
            membershipConfig.Add("passwordAttemptWindow", null);
            membershipConfig.Add("commandTimeout", null);
            //membershipConfig.Add("proiderName", "AspNetSqlMembershipProvider");
            membershipConfig.Add("passwordFormat", null);
            membershipConfig.Add("name", "AspNetSqlMembershipProvider");
            membershipConfig.Add("minRequiredPasswordLength", null);
            membershipConfig.Add("passwordCompatMode", null);
            membership.Initialize("AspNetSqlMembershipProvider", membershipConfig);
            context.Membership = membership;

            foreach (var work in context.Works)
            {
                switch (work.Entity)
                {
                    case WorkType.CompaniesAndUsers:
                        if (!this.DoCompaniesAndUsers(context, work))
                            return false;
                        break;

                    case WorkType.Companies:
                    case WorkType.Users:
                    case WorkType.Events:
                    case WorkType.Groups:
                    default:
                        return this.EndWithError(context, ErrorLevel.Input, "WorkType " + work.Entity + " is not yet supported");
                }
            }

            return true;
        }

        private bool DoCompaniesAndUsers(Context context, Flood.Work work)
        {
            int userErrors = 0;
            int companiesCount = 0, usersCount = 0;

            var categories = context.Services.Repositories.CompanyCategories.GetCategoriesUsedInNetwork(context.Services.NetworkId);
            if (categories.Count == 0)
            {
                categories = context.Services.Repositories.CompanyCategories.GetAll();
            }

            var jobs = context.Services.Repositories.Job.GetJobsUsedInNetwork(context.Services.NetworkId);
            if (jobs.Count == 0)
            {
                jobs = context.Services.Repositories.Job.GetAll();
            }

            var companies = new Dictionary<int, Company>(checked((int)work.Count));
            for (long i = 0; i < work.Count; i++)
            {
                var item = CreateRandomCompany(context, categories);
                item = context.Services.Company.Insert(item);
                foreach (var field in this.CreateRandomCompanyProfileFields(item))
                {
                    // TODO: prefer AppendCompanyProfileField, we don't need to overwrite non-existent fields!
                    context.Services.ProfileFields.SetCompanyProfileField(item.ID, field.ProfileFieldType, field.Value, ProfileFieldSource.None);
                }

                companiesCount++;
                companies.Add(item.ID, item);

                var usersToCreate = random.Next(20);
                for (int u = 0; u < usersToCreate; u++)
                {
                    var user = CreateRandomUser(context, item, jobs);
                    MembershipCreateStatus mbsStatus;
                    var mbsUser = context.Membership.CreateUser(user.Login, Guid.NewGuid().ToString(), user.Email, null, null, true, null, out mbsStatus);
                    switch (mbsStatus)
                    {
                        case MembershipCreateStatus.Success:
                            break;

                        case MembershipCreateStatus.DuplicateEmail:
                        case MembershipCreateStatus.DuplicateProviderUserKey:
                        case MembershipCreateStatus.DuplicateUserName:
                        case MembershipCreateStatus.InvalidAnswer:
                        case MembershipCreateStatus.InvalidEmail:
                        case MembershipCreateStatus.InvalidPassword:
                        case MembershipCreateStatus.InvalidProviderUserKey:
                        case MembershipCreateStatus.InvalidQuestion:
                        case MembershipCreateStatus.InvalidUserName:
                        case MembershipCreateStatus.ProviderError:
                        case MembershipCreateStatus.UserRejected:
                        default:
                            this.Error.WriteLine("ERR: Failed to create user '" + user.Login + "': " + mbsStatus);
                            userErrors++;
                            continue;
                    }

                    user.UserId = (Guid)mbsUser.ProviderUserKey;
                    user = context.Services.People.Insert(user);
                    var source = ProfileFieldSource.None;
                    foreach (var field in this.CreateRandomUserProfileFields())
                    {
                        context.Services.ProfileFields.SetUserProfileField(user.Id, field.ProfileFieldType, field.Value, source);
                    }

                    usersCount++;
                }

                if (i % (work.Count / 20D) == 0)
                {
                    this.Out.WriteLine("CompaniesAndUsers: " + i + "/" + work.Count + " => " + (i / work.Count * 100) + " % (" + usersCount + " users)");
                }
            }

            this.Out.WriteLine("CompaniesAndUsers: DONE");
            this.Out.WriteLine("CompaniesAndUsers: Created " + companiesCount + " companies.");
            this.Out.WriteLine("CompaniesAndUsers: Created " + usersCount + " users.");
            this.Out.WriteLine();

            return true;
        }

        private User CreateRandomUser(Context context, Company company, IList<Sparkle.Entities.Networks.Job> jobs)
        {
            var first = random.NextBoolean() ? RandomData.AnyFirstname : RandomData.AnyName;
            var last = random.NextBoolean() ? RandomData.AnyLastname : RandomData.AnyName;
            var emailAccount = (random.NextBoolean() ? (first + " " + last) : (first.First() + " " + last)).MakeUrlFriendly(false);
            var item = new User
            {
                AccountClosed = null,
                Birthday = RandomData.AnyPastDateTime(60).AddYears(-16),
                CompanyAccess = random.NextBoolean() ? CompanyAccessLevel.Administrator : CompanyAccessLevel.User,
                CompanyID = company.ID,
                CreatedDateUtc = RandomData.AnyPastDateTime(4),
                Email = emailAccount + "@" + company.EmailDomain,
                FirstName = first,
                Gender = random.NextBoolean() ? 1 : 0,
                IsEmailConfirmed = random.NextBoolean(.9),
                JobId = random.NextBoolean(.8) ? jobs[random.Next(jobs.Count)].Id : default(int?),
                LastName = last,
                Login = context.Services.People.MakeUsernameFromName(first, last),
                NetworkAccess = NetworkAccessLevel.User,
                NetworkId = context.Services.NetworkId,
            };
            return item;
        }

        private IList<UserProfileField> CreateRandomUserProfileFields()
        {
            var fields = new List<UserProfileField>
            {
                new UserProfileField { ProfileFieldType = ProfileFieldType.Phone, Value = RandomData.AnyPhone, },
                new UserProfileField { ProfileFieldType = ProfileFieldType.About, Value = RandomData.Lorems, },
                new UserProfileField { ProfileFieldType = ProfileFieldType.City, Value = RandomData.AnyCity, },
            };

            return fields;
        }

        private Company CreateRandomCompany(Context context, IList<CompanyCategory> categories)
        {
            var name = RandomData.AnyCompanyName;
            var domain = name.MakeUrlFriendly(false) + RandomData.AnyTld;
            var date = RandomData.AnyPastDateTime(4);
            var item = new Company
            {
                Alias = context.Services.Company.MakeAlias(name),
                Baseline = RandomData.AnyName,
                CategoryId = checked((short)categories[random.Next(categories.Count)].Id),
                CreatedDateUtc = date,
                EmailDomain = domain,
                IsApproved = true,
                Name = name,
                NetworkId = context.Services.NetworkId,
            };
            return item;
        }

        private IList<CompanyProfileField> CreateRandomCompanyProfileFields(Company company)
        {
            var fields = new List<CompanyProfileField>
            {
                new CompanyProfileField { ProfileFieldType = ProfileFieldType.Site, Value = "http://" + company.Name.MakeUrlFriendly(false) + RandomData.AnyTld + "/", },
                new CompanyProfileField { ProfileFieldType = ProfileFieldType.Phone, Value = RandomData.AnyPhone, },
                new CompanyProfileField { ProfileFieldType = ProfileFieldType.About, Value = RandomData.Lorems, },
                new CompanyProfileField { ProfileFieldType = ProfileFieldType.Twitter, Value = company.Name.MakeUrlFriendly(false), },
            };

            return fields;
        }

        private bool EndWithError(Context context, ErrorLevel errorLevel, string message)
        {
            this.Out.WriteLine(message);
            context.Args.SysLogger.Info("Flood", remoteClient, Environment.UserName, errorLevel, message);
            return false;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle Flood
  {entity number}[entity number]*

THIS COMMAND IS NOT FOR PRODUCTION USE.

Arguments: 

  entity   The entity to create
  number   The number of entities to create

Entities:

  " + string.Join(",", Enum.GetNames(typeof(WorkType))) + @"

";

            registry.Register(
                "Flood",
                "Creates massive amounts of useless data. ",
                () => new Flood(),
                longDesc);
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
            }

            public IServiceFactory Services { get; set; }

            public SparkleCommandArgs Args { get; set; }

            public List<Work> Works { get; set; }

            public string BatchId { get; set; }

            public SqlMembershipProvider Membership { get; set; }
        }

        public class Work
        {
            public WorkType Entity { get; set; }
            public long Count { get; set; }
        }

        public enum WorkType
        {
            CompaniesAndUsers,
            Companies,
            Users,
            Events,
            Groups,
        }
    }

    public static class Extensions
    {
        public static bool NextBoolean(this Random random)
        {
            return random.NextDouble() > .5;
        }

        public static bool NextBoolean(this Random random, double trueProbability)
        {
            return random.NextDouble() < trueProbability;
        }
    }
}
