
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IUserTagsRepository : IBaseNetworkRepository<UserTag, int>, ITagsV2RelationRepository
    {
        IList<UserTag> GetByUserId(int userId);

        int CountByUserAndCategory(int userId, int categoryId, bool countDeleted);

        IDictionary<int, int> GetTop(int[] categoryIds, int networkId);
    }
}
