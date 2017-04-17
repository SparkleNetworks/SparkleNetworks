
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using SrkToolkit;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.EntityClient;

    public class EntityDataTransaction : IDataTransaction
    {
        private NetworksEntities entities;
        private Func<NetworksEntities> entitiesfactory;
        private IRepositoryFactory factory;
        private bool isDisposed;
        private bool isComplete;

        public EntityDataTransaction(NetworksEntities entities, Func<NetworksEntities> factory)
        {
            this.entities = entities;
            this.entitiesfactory = factory;
        }

        protected NetworksEntities model
        {
            get
            {
                this.CheckDisposed();
                return this.entities;
            }
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        public IRepositoryFactory Repositories
        {
            get
            {
                this.CheckDisposed();

                {
                    var entities = new NetworksEntities((System.Data.EntityClient.EntityConnection)this.entities.Connection, true);
                    return this.factory ?? (this.factory = new EntityRepositoryFactory(entities, this.entitiesfactory));
                }

                // this code is nicer to the eye but it creates SQL deadlocks :'(
                ////if (this.factory != null)
                ////{
                ////    return this.factory;
                ////}
                ////else
                ////{
                ////    var entities = this.entities ?? new NetworksEntities((System.Data.EntityClient.EntityConnection)this.entities.Connection, true);
                ////    return this.factory = new EntityRepositoryFactory(entities, this.entitiesfactory);
                ////}
            }
        }

        public DbConnection EntityConnection
        {
            get { return this.entities.Connection; }
        }

        public DbConnection StoreConnection
        {
            get { return ((EntityConnection)this.entities.Connection).StoreConnection; }
        }

        public IDisposable BeginTransaction(IsolationLevel level = IsolationLevel.Unspecified)
        {
            this.Repositories.BeginTransaction(level);
            return new DisposableOnce(() =>
            {
                this.AbortTransaction(false);
                this.Dispose();
            });
        }

        public void CompleteTransaction()
        {
            this.model.SaveChanges();
            this.Repositories.CompleteTransaction();
            this.isComplete = true;
        }

        public void AbortTransaction()
        {
            this.Repositories.AbortTransaction();
        }

        public void AbortTransaction(bool throwIfAlreadyClosed)
        {
            if (!this.isComplete)
            {
                this.Repositories.AbortTransaction(throwIfAlreadyClosed);
            }
        }

        public void ExecuteChanges()
        {
            this.CheckDisposed();
            this.model.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.factory != null)
                    {
                        this.factory.Dispose();
                        this.factory = null;
                    }

                    if (this.entities != null)
                    {
                        this.entities.Dispose();
                        this.entities = null;
                    }
                }

                this.isDisposed = true;
            }
        }
    }
}
