
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public interface ITagsV2RelationRepository
    {
        ITagV2Relation GetNewEntity();
        ITagV2Relation Insert(ITagV2Relation item);
    }
}
