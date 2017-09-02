
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TimelineItemSkillModel
    {
        public TimelineItemSkillModel()
        {
        }

        public TimelineItemSkillModel(TimelineItemSkill item)
        {
            this.Id = item.Id;
            this.SkillId = item.SkillId;
            this.TimelineItemId = item.TimelineItemId;
        }

        public int Id { get; set; }

        public int SkillId { get; set; }

        public int TimelineItemId { get; set; }

        public string SkillName { get; set; }
    }
}
