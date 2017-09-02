
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Common;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;

    public class CompanyContactsService : ServiceBase, ICompanyContactsService
    {
        [DebuggerStepThrough]
        internal CompanyContactsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ICompanyContactsRepository companyContactRepository
        {
            get { return this.Repo.CompanyContacts; }

        }

        public CompanyContact Insert(CompanyContact item)
        {
            return this.companyContactRepository.Insert(item);
        }

        public CompanyContact GetById(int id)
        {
            return this.companyContactRepository.Select()
                .Where(c => c.Id == id)
                .SingleOrDefault();
        }

        public IList<CompanyContact> SelectByToCompanyId(int companyId)
        {
            return this.companyContactRepository.Select()
                .Where(c => c.ToCompanyId == companyId)
                .ToList();
        }

        public IList<CompanyContact> SelectByToCompanyIdAndFromCompanyId(int toCompanyId, int fromCompanyId)
        {
            return this.companyContactRepository.Select()
                .Where(c => c.ToCompanyId == toCompanyId && c.FromCompanyId == fromCompanyId || c.ToCompanyId == fromCompanyId && c.FromCompanyId == toCompanyId)
                .OrderBy(c => c.Date)
                .ToList();
        }

        public IList<CompanyContact> SelectByToCompanyIdAndFromUserEmail(int toCompanyId, string fromUserEmail)
        {
            return this.companyContactRepository.Select()
                .Where(c => c.ToCompanyId == toCompanyId && c.FromUserEmail == fromUserEmail || c.ToUserEmail == fromUserEmail && c.FromCompanyId == toCompanyId)
                .OrderBy(c => c.Date)
                .ToList();
        }
    }
}
