
namespace Sparkle.Common.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.Main.Networks;

    public class SparkleCommandRegistry : CommandRegistry<SparkleCommandArgs, ISparkleCommand>
    {
        private static SparkleCommandRegistry _default;

        public SparkleCommandRegistry()
        {
            this.Executor = new Action<Container<ISparkleCommand>, ISparkleCommand, SparkleCommandArgs>(this.CustomExecutor);
        }

        public event EventHandler<SparkleCommandErrorEventArgs> UniverseError;

        /// <summary>
        /// The default instance.
        /// Lazy instantiation.
        /// </summary>
        public static SparkleCommandRegistry Default
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _default ?? (_default = new SparkleCommandRegistry()); }
        }

        public Func<Application, AppConfiguration, SparkleNetworksApplication> ApplicationConfigurator { get; set; }

        public static void EventLogError(string message)
        {
            EventLog.WriteEntry("Sparkle.Commands", message, EventLogEntryType.Error);
        }

        public static void EventLogInfo(string message)
        {
            EventLog.WriteEntry("Sparkle.Commands", message, EventLogEntryType.Information);
        }

        public static void EventLogWarning(string message)
        {
            EventLog.WriteEntry("Sparkle.Commands", message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Discovers commands from an assembly.
        /// Searches for classes implementing <see cref="ISparkleCommandsInitializer"/>.
        /// </summary>
        /// <param name="assembly">the assembly</param>
        public void DiscoverCommands(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var types = assembly.GetTypes();
            var @interface = typeof(ISparkleCommandsInitializer);
            foreach (var type in types)
            {
                if (type.IsClass && type.GetInterfaces().Any(i => i.Equals(@interface)))
                {
                    var init = Activator.CreateInstance(type) as ISparkleCommandsInitializer;
                    init.Register(this);
                }
            }
        }

        protected override ISparkleCommand CreateCommand(string name, Container<ISparkleCommand> container, SparkleCommandArgs options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            var instance = base.CreateCommand(name, container, options);
            instance.Simulate = options.Simulation;

            return instance;
        }

        protected override void PrintCommandHelp(SparkleCommandArgs options)
        {
            var commandName = options.Arguments.Where(a => a.ToLowerInvariant() != "help").SingleOrDefault();
            if (commandName != null)
            {
                var command = this.Commands.SingleOrDefault(c => c.Name.ToLowerInvariant() == commandName.ToLowerInvariant());
                if (command != null)
                {
                    this.Out.WriteLine(command.Help);
                }
                else
                {
                    this.Out.WriteLine("No such command");
                }
            }
            else
            {
                this.Out.WriteLine("No such command");
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "the executor knows nothing about the command")]
        private void CustomExecutor(Container<ISparkleCommand> container, ISparkleCommand command, SparkleCommandArgs args)
        {
            ////if (args.Universes.Count == 0)
            ////{
            ////    command.RunUniverse(args);
            ////    return;
            ////}

            if (this.ApplicationConfigurator == null)
            {
                this.Error.WriteLine("CommandRegistry not configured.");
                return;
            }

            string name = container.Name;
            var pid = Process.GetCurrentProcess().Id;
            string remoteClient = Environment.MachineName + "/" + pid;

            var applications = GetApplicationsByUniverses(args.Universes);

            /****
             *  Version 2.
             * One logger open all the way. Might be wrong... If it fails, it's not recoverable.
             * Perhaps the idea would be to make the syslogger auto-recoverable?
             ****/
            
            foreach (var config in applications)
            {
                SysLogger log = null;
                try
                {
                    var site = config.Application;
                    if (site.MainStatusValue < 1 && site.MainStatus != ApplicationStatus.DownForMaintenance)
                    {
                        this.Out.WriteLine("App/universe " + site.UniverseName + " is disabled (" + site.MainStatus + "). Will not perform command.");
                        continue;
                    }

                    var app = this.ApplicationConfigurator(site, config);
                    if (app == null)
                    {
                        this.Error.WriteLine("Universe not found: " + site.UniverseName);
                        continue;
                    }

                    this.Out.WriteLine("Current universe is: " + site.UniverseName + "; Application id: " + config.Application.Id);

                    // TODO: set Sparkle.Commands version here
                    string version = "1.0.0.0";
                    var sysloggerFactory = new Func<SysLogger>(() =>
                    {
                        var sysloggerRepository = new SqlSysLogRepository(config.Tree.ConnectionStrings.SysLogger);
                        var syslogger = new SysLogger(config.Application.Id, AppVersion.Full, sysloggerRepository);
                        return syslogger;
                    });
                    log = args.Simulation ? SysLogger.NewEmpty : sysloggerFactory();

                    if (!args.Simulation)
                        log.Start(name, remoteClient, Environment.UserName, ErrorLevel.Success);

                    args.Application = site;
                    args.ApplicationConfiguration = config;
                    args.SysLogger = log;

                    args.App = app;

                    command.RunUniverse(args);

                    if (!args.Simulation)
                        log.Stop(name, remoteClient, Environment.UserName, ErrorLevel.Success);
                }
                catch (Exception ex)
                {
                    string universeName = config != null ? config.Application != null ? config.Application.UniverseName : "" : "";
                    this.Error.WriteLine("Execution failed for universe '" + universeName + "'");
                    this.Error.WriteLine(ex.GetType() + ": " + ex.Message);
                    this.Error.WriteLine(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        this.Error.WriteLine(ex.InnerException.GetType() + ": " + ex.InnerException.Message);
                        this.Error.WriteLine(ex.InnerException.StackTrace);
                    }

                    if (!args.Simulation)
                    {
                        if (log != null)
                        {
                            log.Stop(name, remoteClient, Environment.UserName, ErrorLevel.Internal, ex);
                        }
                        else
                        {
                            Trace.TraceError(ex.ToString());
                        }
                    }

                    var handler = this.UniverseError;
                    if (handler != null)
                    {
                        var eargs = new SparkleCommandErrorEventArgs
                        {
                            Error = ex,
                            Universe = universeName,
                        };
                        handler(this, eargs);
                    }
                }
                finally
                {
                    if (log != null)
                    {
                        log.Dispose();
                    }
                }
            }
            

            /****
             *  Version 3.
             * 
             * One logger open all the way. Might be wrong... If it fails, it's not recoverable.
             * Perhaps the idea would be to make the syslogger auto-recoverable?
             * 
             * Support multiapplication commands
             ****/
            ////command.RunRoot(args);
        }

        private static IEnumerable<AppConfiguration> GetApplicationsByUniverses(IEnumerable<string> universes)
        {
            return AppConfiguration.CreateManyFromConfiguration(universes);
        }
    }
}
