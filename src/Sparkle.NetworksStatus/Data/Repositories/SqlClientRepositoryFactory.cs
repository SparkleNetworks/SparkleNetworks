
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;

    partial class SqlClientRepositoryFactory : Sparkle.NetworksStatus.Domain.Disposable, IRepositoryFactory, IDisposable
    {
        /// <summary>
        /// Indicates the object was disposed.
        /// </summary>
        private bool disposed;

        private string connectionString;

        private PetaContext petaContext;

        private bool isTransaction;

        public SqlClientRepositoryFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public PetaContext PetaContext
        {
            get { return this.petaContext ?? (this.petaContext = this.NewPetaContext); }
        }

        public PetaContext NewPetaContext
        {
            get { return this.disposable.Add(new PetaContext(this.connectionString, "System.Data.SqlClient")); }
        }

        public IRepositoryFactory BeginTransaction()
        {
            if (this.isTransaction)
                throw new InvalidOperationException("Already in transaction. Cannot begin transaction.");

            this.isTransaction = true;
            this.PetaContext.BeginTransaction();
            return this;
        }

        public IRepositoryFactory CompleteTransaction()
        {
            if (!this.isTransaction)
                throw new InvalidOperationException("Not in transaction. Cannot complete transaction.");

            this.isTransaction = false;
            this.PetaContext.CompleteTransaction();
            return this;
        }

        public IRepositoryFactory AbortTransaction()
        {
            if (!this.isTransaction)
                throw new InvalidOperationException("Not in transaction. Cannot abort transaction.");

            this.isTransaction = false;
            this.PetaContext.AbortTransaction();
            return this;
        }

        public void ClearTransaction()
        {
            if (!this.isTransaction)
                return;

            this.isTransaction = false;
            this.PetaContext.AbortTransaction();
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
        /// Releases managed and - optionally - unmanaged resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.disposable.Dispose();
                }

                this.disposed = true;
            }
        }

        #endregion

    }
}
