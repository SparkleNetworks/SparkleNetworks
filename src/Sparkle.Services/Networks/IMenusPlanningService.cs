using Sparkle.Entities.Networks;
using System.Collections.Generic;

namespace Sparkle.Services.Networks
{
    public interface IMenusPlanningService
    {
        int Insert(MenuPlanning item);
        void Delete(MenuPlanning item);
        MenuPlanning Update(MenuPlanning item);
        List<MenuPlanning> SelectAll();
        List<MenuPlanning> SelectThisWeek();

        MenuPlanning SelectById(int mpId);

        List<MenuPlanning> SelectByDate(System.DateTime dateTime);
    }
}
