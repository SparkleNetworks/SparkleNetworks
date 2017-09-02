
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Groups;

    public interface IGroupsService
    {
        int CreateGroup(Group item);
        Group Delete(Group item);
        IList<Group> Search(string request);
        IList<Group> SelectAllGroups();
        Group SelectGroupById(int Id);
        Group UpdateGroup(Group item);

        IList<Group> SelectAllVisibleGroups();
        IList<Group> SelectNewGroups(DateTime start);
        IList<Group> GetCreatedInRange(DateTime start, DateTime end);

        IList<Group> GetWhereUserIsAdmin(int guid);

        Group SelectByImportedId(string id);

        TagModel GetGroupSkill(int groupId, int skillId);
        TagModel GetGroupInterest(int groupId, int interestId);
        TagModel GetGroupRecreation(int groupId, int recreationId);
        IList<TagModel> GetGroupSkills(int groupId);
        IList<TagModel> GetGroupInterests(int groupId);
        IList<TagModel> GetGroupRecreations(int groupId);

        int Count();

        bool CheckUserAcccessToGroup(int groupId, int userId);

        SetProfilePictureResult SetProfilePicture(SetProfilePictureRequest request);

        ProfilePictureModel GetProfilePicture(Group group, PictureAccessMode pictureAccessMode);
        ProfilePictureModel GetProfilePicture(int id, string name, PictureAccessMode mode);

        string GetProfileUrl(Group group, UriKind uriKind);

        string GetProfilePictureUrl(int groupId, Companies.CompanyPictureSize companyPictureSize, UriKind uriKind);
        string GetProfilePictureUrl(Group group, Companies.CompanyPictureSize pictureSize, UriKind uriKind);

        IList<TagModel> GetSkills();

        IList<TagModel> GetRecreations();

        IList<TagModel> GetInterests();

        IDictionary<int, GroupModel> GetById(int[] groupIds, GroupOptions options);
        GroupModel GetById(int groupId, GroupOptions options);

        AjaxTagPickerModel GetAjaxTagPickerModel(int groupId, int actingUserId);

        void Initialize();

        GroupModel GetByAlias(string alias, GroupOptions options);

        bool IsUserAuthorized(int groupId, int actingUserId, GroupAction action);
        string MakeAlias(string name);
    }
}
