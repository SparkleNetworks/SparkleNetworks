
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlLinkedInRedirectionsRepository : ILinkedInRedirectionsRepository
    {
        public LinkedInRedirection GetByState(string state)
        {
            return this.PetaContext.SingleOrDefault<LinkedInRedirection>(Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("State = @0", state));
        }
    }
}
