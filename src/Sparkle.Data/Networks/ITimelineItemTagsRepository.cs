
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ITimelineItemTagsRepository : IBaseNetworkRepository<TimelineItemTag, int>, ITagsV2RelationRepository
    {
        TimelineItemTag GetByTagIdAndTimelineItemId(int tagId, int timelineItemId);

        IList<TimelineItemTag> GetByTimelineItemId(int timelineItemId);

        int CountByTimelineItem(int timelineItemId, bool countDeleted);
        int CountByTimelineItemAndCategory(int timelineItemId, int categoryId, bool countDeleted);
    }
}
