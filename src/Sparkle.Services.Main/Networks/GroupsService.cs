
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Main.Providers;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Definitions;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Data.Options;
    using Sparkle.Services.Networks.Tags.EntityWithTag;
    using Sparkle.Services.Networks.Groups;

    public class GroupsService : ServiceBase, IGroupsService
    {
        #region pictures config

        PictureFormat pictureOriginalFormat = new PictureFormat
        {
            StretchMode = PictureStretchMode.None,
            ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.Unspecified,
            ImageQuality = ImageQuality.Unspecified,
        };

        PictureFormat[] pictureFormats = new PictureFormat[]
        {
            new PictureFormat
            {
                Name = "Small",
                FileNameFormat = "l",
                Width = 50,
                Height = 50,
                StretchMode = PictureStretchMode.Uniform,
                ClipOrigin = PictureClipOrigin.Center,
                ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.PNG,
                ImageQuality = ImageQuality.Medium,
                FilenameMaker = o => 
                {
                    var group = (Group)o;
                    return new string[]
                    {
                        "small",
                        group.Id + ".png",
                    };
                }
            },
            new PictureFormat
            {
                Name = "Medium",
                FileNameFormat = "p",
                Width = 100,
                Height = 100,
                StretchMode = PictureStretchMode.Uniform,
                ClipOrigin = PictureClipOrigin.Center,
                ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.PNG,
                ImageQuality = ImageQuality.High,
                FilenameMaker = o => 
                {
                    var group = (Group)o;
                    return new string[]
                    {
                        group.Id + ".png",
                    };
                }
            },
            new PictureFormat
            {
                Name = "Large",
                FileNameFormat = "l",
                Width = 200,
                Height = 200,
                StretchMode = PictureStretchMode.Uniform,
                ClipOrigin = PictureClipOrigin.Center,
                ImageFormat = Sparkle.Services.Networks.Definitions.ImageFormat.PNG,
                ImageQuality = ImageQuality.Medium,
                FilenameMaker = o => 
                {
                    var group = (Group)o;
                    return new string[]
                    {
                        "big",
                        group.Id + ".png",
                    };
                }
            },
        };

        #endregion

        [DebuggerStepThrough]
        internal GroupsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IGroupsRepository GroupsRepository
        {
            get { return this.Repo.Groups; }
        }

        public Group UpdateGroup(Group item)
        {
            this.VerifyNetwork(item);

            return this.GroupsRepository.Update(item);
        }

        public IList<Group> SelectAllVisibleGroups()
        {
            throw new NotImplementedException();
        }

        public IList<Group> SelectNewGroups(DateTime start)
        {
            return this.GroupsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(g => !g.IsDeleted)
                .NewGroups(start)
                .ToList();
        }

        public IList<Group> GetCreatedInRange(DateTime start, DateTime end)
        {
            return this.GroupsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .CreatedInRange(start, end)
                .Active()
                .ToList();
        }

        public IList<Group> GetWhereUserIsAdmin(int userId)
        {
            IList<GroupMember> groupsMembers = this.Services.GroupsMembers.SelectMyGroupsForGroupsService(userId);
            return groupsMembers.Select(groupMember => SelectGroupById(groupMember.GroupId)).ToList();
        }

        public int Count()
        {
            return this.GroupsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(g => !g.IsDeleted)
                .Count();
        }

        public int CreateGroup(Group item)
        {
            this.SetNetwork(item);

            return this.GroupsRepository.Insert(item).Id;
        }

        public Group Delete(Group item)
        {
            this.VerifyNetwork(item);

            item.IsDeleted = true;
            return this.GroupsRepository.Update(item);
            //this.GroupsRepository.Delete(item);
        }

        public Group SelectGroupById(int Id)
        {
            var item = this.GroupsRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(g => !g.IsDeleted)
                .WithId(Id)
                .SingleOrDefault();
            return item;
        }

        public IList<Group> SelectAllGroups()
        {
            return this.GroupsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(g => !g.IsDeleted)
                .ToList();
        }

        public IList<Group> Search(string request)
        {
            return this.GroupsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(g => !g.IsDeleted)
                            .Contain(request)
                            .Take(5)
                            .ToList();
        }

        public Group SelectByImportedId(string id)
        {
            return this.GroupsRepository.Select()
               .ByNetwork(this.Services.NetworkId)
                .Where(g => !g.IsDeleted)
                           .Where(g => g.ImportedId == id)
                           .SingleOrDefault();
        }

        public TagModel GetGroupSkill(int groupId, int skillId)
        {
            return this.Repo.GroupsSkills.NewQuery(GroupTagOptions.Tag)
                .Where(o => o.GroupId == groupId && o.SkillId == skillId)
                .ToTagModel()
                .SingleOrDefault();
        }

        public TagModel GetGroupInterest(int groupId, int interestId)
        {
            return this.Repo.GroupsInterests.NewQuery(GroupTagOptions.Tag)
                .Where(o => o.GroupId == groupId && o.InterestId == interestId)
                .ToTagModel()
                .SingleOrDefault();
        }

        public TagModel GetGroupRecreation(int groupId, int recreationId)
        {
            return this.Repo.GroupsRecreations.NewQuery(GroupTagOptions.Tag)
                .Where(o => o.GroupId == groupId && o.RecreationId == recreationId)
                .ToTagModel()
                .SingleOrDefault();
        }
        
        public IList<TagModel> GetGroupSkills(int groupId)
        {
            return this.Repo.GroupsSkills.NewQuery(GroupTagOptions.Tag)
                .Where(o => o.GroupId == groupId)
                .ToTagModel()
                .ToList();
        }

        public IList<TagModel> GetGroupInterests(int groupId)
        {
            return this.Repo.GroupsInterests.NewQuery(GroupTagOptions.Tag)
                .Where(o => o.GroupId == groupId)
                .ToTagModel()
                .ToList();
        }

        public IList<TagModel> GetGroupRecreations(int groupId)
        {
            return this.Repo.GroupsRecreations.NewQuery(GroupTagOptions.Tag)
                .Where(o => o.GroupId == groupId)
                .ToTagModel()
                .ToList();
        }

        public bool CheckUserAcccessToGroup(int groupId, int userId)
        {
            var group = this.Repo
                .Groups
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .WithId(groupId)
                .SingleOrDefault();
            
            if (group == null || !group.Members.Any(o => o.UserId == userId))
                return false;
            return true;
        }

        public SetProfilePictureResult SetProfilePicture(SetProfilePictureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (request.PictureStream == null)
                throw new ArgumentNullException("request");
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory))
                throw new InvalidOperationException("Configuration entry 'Storage.UserContentsDirectory' most have a valid value");

            var result = new SetProfilePictureResult(request);
            var item = this.GroupsRepository.GetById(request.UserId);
            IFilesystemProvider provider = new IOFilesystemProvider();
            PictureTransformer transformer = new PictureTransformer();

            string[] basePath = new string[]
            {
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                this.Services.Network.Name,
                "Groups",
            };
            foreach (var format in pictureFormats)
            {
                MemoryStream resized;
                try
                {
                    resized = transformer.FormatPicture(format, request.PictureStream);
                }
                catch (FormatException ex)
                {
                    this.Services.Logger.Error("GroupsServices.SetProfilePicture", ErrorLevel.Business, ex.ToString());
                    result.Errors.Add(SetProfilePictureError.FileIsNotPicture, NetworksEnumMessages.ResourceManager);
                    return result;
                }

                var fileName = format.FilenameMaker(item);
                var pathArray = basePath.CombineWith(fileName);
                var path = provider.EnsureFilePath(pathArray);
                provider.WriteFile(path, resized);
            }

            // save original file
            {
                var uploadPath = new string[]
                {
                    this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                    "Networks",
                    this.Services.Network.Name,
                    "Groups",
                    "Uploads",
                };
                var name = item.Id.ToString().GetIncrementedString(s => !provider.FileExists(provider.EnsureFilePath(uploadPath.CombineWith(s))));
                provider.WriteNewFile(provider.EnsureFilePath(uploadPath.CombineWith(name)), request.PictureStream);
            }

            this.Services.Logger.Info("GroupsServices.SetProfilePicture", ErrorLevel.Success, "Picture for group " + item.Id + " has been updated");
            result.Succeed = true;
            return result;
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(Group item, PictureAccessMode pictureAccessMode)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return this.GetProfilePicture(item.Id, item.Name, pictureAccessMode);
        }

        public Sparkle.Services.Networks.Models.ProfilePictureModel GetProfilePicture(int id, string name, PictureAccessMode mode)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "username");

            var item = new Group
            {
                Id = id,
                Name = name,
            };

            string rootPath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
            string[] basePath = new string[]
            {
                rootPath,
                "Networks",
                this.Services.Network.Name,
                "Groups",
            };
            var commonGroupsPath = Path.Combine(rootPath, "Networks", "Common", "Groups");
            var networkGroupsPath = Path.Combine(rootPath, "Networks", this.Services.Network.Name, "Groups");

            IFilesystemProvider provider = new IOFilesystemProvider();
            var model = new ProfilePictureModel();
            var pictures = new Dictionary<string, PictureAccess>();
            model.Pictures = pictures;
            model.UserId = id;
            model.Username = name;

            pictures.Add("Original", new PictureAccess
            {
                Format = pictureOriginalFormat.Clone(),
                FilePath = null,
                Bytes = null,
            });

            var formats = pictureFormats;
            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];
                bool found = false;

                // search group's picture path
                string remark = "user";
                var filename = format.FilenameMaker(item);
                var pathArray = basePath.CombineWith(filename);
                var path = provider.EnsureFilePath(pathArray);
                if (!(found = File.Exists(path)))
                {
                    // default picture for network
                    remark = "network default";
                    path = Path.Combine(networkGroupsPath, "Default-" + format.Name + ".png");
                    if (!(File.Exists(path)))
                    {
                        // default picture
                        remark = "default";
                        path = Path.Combine(commonGroupsPath, "Default-" + format.Name + ".png");
                        if (!(File.Exists(path)))
                        {
                            path = null;
                        }
                    }
                }

                byte[] bytes = null;
                string mime = null;
                DateTime dateChanged = DateTime.MinValue;
                if (path != null)
                {
                    dateChanged = File.GetLastWriteTimeUtc(path);
                    bytes = mode.HasFlag(PictureAccessMode.Stream) ? File.ReadAllBytes(path) : null;
                    if (bytes != null)
                    {
                        var bytes256 = bytes.Length > 600 ? bytes.Take(600).ToArray() : bytes;
                        var mimeType = MimeDetective.MimeTypes.GetFileType(bytes256);

                        if (mimeType != null)
                            mime = mimeType.Mime;
                    }
                }

                pictures.Add(format.Name, new PictureAccess
                {
                    Format = format.Clone(),
                    FilePath = mode.HasFlag(PictureAccessMode.FilePath) ? path : null,
                    Bytes = mode.HasFlag(PictureAccessMode.Stream) && path != null ? File.ReadAllBytes(path) : null,
                    MimeType = mime,
                    Remark = remark,
                    DateChangedUtc = dateChanged,
                });
            }

            return model;
        }

        private DateTime GetProfilePictureLastChangeDate(int id)
        {
            var item = new Group
            {
                Id = id,
            };

            string rootPath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
            string[] basePath = new string[]
            {
                rootPath,
                "Networks",
                this.Services.Network.Name,
                "Groups",
            };
            var commonGroupsPath = Path.Combine(rootPath, "Networks", "Common", "Groups");
            var networkGroupsPath = Path.Combine(rootPath, "Networks", this.Services.Network.Name, "Groups");

            IFilesystemProvider provider = new IOFilesystemProvider();

            var formats = pictureFormats;
            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];

                // search group's picture path
                var filename = format.FilenameMaker(item);
                var pathArray = basePath.CombineWith(filename);
                var path = provider.EnsureFilePath(pathArray);
                if (File.Exists(path))
                {
                    return File.GetLastWriteTimeUtc(path);
                }
            }

            return DateTime.MinValue;
        }

        public string GetProfileUrl(Group group, UriKind uriKind)
        {
            var path = "Groups/Group/" + group.Id;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetProfilePictureUrl(Group group, Services.Networks.Companies.CompanyPictureSize pictureSize, UriKind uriKind)
        {
            return this.GetProfilePictureUrl(group.Id, pictureSize, uriKind);
        }

        public string GetProfilePictureUrl(int groupId, Services.Networks.Companies.CompanyPictureSize pictureSize, UriKind uriKind)
        {
            var path = string.Format(
                "Data/GroupPicture/{0}/{1}/{2}",
                groupId,
                pictureSize.ToString(),
                this.GetProfilePictureLastChangeDate(groupId).ToString(PictureAccess.CacheDateFormat));
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public IList<TagModel> GetSkills()
        {
            ////this.Repo.GroupsSkills.GetStats();
            ////var items = this.Repo.GroupsSkills.Select()
            ////    .GroupBy(gs => gs.SkillId)
            ////    .Select(s => new
            ////    {
            ////        Item = s.Key,
            ////        Count = s.Where(x => x.DeletedDateUtc == null && !x.Group.IsDeleted).Count(),
            ////    })
            ////    .Where(x => x.Count > 0)
            ////    .ToList();
            var items = this.Repo.GroupsSkills.GetStats();
            return items
                .Select(x => new TagModel
                {
                    Id = x.Key,
                    Weight = x.Value,
                })
                .ToList();
        }

        public IList<TagModel> GetRecreations()
        {
            ////var items = this.Repo.Recreations.Select()
            ////    .OrderBy(s => s.TagName)
            ////    .Select(s => new
            ////    {
            ////        Item = s,
            ////        Count = s.Groups.Where(x => x.DeletedDateUtc == null && !x.Group.IsDeleted).Count(),
            ////    })
            ////    .Where(x => x.Count > 0)
            ////    .ToList();
            var items = this.Repo.GroupsRecreations.GetStats();
            return items
                .Select(x => new TagModel
                {
                    Id = x.Key,
                    Weight = x.Value,
                })
                .ToList();
        }

        public IList<TagModel> GetInterests()
        {
            ////var items = this.Repo.Interests.Select()
            ////    .OrderBy(s => s.TagName)
            ////    .Select(s => new
            ////    {
            ////        Item = s,
            ////        Count = s.Groups.Where(x => x.DeletedDateUtc == null && !x.Group.IsDeleted).Count(),
            ////    })
            ////    .Where(x => x.Count > 0)
            ////    .ToList();
            var items = this.Repo.GroupsInterests.GetStats();
            return items
                .Select(x => new TagModel
                {
                    Id = x.Key,
                    Weight = x.Value,
                })
                .ToList();
        }

        public IDictionary<int, GroupModel> GetById(int[] groupIds, GroupOptions options)
        {
            var items = this.Repo.Groups.GetById(groupIds, this.Services.NetworkId, options);
            var models = new Dictionary<int, GroupModel>(items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                models.Add(items[i].Id, new GroupModel(items[i]));
            }

            return models;
        }

        public GroupModel GetById(int groupId, GroupOptions options)
        {
            var item = this.Repo.Groups.GetById(groupId, this.Services.NetworkId, options);
            return item != null ? new GroupModel(item) : null;
        }

        public AjaxTagPickerModel GetAjaxTagPickerModel(int groupId, int actingUserId)
        {
            var group = this.GroupsRepository.GetById(groupId);
            if (group == null)
                return null;

            if (group.IsDeleted)
                return null;

            this.VerifyNetwork(group);

            var user = this.Services.People.GetActiveById(actingUserId, PersonOptions.None);
            if (group == null || user == null)
                return null;

            bool canEdit = false;
            var membership = this.Repo.GroupsMembers.GetActualMembership(this.Services.NetworkId, groupId, user.Id);
            if (membership != null && membership.RightStatus == GroupMemberRight.Admin)
                canEdit = true;

            var categories = this.Services.Tags.GetCategoriesApplyingToGroups();
            var tags = this.Services.Tags.GetTagsByGroupIdAndCategories(group.Id, categories);
            return new AjaxTagPickerModel(tags, canEdit);
        }

        public string MakeAlias(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (this.Repo.Groups.GetByAlias(alias, GroupOptions.None) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.Groups.GetByAlias(a, GroupOptions.None) == null);
            }

            return alias;
        }

        public void Initialize()
        {
            {
                var missingAliases = this.Repo.Groups.CreateQuery(GroupOptions.None)
                    .Where(x => x.Alias == null)
                    .ToList();
                if (missingAliases.Count > 0)
                {
                    foreach (var item in missingAliases)
                    {
                        var alias = this.MakeAlias(item.Name);
                        item.Alias = alias;
                        this.Repo.Groups.Update(item);
                        this.Services.Logger.Info("GroupsService.Initialize", ErrorLevel.Success, "Updated group #" + item.Id + ": set Alias='" + item.Alias + "'.");
                    }
                }
            }
        }

        public GroupModel GetByAlias(string alias, GroupOptions options)
        {
            var item = this.Repo.Groups.GetByAlias(alias, this.Services.NetworkId, options);
            return item != null ? new GroupModel(item) : null;
        }

        public bool IsUserAuthorized(int groupId, int actingUserId, GroupAction action)
        {
            var group = this.Repo.Groups.GetById(groupId, this.Services.NetworkId, GroupOptions.None);
            if (group == null)
                return false;

            bool userIsAdmin = false, hasRights = false;
            var user = this.Services.People.GetByIdFromAnyNetwork(actingUserId, PersonOptions.Company);

            if (user == null || !user.IsActive)
                return false;

            if (user != null)
            {
                if (user.NetworkAccessLevel != null && (user.NetworkAccessLevel.Value & NetworkAccessLevel.SparkleStaff) == NetworkAccessLevel.SparkleStaff)
                {
                    userIsAdmin = true;
                }
                else if (user.NetworkId == this.Services.NetworkId && user.NetworkAccessLevel != null && user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ModerateNetwork))
                {
                    userIsAdmin = true;
                }
            }

            if (!userIsAdmin)
            {
                var membership = this.Repo.GroupsMembers.GetActualMembership(this.Services.NetworkId, group.Id, user.Id);
                if (membership != null)
                {
                    if (membership.RightStatus == GroupMemberRight.Admin)
                        hasRights = true;
                }
            }

            return userIsAdmin || hasRights;
        }

        internal static void RegisterTags(TagsService tags)
        {
            tags.RegisterEntityValidator("Group", GroupsService.ValidateEntity);
            tags.RegisterTagValidator("Group", GroupsService.ValidateTag);
            tags.RegisterTagRepository("Group", EntityWithTagRepositoryType.Sql, "GroupTags", "RelationId");
        }

        private static bool ValidateEntity(IServiceFactory services, string entityIdentifier, AddOrRemoveTagResult result, out int entityId)
        {
            entityId = 0;
            var group = services.Groups.GetByAlias(entityIdentifier, GroupOptions.None);
            if (group == null)
            {
                if (int.TryParse(entityIdentifier, out entityId))
                {
                    group = services.Groups.GetById(entityId, GroupOptions.None);
                }

                if (group == null)
                {
                    result.Errors.Add(AddOrRemoveTagError.NoSuchGroup, NetworksEnumMessages.ResourceManager);
                    return false;
                }
            }

            entityId = group.Id;
            return true;
        }

        private static bool ValidateTag(IServiceFactory services, string entityIdentifier, string tagId, int? actingUserId, TagCategoryModel tagCategory, AddOrRemoveTagResult result)
        {
            if (!actingUserId.HasValue)
                throw new ArgumentNullException("The value cannot be empty.", "actingUserId");
            if (tagCategory == null || tagCategory.RulesModel == null || !tagCategory.RulesModel.Rules.ContainsKey(RuleType.Group))
                throw new ArgumentNullException("The value cannot be empty.", "tagCategory");

            // Check company exists
            int groupId;
            if (!GroupsService.ValidateEntity(services, entityIdentifier, result, out groupId))
            {
                result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
            }

            // Check user rights
            if (actingUserId == null)
            {
                result.Errors.Add(AddOrRemoveTagError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return false;
            }

            var authorized = services.Groups.IsUserAuthorized(groupId, actingUserId.Value, GroupAction.ChangeTags);
            if (!authorized)
            {
                result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Check TagCategory company rules
            var max = tagCategory.RulesModel.Rules[RuleType.Group].Maximum;
            if (result.Request.AddTag && services.Repositories.GroupTags.CountByGroupAndCategory(groupId, tagCategory.Id, false) >= max)
            {
                result.Errors.Add(AddOrRemoveTagError.MaxNumberOfTagForCategory, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Build EntityWithTag into result
            result.Entity = new SqlEntityWithTag
            {
                EntityId = groupId,
            };

            return true;
        }

        private static bool IsNameAllowed(string name)
        {
            if (name == null)
                return false;

            if (string.IsNullOrEmpty(name.Trim()))
                return false;

            int integer;
            if (int.TryParse(name, out integer))
                return false;

            return true;
        }
    }
}
