
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.Data.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Tags;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    ////using GroupTag = Sparkle.Entities.Networks.GroupTag;
    using Company = Sparkle.Entities.Networks.Company;
    using CompanyTag = Sparkle.Entities.Networks.CompanyTag;
    using Group = Sparkle.Entities.Networks.Group;
    using GroupTag = Sparkle.Entities.Networks.GroupTag;
    using Interest = Sparkle.Entities.Networks.Interest;
    using ITag = Sparkle.Entities.Networks.ITag;
    using ITagV1 = Sparkle.Entities.Networks.ITagV1;
    using ITagV1Relation = Sparkle.Entities.Networks.ITagV1Relation;
    using ITagV2Relation = Sparkle.Entities.Networks.ITagV2Relation;
    using Recreation = Sparkle.Entities.Networks.Recreation;
    using Skill = Sparkle.Entities.Networks.Skill;
    using TagCategory = Sparkle.Entities.Networks.TagCategory;
    using TagDefinition = Sparkle.Entities.Networks.TagDefinition;
    using TagDeleteReason = Sparkle.Entities.Networks.TagDeleteReason;
    using TimelineItem = Sparkle.Entities.Networks.TimelineItem;
    using TimelineItemTag = Sparkle.Entities.Networks.TimelineItemTag;
    using User = Sparkle.Entities.Networks.User;
    using UserTag = Sparkle.Entities.Networks.UserTag;

    /// <summary>
    /// Migrate Tags from V1 to V2.
    /// </summary>
    public class MigrateTags : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private const string commandName = "MigrateTags";
        private Dictionary<int, int> companyIdToUserId;
        private Dictionary<int, int> groupIdToUserId;

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle MigrateTags
    Move tags V1 to tags V2.";

            registry.Register(
                commandName,
                "Move tags V1 to tags V2.",
                () => new MigrateTags(),
                longDesc);
        }

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;

            // simulation: not supported
            if (args.Simulation)
            {
                var message = "Simulating command execution.";
                this.Out.WriteLine(message);
                args.SysLogger.Info(commandName, remoteClient, Environment.UserName, ErrorLevel.Success, message);
            }

            var context = new Context
            {
                Args = args,
                UtcNow = args.Now.ToUniversalTime(),
            };

            if (!this.Run(context))
            {
                context.Services.Logger.Error(commandName + " failed", ErrorLevel.Business);
            }
        }

        private bool Run(Context context)
        {
            var ok = !this.InitializeServices(context)
                || !this.InitializeNetwork(context)
                || !this.VerifyConfiguration(context)
                || !this.VerifyTagsV1(context)
                || !this.VerifyTagsV2(context)
                || !this.PrepareNetworks(context)
                || !this.PrepareChangeset(context)
                || !this.Report(context)
                || this.Commit(context, false);

            if (!ok)
            {
                if (context.Transaction != null)
                {
                    this.Commit(context, true);
                }
            }

            return ok;
        }

        private bool InitializeServices(Context context)
        {
            var services = context.Args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = commandName;
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
                var message = "Network \"" + networkName + "\" does not exist; not running.";
                ////this.Out.WriteLine(message);
                context.Args.SysLogger.Info(commandName, this.remoteClient, Environment.UserName, ErrorLevel.Input, message);
                return false;
            }

            context.Services.NetworkId = network.Id;

            {
                var message = "Ready to run " + commandName + " for network '" + context.Services.Network.ToString() + "' (universe " + context.Args.Application.UniverseName + ") " + (context.Args.Simulation ? "as simulation" : "for real");
                ////this.Out.WriteLine(message);
                context.Args.SysLogger.Info(commandName, this.remoteClient, Environment.UserName, ErrorLevel.Success, message);
            }

            return true;
        }

        private bool VerifyConfiguration(Context context)
        {
            if (!context.Services.AppConfiguration.Tree.Features.Tags.DisableV1)
            {
                context.Args.SysLogger.Warning(commandName, this.remoteClient, Environment.UserName, ErrorLevel.Success, "Do not forget to set AppConfiguration.Tree.Features.Tags.DisableV1 = 1 to ensure the old system is not used anymore by users.");
            }

            return true;
        }

        private bool VerifyTagsV1(Context context)
        {
            return true;
        }

        private bool VerifyTagsV2(Context context)
        {
            var categories = context.Services.Tags.GetCategories();

            var networkId = context.Services.Network.Id;
            context.SkillsCategory = categories.SingleOrDefault(c => c.Name == "Skill" && c.NetworkId == null);
            context.InterestsCategory = categories.SingleOrDefault(c => c.Name == "Interest" && c.NetworkId == null);
            context.RecreationsCategory = categories.SingleOrDefault(c => c.Name == "Recreation" && c.NetworkId == null);

            const string noCategoryMessage = "Cannot find a Tag Category to match the {0} tag table.";
            const string tagsV2alreadyPopulatedMessage = "Cannot import the {0} tag table because the corresponding V2 category already contains tags.";

            var cats = new KeyValuePair<string, TagCategoryModel>[]
            {
                new KeyValuePair<string, TagCategoryModel>("Skills", context.SkillsCategory),
                new KeyValuePair<string, TagCategoryModel>("Interests", context.InterestsCategory),
                new KeyValuePair<string, TagCategoryModel>("Recreations", context.RecreationsCategory),
            };
            context.Categories = cats;

            bool error = false;
            foreach (var cat in cats)
            {
                var name = cat.Key;

                if (cat.Value != null)
                {
                    var count = context.Services.Tags.CountByCategory(cat.Value.Id);
                    if (count > 0)
                    {
                        error = true;
                        context.Args.SysLogger.Error(commandName, this.remoteClient, Environment.UserName, ErrorLevel.Success, string.Format(tagsV2alreadyPopulatedMessage, name));
                    }
                }
                else
                {
                    error = true;
                    context.Args.SysLogger.Error(commandName, this.remoteClient, Environment.UserName, ErrorLevel.Success, string.Format(noCategoryMessage, name));
                }
            }

            return !error;
        }

        private bool PrepareNetworks(Context context)
        {
            var networks = context.Services.Repositories.Networks.GetAll(Entities.Networks.NetworkOptions.None);

            context.Networks = networks;
            context.NetworkContexts = new Dictionary<int, NetworkContext>();
            foreach (var item in networks)
            {
                var ncontext = new NetworkContext();
                ncontext.Aliases = new List<string>();
                context.NetworkContexts.Add(item.Id, ncontext);
            }

            context.Args.SysLogger.Info(commandName, this.remoteClient, Environment.UserName, ErrorLevel.Success, "This change will affect the following networks: " + string.Join(", ", networks));

            return true;
        }

        private bool PrepareChangeset(Context context)
        {
            bool error = false;

            this.Out.WriteLine();
            this.Out.WriteLine("PROCESS");
            this.Out.WriteLine("=======");
            this.Out.WriteLine();
            ////context.NewUserTags = new List<UserTag>();
            context.NewTags = new List<TagDefinition>();
            context.NewTagRelations = new List<ITagV2Relation>();
            context.AliasesComparer = StringComparer.OrdinalIgnoreCase;

            context.Transaction = context.Services.Repositories.NewTransaction();
            context.Transaction.BeginTransaction();

            this.companyIdToUserId = new Dictionary<int, int>();
            this.groupIdToUserId = new Dictionary<int, int>();

            var cat = context.SkillsCategory;
            error |= !this.PrepareChangeset<Skill>(context, cat);

            cat = context.InterestsCategory;
            error |= !this.PrepareChangeset<Interest>(context, cat);

            cat = context.RecreationsCategory;
            error |= !this.PrepareChangeset<Recreation>(context, cat);
            
            return !error;
        }

        private bool PrepareChangeset<T>(Context context, TagCategoryModel cat)
            where T : ITagV1
        {
            var repo = this.GetRepo<T>(context, cat.Name);
            IDictionary<int, ITagV1> olds = repo.GetAll();
            this.Out.WriteLine();
            this.Out.WriteLine("Now processing " + olds.Count + " " + typeof(T).Name + " on " + context.Networks.Count + " networks.");
            this.Out.WriteLine();
            var preserveAliasChar = new char[] { '.', };

            foreach (var network in context.Networks)
            {
                var watch = Stopwatch.StartNew();
                var ncontext = context.NetworkContexts[network.Id];
                this.Out.WriteLine("Processing " + typeof(T).Name + " on network " + network.ToString() + "...");
                this.Out.Write("  " + olds.Count + " to go");
                ////var news = new Dictionary<int, TagDefinition>(olds.Count);
                ////newEntities.Add(network.Id, news);
                foreach (ITagV1 old in olds.Values)
                {
                    var item = new TagDefinition();
                    item.NetworkId = network.Id;
                    item.CategoryId = cat.Id;
                    item.Name = old.TagName.Trim();
                    item.Alias = item.Name.MakeUrlFriendly(true, preserveAliasChar);
                    EnhanceAlias(old.TagName, item);

                    if (ncontext.Aliases.Contains(item.Alias, context.AliasesComparer))
                    {
                        item.Alias = item.Alias.GetIncrementedString(x => !ncontext.Aliases.Contains(x, context.AliasesComparer));
                    }


                    item.CreatedDateUtc = old.Date;
                    item.CreatedByUserId = old.CreatedByUserId;
                    ////news.Add(old.Id, item);
                    var createdRelations = new List<ITagV2Relation>();

                    if (repo.AppliesToUsers)
                    {
                        var oldRelRepo = this.GetOldRelRepo<User, T>(context);
                        var oldRels = oldRelRepo.GetByTagId(old.Id);
                        var newRelRepo = this.GetNewRelRepo<User>(context);

                        foreach (var oldRel in oldRels)
                        {
                            var newRel = newRelRepo.GetNewEntity();
                            var userId = oldRel.RelationId;
                            var user = context.Services.Repositories.People.GetById(userId, Data.Options.PersonOptions.None);
                            if (user.NetworkId == network.Id)
                            {
                                newRel.CreatedByUserId = userId;
                                newRel.DateCreatedUtc = oldRel.Date;
                                newRel.DateDeletedUtc = oldRel.DeletedDateUtc;
                                newRel.DeletedByUserId = oldRel.DeletedDateUtc != null ? userId : default(int?);
                                newRel.DeleteReason = oldRel.DeletedDateUtc != null ? (byte)TagDeleteReason.AuthorDelete : default(byte?);
                                newRel.RelationId = userId;

                                createdRelations.Add(newRel);
                            }
                        }
                    }

                    if (repo.AppliesToCompanies)
                    {
                        var oldRelRepo = this.GetOldRelRepo<Company, T>(context);
                        var oldRels = oldRelRepo.GetByTagId(old.Id);
                        var newRelRepo = this.GetNewRelRepo<Company>(context);

                        foreach (var oldRel in oldRels)
                        {
                            var newRel = newRelRepo.GetNewEntity();
                            var companyId = oldRel.RelationId;
                            var company = context.Services.Repositories.Companies.GetById(companyId);
                            if (company.NetworkId == network.Id)
                            {
                                var userId = FindUserIdForCompany(context, companyId);
                                newRel.CreatedByUserId = userId;
                                newRel.DateCreatedUtc = oldRel.Date;
                                newRel.DateDeletedUtc = oldRel.DeletedDateUtc;
                                newRel.DeletedByUserId = oldRel.DeletedDateUtc != null ? userId : default(int?);
                                newRel.DeleteReason = oldRel.DeletedDateUtc != null ? (byte)TagDeleteReason.AuthorDelete : default(byte?);
                                newRel.RelationId = companyId;

                                createdRelations.Add(newRel);
                            }
                        }
                    }

                    if (repo.AppliesToGroups)
                    {
                        var oldRelRepo = this.GetOldRelRepo<Group, T>(context);
                        var oldRels = oldRelRepo.GetByTagId(old.Id);
                        var newRelRepo = this.GetNewRelRepo<Group>(context);

                        foreach (var oldRel in oldRels)
                        {
                            var newRel = newRelRepo.GetNewEntity();
                            var groupId = oldRel.RelationId;
                            var group = context.Services.Repositories.Groups.GetById(groupId);
                            if (group.NetworkId == network.Id)
                            {
                                var userId = FindUserIdForGroup(context, groupId);


                                newRel.CreatedByUserId = userId;
                                newRel.DateCreatedUtc = oldRel.Date;
                                newRel.DateDeletedUtc = oldRel.DeletedDateUtc;
                                newRel.DeletedByUserId = oldRel.DeletedDateUtc != null ? userId : default(int?);
                                newRel.DeleteReason = oldRel.DeletedDateUtc != null ? (byte)TagDeleteReason.AuthorDelete : default(byte?);
                                newRel.RelationId = groupId;

                                createdRelations.Add(newRel);
                            }
                        }
                    }

                    if (repo.AppliesToTimelineItems)
                    {
                        var oldRelRepo = this.GetOldRelRepo<TimelineItem, T>(context);
                        var oldRels = oldRelRepo.GetByTagId(old.Id);
                        var newRelRepo = this.GetNewRelRepo<TimelineItem>(context);

                        foreach (var oldRel in oldRels)
                        {
                            var newRel = newRelRepo.GetNewEntity();
                            var timelineItemId = oldRel.RelationId;
                            var timelineItem = context.Services.Repositories.Wall.GetById(timelineItemId);
                            if (timelineItem.NetworkId == network.Id)
                            {
                                var userId = timelineItem.PostedByUserId;
                                newRel.CreatedByUserId = userId;
                                newRel.DateCreatedUtc = oldRel.Date;
                                newRel.DateDeletedUtc = oldRel.DeletedDateUtc;
                                newRel.DeletedByUserId = oldRel.DeletedDateUtc != null ? userId : default(int?);
                                newRel.DeleteReason = oldRel.DeletedDateUtc != null ? (byte)TagDeleteReason.AuthorDelete : default(byte?);
                                newRel.RelationId = timelineItemId;

                                createdRelations.Add(newRel);
                            }
                        }
                    }

                    if (createdRelations.Count > 0)
                    {
                        context.NewTags.Add(item);
                        ncontext.Aliases.Add(item.Alias);

                        try
                        {
                            context.Transaction.Repositories.TagDefinitions.Insert(item);
                            ////context.Services.Repositories.TagDefinitions.Insert(item);
                            this.Out.Write(".");
                        }
                        catch (UpdateException ex)
                        {
                            this.Out.WriteLine();
                            this.Out.WriteLine(ex.InnerException.Message);
                            this.Out.WriteLine(item);
                            this.Out.WriteLine();
                            continue;
                        }

                        foreach (var newRel in createdRelations)
                        {
                            var alreadyInserted = context.NewTagRelations
                                .Where(r => r.GetType() == newRel.GetType() && r.RelationId == newRel.RelationId && r.TagId == item.Id)
                                .ToArray();
                            if (alreadyInserted.Length > 0)
                                continue;


                            newRel.TagId = item.Id;

                            var newRelRepo = GetNewRelRepo(context.Transaction.Repositories, newRel);
                            try
                            {
                                newRelRepo.Insert(newRel);

                                context.NewTagRelations.Add(newRel);
                                this.Out.Write(".");
                            }
                            catch (UpdateException ex)
                            {
                                this.Out.WriteLine();
                                this.Out.WriteLine(ex.InnerException.Message);
                                this.Out.WriteLine(newRel);
                                this.Out.WriteLine();
                                continue;
                            }
                        }
                    }
                    else
                    {
                    }
                }

                this.Out.WriteLine();
                this.Out.WriteLine("Processed " + typeof(T).Name + " on network " + network.ToString() + ".");
                this.Out.WriteLine();
            }

            return true;
        }

        private bool Report(Context context)
        {
            this.Out.WriteLine();
            this.Out.WriteLine("REPORT");
            this.Out.WriteLine("=======");
            this.Out.WriteLine();

            foreach (var item in context.NewTags)
            {
                this.Out.WriteLine("NEW Tag: " + item.ToString());
            }

            foreach (var item in context.NewTagRelations)
            {
                this.Out.WriteLine("NEW Rel: " + item.ToString());
            }

            this.Out.WriteLine();
            this.Out.WriteLine();
            return true;
        }

        private bool Commit(Context context, bool forceRollback)
        {
            this.Out.WriteLine();
            this.Out.WriteLine("FINALIZE");
            this.Out.WriteLine("========");
            this.Out.WriteLine();

            if (forceRollback)
            {
                this.Out.WriteLine("Rolling back transaction... ");
                context.Transaction.AbortTransaction();
                this.Out.WriteLine("Done. ");
            }
            if (context.Args.Simulation)
            {
                this.Out.WriteLine("Simulation mode. Rolling back transaction... ");
                context.Transaction.AbortTransaction();
                this.Out.WriteLine("Done. ");
            }
            else
            {
                this.Out.WriteLine("Commiting transaction... ");
                context.Transaction.CompleteTransaction();
                this.Out.WriteLine("Done. ");
            }

            context.Transaction = null;
            this.Out.WriteLine();
            return true;
        }

        private ITagsV1RelationRepository GetOldRelRepo<T1, T2>(Context context)
            where T2 : ITagV1
        {
            if (typeof(T1) == typeof(User))
            {
                if (typeof(T2) == typeof(Skill))
                    return (ITagsV1RelationRepository)context.Services.Repositories.PeoplesSkills;
                if (typeof(T2) == typeof(Recreation))
                    return (ITagsV1RelationRepository)context.Services.Repositories.PeoplesRecreations;
                if (typeof(T2) == typeof(Interest))
                    return (ITagsV1RelationRepository)context.Services.Repositories.PeoplesInterests;
            }
            else if (typeof(T1) == typeof(Company))
            {
                if (typeof(T2) == typeof(Skill))
                    return (ITagsV1RelationRepository)context.Services.Repositories.CompaniesSkills;
                if (typeof(T2) == typeof(Recreation))
                    return null;
                if (typeof(T2) == typeof(Interest))
                    return null;
            }
            else if (typeof(T1) == typeof(Group))
            {
                if (typeof(T2) == typeof(Skill))
                    return (ITagsV1RelationRepository)context.Services.Repositories.GroupsSkills;
                if (typeof(T2) == typeof(Recreation))
                    return (ITagsV1RelationRepository)context.Services.Repositories.GroupsRecreations;
                if (typeof(T2) == typeof(Interest))
                    return (ITagsV1RelationRepository)context.Services.Repositories.GroupsInterests;
            }
            else if (typeof(T1) == typeof(TimelineItem))
            {
                if (typeof(T2) == typeof(Skill))
                    return (ITagsV1RelationRepository)context.Services.Repositories.TimelineItemSkills;
                if (typeof(T2) == typeof(Recreation))
                    return null;
                if (typeof(T2) == typeof(Interest))
                    return null;
            }

            return null;
        }

        private ITagsV2RelationRepository GetNewRelRepo<T1>(Context context)
        {
            if (typeof(T1) == typeof(User))
            {
                return context.Services.Repositories.UserTags;
            }
            else if (typeof(T1) == typeof(Company))
            {
                return context.Services.Repositories.CompanyTags;
            }
            else if (typeof(T1) == typeof(Group))
            {
                return context.Services.Repositories.GroupTags;
            }
            else if (typeof(T1) == typeof(TimelineItem))
            {
                return context.Services.Repositories.TimelineItemTags;
            }

            return null;
        }

        private ITagsV2RelationRepository GetNewRelRepo(IRepositoryFactory repositoryFactory, ITagV2Relation newRel)
        {
            if (newRel is UserTag)
                return repositoryFactory.UserTags;
            else if (newRel is CompanyTag)
                return repositoryFactory.CompanyTags;
            else if (newRel is GroupTag)
                return repositoryFactory.GroupTags;
            else if (newRel is TimelineItemTag)
                return repositoryFactory.TimelineItemTags;
            return null;
        }

        private ITagsV1Repository GetRepo<T>(Context context, string name)
        {
            if (typeof(T) == typeof(Skill))
                return (ITagsV1Repository)context.Services.Repositories.Skills;
            if (typeof(T) == typeof(Interest))
                return (ITagsV1Repository)context.Services.Repositories.Interests;
            if (typeof(T) == typeof(Recreation))
                return (ITagsV1Repository)context.Services.Repositories.Recreations;

            return null;
        }

        private int FindUserIdForCompany(Context context, int companyId)
        {
            if (this.companyIdToUserId.ContainsKey(companyId))
            {
                return this.companyIdToUserId[companyId];
            }

            int? userId = null;
            var company = context.Services.Repositories.Companies.GetById(companyId);
            var companyUsers = context.Services.Company.GetAdministrators(companyId);
            if (companyUsers.Count > 0)
            {
                userId = companyUsers.First().Id;
            }

            if (userId == null)
            {
                companyUsers = context.Services.Company.GetAllUsersByAccessLevel(companyId, Entities.Networks.CompanyAccessLevel.CommunityManager, Data.Options.PersonOptions.None);
                if (companyUsers.Count > 0)
                {
                    userId = companyUsers.First().Id;
                }
            }

            if (userId == null)
            {
                var user = context.Services.Repositories.People.GetAnyByCompanyId(companyId);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            if (userId == null)
            {
                var users = context.Services.People.GetByNetworkAccessLevel(Entities.Networks.NetworkAccessLevel.NetworkAdmin);
                if (users.Count > 0)
                {
                    userId = users.First().Id;
                }
            }

            if (userId == null)
            {
                var users = context.Services.People.GetByNetworkAccessLevel(Entities.Networks.NetworkAccessLevel.ManageCompany);
                if (users.Count > 0)
                {
                    userId = users.First().Id;
                }
            }

            if (userId == null)
            {
                var users = context.Services.People.GetByNetworkAccessLevel(Entities.Networks.NetworkAccessLevel.ModerateNetwork);
                if (users.Count > 0)
                {
                    userId = users.First().Id;
                }
            }

            if (userId == null)
            {
                var users = context.Services.Repositories.People.NewQuery(Data.Options.PersonOptions.None)
                    .Where(x => x.NetworkId == company.NetworkId)
                    .First();
                userId = users.Id;
            }

            if (userId == null)
            {
                throw new InvalidOperationException("Cannot find a user to match company " + companyId + ".");
            }

            this.companyIdToUserId.Add(companyId, userId.Value);
            return userId.Value;
        }

        private int FindUserIdForGroup(Context context, int groupId)
        {
            if (this.groupIdToUserId.ContainsKey(groupId))
            {
                return this.groupIdToUserId[groupId];
            }

            int? userId = null;
            var group = context.Services.Repositories.Groups.GetById(groupId);
            var groupUsers = context.Services.GroupsMembers.SelectAdminsGroupMembers(groupId);
            if (groupUsers.Count > 0)
            {
                userId = groupUsers.First().UserId;
            }

            if (userId == null)
            {
                var users = context.Services.GroupsMembers.GetActiveGroupMembers(groupId);
                if (users.Count > 0)
                {
                    userId = users.First().UserId;
                }
            }

            if (userId == null)
            {
                var users = context.Services.People.GetByNetworkAccessLevel(Entities.Networks.NetworkAccessLevel.NetworkAdmin);
                if (users.Count > 0)
                {
                    userId = users.First().Id;
                }
            }

            if (userId == null)
            {
                var users = context.Services.People.GetByNetworkAccessLevel(Entities.Networks.NetworkAccessLevel.ModerateNetwork);
                if (users.Count > 0)
                {
                    userId = users.First().Id;
                }
            }

            if (userId == null)
            {
                var users = context.Services.Repositories.People.NewQuery(Data.Options.PersonOptions.None)
                    .Where(x => x.NetworkId == group.NetworkId)
                    .First();
                userId = users.Id;
            }

            if (userId == null)
            {
                throw new InvalidOperationException("Cannot find a user to match group " + groupId + ".");
            }

            this.groupIdToUserId.Add(groupId, userId.Value);
            return userId.Value;
        }

        private void EnhanceAlias(string name, TagDefinition item)
        {
            if (name.Equals("C++", StringComparison.OrdinalIgnoreCase))
                item.Alias = "Cpp";
            if (name.Equals("C#", StringComparison.OrdinalIgnoreCase))
                item.Alias = "CSharp";
            if (name.Equals("F#", StringComparison.OrdinalIgnoreCase))
                item.Alias = "FSharp";
            if (name.Equals(".net", StringComparison.OrdinalIgnoreCase))
                item.Alias = ".NET";
        }

        public class Context
        {
            public SparkleCommandArgs Args { get; set; }

            public DateTime UtcNow { get; set; }

            public IServiceFactory Services { get; set; }

            public TagCategoryModel SkillsCategory { get; set; }
            public TagCategoryModel InterestsCategory { get; set; }
            public TagCategoryModel RecreationsCategory { get; set; }

            public KeyValuePair<string, TagCategoryModel>[] Categories { get; set; }

            public IList<Entities.Networks.Network> Networks { get; set; }
            public Dictionary<int, NetworkContext> NetworkContexts { get; set; }

            ////public Dictionary<int, Dictionary<int, TagDefinition>> NewSkills { get; set; }
            ////public Dictionary<int, Dictionary<int, TagDefinition>> NewInterests { get; set; }
            ////public Dictionary<int, Dictionary<int, TagDefinition>> NewRecreations { get; set; }

            ////public List<UserTag> NewUserTags { get; set; }
            ////public List<CompanyTag> NewCompanyTags { get; set; }
            ////public List<TimelineItemTag> NewTimelineItemTags { get; set; }
            ////public List<GroupTag> NewGroupTags { get; set; }

            public List<TagDefinition> NewTags { get; set; }

            public List<ITagV2Relation> NewTagRelations { get; set; }

            public StringComparer AliasesComparer { get; set; }

            public IDataTransaction Transaction { get; set; }
        }

        public class NetworkContext
        {
            public List<string> Aliases { get; set; }
        }
    }
}
