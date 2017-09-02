
namespace Sparkle.Services.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IGroupsCategoriesService
    {
        void DeleteGroupCategory(GroupCategory item);
        int InsertGroupCategory(GroupCategory item);
        GroupCategory SelectGroupCategoryById(int Id);
        GroupCategory UpdateGroupCategory(GroupCategory item);

        IList<GroupCategory> SelectAll();

        int Count();
    }
}
