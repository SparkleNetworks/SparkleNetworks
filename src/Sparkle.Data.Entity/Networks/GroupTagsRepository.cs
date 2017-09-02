
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GroupTagsRepository : BaseNetworkRepositoryInt<GroupTag>, IGroupTagsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupTagsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.GroupTags)
        {
        }

        public GroupTag GetByTagIdAndGroupId(int tagId, int groupId)
        {
            return this.Set
                .Where(o => o.RelationId == groupId && o.TagId == tagId)
                .SingleOrDefault();
        }

        public IList<GroupTag> GetByGroupId(int groupId)
        {
            return this.Set
                .Where(o => o.RelationId == groupId)
                .ToList();
        }

        public int CountByGroup(int groupId, bool countDeleted)
        {
            var query = (IQueryable<GroupTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.RelationId == groupId)
                .Count();
        }

        public int CountByGroupAndCategory(int groupId, int categoryId, bool countDeleted)
        {
            var query = (IQueryable<GroupTag>)this.Set;
            if (!countDeleted)
                query = query.Where(o => !o.DateDeletedUtc.HasValue);

            return query
                .Where(o => o.RelationId == groupId)
                .Where(o => o.TagDefinition.CategoryId == categoryId)
                .Count();
        }

        ITagV2Relation ITagsV2RelationRepository.GetNewEntity()
        {
            return new GroupTag();
        }

        ITagV2Relation ITagsV2RelationRepository.Insert(ITagV2Relation item)
        {
            this.Insert((GroupTag)item);
            return item;
        }
    }
}
