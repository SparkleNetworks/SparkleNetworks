
namespace Sparkle.Commands.Main
{
    using System;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;

    public class DailyGroupNewsletter : GroupNewsletterBase, ISparkleCommandsInitializer
    {
        public override void RunUniverse(SparkleCommandArgs args)
        {
            DateTime end = args.Now.Date;
            DateTime start = end.AddDays(-1D);

            this.Send(args, start, end, NotificationFrequencyType.Daily);
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle DailyGroupNewsletter
    Sends the group newsletter to all appropriate people.
";

            registry.Register(
                "DailyGroupNewsletter",
                "Sends the daily group newsletter to all people accepting it.",
                () => new DailyGroupNewsletter(),
                longDesc);
        }
    }
}
