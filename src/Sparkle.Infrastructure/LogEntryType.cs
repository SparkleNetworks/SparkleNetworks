
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using D = System.Diagnostics;

    /// <summary>
    /// Type of log entry.
    /// </summary>
    public class LogEntryType
    {
        private static readonly LogEntryType _critical = new LogEntryType(1, "Critical");
        private static readonly LogEntryType _error = new LogEntryType(2, "Error");
        private static readonly LogEntryType _warning = new LogEntryType(3, "Warning");
        private static readonly LogEntryType _info = new LogEntryType(4, "Info");
        private static readonly LogEntryType _start = new LogEntryType(5, "Start");
        private static readonly LogEntryType _stop = new LogEntryType(6, "Stop");
        private static readonly LogEntryType _verbose = new LogEntryType(7, "Verbose");

        private short value;
        private string name;

        public LogEntryType(short value, string name)
        {
            this.value = value;
            this.name = name;
        }

        /// <summary>
        /// An error causing system instability.
        /// </summary>
        public static LogEntryType Critical { get { return _critical; } }

        /// <summary>
        /// Internal error.
        /// </summary>
        public static LogEntryType Error { get { return _error; } }

        /// <summary>
        /// Alert.
        /// </summary>
        public static LogEntryType Warning { get { return _warning; } }

        /// <summary>
        /// Information.
        /// </summary>
        public static LogEntryType Info { get { return _info; } }

        /// <summary>
        /// A process has started.
        /// </summary>
        public static LogEntryType Start { get { return _start; } }

        /// <summary>
        /// A process has stopped.
        /// </summary>
        public static LogEntryType Stop { get { return _stop; } }

        /// <summary>
        /// Debugging information.
        /// </summary>
        public static LogEntryType Verbose { get { return _verbose; } }

        /// <summary>
        /// The code of this type.
        /// </summary>
        public short Value { get { return this.value; } }

        /// <summary>
        /// The name of this type.
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// Traces the specified entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="message">The message.</param>
        public static void Trace(short id, string message) {
            switch (id) {
                case 1:
                case 2:
                    D.Trace.TraceError(message);
                    break;
                case 3:
                    D.Trace.TraceWarning(message);
                    break;
                case 4:
                case 5:
                case 6:
                    D.Trace.TraceInformation(message);
                    break;
                case 7:
                    D.Debug.WriteLine(message);
                    break;
                default:
                    D.Trace.TraceWarning("Unknown LogEntryType id: " + id + " -- " + message);
                    D.Debug.WriteLine(message);
                    break;
            }
        }
    }
}
