
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class ResumeSkillsService : ServiceBase, IResumeSkillsService
    {
        [DebuggerStepThrough]
        internal ResumeSkillsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IResumesSkillsRepository rsRepository
        {
            get { return this.Repo.ResumesSkills; }
        }

        public ResumeSkill Insert(ResumeSkill item)
        {
            return this.rsRepository.Insert(item);
        }

        public ResumeSkill Update(ResumeSkill item)
        {
            return this.rsRepository.Update(item);
        }

        public void Delete(ResumeSkill item)
        {
            this.rsRepository.Delete(item);
        }

        public IList<ResumeSkill> SelectByResumeId(int resumeId)
        {
            return this.rsRepository
                    .Select()
                    .Where(o => o.ResumeId == resumeId)
                    .ToList();
        }

        public ResumeSkill GetById(int id)
        {
            return this.rsRepository
                    .Select()
                    .Where(o => o.Id == id)
                    .SingleOrDefault();
        }

        public IList<ResumeSkill> SelectBySkillId(int skillId)
        {
            return this.rsRepository.Select()
                .Where(o => o.SkillId == skillId)
                .ToList();
        }

        public int CountById(int skillId)
        {
            return this.rsRepository
                    .Select()
                    .Where(o => o.SkillId == skillId)
                    .Count();
        }
    }
}
