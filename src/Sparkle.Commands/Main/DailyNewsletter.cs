
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Sparkle.Common.CommandLine;
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Objects;
    using Sparkle.Entities.Networks;

    public class DailyNewsletter : NewsletterBase, ISparkleCommandsInitializer
    {
        public override void RunUniverse(SparkleCommandArgs args)
        {
            this.Send(args, NotificationFrequencyType.Daily);
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle DailyNewsletter
    Sends the newsletter to all registered and invited people.

sparkle DailyNewsletter <firstname.lastname> ...
    Sends the newsletter to the specified usernames.

sparkle DailyNewsletter --invited-only
sparkle DailyNewsletter --registered-only
    Sends the newsletter only to the specified target";

            registry.Register(
                "DailyNewsletter",
                "Sends the daily newsletter to all people accepting it.",
                () => new DailyNewsletter(),
                longDesc);
        }
    }
}
