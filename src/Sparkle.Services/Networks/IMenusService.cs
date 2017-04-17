using Sparkle.Entities.Networks;
using System.Collections.Generic;

namespace Sparkle.Services.Networks
{
    public interface IMenusService
    {
        int Insert(Menu item);
        void Delete(Menu item);
        Menu Update(Menu item);

        List<Menu> SelectAll();

        Menu SelectById(int menuId);
    }
}
