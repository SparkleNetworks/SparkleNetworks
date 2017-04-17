
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using Sparkle.NetworksStatus.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BaseSqlRepository : IRepository
    {
        private readonly SqlClientRepositoryFactory factory;

        public BaseSqlRepository(SqlClientRepositoryFactory factory)
        {
            this.factory = factory;
        }

        public SqlClientRepositoryFactory Factory
        {
            get { return this.factory; }
        }

        public PetaContext PetaContext
        {
            get { return this.Factory.PetaContext; }
        }
    }

    public class BaseSqlRepository<TEntity> : BaseSqlRepository, IRepository<TEntity>
        where TEntity : class, new()
    {
        public BaseSqlRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }

        public IList<TEntity> GetAll()
        {
            return this.PetaContext.Fetch<TEntity>("*");
        }

        public TEntity Insert(TEntity item)
        {
            var result = this.PetaContext.Insert(item);
            return item;
        }

        public void InsertMany(IList<TEntity> items)
        {
            foreach (var item in items)
            {
                this.PetaContext.Insert(item);
            }
        }
    }

    public class BaseSqlRepository<TEntity, TPrimaryKey> : BaseSqlRepository<TEntity>, IRepository<TEntity, TPrimaryKey>
        where TEntity : class, new()
    {
        public BaseSqlRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }

        public TEntity GetById(TPrimaryKey id)
        {
            return this.PetaContext.SingleOrDefault<TEntity>(id) as TEntity;
        }

        public TEntity Update(TEntity item)
        {
            this.PetaContext.Update(item);
            return item;
        }

        public void Delete(TEntity item)
        {
            this.PetaContext.Delete(item);
        }

        public void Delete(TPrimaryKey id)
        {
            this.PetaContext.Delete<TEntity>(id);
        }
    }
}
