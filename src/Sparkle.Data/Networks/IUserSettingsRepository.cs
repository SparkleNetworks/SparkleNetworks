using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparkle.Entities.Networks;

namespace Sparkle.Data.Networks
{
    [Repository]
    public interface IUserSettingsRepository : IBaseNetworkRepository<UserSetting>
    {
        UserSetting Update(UserSetting item);

    }
}
