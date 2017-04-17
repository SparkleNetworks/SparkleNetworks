
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Tags;

    public interface ISkillsService
    {
        Skill Insert(Skill item);

        Skill Update(Skill item);

        void Delete(Skill item);

        void DeleteByUserId(Guid userId);

        IList<Skill> Search(string request, int take);
        IList<Skill> SelectAll();
        IList<Skill> SelectSkillsFromUserId(Guid userId);

        Skill GetById(int id);

        Skill GetByName(string name);

        IList<Skill> GetById(int[] ids);

        int CountCompanyProfiles(int id, bool allNetworks);
        int CountUserProfiles(int id, bool allNetworks);
        int CountTimelineItems(int id, bool allNetworks);
        int CountGroups(int id, bool allNetworks);

        IList<TagModel> GetAll();

        RenameTagResult Rename(RenameTagRequest model);

        IDictionary<string, int> GetByNames(string[] names);

        IDictionary<int, Tag2Model> GetAllForCache();

        IList<TagStat> GetTop();
    }
}
