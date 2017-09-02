
namespace Sparkle.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Holds disposable objects for mass disposal.
    /// </summary>
    public class DataDisposable : IDisposable
    {
        private readonly List<IDisposable> collection = new List<IDisposable>();

        /// <summary>
        /// Adds a disposable object and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposable"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public T Add<T>(T disposable)
            where T : IDisposable
        {
            if (disposable != null)
                this.collection.Add(disposable);
            return disposable;
        }

        /// <returns></returns>
        [DebuggerStepThrough]
        public void Add<T>(IDisposable disposable)
        {
            if (disposable != null)
                this.collection.Add(disposable);
        }

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.collection.ForEach(i =>
                {
                    if (i != null)
                        i.Dispose();
                });
                this.collection.Clear();
            }
        }

        #endregion
    }
}
