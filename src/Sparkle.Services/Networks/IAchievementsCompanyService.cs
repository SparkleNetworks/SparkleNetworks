
namespace Sparkle.Services.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IAchievementsCompanyService
    {
        IList<string> OptionsList { get; set; }
        AchievementsCompany Insert(AchievementsCompany item);
        void Delete(AchievementsCompany item);
        AchievementsCompany Update(AchievementsCompany item);

        IList<AchievementsCompany> SelectAll();

        ////AchievementsUser SelectById(int lostItemId);

        AchievementsCompany SelectById(int achievementId, int companyId);
    }
}
