
namespace Sparkle.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Trace listener that ouput lines to the standard Console (with colors).
    /// </summary>
    public class ConsoleTraceListener : TraceListener
    {
        private readonly object consoleLock = new object();
        private ConsoleColor back;
        private ConsoleColor fore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTraceListener"/> class.
        /// </summary>
        public ConsoleTraceListener()
        {
            this.fore = Console.ForegroundColor;
            this.back = Console.BackgroundColor;
        }

        /// <summary>
        /// Write a message to an output
        /// </summary>
        /// <param name="message">Message to log</param>
        public override void Write(string message)
        {
            Console.Write(message);
        }

        /// <summary>
        /// Write a message to an output
        /// </summary>
        /// <param name="message">Message to log</param>
        public override void Write(object data)
        {
            if(data is LogEntry)
                this.TraceData(((LogEntry)data).Severity, (LogEntry)data);
        }

        /// <summary>
        /// Write a message to an output
        /// Followed by a newline
        /// </summary>
        /// <param name="message">Message parameter</param>
        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Write a message to an output
        /// Followed by a newline
        /// </summary>
        /// <param name="message">Object parameter</param>
        public override void WriteLine(object data)
        {
            if (data is LogEntry)
                this.TraceData(((LogEntry)data).Severity, (LogEntry)data);
        }

        /// <summary>
        /// Trace data 
        /// </summary>
        /// <param name="eventType"> Event type of the log</param>
        /// <param name="data"> Data of the log</param>
        public void TraceData(TraceEventType eventType, LogEntry entry)
        {
            lock (this.consoleLock)
            {
                if (!string.IsNullOrEmpty(entry.Category))
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    this.WriteLine("Category : " + entry.Category);
                }

                switch (entry.Severity)
                {
                    case TraceEventType.Critical:
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Error:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case TraceEventType.Information:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Resume:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Start:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Stop:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Suspend:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Transfer:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case TraceEventType.Verbose:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case TraceEventType.Warning:
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                }

                this.WriteLine("Message : " + entry.Message);

                if (entry.ExtendedProperties != null && entry.ExtendedProperties.ContainsKey("Exception"))
                {
                    var ex = entry.ExtendedProperties["Exception"] as Exception;
                    if (ex != null)
                        this.WriteLine("Exception :\n" + ex.ToString());
                }

                Console.ForegroundColor = this.fore;
                Console.BackgroundColor = this.back;
            }
        }

        public class LogEntry
        {
            public string Message { get; set; }
            public string Category { get; set; }
            public TraceEventType Severity { get; set; }
            public Dictionary<string, object> ExtendedProperties { get; set; }

            public LogEntry(object message, string category, TraceEventType severity, Dictionary<string, object> extendedProperties)
            {
                this.Message = message.ToString();
                this.Category = category;
                this.Severity = severity;
                this.ExtendedProperties = extendedProperties;
            }

            public override string ToString()
            {
                return "Category : " + this.Category + "\r\n"
                     + "Message : " + this.Message + "\r\n"
                     + (this.ExtendedProperties != null && this.ExtendedProperties.ContainsKey("Exception") ? "Exception :\r\n" + this.ExtendedProperties["Exception"] : "")
                     + "\r\n";
            }
        }
    }
}
