
namespace Sparkle.Common.CommandLine
{
    using System.Collections.Generic;
    using Sparkle.Infrastructure;
    using System;

    /// <summary>
    /// Arguments container to execute a <see cref="ISparkleCommand"/>.
    /// </summary>
    public class SparkleCommandArgs
    {
        /// <summary>
        /// The command name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of universes.
        /// </summary>
        public IList<string> Universes { get; set; }

        /// <summary>
        /// Raw options.
        /// </summary>
        public Dictionary<string, IList<string>> Options { get; set; }

        /// <summary>
        /// Raw arguments, including the first (which is the command name).
        /// </summary>
        public IList<string> Arguments { get; set; }

        /// <summary>
        /// Indicates whether to execute the command for real or just for fun.
        /// </summary>
        public bool Simulation { get; set; }

        /// <summary>
        /// The input file path
        /// </summary>
        public string InFile { get; set; }

        /// <summary>
        /// Displays a summary of the command and ask user to continue or cancel.
        /// </summary>
        public bool Confirm { get; set; }

        ///// <summary>
        ///// The syslogger is automatically attached for provide logging capability.
        ///// </summary>
        public SysLogger SysLogger { get; internal set; }

        /// <summary>
        /// This is automatically filled with the application information.
        /// </summary>
        public Application Application { get; internal set; }

        public AppConfiguration ApplicationConfiguration { get; internal set; }

        /// <summary>
        /// Gets or sets the actual date.
        /// </summary>
        /// <value>
        /// The actual date.
        /// </value>
        public DateTime Now { get; set; }

        public Services.Main.Networks.SparkleNetworksApplication App { get; set; }
    }
}
