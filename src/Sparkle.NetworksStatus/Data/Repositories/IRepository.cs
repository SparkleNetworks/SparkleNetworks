
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The base of all repository.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    /// Entity-centric repository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> : IRepository
    {
        /// <summary>
        /// Begins a query.
        /// </summary>
        /// <returns></returns>
        IList<TEntity> GetAll();

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
    }

    /// <summary>
    /// Entity-centric abstract repository class with SCUD operations for entities with one primary key (generic).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : IRepository<TEntity>
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

        /// <summary>
        /// Deletes immediately an existing entity in a isolated context.
        /// </summary>
        /// <param name="item"></param>
        void Delete(TPrimaryKey id);
    }
}
