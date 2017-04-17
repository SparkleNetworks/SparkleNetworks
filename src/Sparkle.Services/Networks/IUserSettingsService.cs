using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparkle.Services.Networks
{
    public interface IUserSettingsService
    {
        Entities.Networks.UserSetting GetByUserIdAndKey(int userId, string key);

        void Save(int userId, string key, string value);
    }
}
