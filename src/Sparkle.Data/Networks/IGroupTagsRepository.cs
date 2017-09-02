
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IGroupTagsRepository : IBaseNetworkRepository<GroupTag, int>, ITagsV2RelationRepository
    {
        GroupTag GetByTagIdAndGroupId(int tagId, int groupId);

        IList<GroupTag> GetByGroupId(int groupId);

        int CountByGroup(int groupId, bool countDeleted);
        int CountByGroupAndCategory(int groupId, int categoryId, bool countDeleted);
    }
}
