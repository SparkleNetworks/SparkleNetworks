using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IAchievementsUsersService
    {
        IList<string> OptionsList { get; set; }
        AchievementsUser Insert(AchievementsUser item);
        void Delete(AchievementsUser item);
        AchievementsUser Update(AchievementsUser item);

        IList<AchievementsUser> SelectAll();

        ////AchievementsUser SelectById(int lostItemId);
    }
}
