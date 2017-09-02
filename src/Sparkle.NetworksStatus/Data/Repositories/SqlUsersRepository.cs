
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlUsersRepository : IUsersRepository
    {
        public IList<User> GetAllSortedById(int id, int pageSize)
        {
            var sql = Sql.Builder
                .Select("TOP " + pageSize + " *")
                .From(TableName)
                .Where("Id > @0", id);
            return this.PetaContext.Query<User>(sql)
                .ToList();
        }

        public int Count()
        {
            var sql = Sql.Builder
                .Select("count(Id)")
                .From(TableName);
            return this.PetaContext.ExecuteScalar<int>(sql);
        }

        public User GetByPrimaryEmailId(int primaryEmailId)
        {
            var sql = Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("PrimaryEmailAddressId = @0", primaryEmailId);
            return this.PetaContext.SingleOrDefault<User>(sql);
        }
    }
}
