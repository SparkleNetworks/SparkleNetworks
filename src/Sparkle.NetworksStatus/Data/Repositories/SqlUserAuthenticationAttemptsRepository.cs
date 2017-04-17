
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlUserAuthenticationAttemptsRepository : BaseSqlRepository<UserAuthenticationAttempt, int>, IUserAuthenticationAttemptsRepository
    {
        public IList<UserAuthenticationAttempt> GetByUserId(int userId)
        {
            var sql = Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("UserId = @0", userId)
                .OrderBy("DateUtc DESC");
            return this.PetaContext.Fetch<UserAuthenticationAttempt>(sql);
        }
    }
}
