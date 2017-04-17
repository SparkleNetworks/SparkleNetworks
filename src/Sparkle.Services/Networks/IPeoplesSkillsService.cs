
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models.Tags;

    public interface IPeoplesSkillsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(UserSkill item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        UserSkill Update(UserSkill item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(UserSkill item);

        UserSkill SelectPeoplesSkillById(int id);
        UserSkill SelectPeoplesSkillBySkillIdAndUserId(int skillId, int userId);
        IList<UserSkill> SelectPeoplesSkillsByUserId(int userId, UserTagOptions options = UserTagOptions.None);

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
        IList<UserSkill> SelectBySkillId(int skillId);

        IList<UserSkill> SelectAll(bool includeDefinition = true);

        int[] GetSkillsIdsByUserId(int userId);

        void AddPeopleSkill(TagModel item, int userId);
    }
}
