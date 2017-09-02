
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

    public class AchievementsUsersService : ServiceBase, IAchievementsUsersService
    {
        [DebuggerStepThrough]
        internal AchievementsUsersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public AchievementsUser Insert(AchievementsUser item)
        {
            item = this.Repo.AchievementsUsers.Insert(item);
            this.Services.Logger.Info("AchievementsUsersService.Insert", ErrorLevel.Success, "User {0} added achievement {1} to user {2}", this.Services.HostingEnvironment.Identity, item.AchievementId, item.UserId);
            return item;
        }

        public void Delete(AchievementsUser item)
        {
            this.Repo.AchievementsUsers.Delete(item);
            this.Services.Logger.Info("AchievementsUsersService.Delete", ErrorLevel.Success, "User {0} removed achievement {1} from user {2}", this.Services.HostingEnvironment.Identity, item.AchievementId, item.UserId);
        }

        public AchievementsUser Update(AchievementsUser item)
        {
            this.Services.Logger.Info("AchievementsUsersService.Update", ErrorLevel.Success, "User {0} changed achievement {1} of user {2}", this.Services.HostingEnvironment.Identity, item.AchievementId, item.UserId);
            return this.Repo.AchievementsUsers.Update(item);
        }

        public IList<AchievementsUser> SelectAll()
        {
            return this.Repo.AchievementsUsers.Select()
                .ToList();
        }

        public AchievementsUser SelectById(int achievementId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
