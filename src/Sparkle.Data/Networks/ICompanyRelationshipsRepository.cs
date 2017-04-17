
namespace Sparkle.Data.Networks
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ICompanyRelationshipsRepository : IBaseNetworkRepository<CompanyRelationship, int>
    {
        IList<CompanyRelationship> GetByCompanyId(int companyId, CompanyRelationshipOptions options);
    }
}
