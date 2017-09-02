
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class GroupsCategoriesService : ServiceBase, IGroupsCategoriesService
    {
        [DebuggerStepThrough]
        internal GroupsCategoriesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void DeleteGroupCategory(GroupCategory item)
        {
            this.Repo.GroupsCategories.Delete(item);
        }

        public int InsertGroupCategory(GroupCategory item)
        {
            return this.Repo.GroupsCategories.Insert(item).Id;
        }

        public GroupCategory SelectGroupCategoryById(int id)
        {
            return this.Repo.GroupsCategories.Select().Where(o => o.Id == id).SingleOrDefault();
        }

        public GroupCategory UpdateGroupCategory(GroupCategory item)
        {
            return this.Repo.GroupsCategories.Update(item);
        }

        public IList<GroupCategory> SelectAll()
        {
            return this.Repo.GroupsCategories.Select()
                    .ToList();
        }

        public int Count()
        {
            return this.Repo.GroupsCategories.Count();
        }
    }
}
