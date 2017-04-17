
namespace Sparkle.Entities.Networks
{
    using System;

    partial class Recreation : ITagV1
    {
        public override string ToString()
        {
            return this.Id + " " + this.TagName;
        }
    }

    partial class UserRecreation : IEntityInt32Id, ITagV1Relation
    {
        int ITagV1Relation.TagId
        {
            get { return this.RecreationId; }
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
    }

    partial class GroupRecreation : IEntityInt32Id, ITagV1Relation
    {
        int ITagV1Relation.TagId
        {
            get { return this.RecreationId; }
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
            return this.Id + " group " + this.GroupId + " has interest " + this.RecreationId;
        }
    }
}
