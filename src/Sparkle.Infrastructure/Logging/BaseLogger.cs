
#if SSC
namespace SparkleSystems.Configuration.Logging
#else
namespace Sparkle.Infrastructure.Logging
#endif
{
    using System;
    using System.Globalization;
#if SSC
    using SparkleSystems.Configuration;
#else
    using Sparkle.Infrastructure;
#endif

    /// <summary>
    /// Simple implementation of <see cref="ILogger"/> that supports an identity provider to trace user/process identity.
    /// </summary>
    public class BaseLogger : ILogger
    {
        private SysLogger sysLogger;
        private bool disposed;
        private Func<string> identityProvider;

        public BaseLogger(SysLogger sysLogger, Func<string> identityProvider)
        {
            if (sysLogger == null)
                throw new ArgumentNullException("sysLogger");
            if (identityProvider == null)
                throw new ArgumentNullException("identityProvider");

            this.sysLogger = sysLogger;
            this.identityProvider = identityProvider;
        }

        protected SysLogger Log
        {
            get { return this.sysLogger; }
            set { this.sysLogger = value; }
        }

        public string RemoteClient { get; set; }

        public string BasePath { get; set; }

        public string Path { get; set; }

        private Func<string> IdentityProvider
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.identityProvider = value;
            }
        }

        private string LogIdentity
        {
            get { return this.identityProvider(); }
        }

        public void Critical(string path, ErrorLevel errorLevel)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel);
        }
        public void Critical(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, data);
        }
        public void Critical(string path, ErrorLevel errorLevel, string dataFormat, params string[] args)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Critical(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Error(string path, ErrorLevel errorLevel)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel);
        }
        public void Error(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, data);
        }
        public void Error(string path, ErrorLevel errorLevel, string dataFormat, params string[] args)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Error(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Warning(string path, ErrorLevel errorLevel)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel);
        }
        public void Warning(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, data);
        }
        public void Warning(string path, ErrorLevel errorLevel, string dataFormat, params string[] args)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Warning(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, exception != null ? exception.ToString() : "no exception");
        }

        public void Info(string path, ErrorLevel errorLevel)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel);
        }
        public void Info(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, data);
        }
        public void Info(string path, ErrorLevel errorLevel, string dataFormat, params string[] args)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Info(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Verbose(string path, ErrorLevel errorLevel)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel);
        }
        public void Verbose(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, data);
        }
        public void Verbose(string path, ErrorLevel errorLevel, string dataFormat, params string[] args)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Verbose(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity, errorLevel, exception != null ? exception.ToString() : null);
        }

        private string GetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The value cannot be empty", "path");

            if (this.BasePath != null)
            {
                if (path != null)
                    return this.BasePath + ", " + path;
                else
                    return this.BasePath;
            }
            else
            {
                return path;
            }
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
                    if (this.sysLogger != null)
                        this.sysLogger.Dispose();
                    this.sysLogger = null;
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
