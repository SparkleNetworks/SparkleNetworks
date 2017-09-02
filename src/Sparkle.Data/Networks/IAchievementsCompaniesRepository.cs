
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IAchievementsCompaniesRepository : IBaseNetworkRepository<AchievementsCompany>
    {
        AchievementsCompany GetById(int achievementId, int companyId);
        AchievementsCompany Update(AchievementsCompany item);
        void Delete(AchievementsCompany item);

        IQueryable<AchievementsCompany> NewQuery(AchievementsCompanyOptions options);

    }
}
