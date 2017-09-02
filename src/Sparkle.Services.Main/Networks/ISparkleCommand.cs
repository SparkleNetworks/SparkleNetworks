
namespace Sparkle.Common.CommandLine {
    /// <summary>
    /// Represents a stateless executable command.
    /// </summary>
    public interface ISparkleCommand : ICommand {

        /// <summary>
        /// Indicates whether to execute the command for real or just for fun.
        /// </summary>
        bool Simulate { get; set; }

        /// <summary>
        /// Displays a summary to the user.
        /// </summary>
        /// <param name="args"></param>
        void Confirm(SparkleCommandArgs args);

        /// <summary>
        /// The executable part of the command.
        /// </summary>
        /// <param name="args"></param>
        void RunRoot(SparkleCommandArgs args);

        /// <summary>
        /// The executable part of the command.
        /// </summary>
        /// <param name="args"></param>
        void RunUniverse(SparkleCommandArgs args);
    }
}
