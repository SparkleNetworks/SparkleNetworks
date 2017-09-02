
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;

    public class BasicStats : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle BasicStats
    Shows basic statistics about all local networks.";

            registry.Register(
                "BasicStats",
                "Shows basic statistics about all local networks.",
                () => new BasicStats(),
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
            var log = args.SysLogger;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "BasicStats";
            services.HostingEnvironment.RemoteClient = this.remoteClient;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("BasicStats", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            services.NetworkId = network.Id;

            this.Out.WriteLine();
            this.Out.WriteLine("======================");
            this.Out.WriteLine(args.Application.UniverseName);
            this.Out.WriteLine("======================");

            this.Out.WriteLine("Universe is " + (args.ApplicationConfiguration.Tree.Site.Enabled ? "ENABLED" : "disabled") + " on this host.");

            var peopleTotal = services.People.CountAll();
            var peopleActive = services.People.CountActive();
            var companies = services.Company.Count();
            var companiesNonEmpty = services.Company.CountNonEmpty();

            this.Out.WriteLine("Users (all):           " + peopleTotal);
            this.Out.WriteLine("Users (active):        " + peopleTotal);
            this.Out.WriteLine("Companies:             " + companies);
            this.Out.WriteLine("Companies (non-empty): " + companiesNonEmpty);
            this.Out.WriteLine();
            this.Out.WriteLine();
        }
    }
}
