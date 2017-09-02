
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

    public class CompanyTagsRepository : BaseNetworkRepositoryInt<CompanyTag>, ICompanyTagsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompanyTagsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanyTags)
        {
        }

        public CompanyTag GetByTagIdAndCompanyId(int tagId, int companyId)
        {
            return this.Set
                .Where(o => o.CompanyId == companyId && o.TagId == tagId)
                .SingleOrDefault();
        }

        public IList<CompanyTag> GetByCompanyId(int companyId)
        {
            return this.Set
                .Where(o => o.CompanyId == companyId)
                .ToList();
        }

        public IList<CompanyTag> GetByCompanyId(int[] companyId)
        {
            return this.Set
                .Where(o => companyId.Contains(o.CompanyId))
                .ToList();
        }

        public int CountByCompany(int companyId, bool countDeleted)
        {
            var query = (IQueryable<CompanyTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.CompanyId == companyId)
                .Count();
        }

        public int CountByCompanyAndCategory(int companyId, int categoryId, bool countDeleted)
        {
            var query = (IQueryable<CompanyTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => !o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.CompanyId == companyId)
                .Where(o => o.TagDefinition.CategoryId == categoryId)
                .Count();
        }

        ITagV2Relation ITagsV2RelationRepository.GetNewEntity()
        {
            return new CompanyTag();
        }

        ITagV2Relation ITagsV2RelationRepository.Insert(ITagV2Relation item)
        {
            this.Insert((CompanyTag)item);
            return item;
        }

        public IDictionary<int, int> GetTop(int[] categoryIds, int networkId)
        {
            var items = this.Set
                .Where(x => x.Company.NetworkId == networkId && categoryIds.Contains(x.TagDefinition.CategoryId))
                .GroupBy(x => x.TagId)
                .OrderByDescending(x => x.Count())
                .ToDictionary(x => x.Key, x => x.Count());
            return items;
        }
    }
}
