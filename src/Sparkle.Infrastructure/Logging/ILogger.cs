
#if SSC
namespace SparkleSystems.Configuration.Logging
#else
namespace Sparkle.Infrastructure.Logging
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Service level logging object.
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Gets or sets the base execution path.
        /// </summary>
        /// <value>
        /// The base execution path.
        /// </value>
        string BasePath { get; set; }

        /// <summary>
        /// Gets or sets the execution path.
        /// </summary>
        /// <value>
        /// The execution path.
        /// </value>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the remote client address.
        /// </summary>
        /// <value>
        /// The remote client address.
        /// </value>
        string RemoteClient { get; set; }

        /// <summary>
        /// Logs an error causing system instability.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Critical(string path, ErrorLevel errorLevel);

        /// <summary>
        /// Logs an error causing system instability.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Critical(string path, ErrorLevel errorLevel, Exception exception);

        /// <summary>
        /// Logs an error causing system instability.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Critical(string path, ErrorLevel errorLevel, string data);

        /// <summary>
        /// Logs an error causing system instability.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Critical(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);

        /// <summary>
        /// Logs an Internal error.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Error(string path, ErrorLevel errorLevel);

        /// <summary>
        /// Logs an Internal error.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Error(string path, ErrorLevel errorLevel, Exception exception);

        /// <summary>
        /// Logs an Internal error.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Error(string path, ErrorLevel errorLevel, string data);

        /// <summary>
        /// Logs an Internal error.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Error(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);

        /// <summary>
        /// Logs an Information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Info(string path, ErrorLevel errorLevel);

        /// <summary>
        /// Logs an Information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Info(string path, ErrorLevel errorLevel, Exception exception);

        /// <summary>
        /// Logs an Information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Info(string path, ErrorLevel errorLevel, string data);

        /// <summary>
        /// Logs an Information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Info(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);

        ////void Start(string path, ErrorLevel errorLevel);
        ////void Start(string path, ErrorLevel errorLevel, Exception exception);
        ////void Start(string path, ErrorLevel errorLevel, string data);
        ////void Start(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);
        ////void Stop(string path, ErrorLevel errorLevel);
        ////void Stop(string path, ErrorLevel errorLevel, Exception exception);
        ////void Stop(string path, ErrorLevel errorLevel, string data);
        ////void Stop(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);

        /// <summary>
        /// Logs Debugging information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Verbose(string path, ErrorLevel errorLevel);

        /// <summary>
        /// Logs Debugging information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Verbose(string path, ErrorLevel errorLevel, Exception exception);

        /// <summary>
        /// Logs Debugging information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Verbose(string path, ErrorLevel errorLevel, string data);

        /// <summary>
        /// Logs Debugging information.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Verbose(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);

        /// <summary>
        /// Logs an Alert.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Warning(string path, ErrorLevel errorLevel);

        /// <summary>
        /// Logs an Alert.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Warning(string path, ErrorLevel errorLevel, Exception exception);

        /// <summary>
        /// Logs an Alert.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Warning(string path, ErrorLevel errorLevel, string data);

        /// <summary>
        /// Logs an Alert.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="errorLevel">The error level.</param>
        void Warning(string path, ErrorLevel errorLevel, string dataFormat, params string[] args);
    }
}
