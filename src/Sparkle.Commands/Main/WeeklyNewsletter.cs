
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

    public class WeeklyNewsletter : NewsletterBase, ISparkleCommandsInitializer
    {
        public override void RunUniverse(SparkleCommandArgs args)
        {
            this.Send(args, NotificationFrequencyType.Weekly);
        }

        public void Register(SparkleCommandRegistry registry)
        {
            string longDesc = @"Command usage:

sparkle WeeklyNewsletter
    Sends the newsletter to all registered and invited people.

sparkle WeeklyNewsletter <firstname.lastname> ...
    Sends the newsletter to the specified usernames.

sparkle WeeklyNewsletter --invited-only
sparkle WeeklyNewsletter --registered-only
    Sends the newsletter only to the specified target";

            registry.Register(
                "WeeklyNewsletter",
                "Sends the weekly newsletter to all people accepting it.",
                () => new WeeklyNewsletter(),
                longDesc);
        }
    }
}
