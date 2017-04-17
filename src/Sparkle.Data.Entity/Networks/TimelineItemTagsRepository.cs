
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

    public class TimelineItemTagsRepository : BaseNetworkRepositoryInt<TimelineItemTag>, ITimelineItemTagsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TimelineItemTagsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TimelineItemTags)
        {
        }

        public TimelineItemTag GetByTagIdAndTimelineItemId(int tagId, int timelineItemId)
        {
            return this.Set
                .Where(o => o.RelationId == timelineItemId && o.TagId == tagId)
                .SingleOrDefault();
        }

        public IList<TimelineItemTag> GetByTimelineItemId(int timelineItemId)
        {
            return this.Set
                .Where(o => o.RelationId == timelineItemId)
                .ToList();
        }

        public int CountByTimelineItem(int timelineItemId, bool countDeleted)
        {
            var query = (IQueryable<TimelineItemTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.RelationId == timelineItemId)
                .Count();
        }

        public int CountByTimelineItemAndCategory(int timelineItemId, int categoryId, bool countDeleted)
        {
            var query = (IQueryable<TimelineItemTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => !o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.RelationId == timelineItemId)
                .Where(o => o.TagDefinition.CategoryId == categoryId)
                .Count();
        }

        ITagV2Relation ITagsV2RelationRepository.GetNewEntity()
        {
            return new TimelineItemTag();
        }

        ITagV2Relation ITagsV2RelationRepository.Insert(ITagV2Relation item)
        {
            this.Insert((TimelineItemTag)item);
            return item;
        }
    }
}
