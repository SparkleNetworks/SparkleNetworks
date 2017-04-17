
namespace Sparkle.Services
{
    using System;
    using Sparkle.Infrastructure;

    /// <summary>
    /// Service level logging object.
    /// </summary>
    public interface ILogger : IDisposable
    {
        IDisposable OverrideIdentity(ServiceIdentity identity);

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
        void Critical(string path, ErrorLevel errorLevel, string dataFormat, params object[] args);

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
        void Error(string path, ErrorLevel errorLevel, string dataFormat, params object[] args);

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
        void Info(string path, ErrorLevel errorLevel, string dataFormat, params object[] args);

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
        void Verbose(string path, ErrorLevel errorLevel, string dataFormat, params object[] args);

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
        void Warning(string path, ErrorLevel errorLevel, string dataFormat, params object[] args);
    }
}
