
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
using Sparkle.Data.Networks.Options;
using Sparkle.Entities.Networks;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class CompanyRelationshipsRepository : BaseNetworkRepositoryInt<CompanyRelationship>, ICompanyRelationshipsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyRelationshipsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyRelationships)
        {
        }

        private IQueryable<CompanyRelationship> NewQuery(CompanyRelationshipOptions options)
        {
            var query = (ObjectQuery<CompanyRelationship>)this.Set;

            if ((options & CompanyRelationshipOptions.Type) == CompanyRelationshipOptions.Type)
                query = query.Include("Type");

            if ((options & CompanyRelationshipOptions.Master) == CompanyRelationshipOptions.Master)
                query = query.Include("Master");

            if ((options & CompanyRelationshipOptions.Slave) == CompanyRelationshipOptions.Slave)
                query = query.Include("Slave");

            return query;
        }

        public IList<CompanyRelationship> GetByCompanyId(int companyId, CompanyRelationshipOptions options)
        {
            return this.NewQuery(options)
                .Where(o => o.MasterId == companyId || o.SlaveId == companyId)
                .ToList();
        }
    }
}
