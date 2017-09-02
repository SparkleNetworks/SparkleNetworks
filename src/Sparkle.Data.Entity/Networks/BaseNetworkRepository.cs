
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    /// <summary>
    /// Entity-centric repository class with SCUD operations for entities with one primary key (Int32).
    /// Entity class must implement <see cref="IEntityInt32Id"/>.
    /// Provides:
    /// - find by PK (int)
    /// - update (inherited)
    /// - delete (inherited)
    /// - insert one/many (inherited)
    /// - set-aware operations (inherited)
    /// - shared context (inherited)
    /// - context factory (inherited)
    /// - dispose implementation (inherited)
    /// </summary>
    public class BaseNetworkRepositoryIntNetwork<TEntity> : BaseNetworkRepositoryInt<TEntity>, IBaseNetworkRepositoryNetwork<TEntity, int>, IDisposable
        where TEntity : class, IEntityInt32Id, INetworkEntity
    {
        [System.Diagnostics.DebuggerStepThrough]
        public BaseNetworkRepositoryIntNetwork(NetworksEntities context, Func<NetworksEntities> factory, Func<NetworksEntities, ObjectSet<TEntity>> set)
            : base(context, factory, set)
        {
        }

        protected TEntity GetById(NetworksEntities model, int id, int networkId)
        {
            return this.GetSet(model).SingleOrDefault(e => e.Id == id && e.NetworkId == networkId);
        }

        public TEntity GetById(int id, int networkId)
        {
            return this.GetById(this.Context, id, networkId);
        }
    }

    /// <summary>
    /// Entity-centric repository class with SCUD operations for entities with one primary key (Int32).
    /// Entity class must implement <see cref="IEntityInt32Id"/>.
    /// </summary>
    public class BaseNetworkRepositoryInt<TEntity> : BaseNetworkRepository<TEntity, int>, IBaseNetworkRepository<TEntity, int>, IDisposable
        where TEntity : class, IEntityInt32Id
    {
        [System.Diagnostics.DebuggerStepThrough]
        public BaseNetworkRepositoryInt(NetworksEntities context, Func<NetworksEntities> factory, Func<NetworksEntities, ObjectSet<TEntity>> set)
            : base(context, factory, set)
        {
        }

        protected override TEntity GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(e => e.Id == id);
        }

        protected override int GetEntityId(TEntity item)
        {
            return item.Id;
        }
    }

    /// <summary>
    /// Entity-centric abstract repository class with SCUD operations for entities with one primary key (generic).
    /// Provides:
    /// - find by PK
    /// - update
    /// - delete
    /// - insert one/many (inherited)
    /// - set-aware operations (inherited)
    /// - shared context (inherited)
    /// - context factory (inherited)
    /// - dispose implementation (inherited)
    /// </summary>
    public abstract class BaseNetworkRepository<TEntity, TPrimaryKey> : BaseNetworkRepository<TEntity>, IBaseNetworkRepository<TEntity, TPrimaryKey>, IDisposable
        where TEntity : class
    {
        [System.Diagnostics.DebuggerStepThrough]
        public BaseNetworkRepository(NetworksEntities context, Func<NetworksEntities> factory, Func<NetworksEntities, ObjectSet<TEntity>> set)
            : base(context, factory, set)
        {
        }

        /// <summary>
        /// Updates immediately an existing entity in a isolated context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TEntity Update(TEntity item)
        {
            if (this.Context.IsTransactionnal)
            {
                var set = this.GetSet(this.Context);
                this.Context.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = this.Context.CreateEntityKey(set.EntitySet.Name, item);
                object outItem;
                if (this.Context.TryGetObjectByKey(key, out outItem))
                {
                    set.ApplyCurrentValues(item);
                    this.OnUpdateOverride(this.Context, item);
                    this.Context.SaveChanges();
                }
            }
            else
            {
                using (var model = this.GetNewContext())
                {
                    var set = this.GetSet(model);
                    model.ContextOptions.LazyLoadingEnabled = true;
                    EntityKey key = model.CreateEntityKey(set.EntitySet.Name, item);
                    object outItem;
                    if (model.TryGetObjectByKey(key, out outItem))
                    {
                        set.ApplyCurrentValues(item);
                        this.OnUpdateOverride(model, item);
                        model.SaveChanges();
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Deletes immediately an existing entity in a isolated context.
        /// </summary>
        /// <param name="item"></param>
        public void Delete(TEntity item)
        {
            if (this.Context.IsTransactionnal)
            {
                var set = this.GetSet(this.Context);
                this.Context.ContextOptions.LazyLoadingEnabled = true;
                EntityKey key = this.Context.CreateEntityKey(set.EntitySet.Name, item);
                Object outItem;
                if (this.Context.TryGetObjectByKey(key, out outItem))
                {
                    TEntity itemInDc = (TEntity)outItem;
                    this.OnDeleteOverride(this.Context, item, itemInDc);
                    this.Context.DeleteObject(itemInDc);
                    this.Context.SaveChanges();
                }
            }
            else
            {
                using (var model = this.GetNewContext())
                {
                    var set = this.GetSet(model);
                    model.ContextOptions.LazyLoadingEnabled = true;
                    EntityKey key = model.CreateEntityKey(set.EntitySet.Name, item);
                    Object outItem;
                    if (model.TryGetObjectByKey(key, out outItem))
                    {
                        TEntity itemInDc = (TEntity)outItem;
                        this.OnDeleteOverride(model, item, itemInDc);
                        model.DeleteObject(itemInDc);
                        model.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the desired entity or null</returns>
        public TEntity GetById(TPrimaryKey id)
        {
            return this.GetById(this.Context, id);
        }

        /// <summary>
        /// Override for the <see cref="Update"/> method permitting to execute custom actions before commiting a update order.
        /// The base method does nothing.
        /// </summary>
        /// <param name="model">the model</param>
        /// <param name="itemToInsert">the passed item to insert (not yet added to the set)</param>
        protected virtual void OnUpdateOverride(NetworksEntities model, TEntity itemToUpdate)
        {
            if (itemToUpdate is INetworkEntity)
            {
                this.VerifyNetworkId((INetworkEntity)itemToUpdate);
            }
        }

        /// <summary>
        /// Override for the <see cref="Delete"/> method permitting to execute custom actions before commiting a delete order.
        /// The base method does nothing.
        /// </summary>
        /// <param name="model">the model</param>
        /// <param name="itemToDelete">the passed item to delete (owned by another context, don't change it)</param>
        /// <param name="actualItemToDelete">the object to delete in the transctional context</param>
        protected virtual void OnDeleteOverride(NetworksEntities model, TEntity itemToDelete, TEntity actualItemToDelete)
        {
        }

        /// <summary>
        /// Gets an entity by its id using the read-only datacontext.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract TPrimaryKey GetEntityId(TEntity item);

        /// <summary>
        /// Gets an entity by its id using the given datacontext.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract TEntity GetById(NetworksEntities model, TPrimaryKey id);
    }

    /// <summary>
    /// Entity-centric repository class.
    /// Provides:
    /// - insert one/many
    /// - set-aware operations
    /// - shared context (inherited)
    /// - context factory (inherited)
    /// - dispose implementation (inherited)
    /// </summary>
    public class BaseNetworkRepository<TEntity> : BaseNetworkRepository, IBaseNetworkRepository<TEntity>, IDisposable
        where TEntity : class
    {
        private Func<NetworksEntities, ObjectSet<TEntity>> set;

        [System.Diagnostics.DebuggerStepThrough]
        public BaseNetworkRepository(NetworksEntities context, Func<NetworksEntities> factory, Func<NetworksEntities, ObjectSet<TEntity>> set)
            : base(context, factory)
        {
            if (set == null)
                throw new ArgumentNullException("set");
            this.set = set;
        }

        /// <summary>
        /// Starts a query.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Select()
        {
            return this.Set;
        }

        /// <summary>
        /// Starts a query using includes.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Select(IList<string> options)
        {
            if (options.Count > 0)
            {
                return this.SelectWithOptions(this.Set, options);
            }

            return this.Set;
        }

        /// <summary>
        /// Inserts immediately a new entity in an isolated context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TEntity Insert(TEntity item)
        {
            if (this.Context.IsTransactionnal)
            {
                this.OnInsertOverride(this.Context, item);
                this.Context.SaveChanges();
            }
            else
            {
                using (var model = this.GetNewContext())
                {
                    this.OnInsertOverride(model, item);
                    model.SaveChanges();
                }
            }

            return item;
        }

        /// <summary>
        /// Inserts immediately many entities in an isolated context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void InsertMany(IList<TEntity> items)
        {
            if (this.Context.IsTransactionnal)
            {
                foreach (var item in items)
                {
                    this.OnInsertOverride(this.Context, item);
                }

                this.Context.SaveChanges();
            }
            else
            {
                using (var model = this.GetNewContext())
                {
                    foreach (var item in items)
                    {
                        this.OnInsertOverride(model, item); 
                    }

                    model.SaveChanges();
                }
            }
        }

        public void Attach(TEntity item)
        {
            this.Set.AddObject(item);
        }

        /// <summary>
        /// Override for the <see cref="Insert"/> method permitting to execute custom actions before commiting a insert order.
        /// The base method associates the entity to a new context.
        /// </summary>
        /// <param name="model">the model</param>
        /// <param name="itemToInsert">the passed item to insert (not yet added to the set)</param>
        protected virtual void OnInsertOverride(NetworksEntities model, TEntity itemToInsert)
        {
            if (itemToInsert is INetworkEntity)
            {
                this.VerifyNetworkId((INetworkEntity)itemToInsert);
            }

            this.GetSet(model).AddObject(itemToInsert);
        }

        /// <summary>
        /// Returns the entityset for the given repository.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ObjectSet<TEntity> GetSet(NetworksEntities model)
        {
            return this.set(model);
        }

        /// <summary>
        /// Returns the read-only entityset for this repository.
        /// </summary>
        protected ObjectSet<TEntity> Set
        {
            get { return this.GetSet(this.Context); }
        }

        /// <summary>
        /// Verifies the NetworkId is set.
        /// </summary>
        /// <param name="networkEntity"></param>
        /// <exception cref="InvalidOperationException">NetworkId == 0</exception>
        protected void VerifyNetworkId(INetworkEntity networkEntity)
        {
            if (networkEntity.NetworkId == 0)
            {
                throw new InvalidOperationException("Entity of type " + networkEntity.GetType().Name + " must specify the NetworkId");
            }
        }
    }

    /// <summary>
    /// Most basic repository class.
    /// Provides:
    /// - shared context
    /// - context factory
    /// - dispose implementation
    /// </summary>
    public class BaseNetworkRepository : IDisposable
    {
        /// <summary>
        /// Indicates the object was disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The datacontext.
        /// </summary>
        private NetworksEntities context;

        /// <summary>
        /// The datacontext factory.
        /// </summary>
        private Func<NetworksEntities> factory;

        [System.Diagnostics.DebuggerStepThrough]
        public BaseNetworkRepository(NetworksEntities context, Func<NetworksEntities> factory)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
            this.factory = factory;
        }

        /// <summary>
        /// Returns the read-only context.
        /// </summary>
        protected NetworksEntities Context {
            get { return this.context ?? (this.context = this.factory()); }
        }

        /// <summary>
        /// Creates a new context for a transaction.
        /// </summary>
        protected NetworksEntities GetNewContext()
        {
            return this.factory();
        }

        /// <summary>
        /// Starts a query using includes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ObjectQuery<T> SelectWithOptions<T>(ObjectQuery<T> query, IList<string> options) where T : class
        {
            return options.Aggregate(query, (current, i) => current.Include(i));
        }

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases managed and - optionally - unmanaged resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (!this.disposed) {
                if (disposing) {
                    if (this.context != null)
                        this.context.Dispose();
                    this.factory = null;
                }
                this.disposed = true;
            }
        }

        #endregion
    }
}