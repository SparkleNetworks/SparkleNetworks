using System;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks.Models.Tags;

namespace Sparkle.Services.Networks
{
    public interface IPeoplesInterestsService
    {
        void DeletePeoplesInterest(UserInterest item);
        int InsertPeoplesInterest(UserInterest item);
        UserInterest SelectPeoplesInterestById(int id);
        UserInterest SelectPeoplesInterestBySkillIdAndUserId(int skillId, int userId);
        System.Collections.Generic.IList<UserInterest> SelectPeoplesInterestsByUserId(int userId, UserTagOptions options = UserTagOptions.None);
        UserInterest UpdatePeoplesInterest(UserInterest item);

        int[] GetInterestsIdsByUserId(int userId);

        void AddPeopleInterest(TagModel item, int userId);
    }
}
