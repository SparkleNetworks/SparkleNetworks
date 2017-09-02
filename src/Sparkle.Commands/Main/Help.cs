
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Common.CommandLine;

    public class Help : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        private SparkleCommandRegistry registry;

        public Help()
        {
        }

        public Help(SparkleCommandRegistry registry)
        {
            this.registry = registry;
        }

        public override void RunRoot(SparkleCommandArgs args)
        {
            base.RunRoot(args);
            if (args.Arguments.Count == 2)
            {
                var name = args.Arguments[1];
                var mname = name.ToUpperInvariant();
                var cmd = this.registry.Commands.FirstOrDefault(c => c.Name.ToUpperInvariant() == mname);
                if (cmd != null)
                {
                    this.Out.WriteLine("---------------------------");
                    this.Out.WriteLine("Command  " + name);
                    this.Out.WriteLine("---------------------------");
                    this.Out.WriteLine();
                    this.Out.WriteLine(cmd.Description);
                    this.Out.WriteLine();
                    this.Out.WriteLine(cmd.Help);
                }
                else
                {
                    this.Error.WriteLine("No such command");
                }
            }
            else
            {
                this.Out.WriteLine("Specifiy a command name to get help about it.");
            }
        }

        public override void RunUniverse(SparkleCommandArgs args)
        {
            if (args.Arguments.Count == 2)
            {
                var name = args.Arguments[1];
                var mname = name.ToUpperInvariant();
                var cmd = this.registry.Commands.FirstOrDefault(c => c.Name.ToUpperInvariant() == mname);
                if (cmd != null)
                {
                    this.Out.WriteLine("---------------------------");
                    this.Out.WriteLine("Command  " + name);
                    this.Out.WriteLine("---------------------------");
                    this.Out.WriteLine();
                    this.Out.WriteLine(cmd.Description);
                    this.Out.WriteLine();
                    this.Out.WriteLine(cmd.Help);
                }
                else
                {
                    this.Error.WriteLine("No such command");
                }
            }
            else
            {
                this.Out.WriteLine("Specifiy a command name to get help about it.");
            }
        }

        public void Register(SparkleCommandRegistry registry)
        {
            registry.Register(
                "Help",
                "Show help about a command.",
                () => new Help(registry),
                "Asking help about the about the help command... This is funny. You owe a badge for this.");
        }
    }
}
