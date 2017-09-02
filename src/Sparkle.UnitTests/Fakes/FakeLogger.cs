
namespace Sparkle.UnitTests.Fakes
{
    using Sparkle.Infrastructure;
    using Sparkle.Services;
    using System;
    using System.Globalization;

    public class FakeLogger : ILogger, IDisposable
    {
        private SysLogger sysLogger;

        public IDisposable OverrideIdentity(ServiceIdentity identity)
        {
            return this;
        }

        protected SysLogger Log
        {
            get { return this.sysLogger ?? (this.sysLogger = SysLogger.NewEmpty); }
        }

        private ServiceIdentity LogIdentity
        {
            get { return ServiceIdentity.Anonymous; }
        }

        public string BasePath { get; set; }

        public string Path { get; set; }

        public string RemoteClient { get; set; }

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
            this.Log.Warning(this.GetPath(path), this.RemoteClient, this.LogIdentity.ToString(), errorLevel, exception != null ? exception.ToString() : null);
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

        public void Dispose()
        {
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
    }
}
