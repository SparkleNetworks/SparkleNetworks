
namespace Sparkle.Services.Main
{
    using System;
    using System.Globalization;
    using Sparkle.Data.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Data;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using System.Diagnostics;

    public class Logger : ILogger
    {
        private readonly MainServiceFactory serviceFactory;
        private SysLogger sysLogger;
        private bool disposed;
        private HostingEnvironment hosting;

        // this class cannot be instantiated like other services : /
        ////internal Logger(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory) : base(repositoryFactory, serviceFactory)
        ////{
        ////}

        internal Logger(IRepositoryFactory repositoryFactory, MainServiceFactory serviceFactory, SysLogger sysLogger, HostingEnvironment hosting)
        {
            if (serviceFactory == null)
                throw new ArgumentNullException("serviceFactory");
            if (sysLogger == null)
                throw new ArgumentNullException("sysLogger");
            if (hosting == null)
                throw new ArgumentNullException("hosting");

            this.sysLogger = sysLogger;
            this.serviceFactory = serviceFactory;
            this.hosting = hosting;
        }

        protected SysLogger Log
        {
            get { return this.sysLogger; }
            set { this.sysLogger = value; }
        }

        private string RemoteClient
        {
            get { return this.hosting.RemoteClient; }
        }

        private string BasePath
        {
            get { return this.hosting.LogBasePath; }
        }

        private string Path
        {
            get { return this.hosting.LogPath; }
        }

        public ServiceIdentity Identity
        {
            get
            {
                if (this.hosting != null)
                    return this.hosting.Identity;
                if (this.serviceFactory != null)
                    return ((IServiceFactory)this.serviceFactory).HostingEnvironment.Identity;
                return null;
            }
        }

        private ServiceIdentity IdentityOverride { get; set; }

        private ServiceIdentity LogIdentity
        {
            get { return this.IdentityOverride ?? this.Identity; }
        }

        public void Critical(string path, ErrorLevel errorLevel)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel);
        }
        public void Critical(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, data);
        }
        public void Critical(string path, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Critical(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Critical(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Error(string path, ErrorLevel errorLevel)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel);
        }
        public void Error(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, data);
        }
        public void Error(string path, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Error(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Error(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Warning(string path, ErrorLevel errorLevel)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel);
        }
        public void Warning(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, data);
        }
        public void Warning(string path, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Warning(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, exception != null ? exception.ToString() : "no exception");
        }

        public void Info(string path, ErrorLevel errorLevel)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel);
        }
        public void Info(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, data);
        }
        public void Info(string path, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Info(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Info(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, exception != null ? exception.ToString() : null);
        }

        public void Verbose(string path, ErrorLevel errorLevel)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel);
        }
        public void Verbose(string path, ErrorLevel errorLevel, string data)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, data);
        }
        public void Verbose(string path, ErrorLevel errorLevel, string dataFormat, params object[] args)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, string.Format(CultureInfo.InvariantCulture, dataFormat, args));
        }
        public void Verbose(string path, ErrorLevel errorLevel, Exception exception)
        {
            this.Log.Verbose(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, exception != null ? exception.ToString() : null);
        }

        public IDisposable OverrideIdentity(ServiceIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");

            return new LoggerIdentityOverride(this, identity);
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

        public class LoggerIdentityOverride : IDisposable
        {
            private Logger logger;
            private bool disposed;

            public LoggerIdentityOverride(Logger logger, ServiceIdentity identity)
            {
                this.logger = logger;
                this.logger.IdentityOverride = identity;
            }
            
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
                        if (this.logger != null)
                        {
                            this.logger.IdentityOverride = null;
                            this.logger = null;
                        }
                    }

                    this.disposed = true;
                }
            }
        }

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
    }
}
