
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Sparkle.Infrastructure;

    public class AchievementsCompanyService : ServiceBase, IAchievementsCompanyService
    {
        [DebuggerStepThrough]
        internal AchievementsCompanyService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public AchievementsCompany Insert(AchievementsCompany item)
        {
            this.Services.Logger.Info("AchievementsCompanyService.Insert", ErrorLevel.Success, "User {0} added achievement {1} to company {2}", this.Services.HostingEnvironment.Identity, item.AchievementId, item.CompanyId);
            return this.Repo.AchievementsCompanies.Insert(item);
        }

        public void Delete(AchievementsCompany item)
        {
            this.Services.Logger.Info("AchievementsCompanyService.Delete", ErrorLevel.Success, "User {0} removed achievement {1} from company {2}", this.Services.HostingEnvironment.Identity, item.AchievementId, item.CompanyId);
            this.Repo.AchievementsCompanies.Delete(item);
        }

        public AchievementsCompany Update(AchievementsCompany item)
        {
            this.Services.Logger.Info("AchievementsCompanyService.Update", ErrorLevel.Success, "User {0} changed achievement {1} of company {2}", this.Services.HostingEnvironment.Identity, item.AchievementId, item.CompanyId);
            return this.Repo.AchievementsCompanies.Update(item);
        }

        public IList<AchievementsCompany> SelectAll()
        {
            return this.Repo.AchievementsCompanies.Select()
                    .ToList();
        }

        public AchievementsCompany SelectById(int achievementId, int companyId)
        {
            return this.Repo.AchievementsCompanies.Select()
                .Where(o => o.AchievementId == achievementId && o.CompanyId == companyId)
                .SingleOrDefault();
        }
    }
}
