
namespace Sparkle.Common.CommandLine
{
    using System;
    using System.IO;

    public abstract class BaseSparkleCommand : ISparkleCommand
    {
        private bool disposed;

        protected BaseSparkleCommand()
        {
        }

        /// <summary>
        /// Gets or sets the standard error output stream.
        /// </summary>
        public TextWriter Out { get; set; }

        /// <summary>
        /// Gets or sets the standard input stream.
        /// </summary>
        public TextReader In { get; set; }

        /// <summary>
        /// Gets or sets the standard output stream.
        /// </summary>
        public TextWriter Error { get; set; }

        public bool Simulate { get; set; }

        public void Confirm(SparkleCommandArgs args)
        {
            throw new NotImplementedException();
        }

        public virtual void RunRoot(SparkleCommandArgs args)
        {
        }

        public abstract void RunUniverse(SparkleCommandArgs args);

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
                }
                this.disposed = true;
            }
        }

        #endregion
    }
}
