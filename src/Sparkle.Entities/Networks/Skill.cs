
namespace Sparkle.Entities.Networks
{
    using System;

    partial class Skill : ITagV1, IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.TagName;
        }
    }

    partial class UserSkill : IEntityInt32Id, ITagV1Relation
    {
        int ITagV1Relation.TagId
        {
            get { return this.SkillId; }
        }

        int ITagV1Relation.RelationId
        {
            get { return this.UserId; }
        }

        DateTime ITagV1Relation.Date
        {
            get { return this.Date; }
        }

        DateTime? ITagV1Relation.DeletedDateUtc
        {
            get { return null; }
        }

        public override string ToString()
        {
            return this.Id + " user " + this.UserId + " has skill " + this.SkillId;
        }
    }

    partial class CompanySkill : IEntityInt32Id, ITagV1Relation
    {
        int ITagV1Relation.TagId
        {
            get { return this.SkillId; }
        }

        int ITagV1Relation.RelationId
        {
            get { return this.CompanyId; }
        }

        DateTime ITagV1Relation.Date
        {
            get { return this.Date; }
        }

        DateTime? ITagV1Relation.DeletedDateUtc
        {
            get { return null; }
        }

        public override string ToString()
        {
            return this.Id + " company " + this.CompanyId + " has skill " + this.SkillId;
        }
    }

    partial class GroupSkill : IEntityInt32Id, ITagV1Relation
    {
        public WallItemDeleteReason? DeleteReasonValue
        {
            get { return this.DeleteReason != null ? (WallItemDeleteReason)this.DeleteReason : default(WallItemDeleteReason?); }
            set { this.DeleteReason = value != null ? (byte)value : default(byte?); }
        }

        int ITagV1Relation.TagId
        {
            get { return this.SkillId; }
        }

        int ITagV1Relation.RelationId
        {
            get { return this.GroupId; }
        }

        DateTime ITagV1Relation.Date
        {
            get { return this.DateCreatedUtc; }
        }

        public override string ToString()
        {
            return this.Id + " group " + this.GroupId + " has skill " + this.SkillId;
        }
    }

    public partial class TimelineItemSkill : IEntityInt32Id, ITagV1Relation
    {
        public WallItemDeleteReason? DeleteReasonValue
        {
            get { return this.DeleteReason != null ? (WallItemDeleteReason)this.DeleteReason : default(WallItemDeleteReason?); }
            set { this.DeleteReason = value != null ? (byte)value : default(byte?); }
        }

        int ITagV1Relation.TagId
        {
            get { return this.SkillId; }
        }

        int ITagV1Relation.RelationId
        {
            get { return this.TimelineItemId; }
        }

        DateTime ITagV1Relation.Date
        {
            get { return this.DateUtc; }
        }

        public override string ToString()
        {
            return this.Id + " Skill:" + this.SkillId + " TimelineItem:" + this.TimelineItemId;
        }
    }

    public enum TimelineItemSkillOptions
    {
        None = 0x0000,
        Skill = 0x0001,
        TimelineItem = 0x0002,
    }   
}
