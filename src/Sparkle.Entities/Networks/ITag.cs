
namespace Sparkle.Entities.Networks
{
    using System;

    public interface ITag
    {
        int Id { get; }
        string TagName { get; set; }
    }

    public interface ITagV1 : ITag
    {
        DateTime Date { get; set; }
        int CreatedByUserId { get; set; }
    }

    public interface ITagV1Relation
    {
        ////int EntityId { get; }
        int TagId { get; }
        int RelationId { get; }
        DateTime Date { get; }
        DateTime? DeletedDateUtc { get; }
    }

    public interface ITagV2Relation
    {
        int Id { get; set; }
        int TagId { get; set; }
        int RelationId { get; set; }
        DateTime DateCreatedUtc { get; set; }
        int CreatedByUserId { get; set; }
        DateTime? DateDeletedUtc { get; set; }
        int? DeletedByUserId { get; set; }
        byte? DeleteReason { get; set; }
        int SortOrder { get; set; }
    }
}
