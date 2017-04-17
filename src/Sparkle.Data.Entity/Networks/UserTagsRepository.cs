
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public class UserTagsRepository : BaseNetworkRepositoryInt<UserTag>, IUserTagsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public UserTagsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserTags)
        {
        }

        public IList<UserTag> GetByUserId(int userId)
        {
            return this.Set
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public int CountByUserAndCategory(int userId, int categoryId, bool countDeleted)
        {
            var query = (IQueryable<UserTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => !o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.UserId == userId)
                .Where(o => o.TagDefinition.CategoryId == categoryId)
                .Count();
        }

        ITagV2Relation ITagsV2RelationRepository.GetNewEntity()
        {
            return new UserTag();
        }

        ITagV2Relation ITagsV2RelationRepository.Insert(ITagV2Relation item)
        {
            this.Insert((UserTag)item);
            return item;
        }

        public IDictionary<int, int> GetTop(int[] categoryIds, int networkId)
        {
            var items = this.Set
                .Where(x => x.User.NetworkId == networkId && categoryIds.Contains(x.TagDefinition.CategoryId))
                .GroupBy(x => x.TagId)
                .OrderByDescending(x => x.Count())
                .ToDictionary(x => x.Key, x => x.Count());
            return items;
        }
    }
}
