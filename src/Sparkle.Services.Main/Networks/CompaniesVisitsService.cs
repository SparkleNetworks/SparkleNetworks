
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class CompaniesVisitsService : ServiceBase, ICompaniesVisitsService
    {
        [DebuggerStepThrough]
        internal CompaniesVisitsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ICompaniesVisitsRepository companiesVisitsRepository
        {
            get { return this.Repo.CompaniesVisits; }
        }

        public int Insert(CompaniesVisit item)
        {
            return this.companiesVisitsRepository.Insert(item).Id;
        }

        public CompaniesVisit Update(CompaniesVisit item)
        {
            return this.companiesVisitsRepository.Update(item);
        }

        public void Delete(CompaniesVisit item)
        {
            this.companiesVisitsRepository.Delete(item);
        }

        public CompaniesVisit GetByCompanyIdAndUserIdAndDay(int companyId, int userId, DateTime date)
        {
            return this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId && o.UserId == userId && o.Date == date)
                .FirstOrDefault();
        }

        public int CountByCompanyAndDay(int companyId, DateTime day)
        {
            day = day.Date;

            var visites = this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId && o.Date == day)
                .ToList();

            return visites.Aggregate(0, (current, visit) => current + visit.ViewCount);
        }

        public List<CompaniesVisit> GetByCompanyAndDay(int companyId, DateTime day)
        {
            day = day.Date;

            return this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId && o.Date == day)
                .ToList();
        }

        public List<CompaniesVisit> SelectByCompanyId(int companyId)
        {
            return this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId)
                .OrderByDescending(v => v.Date)
                .ToList();
        }

        public int CountByCompanyAndMonth(int companyId, DateTime month)
        {
            var visites = this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId && o.Date.Month == month.Month)
                .ToList();

            return visites.Aggregate(0, (current, visit) => current + visit.ViewCount);
        }

        public int CountByCompanyAndRange(int companyId, DateTime firstDate, DateTime lastDate)
        {
            var visites = this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId && o.Date >= firstDate && o.Date <= lastDate)
                .ToList();

            return visites.Aggregate(0, (current, visit) => current + visit.ViewCount);
        }

        public int CountByCompany(int companyId)
        {
            var visites = this.companiesVisitsRepository
                .Select()
                .Where(o => o.CompanyId == companyId)
                .ToList();

            return visites.Aggregate(0, (current, visit) => current + visit.ViewCount);
        }


        public IList<CompaniesVisit> SelectAll()
        {
            return this.companiesVisitsRepository
                    .Select()
                    .ToList();
        }
    }
}
