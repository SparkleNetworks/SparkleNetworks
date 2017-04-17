
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface ITagDefinitionsRepository : IBaseNetworkRepository<TagDefinition, int>
    {
        IList<TagDefinition> GetAll(int networkId);
        IList<TagDefinition> GetByType(int networkId, TagType type);
        IList<TagDefinition> SearchFor(int networkId, string search, params TagType[] type);
        IList<TagDefinition> GetByIds(int networkId, int[] ids);
        IList<TagDefinition> GetByIdsAndCategoryId(int networkId, int[] ids, int categoryId);

        IList<TagDefinition> FindByNames(int networkId, string[] names);

        TagDefinition GetByAlias(int networkId, string alias);
        TagDefinition[] GetByAlias(int networkId, string[] alias);

        IList<TagDefinition> GetByCategoryId(int networkId, int categoryId);

        TagDefinition GetByNameAndCategoryId(int networkId, string name, int categoryId);

        int CountByCategory(int categoryId);

        IList<TagDefinition> FindByNames(string name, int categoryId);
        IList<TagDefinition> FindByNames(string name, int[] categoryId);

        IList<TagDefinition> GetByCategoryId(int networkId, int categoryId, int offset, int count);
    }
}
