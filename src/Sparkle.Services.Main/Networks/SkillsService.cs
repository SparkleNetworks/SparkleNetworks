
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.Domain;
    using Sparkle.Infrastructure;

    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public class SkillsService : ServiceBase, ISkillsService
    {
        private static readonly string[] excludeWords = new string[]
        {
            "le", "la", "les", "l",
            "the", "a",
        };

        internal SkillsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ISkillsRepository SkillsRepository
        {
            get { return this.Repo.Skills; }
        }

        /// <summary>
        /// Updates the skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Skill Update(Skill item)
        {
            return this.SkillsRepository.Update(item);
        }

        /// <summary>
        /// Inserts the skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Skill Insert(Skill item)
        {
            return this.SkillsRepository.Insert(item);
        }

        /// <summary>
        /// Deletes the skill.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(Skill item)
        {
            this.SkillsRepository.Delete(item);
        }

        public void DeleteByUserId(Guid userId)
        {
            //this.SkillsRepository.DeleteSkill(item);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<Skill> SelectAll()
        {
            return SkillsRepository
                .Select()
                //.ChildOf(1)
                .OrderBy(o => o.TagName)
                .ToList();
        }

        /// <summary>
        /// Selects the skills by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Skill GetById(int id)
        {
            return SkillsRepository
                .Select()
                .ById(id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the name of the skills by skill.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Skill GetByName(string name)
        {
            return SkillsRepository
                .Select()
                .ByName(name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the skills from user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Skill> SelectSkillsFromUserId(Guid userId)
        {
            return SkillsRepository
                .Select()
                .ChildOf(1)
                .OrderBy(o => o.TagName)
                .ToList();
        }

        /// <summary>
        /// Searches the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="take">The take.</param>
        /// <returns></returns>
        public IList<Skill> Search(string request, int take)
        {
            var list = new Dictionary<int, Skill>();

            list.AddRange(this.SearchImpl(request, take), s => s.Id, s => s);

            var splitted = request.Split(' ', '\t', '\r', '\n', ',', ';', '/', '&', '\'');
            splitted = splitted
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => !string.IsNullOrEmpty(s) && !excludeWords.Contains(s))
                .Select(s => s.EndsWith("ies") ? s.Substring(0, s.Length - 3) : s.EndsWith("s") ? s.Substring(0, s.Length - 1) : s)
                .ToArray();

            for (int i = 0; i < splitted.Length; i++)
            {
                var results = this.SearchImpl(splitted[i], take);
                for (int j = 0; j < results.Length; j++)
                {
                    if (!list.ContainsKey(results[j].Id))
                        list.Add(results[j].Id, results[j]);
                }
            }

            return list.Values.Take(take).ToList();
        }

        private Skill[] SearchImpl(string request, int take)
        {
            return this.SkillsRepository
                .Select()
                .Contain(request)
                .OrderBy(o => o.TagName)
                .Take(take)
                .ToArray();
        }

        public IList<Skill> GetById(int[] ids)
        {
            return SkillsRepository
                .Select()
                .Where(s => ids.Contains(s.Id))
                .ToList();
        }

        public int CountCompanyProfiles(int id, bool allNetworks)
        {
            if (allNetworks)
            {
                return this.Repo.CompaniesSkills.CountCompaniesBySkillId(id);
            }
            else
            {
                return this.Repo.CompaniesSkills.CountCompaniesBySkillId(id, this.Services.NetworkId);
            }
        }

        public int CountUserProfiles(int id, bool allNetworks)
        {
            if (allNetworks)
            {
                return this.Repo.PeoplesSkills.CountProfilesBySkillId(id);
            }
            else
            {
                return this.Repo.PeoplesSkills.CountProfilesBySkillId(id, this.Services.NetworkId);
            }
        }

        public int CountTimelineItems(int id, bool allNetworks)
        {
            if (allNetworks)
            {
                return this.Repo.TimelineItemSkills.CountBySkillId(id);
            }
            else
            {
                return this.Repo.TimelineItemSkills.CountBySkillId(id, this.Services.NetworkId);
            }
        }

        public int CountGroups(int id, bool allNetworks)
        {
            if (allNetworks)
            {
                return this.Repo.GroupsSkills.CountBySkillId(id);
            }
            else
            {
                return this.Repo.GroupsSkills.CountBySkillId(id, this.Services.NetworkId);
            }
        }

        public IList<TagModel> GetAll()
        {
            return this.Repo.Skills.GetAll().Select(t => new TagModel(t.Value)).ToList();
        }

        public RenameTagResult Rename(RenameTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new RenameTagResult(request);

            var actingUser = this.Repo.People.GetLiteById(this.Services.Context.UserId.Value);

            var item = this.SkillsRepository.GetById(request.Id);
            if (item == null)
                result.Errors.Add(RenameTagError.NoSuchTag, NetworksEnumMessages.ResourceManager);

            var sameName = this.SkillsRepository.GetByName(request.NewName);
            if (sameName.Id == item.Id)
                result.Errors.Add(RenameTagError.SameTag, NetworksEnumMessages.ResourceManager);

            if (result.Errors.Count > 0)
                return result;

            var oldName = item.TagName;
            if (request.AllNetworks)
            {
                item.TagName = request.NewName;
                this.Repo.Skills.Update(item);
                this.Services.Logger.Info("SkillsService", ErrorLevel.Business, "User {3} renamed all-networks Skill {0} from '{1}' to '{2}'", item.Id.ToString(), oldName, item.TagName, actingUser.ToString());
            }
            else
            {
                var targetSkill = this.Repo.Skills.GetByName(request.NewName);
                if (targetSkill == null)
                {
                    targetSkill = new Skill
                    {
                        CreatedByUserId = actingUser.Id,
                        Date = DateTime.Now,
                        TagName = request.NewName,
                    };
                    this.Repo.Skills.Insert(targetSkill);
                }

                var companies = this.Repo.CompaniesSkills.GetBySkillId(request.Id, this.Services.NetworkId);
                var people = this.Repo.PeoplesSkills.GetBySkillId(request.Id, this.Services.NetworkId);
                var groups = this.Repo.GroupsSkills.GetBySkillId(request.Id, this.Services.NetworkId, GroupOptions.None);
                var timelineItems = this.Repo.TimelineItemSkills.GetBySkillId(request.Id, this.Services.NetworkId, TimelineItemOptions.None);

                foreach (var association in companies)
                {
                    association.SkillId = targetSkill.Id;
                    this.Repo.CompaniesSkills.Update(association);
                }

                foreach (var association in people)
                {
                    association.SkillId = targetSkill.Id;
                    this.Repo.PeoplesSkills.Update(association);
                }

                foreach (var association in groups)
                {
                    association.SkillId = targetSkill.Id;
                    this.Repo.GroupsSkills.Update(association);
                }

                foreach (var association in timelineItems)
                {
                    association.SkillId = targetSkill.Id;
                    this.Repo.TimelineItemSkills.Update(association);
                }

                this.Services.Logger.Info("SkillsService", ErrorLevel.Business, "User {3} renamed Skill {0} from '{1}' to '{2}' for network {4}", item.Id.ToString(), oldName, item.TagName, actingUser.ToString(), this.Services.NetworkId.ToString());
            }

            result.Succeed = true;
            return result;
        }

        public IDictionary<string, int> GetByNames(string[] names)
        {
            return this.Repo.Skills
                .GetByNames(names)
                .ToDictionary(o => o.TagName.RemoveDiacritics().ToLowerInvariant(), o => o.Id);
        }

        public IDictionary<int, Tag2Model> GetAllForCache()
        {
            return this.Repo.Skills.GetAll()
                .ToDictionary(t => t.Key, t => new Tag2Model(t.Value));
        }

        public IList<TagStat> GetTop()
        {
            if (!this.Services.AppConfiguration.Tree.Features.Tags.DisableV1)
            {
                var items = this.SkillsRepository.GetTop(this.Services.NetworkId);
                return items;
            }
            else
            {
                var category = this.Services.Tags.GetCategoryByAlias("Skill");
                if (category == null)
                    return new List<TagStat>();

                var categoryIds = new int[] { category.Id, };
                var usersTops = this.Repo.UserTags.GetTop(categoryIds, this.Services.NetworkId);
                var companiesTops = this.Repo.CompanyTags.GetTop(categoryIds, this.Services.NetworkId);
                var ids = usersTops.Keys.Concat(companiesTops.Keys).ToArray();
                var items = this.Services.Tags.GetTagsById(ids)
                    .Select(x => new TagStat
                    {
                        TagId = x.Id,
                        TagName = x.Name,
                    })
                    .ToList();

                foreach (var item in items)
                {
                    if (usersTops.ContainsKey(item.TagId))
                    {
                        item.UsersCount = usersTops[item.TagId];
                    }

                    if (companiesTops.ContainsKey(item.TagId))
                    {
                        item.CompaniesCount = companiesTops[item.TagId];
                    }

                    item.TotalCount = item.UsersCount + item.CompaniesCount;
                }

                return items
                    .OrderByDescending(x => x.TotalCount)
                    .ToList();
            }
        }
    }
}
