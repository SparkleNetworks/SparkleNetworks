
namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models.Tags;
    using System;
    
    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public class PeoplesSkillsService : ServiceBase, IPeoplesSkillsService
    {
        /// <summary>
        /// Gets the peoples skills repository.
        /// </summary>
        protected IPeoplesSkillsRepository PeoplesSkillsRepository
        {
            get { return this.Repo.PeoplesSkills; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeoplesSkillsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal PeoplesSkillsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Updates the peoples skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public UserSkill Update(UserSkill item)
        {
            return this.PeoplesSkillsRepository.Update(item);
        }

        /// <summary>
        /// Inserts the peoples skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int Insert(UserSkill item)
        {
            return this.PeoplesSkillsRepository.Insert(item).Id;
        }

        /// <summary>
        /// Deletes the peoples skill.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(UserSkill item)
        {
            this.PeoplesSkillsRepository.Delete(item);
        }

        /// <summary>
        /// Selects the peoples skill by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public UserSkill SelectPeoplesSkillById(int id)
        {
            return this.PeoplesSkillsRepository.Select()
                .WithId(id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the by skill id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <returns></returns>
        public IList<UserSkill> SelectBySkillId(int skillId)
        {
            return this.PeoplesSkillsRepository.Select()
                .Where(s => s.SkillId == skillId)
                .ToList();
        }

        /// <summary>
        /// Selects the peoples skill by skill id and user id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public UserSkill SelectPeoplesSkillBySkillIdAndUserId(int skillId, int userId)
        {
            return this.PeoplesSkillsRepository.Select()
                .WithSkillIdAndUserId(skillId, userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the peoples skills by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<UserSkill> SelectPeoplesSkillsByUserId(int userId, UserTagOptions options = UserTagOptions.None)
        {
            return this.PeoplesSkillsRepository
                .NewQuery(options)
                .WithUserId(userId)
                .ToList();
        }

        /// <summary>
        /// Counts the by id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <returns></returns>
        public int CountById(int skillId)
        {
            return this.PeoplesSkillsRepository.Select()
                .Where(s => s.SkillId == skillId)
                .Count();
        }

        public IList<UserSkill> SelectAll(bool includeDefinition = true)
        {
            return this.Repo.PeoplesSkills
                .NewQuery(includeDefinition ? UserTagOptions.Tag : UserTagOptions.None)
                .Where(i => i.User.NetworkId == this.Services.NetworkId)
                .ToList();
        }

        public int[] GetSkillsIdsByUserId(int userId)
        {
            return this.Repo.PeoplesSkills
                .GetByUserId(userId)
                .Select(o => o.SkillId)
                .ToArray();
        }

        public void AddPeopleSkill(TagModel item, int userId)
        {
            var skill = this.Services.Skills.GetByName(item.Name);
            if (skill == null)
            {
                skill = this.Services.Skills.Insert(new Skill
                {
                    ParentId = 0,
                    TagName = item.Name.TrimTextRight(50),
                    Date = DateTime.UtcNow,
                    CreatedByUserId = userId,
                });
            }

            this.Insert(new UserSkill
            {
                SkillId = skill.Id,
                Date = DateTime.UtcNow,
                UserId = userId,
            });
        }
    }
}
