
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The base of all repository.
    /// </summary>
    public interface IBaseNetworkRepository : IDisposable
    {
    }

    /// <summary>
    /// Entity-centric repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseNetworkRepository<TEntity> : IBaseNetworkRepository
    {
        /// <summary>
        /// Begins a query.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Select();

        /// <summary>
        /// Starts a query using includes.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [Obsolete]
        IQueryable<TEntity> Select(IList<string> options);

        /// <summary>
        /// Inserts immediately a new entity in an isolated context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        TEntity Insert(TEntity item);

        /// <summary>
        /// Inserts immediately many entities in an isolated context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        void InsertMany(IList<TEntity> items);

        void Attach(TEntity item);
    }

    /// <summary>
    /// Entity-centric abstract repository class with SCUD operations for entities with one primary key (generic).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IBaseNetworkRepository<TEntity, TPrimaryKey> : IBaseNetworkRepository<TEntity>
    {
        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the desired entity or null</returns>
        TEntity GetById(TPrimaryKey id);

        /// <summary>
        /// Updates immediately an existing entity in a isolated context.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        TEntity Update(TEntity item);

        /// <summary>
        /// Deletes immediately an existing entity in a isolated context.
        /// </summary>
        /// <param name="item"></param>
        void Delete(TEntity item);
    }

    /// <summary>
    /// Entity-centric abstract repository class with SCUD operations for entities with one primary key (generic) implementing <see cref="INetworkEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IBaseNetworkRepositoryNetwork<TEntity, TPrimaryKey> : IBaseNetworkRepository<TEntity, TPrimaryKey>
        where TEntity : Sparkle.Entities.Networks.INetworkEntity
    {
        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="networkId">The network id.</param>
        /// <returns>
        /// the desired entity or null
        /// </returns>
        TEntity GetById(TPrimaryKey id, int networkId);
    }
}
