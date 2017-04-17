
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Linq;
    using System.Data.Objects;
    using System.Data;

    public class AchievementsCompaniesRepository : BaseNetworkRepository<AchievementsCompany>, IAchievementsCompaniesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public AchievementsCompaniesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.AchievementsCompanies)
        {
        }

        public AchievementsCompany GetById(int achievementId, int companyId)
        {
            return this.Set.Where(o => o.AchievementId == achievementId && o.CompanyId == companyId).SingleOrDefault();
        }

        public AchievementsCompany Update(AchievementsCompany item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("AchievementsCompaniesRepository.Update: This cannot be used within a transaction.");

            using (var dc = this.GetNewContext())
            {
                EntityKey key = dc.CreateEntityKey(dc.AchievementsCompanies.EntitySet.Name, item);
                Object outItem;
                if (dc.TryGetObjectByKey(key, out outItem))
                {
                    dc.AchievementsCompanies.ApplyCurrentValues(item);
                    dc.SaveChanges();
                }
            }
            return item;
        }

        public void Delete(AchievementsCompany item)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("AchievementsCompaniesRepository.Delete: This cannot be used within a transaction.");

            using (var dc = this.GetNewContext())
            {
                EntityKey key = dc.CreateEntityKey(dc.AchievementsCompanies.EntitySet.Name, item);
                Object outItem;
                if (dc.TryGetObjectByKey(key, out outItem))
                {
                    dc.DeleteObject(outItem);
                    dc.SaveChanges();
                }
            }
        }

        public IQueryable<AchievementsCompany> NewQuery(AchievementsCompanyOptions options)
        {
            ObjectQuery<AchievementsCompany> query = this.Set;

            if ((options & AchievementsCompanyOptions.Company) == AchievementsCompanyOptions.Company)
                query = query.Include("Company");

            if ((options & AchievementsCompanyOptions.Achievement) == AchievementsCompanyOptions.Achievement)
                query = query.Include("Achievement");

            return query;
        }
    }
}
