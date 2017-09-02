
namespace Sparkle.Commands.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Common.CommandLine;

    public class MainCommands : ISparkleCommandsInitializer
    {
        public void Register(SparkleCommandRegistry registry)
        {
            registry.Register(
                "RecallMail",
                "",
                () => new RecallMail());
            registry.Register(
                "MailChimpUpdater",
                "Synchronizes members email addresses with a mailchimp service",
                () => new MailChimpUpdater());
        }
    }
}
