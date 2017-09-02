
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlNetworkRequestsRepository : BaseSqlRepository<NetworkRequest, int>, INetworkRequestsRepository
    {
        public NetworkRequest GetByWebId(Guid webId)
        {
            return this.PetaContext.Single<NetworkRequest>(Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("WebId = @0", webId));
        }

        public int Count()
        {
            return this.PetaContext.ExecuteScalar<int>(Sql.Builder
                .Select("COUNT(Id)")
                .From(TableName));
        }
    }
}
