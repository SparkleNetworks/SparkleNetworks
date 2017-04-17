
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IAdTagsRepository : IBaseNetworkRepository<AdTag, int>, ITagsV2RelationRepository
    {
        AdTag GetByTagIdAndRelationId(int tagId, int relationId);

        IList<AdTag> GetByRelationId(int relationId);

        int CountByRelation(int relationId, bool countDeleted);
        int CountByRelationAndCategory(int relationId, int categoryId, bool countDeleted);
    }
}
