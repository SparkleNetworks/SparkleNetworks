
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Sparkle.Commands.Main.Import;
    using Sparkle.Common.CommandLine;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;

    public class InteractiveImport : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private string remoteClient;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.None,
        };

        public override void RunUniverse(SparkleCommandArgs args)
        {
            var project = new ImportProject();
            var context = new Context
            {
                Args = args,
            };

            if (args.Application == null)
            {
                this.Error.WriteLine("No universe selected");
                return;
            }

            this.remoteClient = Environment.MachineName + "/" + System.Diagnostics.Process.GetCurrentProcess().Id;
            var services = args.App.GetNewServiceFactoryWithoutCache();
            services.HostingEnvironment.Identity = ServiceIdentity.Unsafe.Root;
            services.HostingEnvironment.LogBasePath = "InteractiveImport";
            services.HostingEnvironment.RemoteClient = this.remoteClient;
            this.Out.WriteLine();
            context.Services = services;

            // network
            string networkName = args.ApplicationConfiguration.Tree.NetworkName ?? args.Application.UniverseName;
            var network = services.Networks.GetByName(networkName);
            if (network == null)
            {
                this.Out.WriteLine("Network \"" + networkName + "\" does not exist; not running.");
                args.SysLogger.Info("InteractiveImport", remoteClient, Environment.UserName, ErrorLevel.Input, "Network \"" + networkName + "\" does not exist; not running.");
                return;
            }

            // simulation: not supported
            if (args.Simulation)
            {
                this.Out.WriteLine("Simulation is not supported; not runnin'");
                args.SysLogger.Info("InteractiveImport", remoteClient, Environment.UserName, ErrorLevel.Success, "Simulation is not supported; not running.");
                return;
            }

            services.NetworkId = network.Id;

            throw new NotImplementedException("This is not yet ready.");

            if (!this.ParseArgs(context, project))
                return;

            




        }

        private bool ParseArgs(Context context, ImportProject project)
        {
            var filePaths = new List<string>();
            context.ImportId = "import-" + DateTime.Now.ToString("yyyMMdd-HHmmss");
            context.ImportFolder = Path.Combine(Environment.CurrentDirectory, context.ImportId);
            context.InputFiles = new List<string>();

            project.CurrentDirectory = Environment.CurrentDirectory;
            project.CurrentMachine = Environment.MachineName;

            const string loadParam = "load";

            foreach (var arg in context.Args.Arguments)
            {
                if (arg.ToLowerInvariant() == "InteractiveImport".ToLowerInvariant())
                {
                }
                else if (arg.StartsWith("--"))
                {
                    var eqIndex = arg.IndexOf('=');
                    var carg = arg.Substring(2, eqIndex > 0 ? eqIndex - 2 : arg.Length - 2);
                    var carglo = carg.ToLowerInvariant();
                    var cargValue = eqIndex > 0 ? arg.Substring(eqIndex) : null;

                    if (carglo == loadParam.ToLowerInvariant())
                    {
                        if (context.LoadProject != null)
                        {
                            return this.EndWithError(context, ErrorLevel.Input, "Cannot load multiple projects");
                        }
                        else if (File.Exists(cargValue))
                        {
                            context.LoadProject = cargValue;
                        }
                    }
                    else
                    {
                        return this.EndWithError(context, ErrorLevel.Input, "Invalid parameter '" + arg + "'");
                    }
                }
                else if (File.Exists(arg))
                {
                    context.InputFiles.Add(arg);
                }
                else
                {
                    return this.EndWithError(context, ErrorLevel.Input, "File not found '" + arg + "'");
                }
            }

            return true;
        }

        private bool EndWithError(Context context, ErrorLevel errorLevel, string message)
        {
            this.Out.WriteLine(message);
            context.Args.SysLogger.Info("InteractiveImport", remoteClient, Environment.UserName, errorLevel, message);
            return false;
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle InteractiveImport
    Imports data interactively.
";

            registry.Register(
                "InteractiveImport",
                "Imports data interactively.",
                () => new InteractiveImport(),
                longDesc);
        }

        public class Context
        {

            public IServiceFactory Services { get; set; }

            public SparkleCommandArgs Args { get; set; }

            public string ImportId { get; set; }

            public string ImportFolder { get; set; }

            public List<string> InputFiles { get; set; }

            public string LoadProject { get; set; }
        }
    }
}
