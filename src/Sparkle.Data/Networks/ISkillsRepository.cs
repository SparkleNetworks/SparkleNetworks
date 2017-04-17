
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ISkillsRepository : IBaseNetworkRepository<Skill, int>, ITagsV1Repository
    {
        IDictionary<int, Skill> GetAll();

        Skill GetByName(string nme);

        IList<Skill> GetByNames(string[] names);

        IList<TagStat> GetTop(int networkId);
    }

    [Repository]
    public interface IRecreationsRepository : IBaseNetworkRepository<Recreation, int>, ITagsV1Repository
    {
        Recreation GetByName(string name);

        IDictionary<int, Recreation> GetAll();
    }

    [Repository]
    public interface IInterestsRepository : IBaseNetworkRepository<Interest, int>, ITagsV1Repository
    {
        Interest GetByName(string name);
        IList<Interest> GetByNames(string[] names);

        IDictionary<int, Interest> GetAll();
    }

    [Repository]
    public interface IPeoplesSkillsRepository : IBaseNetworkRepository<UserSkill, int>, ITagsV1RelationRepository
    {
        int CountProfilesBySkillId(int id);

        int CountProfilesBySkillId(int id, int networkId);

        IList<UserSkill> GetBySkillId(int skillId);
        IList<UserSkill> GetBySkillId(int skillId, int networkId);

        IList<UserSkill> GetByUserId(int userId);

        IQueryable<UserSkill> NewQuery(UserTagOptions options);
    }

    [Repository]
    public interface ICompaniesSkillsRepository : IBaseNetworkRepository<CompanySkill, int>, ITagsV1RelationRepository
    {
        int CountCompaniesBySkillId(int id);

        int CountCompaniesBySkillId(int id, int networkId);

        IList<CompanySkill> GetBySkillId(int skillId);
        IList<CompanySkill> GetBySkillId(int skillId, int networkId);

        IQueryable<CompanySkill> NewQuery(CompanyTagOptions options);

        IDictionary<int, IList<CompanySkill>> GetByCompanyId(int[] companyIds, bool includeDefinition);
        IList<CompanySkill> GetListByCompanyId(int[] companyIds, bool includeDefinition);
    }

    [Repository]
    public interface IResumesSkillsRepository : IBaseNetworkRepository<ResumeSkill, int>
    {
    }

    [Repository]
    public interface IGroupsSkillsRepository : IBaseNetworkRepository<GroupSkill, int>, ITagsV1RelationRepository
    {
        int CountBySkillId(int id);

        int CountBySkillId(int id, int networkId);

        IList<GroupSkill> GetBySkillId(int skillId, GroupOptions options);
        IList<GroupSkill> GetBySkillId(int skillId, int networkId, GroupOptions options);

        IDictionary<int, int> GetStats();

        IQueryable<GroupSkill> NewQuery(GroupTagOptions options);
    }

    [Repository]
    public interface IGroupsRecreationsRepository : IBaseNetworkRepository<GroupRecreation, int>, ITagsV1RelationRepository
    {
        IQueryable<GroupRecreation> NewQuery(GroupTagOptions options);

        IDictionary<int, int> GetStats();
    }

    [Repository]
    public interface IGroupsInterestsRepository : IBaseNetworkRepository<GroupInterest, int>, ITagsV1RelationRepository
    {
        IQueryable<GroupInterest> NewQuery(GroupTagOptions options);

        IDictionary<int, int> GetStats();
    }

    [Repository]
    public interface ITimelineItemSkillsRepository : IBaseNetworkRepository<TimelineItemSkill, int>, ITagsV1RelationRepository
    {
        IQueryable<TimelineItemSkill> NewQuery(TimelineItemSkillOptions options);

        TimelineItemSkill GetByIds(int timelineItemId, int skillId, TimelineItemSkillOptions options);

        int CountBySkillId(int id);

        int CountBySkillId(int id, int networkId);

        IList<TimelineItemSkill> GetBySkillId(int skillId, TimelineItemOptions options);
        IList<TimelineItemSkill> GetBySkillId(int skillId, int networkId, TimelineItemOptions options);
    }
}
