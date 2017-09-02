
namespace Sparkle.Commands
{
    using Mono.Options;
    using Sparkle.Common.CommandLine;
    using Sparkle.Data.Networks;
    using Sparkle.EmailTemplates;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.UI;
    using Srk.BetaServices.ClientApi;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    class Program
    {
        private static DateTime appStartTime = DateTime.UtcNow;

        static int Main(string[] args)
        {
            Trace.Listeners.Add(new Sparkle.Common.ConsoleTraceListener());

            // TODO: apply culture on a per-user basis
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            InitializeErrorReporting();

            // prepare
            DateTime now = DateTime.Now;
            bool help = false, simulate = false, confirm = false, root = false, debug = false;
            var universes = new List<string>();
            int verbose = 0;
            string inFile = null, outFile = null;////, errFile = null, user = null, traceFile = null, service;

            var set = new OptionSet()
                .Add("?|h|help", "Help summary", o => help = o != null)
                .Add("u|universe=", "Select a target universe (key)", o => universes.AddRange(o.Split(new char[] { ',', ';' }).Where(x => !string.IsNullOrEmpty(x))))
                .Add("s|simulate", o => simulate = o != null)
                .Add("v|verbose", "Increases the debug level", o => verbose++)
                .Add("q|quiet", "Sets the debug level to zero (almost no output)", o => verbose = 0)
                // UNDONE: .Add("c|confirm", "Displays a summary of the command and ask user to continue or cancel.", o => confirm = true)
                // UNDONE: .Add("u|user=", "Run the command as the specified user", o => user = o)
                // UNDONE: .Add("service=", "Run the command as the specified service", o => service = o)
                // UNDONE: .Add("root", "Run the command with all privileges", o => root = o != null)
                .Add("in=", "Defines the input file", o => inFile = o)
                //////.Add("trace=", "Defines the trace file", o => traceFile = o)
                .Add("out=", "Defines the console output file", o => outFile = o)
                //////.Add("err", "Defines the error file", o => errFile = o)
                .Add("now=", "Sets the actual date", o => now = DateTime.Parse(o))
                .Add("debug", "Enables debugger", o => debug = true)
            ;

            // parse CLI arguments
            IList<string> extras;
            try
            {
                extras = set.Parse(args);
            }
            catch (OptionException)
            {
                ShowHelp("Invalid arguments", set);
                return 1;
            }
            catch (FormatException ex)
            {
                ShowHelp("Invalid arguments: " + ex.Message, set);
                return 1;
            }

            // attach debugger if --debug
            if (debug && !Debugger.IsAttached)
            {
                if (!Debugger.Launch() || !Debugger.IsAttached)
                {
                    Console.Error.WriteLine("Debugger could not attach");
                    return 1;
                }
            }

            // show help if /? or --help
            if (help)
            {
                ShowHelp(null, set);
                return 0;
            }

            // don't forget the command, man!
            if (extras.Count == 0)
            {
                ShowHelp("A command must be specified", set);
                return 1;
            }

            SparkleCommandRegistry.Default.In = Console.In;

            Stream outFileStream = null;
            StreamWriter outFileWriter = null;
            try
            {
                if (outFile != null)
                {
                    outFileStream = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    outFileStream.SetLength(0L);
                    outFileWriter = new StreamWriter(outFileStream, Encoding.UTF8);
                    outFileWriter.AutoFlush = true;
                    SparkleCommandRegistry.Default.Out = new CompositeTextWriter(Console.Out, outFileWriter);
                    SparkleCommandRegistry.Default.Error = new CompositeTextWriter(Console.Error, outFileWriter);
                }
                else
                {
                    SparkleCommandRegistry.Default.Out = Console.Out;
                    SparkleCommandRegistry.Default.Error = Console.Error;
                }

                // this super lambda configures data and services for the specified appconfig
                SparkleCommandRegistry.Default.ApplicationConfigurator = (app1, cfg) =>
                {
                    var app = new SparkleNetworksApplication(cfg, () => new DefaultEmailTemplateProvider());
                    using (var services = app.GetNewServiceFactory(new BasicServiceCache()))
                    {
                        NetworkType networkType = services.Networks.GetNetworkType(services.Network.NetworkTypeId);

                        // configure strings
                        try
                        {
                            var source = Sparkle.Services.Main.Networks.SparkleLang.CreateStrings(
                                app.Config.Tree.Storage.SparkleLangDirectory, ////Path.Combine(Environment.CurrentDirectory, "Lang"),
                                app1.UniverseName,
                                networkType.Name);
                            Sparkle.UI.Lang.Source = source;
                            Sparkle.UI.Lang.AvailableCultures = source.AvailableCultures;

                            ////Lang.Source = Strings.Load(Path.Combine(Environment.CurrentDirectory, "Lang"), app.UniverseName, cfg.Tree.DefaultCulture);
                            SparkleCommandRegistry.Default.Out.WriteLine();
                            SparkleCommandRegistry.Default.Out.WriteLine("Loaded resources for " + app1.UniverseName);
                        }
                        catch (InvalidOperationException ex)
                        {
                            SparkleCommandRegistry.EventLogError(string.Join(" ", args) + Environment.NewLine + ex.ToString());
                            SparkleCommandRegistry.Default.Error.WriteLine("Cannot load resources for " + app1.UniverseName);
                            SparkleCommandRegistry.Default.Error.WriteLine(ex.Message);
                            ////return false;
                            return null;
                        }
                    }

                    SparkleCommandRegistry.Default.Out.WriteLine("Data access and services configured for " + app1.UniverseName);
                    ////return true;

                    return app;
                };

                SparkleCommandRegistry.Default.UniverseError += (o, a) =>
                {
                    if (a != null && a.Error != null)
                    {
                        ReportError(a.Error, "Universe: " + a.Universe + Environment.NewLine + "CLI Args: " + string.Join(" ", args));
                    }
                };

                // discover commands
                try
                {
                    SparkleCommandRegistry.Default.DiscoverCommands(Assembly.GetExecutingAssembly());
                }
                catch (Exception ex)
                {
                    SparkleCommandRegistry.EventLogError(string.Join(" ", args) + Environment.NewLine + ex.ToString());
                    Console.Error.WriteLine("Error discovering commands");
                    DebugBreak();
                    ReportError(ex, "SparkleCommandRegistry.Default.DiscoverCommands" + Environment.NewLine + "CLI Args: " + string.Join(" ", args));
                    return 3;
                }

                // special universes?
                if (universes.Count == 0)
                {
                    universes = AppConfiguration.GetUniversesFromConfiguration()
                        .Select(app => app.UniverseName)
                        .ToList();
                }

                // and finally: execute the command
                var command = extras.First();
                try
                {
                    SparkleCommandRegistry.Default.Execute(command, new SparkleCommandArgs
                    {
                        Name = command,
                        Universes = universes,
                        Arguments = extras,
                        Simulation = simulate,
                        InFile = inFile,
                        Confirm = confirm,
                        Options = null,
                        Now = now,
                    });
                }
                catch (ArgumentException ex)
                {
                    SparkleCommandRegistry.EventLogError(string.Join(" ", args) + Environment.NewLine + ex.ToString());
                    Console.Error.WriteLine(ex.Message);
                    ReportError(ex, "SparkleCommandRegistry.Default.Execute" + Environment.NewLine + "CLI Args: " + string.Join(" ", args));
                    DebugBreak();
                    return 1;
                }
                catch (CommandException ex)
                {
                    SparkleCommandRegistry.EventLogError(string.Join(" ", args) + Environment.NewLine + ex.ToString());
                    Console.Error.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.Error.WriteLine(ex.InnerException.Message);
                    ReportError(ex.InnerException ?? ex, "SparkleCommandRegistry.Default.Execute" + Environment.NewLine + "CLI Args: " + string.Join(" ", args));
                    DebugBreak();
                    return 2;
                }
                catch (Exception ex)
                {
                    SparkleCommandRegistry.EventLogError(string.Join(" ", args) + Environment.NewLine + ex.ToString());
                    Console.Error.WriteLine(ex.Message);
                    ReportError(ex, "SparkleCommandRegistry.Default.Execute" + Environment.NewLine + "CLI Args: " + string.Join(" ", args));
                    DebugBreak();
                    return 2;
                }
            }
            finally
            {
                if (outFileWriter != null)
                {
                    outFileWriter.Dispose();
                }

                if (outFileStream != null)
                {
                    outFileStream.Dispose();
                }
            }

            DebugBreak();
            return 0;
        }

        internal static void ReportError(Exception error, string comment)
        {
            var client = BetaservicesClientFactory.Default.CreateDefaultClient();
            if (client == null)
                return;

            var applicationEntryAssembly = typeof(Program).Assembly;
            var entryAssembly = applicationEntryAssembly ?? typeof(Program).Assembly;
            var entryAssemblyTitle = AssemblyTitleAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
            var entryAssemblyVersion = AssemblyFileVersionAttribute.GetCustomAttribute(entryAssembly, typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
            var process = Process.GetCurrentProcess();
            var currentmem = process.PrivateMemorySize64;
            var maxmem = process.PeakWorkingSet64;

            var report = new ErrorReport
            {
                AssemblyName = entryAssemblyTitle != null ? entryAssemblyTitle.Title : "Sparkle.Web",
                AssemblyVersion = entryAssemblyVersion != null ? entryAssemblyVersion.Version : "1.0.0.0",
                OSPlatform = Environment.OSVersion.Platform.ToString(),
                OSVersion = Environment.OSVersion.Version.ToString(),
                AppStartTime = appStartTime,
                AppErrorTime = DateTime.UtcNow,
                Culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name,
                AppCurrentMemoryUsage = currentmem,
                AppPeakMemoryUsage = maxmem,
                Date = DateTime.UtcNow,
            };

            if (error != null)
                report.SetException(error);
            else
                report.SetNonException("no exception");

            if (comment != null)
                report.AppendComment(comment);

            try
            {
                report.DeploymentKind = "CLI App";
                report.AppExitTime = DateTime.UtcNow;
                report.DeploymentInstance = Environment.MachineName;

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (!string.IsNullOrEmpty(report.Comment))
                        report.Comment += Environment.NewLine;
                    report.Comment += "Debugger was attached during report. ";
                }

                int id = client.ReportCrash(report);
                Trace.TraceInformation("Error report #" + id + " sent.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Sparkle.Commands.Program.ReportError: error posting error report" + Environment.NewLine + ex.ToString());
            }
        }

        private static void ShowHelp(string message, OptionSet set)
        {
            Console.WriteLine("Usage: > Sparkle commands");
            Console.WriteLine("       Display a list of available commands.");
            Console.WriteLine("Usage: > Sparkle help {command}");
            Console.WriteLine("       Display description of a command.");
            Console.WriteLine("Usage: > Sparkle [-o[=value]]* {command} [--option[=value]]*");
            Console.WriteLine("       Executes the specified command.");
            Console.WriteLine("");
            
            Console.WriteLine("Available options:");
            set.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");

            if (message != null)
                Console.Error.WriteLine(message);
        }

        private static void DebugBreak()
        {
#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.Read();
#endif
        }

        private static void InitializeErrorReporting()
        {
            // find configuration values
            var url = ConfigurationManager.AppSettings["CrashReport.Url"];
            var key = ConfigurationManager.AppSettings["CrashReport.ApiKey"];
            var isEnabled = ConfigurationManager.AppSettings["CrashReport.IsEnabled"];
            bool isEnabledValue;

            // if configured, then setup the BetaservicesClientFactory to be used.
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(key) && bool.TryParse(isEnabled, out isEnabledValue) && isEnabledValue)
            {
                BetaservicesClientFactory.Default = new BetaservicesClientFactory(key, "Sparkle.Commands", url);
            }
        }
    }
}
