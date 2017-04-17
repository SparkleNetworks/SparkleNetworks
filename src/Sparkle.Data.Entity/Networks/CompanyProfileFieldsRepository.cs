
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CompanyProfileFieldsRepository : BaseNetworkRepositoryInt<CompanyProfileField>, ICompanyProfileFieldsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyProfileFieldsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyProfileFields)
        {
        }

        ////public CompanyProfileField GetByCompanyIdAndFieldType(int companyId, ProfileFieldType profileFieldType)
        ////{ // WRONG
        ////    return this.Set
        ////        .Where(o => o.CompanyId == companyId && o.ProfileFieldId == (int)profileFieldType)
        ////        .SingleOrDefault();
        ////}

        public IList<CompanyProfileField> GetManyByUserIdAndFieldType(int companyId, ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.CompanyId == companyId)
                .Where(o => o.ProfileFieldId == (int)type)
                .ToList();
        }

        public IList<CompanyProfileField> GetByCompanyId(int companyId)
        {
            return this.Set
                .Where(o => o.CompanyId == companyId)
                .ToList();
        }

        public IList<CompanyProfileField> GetAll(int? networkId)
        {
            var query = (IQueryable<CompanyProfileField>)this.Set;
            if (networkId.HasValue)
                query = query.Where(o => o.Company.NetworkId == networkId.Value);

            return query.ToList();
        }

        public IList<CompanyProfileField> GetByCompanyIdAndFieldType(int[] companyIds, ProfileFieldType[] profileFieldTypes)
        {
            var types = profileFieldTypes.Cast<int>().ToArray();
            return this.Set
                .Where(o => companyIds.Contains(o.CompanyId) && types.Contains(o.ProfileFieldId))
                .ToList();
        }

        public IList<CompanyProfileField> GetByCompanyIdAndFieldType(int companyId, ProfileFieldType[] profileFieldTypes)
        {
            var types = profileFieldTypes.Cast<int>().ToArray();
            return this.Set
                .Where(o => companyId == o.CompanyId && types.Contains(o.ProfileFieldId))
                .ToList();
        }

        public IList<CompanyProfileField> GetByCompanyIdAndFieldType(int companyId, ProfileFieldType profileFieldType)
        {
            var types = (int)profileFieldType;
            return this.Set
                .Where(o => companyId == o.CompanyId && types == o.ProfileFieldId)
                .ToList();
        }

        public IList<CompanyProfileField> GetByFieldType(ProfileFieldType type)
        {
            return this.GetByFieldType(new ProfileFieldType[] { type });
        }

        public IList<CompanyProfileField> GetByFieldType(ProfileFieldType[] type)
        {
            var types = type.Select(o => (int)o).ToArray();
            return this.Set
                .Where(o => types.Contains(o.ProfileFieldId))
                .ToList();
        }

        public IList<CompanyProfileField> GetByFieldType(int networkId, ProfileFieldType[] type)
        {
            var types = type.Select(o => (int)o).ToArray();
            return this.Set
                .Where(o => types.Contains(o.ProfileFieldId) && o.Company.NetworkId == networkId)
                .ToList();
        }

        public IList<CompanyProfileField> GetByFieldTypeAndValue(ProfileFieldType type, string value)
        {
            var typeId = (int)type;
            return this.Set
                .Where(o => o.ProfileFieldId == typeId && o.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public int[][] GetCounts()
        {
            // select ProfileFieldId, COUNT(CompanyId) Subjects, COUNT(id) [Values] 
            // from CompanyProfileFields
            // group by ProfileFieldId

            var query = this.Set
                .GroupBy(x => x.ProfileFieldId)
                .Select(x => new
                {
                    ProfileFieldId = x.Key,
                    Subjects = x.GroupBy(y => y.CompanyId).Count(),
                    Values = x.Count(),
                });
            var result = query
                .ToArray()
                .Select(x => new int[] { x.ProfileFieldId, x.Subjects, x.Values, })
                .ToArray();
            return result;
        }
    }
}
