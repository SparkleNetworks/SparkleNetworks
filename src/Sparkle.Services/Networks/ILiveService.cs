
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface ILiveService
    {
        Live GetById(int userId);

        bool IsOnline(int userId);

        IList<Live> GetOnline();

        void Log(byte status, int userId);

        int GetOnlineCount();

        System.DateTime? GetLastActivityDateByPersonId(int profileId);

        int GetTodaysCount();

        Dictionary<int, DateTime> GetAllLastActivityDate();

        int GetUsersDaysCount(int userId);
    }
}
