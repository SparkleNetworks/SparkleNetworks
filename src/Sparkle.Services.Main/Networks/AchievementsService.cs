
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;

    public class AchievementsService: ServiceBase, IAchievementsService
    {
        [DebuggerStepThrough]
        internal AchievementsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public Achievement Insert(Achievement item)
        {
            item = this.Repo.Achievements.Insert(item);
            this.Services.Logger.Info("AchievementsService.Insert", ErrorLevel.Success, "User {0} added achievement {1}", this.Services.HostingEnvironment.Identity, item.Id);
            return item;
        }

        public void Delete(Achievement item)
        {
            this.Repo.Achievements.Delete(item);
            this.Services.Logger.Info("AchievementsService.Delete", ErrorLevel.Success, "User {0} deleted achievement {1}", this.Services.HostingEnvironment.Identity, item.Id);
        }

        public Achievement Update(Achievement item)
        {
            this.Services.Logger.Info("AchievementsService.Update", ErrorLevel.Success, "User {0} updated achievement {1}", this.Services.HostingEnvironment.Identity, item.Id);
            return this.Repo.Achievements.Update(item);
        }

        public IList<AchievementModel> SelectAll()
        {
            return this.Repo.Achievements.Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .ToArray()
                .Select(o => new AchievementModel(o))
                .ToList();
        }

        public IList<Achievement> SelectByTheme(AchievementsThemes theme)
        {
            int themeId = (int)theme;
            if (themeId > 0)
            {
                return this.Repo.Achievements.Select()
                    .ByNetworkOrCommon(this.Services.NetworkId)
                    .ThemeId(themeId)
                    .ToList();
            }
            else
            {
                return this.Repo.Achievements.Select()
                    .ByNetworkOrCommon(this.Services.NetworkId)
                    .ToList();
            }
        }

        public Achievement CheckForAchievement(User me, AchievementsThemes theme)
        {
            switch (theme)
            {
                case AchievementsThemes.Profile:
                    break;
                case AchievementsThemes.Events:
                    // Verifie qu'il n'a pas deja tous les achievements
                    ////IList<Achievement> achievements = serviceFactory.Achievements.SelectByTheme(Sparkle.Services.AchievementsThemes.Events);

                    ////IList<AchievementsUser> achievementsUsers = serviceFactory.AchievementsUsers.SelectByTheme(3);



                    // Createur
                    ////int count = serviceFactory.Events.CountEventByCreator(me.UserID);


                    // Participant


                    break;
                ////case AchievementsThemes.Groups:
                ////    break;
                default:
                    throw new ArgumentOutOfRangeException("theme");
            }

            return null;
        }

        public int Count()
        {
            return this.Repo.Achievements
                .Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .Count();
        }

        public IList<AchievementModel> SelectByCompanyId(int companyId)
        {
            return this.Repo.AchievementsCompanies.NewQuery(AchievementsCompanyOptions.Achievement)
                .Where(ac => ac.CompanyId == companyId).ToArray().Select(o => new AchievementModel(o)).ToList();
        }

        public IList<AchievementModel> SelectAllByCompanyId(int companyId)
        {
            var earned = this.Repo.AchievementsCompanies.Select()
                .Where(ac => ac.CompanyId == companyId)
                .Select(o => o.AchievementId)
                .ToArray();

            var result = this.Repo.Achievements.Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .Where(o => o.Level == 1)
                .ToArray()
                .Select(o => new AchievementModel(o))
                .ToList();

            foreach (var item in result.Where(o => earned.Contains(o.Id)))
            {
                item.DateEarned = DateTime.Now;
                item.Unlocked = true;
            };

            return result;
        }

        public AchievementModel GetById(int achievementId)
        {
            return this.Repo.Achievements.Select()
                .Where(ac => ac.Id == achievementId).ToArray().Select(o => new AchievementModel(o)).FirstOrDefault();
        }
    }
}
