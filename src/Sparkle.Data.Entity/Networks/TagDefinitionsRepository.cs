
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TagDefinitionsRepository : BaseNetworkRepositoryInt<TagDefinition>, ITagDefinitionsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TagDefinitionsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.TagDefinitions)
        {
        }

        public IList<TagDefinition> GetAll(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public IList<TagDefinition> GetByType(int networkId, TagType type)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => o.CategoryId == (int)type)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public IList<TagDefinition> SearchFor(int networkId, string search, params TagType[] types)
        {
            var query = this.Set
                .ByNetwork(networkId)
                .Where(o => o.Name.Contains(search));
            foreach (var type in types)
            {
                query = query.Where(o => o.CategoryId == (int)type);
            }

            return query
                .OrderBy(t => t.Name)
                .ToList();
        }

        public IList<TagDefinition> GetByIds(int networkId, int[] ids)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => ids.Contains(o.Id))
                .OrderBy(t => t.Name)
                .ToList();
        }

        public IList<TagDefinition> FindByNames(int networkId, string[] names)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => names.Contains(o.Name))
                .OrderBy(t => t.Name)
                .ToList();
        }

        public TagDefinition GetByAlias(int networkId, string alias)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => o.Alias == alias)
                .OrderBy(t => t.Name)
                .SingleOrDefault();
        }

        public TagDefinition[] GetByAlias(int networkId, string[] aliases)
        {
            var items = new TagDefinition[aliases.Length];
            var entities = this.Set
                .ByNetwork(networkId)
                .Where(o => aliases.Contains(o.Alias))
                .OrderBy(t => t.Name)
                .ToArray();

            foreach (var entity in entities)
            {
                for (int i = 0; i < aliases.Length; i++)
                {
                    var alias = aliases[i];
                    if (entity.Alias.Equals(alias, StringComparison.OrdinalIgnoreCase))
                    {
                        items[i] = entity;
                    }
                }
            }

            return items;
        }

        public IList<TagDefinition> GetByCategoryId(int networkId, int categoryId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => o.CategoryId == categoryId)
                .ToList();
        }

        public IList<TagDefinition> GetByCategoryId(int networkId, int categoryId, int offset, int count)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => o.CategoryId == categoryId)
                .OrderBy(x => x.Name)
                .Skip(offset).Take(count)
                .ToList();
        }

        public IList<TagDefinition> GetByIdsAndCategoryId(int networkId, int[] ids, int categoryId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => ids.Contains(o.Id) && o.CategoryId == categoryId)
                .ToList();
        }

        public TagDefinition GetByNameAndCategoryId(int networkId, string name, int categoryId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(o => o.Name == name && o.CategoryId == categoryId)
                .SingleOrDefault();
        }

        public int CountByCategory(int categoryId)
        {
            return this.Set
                .Where(x => x.CategoryId == categoryId)
                .Count();
        }

        public IList<TagDefinition> FindByNames(string name, int categoryId)
        {
            return this.Set
                .Where(o => o.Name.Contains(name) && o.CategoryId == categoryId)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public IList<TagDefinition> FindByNames(string name, int[] categoryId)
        {
            return this.Set
                .Where(o => o.Name.Contains(name) && categoryId.Contains(o.CategoryId))
                .OrderBy(t => t.Name)
                .ToList();
        }
    }
}
