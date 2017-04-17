
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CompanyRelationshipTypesRepository : BaseNetworkRepositoryInt<CompanyRelationshipType>, ICompanyRelationshipTypesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyRelationshipTypesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyRelationshipTypes)
        {
        }

        public IList<CompanyRelationshipType> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }

        public CompanyRelationshipType GetByAlias(string alias, int networkId)
        {
            return this.Set
                .Where(o => o.Alias == alias && o.NetworkId == networkId)
                .SingleOrDefault();
        }

        public IList<CompanyRelationshipType> GetByKnownType(KnownCompanyRelationshipType type, int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId && o.KnownType == (byte)type)
                .ToList();
        }
    }
}
