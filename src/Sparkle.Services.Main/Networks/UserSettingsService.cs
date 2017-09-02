using System.Linq;
using Sparkle.Data.Networks;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;

namespace Sparkle.Services.Main.Networks
{
    public class UserSettingsService : ServiceBase, IUserSettingsService
    {
        internal UserSettingsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public UserSetting GetByUserIdAndKey(int userId, string key)
        {
            return this.Repo.UserSettings.Select().Where(s => s.UserId == userId && s.Key == key).SingleOrDefault();
        }

        public void Save(int userId, string key, string value)
        {
            UserSetting setting = this.Repo.UserSettings.Select().Where(s => s.UserId == userId && s.Key == key).SingleOrDefault();
            if(setting == null)
            {
                setting = new UserSetting {UserId = userId, Key = key, Value = value};
                this.Repo.UserSettings.Insert(setting);
            }
            else
            {
                setting.Value = value;
                this.Repo.UserSettings.Update(setting);
            }
        }
    }
}
