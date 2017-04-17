
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ICompanyTagsRepository : IBaseNetworkRepository<CompanyTag, int>, ITagsV2RelationRepository
    {
        CompanyTag GetByTagIdAndCompanyId(int tagId, int companyId);

        IList<CompanyTag> GetByCompanyId(int companyId);
        IList<CompanyTag> GetByCompanyId(int[] companyIds);

        int CountByCompany(int companyId, bool countDeleted);
        int CountByCompanyAndCategory(int companyId, int categoryId, bool countDeleted);

        IDictionary<int, int> GetTop(int[] categoryIds, int networkId);
    }
}
