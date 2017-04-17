
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Diagnostics;
    using System.Data;
#if SSC
    using SparkleSystems.Configuration.Data;
#else
    using Sparkle.Infrastructure.Data;
#endif

    /// <summary>
    /// The business system logger.
    /// </summary>
    public class SysLogger : IDisposable
    {
        private readonly int applicationId;
        private readonly string applicationVersion;
        private ISysLogRepository repo;
        private bool disposed;

        /// <summary>
        /// Creates a logger for the specified application.
        /// </summary>
        /// <param name="applicationId">the application id</param>
        /// <param name="applicationVersion">the application version</param>
        public SysLogger(int applicationId, string applicationVersion, ISysLogRepository repo)
        {
            this.repo = repo;
            this.applicationId = applicationId;
            this.applicationVersion = applicationVersion;

            this.Check();
        }

        public static SysLogger NewEmpty
        {
            get { return new SysLogger(0, "0.0.0.0", new FakeSysLogRepository()); }
        }

        /// <summary>
        /// Returns the anonymous user identity.
        /// </summary>
        public static string AnonymousIdentity { get { return "-anonymous-"; } }

        public static string NoIdentity { get { return "-no-identity-"; } }

        /// <summary>
        /// Returns the root user identity.
        /// </summary>
        public static string RootIdentity { get { return "-root-"; } }

        protected ISysLogRepository Repository
        {
            get
            {
                if (this.disposed)
                    throw new ObjectDisposedException("SysLogger");
                return this.repo;
            }
        }

        private void Write(short logEntryType, string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            if (data != null)
                LogEntryType.Trace(logEntryType, (remoteClient ?? "-") + " " + (identity ?? "-") + " " + DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + " " + errorLevel.Name +" " + path + Environment.NewLine + data);
            else
                LogEntryType.Trace(logEntryType, (remoteClient ?? "-") + " " + (identity ?? "-") + " " + DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + " " + errorLevel.Name + " " + path);

            this.Repository.Write(logEntryType, this.applicationId, this.applicationVersion, path, remoteClient, identity, errorLevel.Value, data);
        }

        public void Critical(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Critical.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Critical(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Critical.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Critical(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Critical.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Critical(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Critical.Value, path, remoteClient, identity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Error(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Error.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Error(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Error.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Error(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Error.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Error(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Error.Value, path, remoteClient, identity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Warning(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Warning.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Warning(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Warning.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Warning(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Warning.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Warning(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Warning.Value, path, remoteClient, identity, errorLevel, exception != null ? exception.ToString() : "no exception");
        }

        public void Info(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Info.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Info(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Info.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Info(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Info.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Info(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Info.Value, path, remoteClient, identity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Start(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Start.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Start(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Start.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Start(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Start.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Start(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Start.Value, path, remoteClient, identity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Stop(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Stop.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Stop(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Stop.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Stop(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Stop.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Stop(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Stop.Value, path, remoteClient, identity, errorLevel, exception != null ? exception != null ? exception.ToString() : null : null);
        }

        public void Verbose(string path, string remoteClient, string identity, ErrorLevel errorLevel)
        {
            this.Write(LogEntryType.Verbose.Value, path, remoteClient, identity, errorLevel, null);
        }
        public void Verbose(string path, string remoteClient, string identity, ErrorLevel errorLevel, string data)
        {
            this.Write(LogEntryType.Verbose.Value, path, remoteClient, identity, errorLevel, data);
        }
        public void Verbose(string path, string remoteClient, string identity, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Write(LogEntryType.Verbose.Value, path, remoteClient, identity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Verbose(string path, string remoteClient, string identity, ErrorLevel errorLevel, Exception exception)
        {
            this.Write(LogEntryType.Verbose.Value, path, remoteClient, identity, errorLevel, exception != null ? exception.ToString() : null);
        }

        #region IDisposable members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.repo != null)
                    {
                        this.repo.Dispose();
                        this.repo = null;
                    }
                }

                this.disposed = true;
            }
        }

        #endregion

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void Check()
        {
            // TODO: VERIFY THE LOGGER HERE
        }
    }
}
