
namespace Sparkle.Entities.Networks
{
    using System;

    partial class Interest : ITagV1, IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.TagName;
        }
    }

    partial class UserInterest : IEntityInt32Id, ITagV1Relation
    {
        int ITagV1Relation.TagId
        {
            get { return this.InterestId; }
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

    partial class GroupInterest : IEntityInt32Id, ITagV1Relation
    {
        int ITagV1Relation.TagId
        {
            get { return this.InterestId; }
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
            return this.Id + " group " + this.GroupId + " has interest " + this.InterestId;
        }
    }
}
