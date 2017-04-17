
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Networks;
    using Sparkle.Data;
    using System.Collections.Generic;
    using System.Data.Objects;

    public class SkillsRepository : BaseNetworkRepositoryInt<Skill>, ISkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Skills)
        {
        }

        public IDictionary<int, Skill> GetAll()
        {
            return this.Set.OrderBy(t => t.TagName).ToDictionary(t => t.Id, t => t);
        }

        public Skill GetByName(string name)
        {
            return this.Set.Where(t => t.TagName == name).SingleOrDefault();
        }

        public IList<Skill> GetByNames(string[] names)
        {
            return this.Set
                .Where(o => names.Contains(o.TagName))
                .ToList();
        }

        public IList<TagStat> GetTop(int networkId)
        {
            var items = this.Context.GetTopSkills(networkId)
                .Select(x => new TagStat(x))
                .ToList();
            return items;
        }

        public bool AppliesToUsers
        {
            get { return true; }
        }

        public bool AppliesToCompanies
        {
            get { return true; }
        }

        public bool AppliesToGroups
        {
            get { return true; }
        }

        public bool AppliesToTimelineItems
        {
            get { return true; }
        }

        IDictionary<int, ITagV1> ITagsV1Repository.GetAll()
        {
            return this.Set.ToDictionary(x => x.Id, x => (ITagV1)x);
        }
    }

    public class PeoplesSkillsRepository : BaseNetworkRepositoryInt<UserSkill>, IPeoplesSkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PeoplesSkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserSkills)
        {
        }

        public int CountProfilesBySkillId(int id)
        {
            return this.Set
                .Where(x => x.SkillId == id)
                .Count();
        }

        public int CountProfilesBySkillId(int id, int networkId)
        {
            return this.Set
                .Where(x => x.SkillId == id && x.User.NetworkId == networkId)
                .Count();
        }

        public IList<UserSkill> GetBySkillId(int skillId)
        {
            return this.Set
                .Where(x => x.SkillId == skillId)
                .ToList();
        }

        public IList<UserSkill> GetBySkillId(int skillId, int networkId)
        {
            return this.Set
                .Where(x => x.SkillId == skillId && x.User.NetworkId == networkId)
                .ToList();
        }

        public IList<UserSkill> GetByUserId(int userId)
        {
            return this.Set
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public IQueryable<UserSkill> NewQuery(UserTagOptions options)
        {
            ObjectQuery<UserSkill> query = this.Set;

            if ((options & UserTagOptions.Tag) == UserTagOptions.Tag)
                query = query.Include("Skill");

            if ((options & UserTagOptions.User) == UserTagOptions.User)
                query = query.Include("User");

            return query;
    }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.SkillId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }

    public class CompaniesSkillsRepository : BaseNetworkRepositoryInt<CompanySkill>, ICompaniesSkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public CompaniesSkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.CompanySkills)
        {
        }

        public int CountCompaniesBySkillId(int id)
        {
            return this.Set
                .Where(x => x.SkillId == id)
                .Count();
        }

        public int CountCompaniesBySkillId(int id, int networkId)
        {
            return this.Set
                .Where(x => x.SkillId == id && x.Company.NetworkId == networkId)
                .Count();
        }

        public IList<CompanySkill> GetBySkillId(int skillId)
        {
            return this.Set
                .Include("Company")
                .Where(x => x.SkillId == skillId)
                .ToList();
        }

        public IList<CompanySkill> GetBySkillId(int skillId, int networkId)
        {
            return this.Set
                .Include("Company")
                .Where(x => x.SkillId == skillId && x.Company.NetworkId == networkId)
                .ToList();
        }

        public IQueryable<CompanySkill> NewQuery(CompanyTagOptions options)
        {
            ObjectQuery<CompanySkill> query = this.Set;

            if ((options & CompanyTagOptions.Company) == CompanyTagOptions.Company)
                query = query.Include("Company");

            if ((options & CompanyTagOptions.Tag) == CompanyTagOptions.Tag)
                query = query.Include("Skill");

            return query;
    }

        public IDictionary<int, IList<CompanySkill>> GetByCompanyId(int[] companyIds, bool includeDefinition)
        {
            var query = this.NewQuery(includeDefinition ? CompanyTagOptions.Tag : CompanyTagOptions.None);

            return query
                .Where(cs => companyIds.Contains(cs.CompanyId))
                .ToArray()
                .GroupBy(cs => cs.CompanyId)
                .ToDictionary(g => g.Key, g => (IList<CompanySkill>)g.ToList());
        }

        public IList<CompanySkill> GetListByCompanyId(int[] companyIds, bool includeDefinition)
        {
            var query = this.NewQuery(includeDefinition ? CompanyTagOptions.Tag : CompanyTagOptions.None);

            return query
                .Where(cs => companyIds.Contains(cs.CompanyId))
                .ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.SkillId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }

    public class ResumesSkillsRepository : BaseNetworkRepositoryInt<ResumeSkill>, IResumesSkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ResumesSkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ResumeSkills)
        {
        }
    }

    public class GroupsSkillsRepository : BaseNetworkRepositoryInt<GroupSkill>, IGroupsSkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public GroupsSkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.GroupSkills)
        {
        }

        public int CountBySkillId(int id)
        {
            return this.Set
                .Where(x => x.SkillId == id)
                .Count();
        }

        public int CountBySkillId(int id, int networkId)
        {
            return this.Set
                .Where(x => x.SkillId == id && x.Group.NetworkId == networkId)
                .Count();
        }

        public IList<GroupSkill> GetBySkillId(int skillId, GroupOptions options)
        {
            if (options != GroupOptions.None)
                throw new NotSupportedException();

            return this.Set
                .Where(x => x.SkillId == skillId)
                .ToList();
        }

        public IList<GroupSkill> GetBySkillId(int skillId, int networkId, GroupOptions options)
        {
            if (options != GroupOptions.None)
                throw new NotSupportedException();

            return this.Set
                .Where(x => x.SkillId == skillId && x.Group.NetworkId == networkId)
                .ToList();
        }

        public IDictionary<int, int> GetStats()
        {
            var values = new Dictionary<int, int>();
            var cxx = ((System.Data.EntityClient.EntityConnection)this.Context.Connection);
            var cmd = cxx.StoreConnection.CreateCommand();
            cmd.CommandText = @"select gs.SkillId, COUNT(gs.GroupId)
from dbo.GroupSkills as gs
inner join dbo.Groups as g on g.Id = gs.GroupId and g.IsDeleted = 0
where gs.DeletedDateUtc is null
group by gs.SkillId";
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(reader.GetInt32(0), reader.GetInt32(1));
                }
            }

            return values;
        }

        public IQueryable<GroupSkill> NewQuery(GroupTagOptions options)
        {
            ObjectQuery<GroupSkill> query = this.Set;

            if ((options & GroupTagOptions.Group) == GroupTagOptions.Group)
                query = query.Include("Group");

            if ((options & GroupTagOptions.Tag) == GroupTagOptions.Tag)
                query = query.Include("Skill");

            return query;
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.SkillId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }
}
