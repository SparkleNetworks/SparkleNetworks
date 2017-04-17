
namespace Sparkle.Data.Networks
{
    using Sparkle.Data.Entity.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Text;

    public class TimelineItemSkillsRepository : BaseNetworkRepositoryInt<TimelineItemSkill>, ITimelineItemSkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TimelineItemSkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TimelineItemSkills)
        {
        }

        public IQueryable<TimelineItemSkill> NewQuery(TimelineItemSkillOptions options)
        {
            var query = (ObjectQuery<TimelineItemSkill>)this.Set;

            if ((options & TimelineItemSkillOptions.Skill) == TimelineItemSkillOptions.Skill)
                query = query.Include("Skill");

            if ((options & TimelineItemSkillOptions.TimelineItem) == TimelineItemSkillOptions.TimelineItem)
                query = query.Include("TimelineItem");

            return query;
        }

        public TimelineItemSkill GetByIds(int timelineItemId, int skillId, TimelineItemSkillOptions options)
        {
            return this.NewQuery(options)
                .Where(x => x.TimelineItemId == timelineItemId && x.SkillId == skillId)
                .SingleOrDefault();
        }

        public int CountBySkillId(int id)
        {
            return this.Set
                .Where(x => x.SkillId == id)
                .Count();
        }

        public int CountBySkillId(int id, int networkId)
        {
            return this.Set
                .Where(x => x.SkillId == id && x.TimelineItem.NetworkId == networkId)
                .Count();
        }

        public IList<TimelineItemSkill> GetBySkillId(int skillId, TimelineItemOptions options)
        {
            return this.Set
                .Where(x => x.SkillId == skillId)
                .ToList();
        }

        public IList<TimelineItemSkill> GetBySkillId(int skillId, int networkId, TimelineItemOptions options)
        {
            return this.Set
                .Where(x => x.SkillId == skillId && x.TimelineItem.NetworkId == networkId)
                .ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.SkillId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }
}
