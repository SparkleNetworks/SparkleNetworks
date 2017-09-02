
namespace Sparkle.Common.CommandLine
{
    using System.IO;

    /// <summary>
    /// Represents a stateless executable command.
    /// </summary>
    public interface ICommand : System.IDisposable
    {
        /// <summary>
        /// Gets or sets the standard error output stream.
        /// </summary>
        TextWriter Out { get; set; }

        /// <summary>
        /// Gets or sets the standard input stream.
        /// </summary>
        TextReader In { get; set; }

        /// <summary>
        /// Gets or sets the standard output stream.
        /// </summary>
        TextWriter Error { get; set; }
    }
}
