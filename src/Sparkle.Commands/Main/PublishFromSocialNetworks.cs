
namespace Sparkle.Commands.Main
{
    using LinqToTwitter;
    using Newtonsoft.Json;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Objects;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Services.Main.Networks;

    public class PublishFromSocialNetworks : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.None,
        };

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "PublishFromSocialNetworks";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            this.Out.WriteLine();

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            // simulation: not supported
            if (args.Simulation)
            {
                this.Out.WriteLine("Simulation is not supported; not runnin.");
                args.SysLogger.Info("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Success, "Simulation is not supported; not running.");
                return;
            }

            services.NetworkId = network.Id;

            // for all social network connections
            var connections = services.SocialNetworkStates.GetAll();
            foreach (SocialNetworkState connection in connections)
            {
                var socialType = connection.SocialNetworkConnectionType;

                if (connection.IsProcessing)
                {
                    this.Out.WriteLine("NOT Running PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " because previous process is still running (since: "+((connection.LastStartDate.HasValue) ? (DateTime.UtcNow.Subtract(connection.LastStartDate.Value).ToString()) : ("unknown start date"))+", connection id: "+connection.Id+")");
                    args.SysLogger.Warning("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Internal, "NOT Running PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " because previous process is still running (" + ((connection.LastStartDate.HasValue) ? (DateTime.UtcNow.Subtract(connection.LastStartDate.Value).ToString()) : ("unknown start date")) + ")");
                }
                else if (!connection.IsConfigured)
                {
                    this.Out.WriteLine("NOT Running PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " because connection is not configured");
                    args.SysLogger.Info("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Internal, "NOT Running PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " because connection is not configured");
                }
                else
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    this.Out.WriteLine("Running PublishFromSocialNetworks from " + socialType + " for network: " + network.Name);
                    args.SysLogger.Info("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Success, "Running PublishFromSocialNetworks from " + socialType + " for network: " + network.Name);

                    connection.IsProcessing = true;
                    connection.LastStartDate = DateTime.UtcNow;
                    services.SocialNetworkStates.Update(connection);

                    IList<string> log = new List<string>();
                    try
                    {
                        switch (connection.SocialNetworkConnectionType)
                        {
                            case SocialNetworkConnectionType.Twitter:
                                this.RunTwitter(services, network, connection, log);
                                break;

                            case SocialNetworkConnectionType.Facebook:
                            default:
                                throw new NotSupportedException("SocialNetworkConnectionType " + connection.SocialNetworkConnectionType + " is not supported");
                        }

                        connection.IsProcessing = false;
                        connection.LastEndDate = DateTime.UtcNow;
                        services.SocialNetworkStates.Update(connection);

                        watch.Stop();
                        this.Out.WriteLine("Ran PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " in " + watch.Elapsed);
                        this.Out.WriteLine(string.Join(Environment.NewLine, log));
                        args.SysLogger.Info("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Success, "Ran PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " in " + watch.Elapsed);
                    }
                    catch (Exception ex)
                    {
                        connection.IsProcessing = false;
                        connection.LastEndDate = DateTime.UtcNow;
                        services.SocialNetworkStates.Update(connection);

                        watch.Stop();
                        this.Out.WriteLine("Failed to run PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " in " + watch.Elapsed + Environment.NewLine + ex.ToString());
                        this.Out.WriteLine(string.Join(Environment.NewLine, log));
                        args.SysLogger.Error("PublishFromSocialNetworks", remoteClient, Environment.UserName, ErrorLevel.Internal, "Failed to run PublishFromSocialNetworks from " + socialType + " for network: " + network.Name + " in " + watch.Elapsed + " with error " + ex.Message);
                    }
                }
            }
        }

        private void RunTwitter(IServiceFactory services, Network network, SocialNetworkState connection, IList<string> log)
        {
            ////var companySubs = services.SocialNetworkCompanySubscriptions.GetAllActive(socialType);
            ////var userSubs = services.SocialNetworkUserSubscriptions.GetAllActive(socialType);

            // configure provider
            var credentials = new InMemoryCredentials
            {
                ConsumerKey = services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                ConsumerSecret = services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                OAuthToken = connection.OAuthAccessToken,
                AccessToken = connection.OAuthAccessSecret,
            };
            var authorizer = new SingleUserAuthorizer
            {
                Credentials = credentials,
            };
            var twitter = new TwitterContext(authorizer);

            // find list
            var listName = services.SocialNetworkStates.GetTwitterFollowListName(network);
            var lists = twitter.List.Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == connection.Username).ToList();
            var list = lists.SingleOrDefault(l => l.Name == listName);
            ////var list = twitter.List.Where(l => l.Slug == listName && l.Type == ListType.Show && l.OwnerScreenName == connection.Username).SingleOrDefault();
            if (list != null)
            {
                if (connection.LastItemId.HasValue)
                {
                    log.Add("Latest tweet from list " + listName + " has the id " + connection.LastItemId.Value);
                    // fetch statuses from latest run
                    var statuses = new List<LinqToTwitter.Status>();
                    int page = 0, itemsPerPage = 80;
                    ulong sinceId = unchecked((ulong)connection.LastItemId.Value);
                    IList<LinqToTwitter.Status> pageResult;
                    do
                    {
                        var pageResultList = twitter.List
                            .Where(l => l.Type == ListType.Statuses
                                     && l.Slug == listName
                                     && l.OwnerScreenName == connection.Username
                                     ////&& l.ListID == list.ListID
                                     && l.Count == itemsPerPage
                                     && l.Page == page
                                     && l.SinceID == sinceId)
                            .ToList();
                        pageResult = pageResultList.SelectMany(l => l.Statuses).ToArray();
                        log.Add("Loaded page " + page + " from " + listName + " with " + pageResult.Count + " elements (" + itemsPerPage + " elements requested)");
                        page++;
                        statuses.AddRange(pageResult);
                    } while (pageResult.Count == itemsPerPage);

                    log.Add("Loaded " + page + " pages -> " + statuses.Count + " elements to handle");
                    var inserts = new List<TimelineItem>();
                    var statusGroups = statuses.GroupBy(s => s.User.Identifier.ScreenName).ToArray();
                    foreach (var statusGroup in statusGroups)
                    {
                        var networkConnectionUsername = statusGroup.Key;

                        // fetch companies linked to this profile
                        var companies = services.SocialNetworkCompanySubscriptions.GetByNetworkConnectionUsername(networkConnectionUsername, connection.SocialNetworkConnectionType);

                        // fetch users linked to this profile
                        var users = services.SocialNetworkUserSubscriptions.GetByNetworkConnectionUsername(networkConnectionUsername, connection.SocialNetworkConnectionType);

                        log.Add("Processing " + statusGroup.Count() + " tweets from '" + networkConnectionUsername + "' -> " + companies.Count + " companies and " + users.Count + " users");

                        // post statuses
                        foreach (var status in statusGroup)
                        {
                            var entry = new TimelineSocialEntry
                            {
                                Source = "Twitter",
                                Id = status.StatusID,
                                UserId = status.User.Identifier.ID,
                                UserName = status.User.Identifier.ScreenName,
                                DisplayUrl = "https://twitter.com/" + status.User.Identifier.ScreenName + "/status/" + status.StatusID + "/",
                            };

                            if (status.Entities != null)
                            {
                                if (status.Entities.MediaEntities != null && status.Entities.MediaEntities.Count > 0)
                                {
                                    entry.Pictures = new List<TimelineSocialPictureEntry>();
                                    foreach (var item in status.Entities.MediaEntities)
                                    {
                                        entry.Pictures.Add(new TimelineSocialPictureEntry
                                        {
                                            FullDisplayUrl = item.ExpandedUrl,
                                            ShortDisplayUrl = item.Url,
                                            MediaUrl = item.MediaUrl,
                                            MediaUrlHttps = item.MediaUrlHttps,
                                            Sizes = item.Sizes != null ? item.Sizes.Select(s => new TimelineSocialPictureSizeEntry
                                            {
                                                Type = s.Type,
                                                Height = unchecked((short)s.Height),
                                                Width = unchecked((short)s.Width),
                                            }).ToList() : null,
                                        });
                                    }
                                }
                            }

                            if (services.AppConfiguration.Tree.Features.Companies.SocialPull.IsEnabled)
                            {
                                foreach (var company in companies)
                                {
                                    bool publish = company.AutoPublish;
                                    if (company.ContentContainsFilter != null)
                                    {
                                        var matchString = "#" + company.ContentContainsFilter;
                                        publish &= status.Text.IndexOf(matchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
                                    }

                                    if (publish)
                                    {
                                        TimelineItem item = this.GenerateTwitterTimelineItem(network, status, entry);
                                        item.CompanyId = company.CompanyId;
                                        item.PostedByUserId = company.SocialNetworkConnection.CreatedByUserId;
                                        inserts.Add(item);
                                    }
                                }
                            }

                            if (services.AppConfiguration.Tree.Features.Users.SocialPull.IsEnabled)
                            {
                                foreach (var user in users)
                                {
                                    bool publish = user.AutoPublish;
                                    if (user.ContentContainsFilter != null)
                                    {
                                        var matchString = "#" + user.ContentContainsFilter;
                                        publish &= status.Text.IndexOf(matchString, StringComparison.InvariantCultureIgnoreCase) >= 0;
                                    }

                                    if (publish)
                                    {
                                        TimelineItem item = this.GenerateTwitterTimelineItem(network, status, entry);
                                        item.UserId = user.UserId;
                                        item.PostedByUserId = user.UserId;
                                        inserts.Add(item);
                                    }
                                }
                            }
                        }
                    }

                    if (statuses.Count > 0)
                    {
                        long maxId = statuses.Max(s => long.Parse(s.StatusID));
                        log.Add("Setting latest processed tweet to " + maxId + ".");
                        connection.LastItemId = maxId;
                    }

                    log.Add("About to insert " + inserts.Count + " timeline items.");
                    connection = services.Wall.InsertManyAndUpdateSocialState(inserts, connection);
                    log.Add("Succeed.");
                }
                else
                {
                    // never ran before
                    ////var latestItem = list.Statuses.FirstOrDefault();
                    var latestItemList = twitter.List
                        .Where(l => l.Type == ListType.Statuses && l.Slug == listName && l.OwnerScreenName == connection.Username && l.Count == 1)
                        .LastOrDefault();

                    var latestItem = latestItemList.Statuses.FirstOrDefault();
                    if (latestItem != null)
                    {
                        // mark latest processed item as latest item in list
                        connection.LastItemId = long.Parse(latestItem.StatusID);
                        log.Add("First run, setting latest tweet ID to " + connection.LastItemId);
                        connection = services.SocialNetworkStates.Update(connection);
                    }
                    else
                    {
                        // list is empty
                        log.Add("First run, list does not contain tweets.");
                    }
                }
            }
            else
            {
                log.Add("List " + listName + " does not exist");
            }
        }

        private TimelineItem GenerateTwitterTimelineItem(Network network, Status status, TimelineSocialEntry entry)
        {
            var item = new TimelineItem
            {
                TimelineItemType = TimelineItemType.Twitter,
                CreateDate = status.CreatedAt.ToLocalTime(),
                NetworkId = network.Id,
                Text = status.Text,
                ExtraTypeValue = TimelineItemExtraType.TimelineSocialEntry,
                Extra = JsonConvert.SerializeObject(entry, this.jsonSettings),
            };

            return item;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle PublishFromSocialNetworks
    Publish news from social networks based on users and companies feeds.
";

            registry.Register(
                "PublishFromSocialNetworks",
                "Publish news from social networks based on users and companies feeds.",
                () => new PublishFromSocialNetworks(),
                longDesc);
        }
    }
}
