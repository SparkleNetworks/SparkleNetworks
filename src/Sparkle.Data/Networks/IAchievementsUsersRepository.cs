
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IAchievementsUsersRepository : IBaseNetworkRepository<AchievementsUser>
    {
        AchievementsUser GetById(int achievementId, int userId);
        AchievementsUser Update(AchievementsUser item);
        void Delete(AchievementsUser item);
    }
}
