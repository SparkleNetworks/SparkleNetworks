
namespace Sparkle.Commands.Main
{
    using Sparkle.Common.CommandLine;
    using Sparkle.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LangTest : BaseSparkleCommand, ISparkleCommandsInitializer
    {
        public override void RunUniverse(SparkleCommandArgs args)
        {
            var baseKeys = new string[]
            {
                "CommonLevelResourceName",
                "NetworkTypeLevelResourceName",
                "NetworkLevelResourceName",
                "AdherentCompany",
                "NetworkType",
                "AppNameKey",
                "AppName",
                "AppDomain",
            };

            var keys = new List<string>(8);

            if (args.Arguments.Count < 2)
            {
                keys.AddRange(baseKeys);
            }
            else
            {
                for (int i = 1; i < args.Arguments.Count; i++)
                {
                    keys.Add(args.Arguments[i]);
                }
            }

            var values = new List<string>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                values.Add(Lang.T(keys[i]));
            }

            var makKeyLength = keys.Max(p => p.Length);
            var makValueLength = values.Max(p => p.Length);

            this.Out.WriteLine();
            for (int i = 0; i < keys.Count; i++)
            {
                this.Out.WriteLine(
                    keys[i].PadRight(makKeyLength) + " | " +
                    values[i]);
            }

            this.Out.WriteLine();
        }

        public void Register(SparkleCommandRegistry registry)
        {
            registry.Register(
                "LangTest",
                "LangTest",
                () => new LangTest(),
                null);
        }
    }
}
