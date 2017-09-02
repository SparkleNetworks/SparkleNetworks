
namespace Sparkle.Commands.Main
{
    using System;
    using Sparkle.Common.CommandLine;
    using Sparkle.Entities.Networks;

    public class WeeklyGroupNewsletter : GroupNewsletterBase, ISparkleCommandsInitializer
    {
        public override void RunUniverse(SparkleCommandArgs args)
        {
            DateTime end = args.Now.Date;
            DateTime start = end.AddDays(-7D);

            this.Send(args, start, end, NotificationFrequencyType.Weekly);
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle WeeklyGroupNewsletter
    Sends the group newsletter to all appropriate people.
";

            registry.Register(
                "WeeklyGroupNewsletter",
                "Sends the weekly group newsletter to all people accepting it.",
                () => new WeeklyGroupNewsletter(),
                longDesc);
        }
    }
}
