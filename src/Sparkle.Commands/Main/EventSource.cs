
namespace Sparkle.Commands.Main
{
    using System;
    using System.Diagnostics;
    using Sparkle.Common.CommandLine;

    public class EventSource : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Arguments.Count == 3)
            {
                var command = args.Arguments[1].ToUpperInvariant();
                var name = args.Arguments[2];

                switch (command)
                {
                    case "CREATE":
                        try
                        {
                            bool exists = EventLog.SourceExists(name);
                            if (exists)
                            {
                                this.Out.WriteLine("Source '" + name + "' already exists.");
                            }
                            else
                            {
                                EventLog.CreateEventSource(name, "Application");
                                this.Out.WriteLine("Source '" + name + "' created.");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Error.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "DELETE":
                        try
                        {
                            bool exists = EventLog.SourceExists(name);
                            if (!exists)
                            {
                                this.Out.WriteLine("Source '" + name + "' does not exist.");
                            }
                            else
                            {
                                EventLog.DeleteEventSource(name, "Application");
                                this.Out.WriteLine("Source '" + name + "' deleted.");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Error.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    case "STATE":
                        try
                        {
                            bool exists = EventLog.SourceExists(name);
                            if (exists)
                                this.Out.WriteLine("Source '" + name + "' exists.");
                            else
                                this.Out.WriteLine("Source '" + name + "' does not exist.");
                        }
                        catch (Exception ex)
                        {
                            this.Error.WriteLine("Error: " + ex.Message);
                        }
                        break;
                    default:
                        break;
                }

            }
            else
            {
                this.Out.WriteLine("Invalid arguments");
            }
        }

        public void Register(SparkleCommandRegistry registry)
        {
            var desc = @"EventSource Create <sourcename>
    to install <sourcename> as a event viewer source
EventSource Delete <sourcename>
    to uninstall <sourcename> as a event viewer source
EventSource State <sourcename>
    to display wheter <sourcename> is a valid event source";
            registry.Register("EventSource", "Manages event viewer sources.", () => new EventSource(), desc);
        }
    }
}
