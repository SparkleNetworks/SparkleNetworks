
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using SrkToolkit.Domain;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public class GroupsTagsService : ServiceBase, IGroupsTagsService
    {
        [DebuggerStepThrough]
        internal GroupsTagsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public AddGroupTagResult AddSkill(AddGroupTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            var result = new AddGroupTagResult(request);

            // Check de l'existence du groupe
            var group = this.Services.Groups.SelectGroupById(request.GroupId);
            if (group == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // Check de l'existence du tag
            Skill tag = null;
            if (request.TagId == -1)
            {
                // S'il n'existe pas on le créé
                tag = this.Services.Skills.Insert(new Skill
                    {
                        TagName = request.TagName,
                        Date = DateTime.UtcNow,
                        CreatedByUserId = request.UserId,
                    });
            }
            else
                tag = this.Services.Skills.GetById(request.TagId);

            // Check de l'existence du tag dans le groupe
            var relation = this.Repo.GroupsSkills.Select().Where(o => o.SkillId == tag.Id && o.GroupId == group.Id).SingleOrDefault();
            if (relation != null)
            {
                // Soit le tag est toujours présent
                if (relation.DeletedByUserId == null)
                    result.Errors.Add(AddGroupTagError.AlreadyAdded, NetworksEnumMessages.ResourceManager);
                // Soit il est présent mais marqué comme supprimé
                else
                {
                    // Si le user qui l'a supprimé est le user courant, on rajoute le tag
                    if (relation.DeletedByUserId == request.UserId)
                    {
                        relation.DeletedByUserId = null;
                        relation.DeletedDateUtc = null;
                        relation.DeleteReason = null;
                        this.Repo.GroupsSkills.Update(relation);
                        result.TagId = request.TagId;
                        result.Succeed = true;
                        return result;
                    }
                    else
                    {
                        var userDeleted = this.Services.People.SelectWithId(relation.DeletedByUserId ?? 0);
                        if (userDeleted != null)
                        {
                            result.Errors.Add(AddGroupTagError.CannotAddDeletedTag, NetworksEnumMessages.ResourceManager);
                            result.OptionnalMessage = userDeleted.FullName + "|" + new EnumValueText<WallItemDeleteReason>((WallItemDeleteReason)relation.DeleteReason, NetworksEnumMessages.ResourceManager).Text;
                        }
                        else
                            result.Errors.Add(AddGroupTagError.CannotAddDeletedTagDefault, NetworksEnumMessages.ResourceManager);
                    }
                }

                return result;
            }

            // Enfin on ajoute le tag au group
            this.Repo.GroupsSkills.Insert(new GroupSkill
            { 
                GroupId = group.Id,
                SkillId = tag.Id,
                DateCreatedUtc = DateTime.UtcNow,
                CreatedByUserId = request.UserId,
            });
            result.TagId = tag.Id;
            result.Succeed = true;
            return result;
        }

        public AddGroupTagResult AddInterest(AddGroupTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            var result = new AddGroupTagResult(request);

            // Check de l'existence du groupe
            var group = this.Services.Groups.SelectGroupById(request.GroupId);
            if (group == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // Check de l'existence du tag
            Interest tag = null;
            if (request.TagId == -1)
            {
                // S'il n'existe pas on le créé
                tag = this.Services.Interests.Insert(new Interest
                {
                    TagName = request.TagName,
                    Date = DateTime.UtcNow,
                    CreatedByUserId = request.UserId,
                });
            }
            else
                tag = this.Services.Interests.GetById(request.TagId);

            // Check de l'existence du tag dans le groupe
            var relation = this.Repo.GroupsInterests.Select().Where(o => o.InterestId == tag.Id && o.GroupId == group.Id).SingleOrDefault();
            if (relation != null)
            {
                // Soit le tag est toujours présent
                if (relation.DeletedByUserId == null)
                    result.Errors.Add(AddGroupTagError.AlreadyAdded, NetworksEnumMessages.ResourceManager);
                // Soit il est présent mais marqué comme supprimé
                else
                {
                    // Si le user qui l'a supprimé est le user courant, on rajoute le tag
                    if (relation.DeletedByUserId == request.UserId)
                    {
                        relation.DeletedByUserId = null;
                        relation.DeletedDateUtc = null;
                        relation.DeleteReason = null;
                        this.Repo.GroupsInterests.Update(relation);
                        result.TagId = request.TagId;
                        result.Succeed = true;
                        return result;
                    }
                    else
                    {
                        var userDeleted = this.Services.People.SelectWithId(relation.DeletedByUserId ?? 0);
                        if (userDeleted != null)
                        {
                            result.Errors.Add(AddGroupTagError.CannotAddDeletedTag, NetworksEnumMessages.ResourceManager);
                            result.OptionnalMessage = userDeleted.FullName + "|" + new EnumValueText<WallItemDeleteReason>((WallItemDeleteReason)relation.DeleteReason, NetworksEnumMessages.ResourceManager).Text;
                        }
                        else
                            result.Errors.Add(AddGroupTagError.CannotAddDeletedTagDefault, NetworksEnumMessages.ResourceManager);
                    }
                }
                return result;
            }

            // Enfin on ajoute le tag au group
            this.Repo.GroupsInterests.Insert(new GroupInterest
            {
                GroupId = group.Id,
                InterestId = tag.Id,
                DateCreatedUtc = DateTime.UtcNow,
                CreatedByUserId = request.UserId,
            });
            result.TagId = tag.Id;
            result.Succeed = true;
            return result;
        }

        public AddGroupTagResult AddRecreation(AddGroupTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            var result = new AddGroupTagResult(request);

            // Check de l'existence du groupe
            var group = this.Services.Groups.SelectGroupById(request.GroupId);
            if (group == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // Check de l'existence du tag
            Recreation tag = null;
            int tagId = request.TagId;
            if (request.TagId == -1)
            {
                // S'il n'existe pas on le créé
                tagId = this.Services.Recreations.Insert(new Recreation
                {
                    TagName = request.TagName,
                    Date = DateTime.UtcNow,
                    CreatedByUserId = request.UserId,
                });
            }
            tag = this.Services.Recreations.GetById(tagId);

            // Check de l'existence du tag dans le groupe
            var relation = this.Repo.GroupsRecreations.Select().Where(o => o.RecreationId == tag.Id && o.GroupId == group.Id).SingleOrDefault();
            if (relation != null)
            {
                // Soit le tag est toujours présent
                if (relation.DeletedByUserId == null)
                    result.Errors.Add(AddGroupTagError.AlreadyAdded, NetworksEnumMessages.ResourceManager);
                // Soit il est présent mais marqué comme supprimé
                else
                {
                    // Si le user qui l'a supprimé est le user courant, on rajoute le tag
                    if (relation.DeletedByUserId == request.UserId)
                    {
                        relation.DeletedByUserId = null;
                        relation.DeletedDateUtc = null;
                        relation.DeleteReason = null;
                        this.Repo.GroupsRecreations.Update(relation);
                        result.TagId = request.TagId;
                        result.Succeed = true;
                        return result;
                    }
                    else
                    {
                        var userDeleted = this.Services.People.SelectWithId(relation.DeletedByUserId ?? 0);
                        if (userDeleted != null)
                        {
                            result.Errors.Add(AddGroupTagError.CannotAddDeletedTag, NetworksEnumMessages.ResourceManager);
                            result.OptionnalMessage = userDeleted.FullName + "|" + new EnumValueText<WallItemDeleteReason>((WallItemDeleteReason)relation.DeleteReason, NetworksEnumMessages.ResourceManager).Text;
                        }
                        else
                            result.Errors.Add(AddGroupTagError.CannotAddDeletedTagDefault, NetworksEnumMessages.ResourceManager);
                    }
                }
                return result;
            }

            // Enfin on ajoute le tag au group
            this.Repo.GroupsRecreations.Insert(new GroupRecreation
            {
                GroupId = group.Id,
                RecreationId = tag.Id,
                DateCreatedUtc = DateTime.UtcNow,
                CreatedByUserId = request.UserId,
            });
            result.TagId = tag.Id;
            result.Succeed = true;
            return result;
        }

        public AddGroupTagResult RemoveSkill(AddGroupTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            var result = new AddGroupTagResult(request);

            // On check l'existence du groupe courant
            var group = this.Services.Groups.SelectGroupById(request.GroupId);
            if (group == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // On check l'existence du tag dans le groupe
            var relation = group.Skills.Where(o => o.SkillId == request.TagId).SingleOrDefault();
            if (relation == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchTagInGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }
            // Si il est deja marque comme supprime on arrete
            if (relation.DeletedByUserId != null && relation.DeletedDateUtc != null)
            {
                result.Errors.Add(AddGroupTagError.AlreadyDeleted, NetworksEnumMessages.ResourceManager);
                return result;
            }

            relation.DeletedByUserId = request.UserId;
            relation.DeletedDateUtc = DateTime.UtcNow;
            relation.DeleteReason = (byte)request.Reason;
            this.Repo.GroupsSkills.Update(relation);
            result.Succeed = true;
            return result;
        }

        public AddGroupTagResult RemoveInterest(AddGroupTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            var result = new AddGroupTagResult(request);

            // On check l'existence du groupe courant
            var group = this.Services.Groups.SelectGroupById(request.GroupId);
            if (group == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // On check l'existence du tag dans le groupe
            var relation = group.Interests.Where(o => o.InterestId == request.TagId).SingleOrDefault();
            if (relation == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchTagInGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }
            // Si il est deja marque comme supprime on arrete
            if (relation.DeletedByUserId != null && relation.DeletedDateUtc != null)
            {
                result.Errors.Add(AddGroupTagError.AlreadyDeleted, NetworksEnumMessages.ResourceManager);
                return result;
            }

            relation.DeletedByUserId = request.UserId;
            relation.DeletedDateUtc = DateTime.UtcNow;
            relation.DeleteReason = (byte)request.Reason;
            this.Repo.GroupsInterests.Update(relation);
            result.Succeed = true;
            return result;
        }
        
        public AddGroupTagResult RemoveRecreation(AddGroupTagRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            var result = new AddGroupTagResult(request);

            // On check l'existence du groupe courant
            var group = this.Services.Groups.SelectGroupById(request.GroupId);
            if (group == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }

            // On check l'existence du tag dans le groupe
            var relation = group.Recreations.Where(o => o.RecreationId == request.TagId).SingleOrDefault();
            if (relation == null)
            {
                result.Errors.Add(AddGroupTagError.NoSuchTagInGroup, NetworksEnumMessages.ResourceManager);
                return result;
            }
            // Si il est deja marque comme supprime on arrete
            if (relation.DeletedByUserId != null && relation.DeletedDateUtc != null)
            {
                result.Errors.Add(AddGroupTagError.AlreadyDeleted, NetworksEnumMessages.ResourceManager);
                return result;
            }

            relation.DeletedByUserId = request.UserId;
            relation.DeletedDateUtc = DateTime.UtcNow;
            relation.DeleteReason = (byte)request.Reason;
            this.Repo.GroupsRecreations.Update(relation);
            result.Succeed = true;
            return result;
        }

        public IList<GroupSkill> GetGroupSkillsByIds(int[] skillIds)
        {
            return this.Repo.GroupsSkills.Select()
                .Where(gs => skillIds.Contains(gs.SkillId))
                .ToList();
        }

        public IList<GroupInterest> GetGroupInterestsByIds(int[] interestIds)
        {
            return this.Repo.GroupsInterests.Select()
                .Where(gi => interestIds.Contains(gi.InterestId))
                .ToList();
        }

        public IList<GroupRecreation> GetGroupRecreationsByIds(int[] recreationIds)
        {
            return this.Repo.GroupsRecreations.Select()
                .Where(gr => recreationIds.Contains(gr.RecreationId))
                .ToList();
        }

        public IList<GroupSkill> GetAllGroupSkills(GroupTagOptions options)
        {
            return this.Repo.GroupsSkills
                .NewQuery(options)
                .ToList();
        }

        public IList<GroupInterest> GetAllGroupInterests(GroupTagOptions options)
        {
            return this.Repo.GroupsInterests
                .NewQuery(options)
                .ToList();
        }

        public IList<GroupRecreation> GetAllGroupRecreations(GroupTagOptions options)
        {
            return this.Repo.GroupsRecreations
                .NewQuery(options)
                .ToList();
        }
    }
}
