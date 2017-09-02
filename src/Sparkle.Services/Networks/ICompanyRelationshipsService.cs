
namespace Sparkle.Services.Networks
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks.Companies;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICompanyRelationshipsService
    {
        void Initialize();

        IList<CompanyRelationshipTypeModel> GetAllTypes();
        IList<CompanyRelationshipTypeModel> GetNonSystemTypes();

        IList<CompanyRelationshipModel> GetByCompanyId(int companyId, CompanyRelationshipOptions options);
        IDictionary<CompanyRelationshipTypeModel, IList<CompanyRelationshipModel>> GetByCompanyIdSortedByType(int companyId);

        CompanyRelationshipTypeModel GetTypeById(int? id);

        void InsertFromPoco(CompanyRelationshipPoco item);

        IList<CompanyRelationshipTypeModel> GetTypesByKnownType(KnownCompanyRelationshipType type);

        void Insert(CompanyRelationship item);
    }
}
