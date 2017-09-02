
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ICompanyProfileFieldsRepository : IBaseNetworkRepository<CompanyProfileField, int>
    {
        IList<CompanyProfileField> GetManyByUserIdAndFieldType(int companyId, ProfileFieldType type);

        IList<CompanyProfileField> GetByCompanyId(int companyId);

        IList<CompanyProfileField> GetAll(int? networkId);

        IList<CompanyProfileField> GetByCompanyIdAndFieldType(int[] companyIds, ProfileFieldType[] profileFieldTypes);
        IList<CompanyProfileField> GetByCompanyIdAndFieldType(int companyId, ProfileFieldType[] profileFieldTypes);
        IList<CompanyProfileField> GetByCompanyIdAndFieldType(int companyId, ProfileFieldType profileFieldType);

        ////CompanyProfileField GetByCompanyIdAndFieldType(int companyId, ProfileFieldType profileFieldType); // wrong

        IList<CompanyProfileField> GetByFieldType(ProfileFieldType type);
        IList<CompanyProfileField> GetByFieldType(ProfileFieldType[] type);
        IList<CompanyProfileField> GetByFieldType(int networkId, ProfileFieldType[] type);

        IList<CompanyProfileField> GetByFieldTypeAndValue(ProfileFieldType type, string value);

        /// <summary>
        /// Returns a list of profile field types with numbers: ProfileFieldId, COUNT(CompanyId), COUNT(id).
        /// </summary>
        /// <returns></returns>
        int[][] GetCounts();
    }
}
