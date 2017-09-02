
namespace Sparkle.Common.CommandLine
{
    /// <summary>
    /// An object able to register one or many command.
    /// </summary>
    public interface ISparkleCommandsInitializer
    {
        void Register(SparkleCommandRegistry registry);
    }
}
