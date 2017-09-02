using System;
using System.Collections.Generic;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks.Models.Tags;

namespace Sparkle.Services.Networks
{
    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public interface IPeoplesRecreationsService
    {
        void DeletePeoplesSkill(UserRecreation item);
        int InsertPeoplesSkill(UserRecreation item);
        UserRecreation SelectPeoplesRecreationById(int id);
        UserRecreation SelectPeoplesRecreationBySkillIdAndUserId(int skillId, int userId);
        IList<UserRecreation> SelectPeoplesRecreationsByUserId(int userId, UserTagOptions options = UserTagOptions.None);
        UserRecreation UpdatePeoplesSkill(UserRecreation item);

        void AddPeopleRecreation(TagModel item, int userId);
    }
}
