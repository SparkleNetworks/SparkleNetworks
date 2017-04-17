
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IGroupsTagsService
    {
        AddGroupTagResult AddSkill(AddGroupTagRequest request);
        AddGroupTagResult AddInterest(AddGroupTagRequest request);
        AddGroupTagResult AddRecreation(AddGroupTagRequest request);

        AddGroupTagResult RemoveSkill(AddGroupTagRequest request);
        AddGroupTagResult RemoveInterest(AddGroupTagRequest request);
        AddGroupTagResult RemoveRecreation(AddGroupTagRequest request);

        IList<GroupSkill> GetGroupSkillsByIds(int[] skillIds);
        IList<GroupInterest> GetGroupInterestsByIds(int[] interestIds);
        IList<GroupRecreation> GetGroupRecreationsByIds(int[] recreationIds);

        IList<GroupSkill> GetAllGroupSkills(GroupTagOptions options);
        IList<GroupInterest> GetAllGroupInterests(GroupTagOptions options);
        IList<GroupRecreation> GetAllGroupRecreations(GroupTagOptions options);
    }
}
