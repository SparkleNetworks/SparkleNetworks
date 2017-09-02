
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Maintenance : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private const string commandName = "Maintenance";

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
                var message = "Simulation is not supported; not running.";
                this.Out.WriteLine(message);
                args.SysLogger.Error(commandName, remoteClient, Environment.UserName, ErrorLevel.Input, message);
                return;
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
            return !this.ParseArgs(context)
                || !this.InitializeServices(context)
                || !this.InitializeNetwork(context)
                || this.DoInitialize(context);
        }

        private bool ParseArgs(Context context)
        {
            return true;
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
            context.Services.Verify();

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

        private bool DoInitialize(Context context)
        {
            context.Services.Initialize();
            return true;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle Maintenance
    Run the maintenance (periodic update of externals data and data structure ponctual update).";

            registry.Register(
                commandName,
                "Run the maintenance (periodic update of externals data and data structure ponctual update).",
                () => new Maintenance(),
                longDesc);
        }

        public class Context
        {
            public SparkleCommandArgs Args { get; set; }

            public DateTime UtcNow { get; set; }

            public IServiceFactory Services { get; set; }
        }
    }
}
