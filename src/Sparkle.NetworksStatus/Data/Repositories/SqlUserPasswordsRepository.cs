
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlUserPasswordsRepository : IUserPasswordsRepository
    {
        public IList<UserPassword> GetUserId(int userId)
        {
            var sql = Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("UserId = @0", userId);
            return this.PetaContext.Fetch<UserPassword>(sql);
        }
    }
}
