
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public interface ITagsV1Repository
    {
        bool AppliesToUsers { get; }
        bool AppliesToCompanies { get; }
        bool AppliesToGroups { get; }
        bool AppliesToTimelineItems { get; }

        IDictionary<int, ITagV1> GetAll();
    }

    public interface ITagsV1RelationRepository//<T>
    ////where TSkill : ITagV1
    ////where T : ITagV1Relation, new()
    {
        ////IList<T> GetAll();
        IList<ITagV1Relation> GetAll();
        IList<ITagV1Relation> GetByTagId(int tagId);
    }
}
