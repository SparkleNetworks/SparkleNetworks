
namespace Sparkle.Services.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IAchievementsService
    {
        IList<string> OptionsList { get; set; }
        Achievement Insert(Achievement item);
        void Delete(Achievement item);
        Achievement Update(Achievement item);

        IList<AchievementModel> SelectAll();

        IList<Achievement> SelectByTheme(AchievementsThemes theme);

        int Count();

        IList<AchievementModel> SelectByCompanyId(int companyId);

        IList<AchievementModel> SelectAllByCompanyId(int companyId);

        AchievementModel GetById(int achievementId);
    }

    public enum AchievementsThemes
    {
        All = 0,
        Plateforme = 1,
        Group = 2,
        Events = 3,
        Profile = 4,
        Service = 5
    }
}
