
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface ITimelineItemSkillsRepository : IBaseNetworkRepository<TimelineItemSkill, int>
    {
        IQueryable<TimelineItemSkill> NewQuery(TimelineItemSkillOptions options);

        TimelineItemSkill GetByIds(int timelineItemId, int skillId, TimelineItemSkillOptions options);

        int CountBySkillId(int id);

        int CountBySkillId(int id, int networkId);

        IList<TimelineItemSkill> GetBySkillId(int skillId, TimelineItemOptions options);
        IList<TimelineItemSkill> GetBySkillId(int skillId, int networkId, TimelineItemOptions options);
    }
}
