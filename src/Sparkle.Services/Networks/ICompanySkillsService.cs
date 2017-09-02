
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public interface ICompanySkillsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(CompanySkill item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        CompanySkill Update(CompanySkill item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(CompanySkill item);

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        CompanySkill GetById(int id);

        /// <summary>
        /// Gets the by skill id and company id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        CompanySkill GetBySkillIdAndCompanyId(int skillId, int companyId);

        /// <summary>
        /// Selects the by company id.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        IList<CompanySkill> SelectByCompanyId(int companyId, CompanyTagOptions options = CompanyTagOptions.None);

        /// <summary>
        /// Selects the by skill ids.
        /// </summary>
        /// <param name="skillIds">The skill ids.</param>
        /// <returns></returns>
        IList<CompanySkill> SelectBySkillIds(List<int> skillIds);

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
        IList<CompanySkill> SelectBySkillId(int skillId);
    }
}
