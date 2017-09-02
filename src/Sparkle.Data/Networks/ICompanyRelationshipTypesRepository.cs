
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ICompanyRelationshipTypesRepository : IBaseNetworkRepository<CompanyRelationshipType, int>
    {
        IList<CompanyRelationshipType> GetAll(int networkId);

        CompanyRelationshipType GetByAlias(string alias, int networkId);

        IList<CompanyRelationshipType> GetByKnownType(KnownCompanyRelationshipType type, int networkId);
    }
}
