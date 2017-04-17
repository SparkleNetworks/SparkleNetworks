
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

    public class AdTagsRepository : BaseNetworkRepositoryInt<AdTag>, IAdTagsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public AdTagsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.AdTags)
        {
        }

        public AdTag GetByTagIdAndRelationId(int tagId, int adId)
        {
            return this.Set
                .Where(o => o.RelationId == adId && o.TagId == tagId)
                .SingleOrDefault();
        }

        public IList<AdTag> GetByRelationId(int adId)
        {
            return this.Set
                .Where(o => o.RelationId == adId)
                .ToList();
        }

        public int CountByRelation(int adId, bool countDeleted)
        {
            var query = (IQueryable<AdTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.RelationId == adId)
                .Count();
        }

        public int CountByRelationAndCategory(int adId, int categoryId, bool countDeleted)
        {
            var query = (IQueryable<AdTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => !o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.RelationId == adId)
                .Where(o => o.TagDefinition.CategoryId == categoryId)
                .Count();
        }

        ITagV2Relation ITagsV2RelationRepository.GetNewEntity()
        {
            return new AdTag();
        }

        ITagV2Relation ITagsV2RelationRepository.Insert(ITagV2Relation item)
        {
            this.Insert((AdTag)item);
            return item;
        }
    }
}
