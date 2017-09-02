
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface ICompanyCategoriesRepository : IBaseNetworkRepository<CompanyCategory, short>
    {
        IList<CompanyCategory> GetAll();

        IList<CompanyCategory> GetCategoriesUsedInNetwork(int networkId);

        CompanyCategory GetByName(string name);

        IList<CompanyCategory> GetAll(int networkId);

        IDictionary<short, int> GetActiveCompaniesUsingCount();
        IDictionary<short, int> GetInactiveCompaniesUsingCount();

        void SetDefaultCategory(short categoryId, int networkId);

        IList<CompanyCategory> GetByAliasNull();

        CompanyCategory GetByAlias(string alias, int networkId);

        CompanyCategory GetDefault(int networkId);

        CompanyCategory GetAny(int networkId);

        IDictionary<short, CompanyCategory> GetById(short[] ids);

        int Count(int networkId);
    }
}
