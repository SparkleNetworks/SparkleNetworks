
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Holds disposable objects for mass disposal.
    /// </summary>
    public class CompositeDisposable : IDisposable
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

        [DebuggerStepThrough]
        public void Add(IDisposable disposable)
        {
            if (disposable != null)
                this.collection.Add(disposable);
        }

        #region IDisposable members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var copy = this.collection.ToList();
                this.collection.Clear();

                copy.ForEach(i =>
                {
                    if (i != null)
                        i.Dispose();
                });
            }
        }

        #endregion
    }
}
