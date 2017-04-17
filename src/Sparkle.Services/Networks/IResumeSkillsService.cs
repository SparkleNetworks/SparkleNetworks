using System;
using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IResumeSkillsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        ResumeSkill Insert(ResumeSkill item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        ResumeSkill Update(ResumeSkill item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(ResumeSkill item);

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="resumeId">The resume id.</param>
        /// <returns></returns>
        ResumeSkill GetById(int id);

        /// <summary>
        /// Selects the by resume id.
        /// </summary>
        /// <param name="resumeId">The resume id.</param>
        /// <returns></returns>
        IList<ResumeSkill> SelectByResumeId(int resumeId);

        /// <summary>
        /// Counts the by id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <returns></returns>
        int CountById(int skillId);

        /// <summary>
        /// Selects the by skill id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <returns></returns>
        IList<ResumeSkill> SelectBySkillId(int skillId);
    }
}
