
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    
    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public class CompanySkillsService : ServiceBase, ICompanySkillsService
    {
        [DebuggerStepThrough]
        internal CompanySkillsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ICompaniesSkillsRepository CompaniesSkillsRepository
        {
            get { return this.Repo.CompaniesSkills; }
        }

        public CompanySkill Update(CompanySkill item)
        {
            return this.CompaniesSkillsRepository.Update(item);
        }

        public int Insert(CompanySkill item)
        {
            return this.CompaniesSkillsRepository.Insert(item).Id;
        }

        public void Delete(CompanySkill item)
        {
            this.CompaniesSkillsRepository.Delete(item);
        }

        public CompanySkill GetById(int id)
        {
            return this.CompaniesSkillsRepository.Select()
                .WithId(id)
                .FirstOrDefault();
        }

        public CompanySkill GetBySkillIdAndCompanyId(int skillId, int companyId)
        {
            return this.CompaniesSkillsRepository.Select()
                .WithSkillIdAndCompanyId(skillId, companyId)
                .FirstOrDefault();
        }

        public IList<CompanySkill> SelectByCompanyId(int companyId, CompanyTagOptions options = CompanyTagOptions.None)
        {
            return this.CompaniesSkillsRepository
                .NewQuery(options)
                .WithCompanyId(companyId)
                .ToList();
        }

        public IList<CompanySkill> SelectBySkillIds(List<int> skillIds)
        {
            return this.CompaniesSkillsRepository.Select()
                .Where(o => skillIds.Contains(o.SkillId))
                .ToList();
        }

        public IList<CompanySkill> SelectBySkillId(int skillId)
        {
            return this.CompaniesSkillsRepository.Select()
                .Where(o => o.SkillId == skillId)
                .ToList();
        }

        public int CountById(int skillId)
        {
            return this.CompaniesSkillsRepository.Select()
                .Where(s => s.SkillId == skillId)
                .Count();
        }
    }
}
