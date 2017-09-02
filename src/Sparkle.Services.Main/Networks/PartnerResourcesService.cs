
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Main.Providers;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Definitions;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.PartnerResources;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PartnerResourcesService : ServiceBase, IPartnerResourcesService
    {
        [DebuggerStepThrough]
        internal PartnerResourcesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        #region pictureformats
        PictureFormat[] pictureFormats = new PictureFormat[]
        {
            new PictureFormat
            {
                Name = "Original",
                FileNameFormat = "",
                StretchMode = PictureStretchMode.None,
                ImageFormat = ImageFormat.JPEG,
                ImageQuality = ImageQuality.Lossless,
                FilenameMaker = o =>
                {
                    var id = (string)o;
                    return new string[]
                    {
                        id,
                        ".jpg",
                    };
                }
            },
            new PictureFormat
            {
                Name = "Small",
                FileNameFormat = "s",
                Width = 50,
                Height = 50,
                StretchMode = PictureStretchMode.Uniform,
                ClipOrigin = PictureClipOrigin.Center,
                ImageFormat = ImageFormat.JPEG,
                ImageQuality = ImageQuality.Medium,
                FilenameMaker = o =>
                {
                    var id = (string)o;
                    return new string[]
                    {
                        id,
                        "s",
                        ".jpg",
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
                ImageFormat = ImageFormat.JPEG,
                ImageQuality = ImageQuality.Medium,
                FilenameMaker = o =>
                {
                    var id = (string)o;
                    return new string[]
                    {
                        id,
                        "p",
                        ".jpg",
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
                ImageFormat = ImageFormat.JPEG,
                ImageQuality = ImageQuality.Medium,
                FilenameMaker = o =>
                {
                    var id = (string)o;
                    return new string[]
                    {
                        id,
                        "l",
                        ".jpg",
                    };
                }
            },
        }; 
        #endregion

        public PartnerResourceEditRequest GetEditRequest(string alias, string city = null, bool isAdmin = false)
        {
            var request = new PartnerResourceEditRequest();
            this.FillPartnerResourceEditRequestLists(request);

            PartnerResource item = null;
            if (isAdmin)
            {
                item = this.Repo.PartnerResources.GetByAlias(alias);
            }
            else
            {
                item = this.Repo.PartnerResources.GetActiveByAlias(alias);
            }

            if (item != null)
            {
                var profileFields = this.Repo.PartnerResourceProfileFields.GetByPartnerResourceId(item.Id);
                request.UpdateFrom(item, profileFields);

                var tags = this.Services.Tags.GetTagsFromPartnerResourceId(item.Id);
                request.TagModels = tags;
                request.Cities = this.Services.Tags.GetSOTagFromTags(tags.Where(o => o.CategoryValue == TagType.City).ToList());

                if (tags.Any(o => o.CategoryValue == TagType.PartnerResource))
                {
                    var category = tags.Where(o => o.CategoryValue == TagType.PartnerResource).SingleOrDefault();
                    request.Category = category.Alias;
                    request.CategoryTitle = category.Name;
                }

                request.CreatedBy = new UserModel(item.CreatedByUser);
                if (item.ApprovedByUserId.HasValue)
                    request.ApprovedBy = new UserModel(item.ApprovedByUser);
            }
            else
            {
                if (!string.IsNullOrEmpty(city))
                {
                    var cityTag = this.Services.Tags.GetByAlias(city);
                    if (cityTag != null)
                        request.Cities = cityTag.Id + "_" + cityTag.Name;
                }
            }

            return request;
        }

        public PartnerResourceEditResult SavePartnerResource(PartnerResourceEditRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            this.FillPartnerResourceEditRequestLists(request);
            var result = new PartnerResourceEditResult(request);

            var user = this.Services.People.SelectWithId(request.UserId);
            if (user == null || !this.Services.People.IsActive(user))
            {
                result.Errors.Add(PartnerResourceEditError.NoSuchUserOrInactiveOrUnauthorized, NetworksEnumMessages.ResourceManager);
                return result;
            }
            var tags = this.Services.Tags.GetTagsFromSOTagString(request.Cities);
            if (tags.Count == 0)
            {
                result.Errors.Add(PartnerResourceEditError.CityIsRequired, NetworksEnumMessages.ResourceManager);
                return result;
            }

            var isApproved = user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManagePartnerResources, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);
            PartnerResource item = null;
            if (request.PartnerId.HasValue)
            {
                item = this.Repo.PartnerResources.GetActiveById(request.PartnerId.Value);
                if (item == null)
                {
                    result.Errors.Add(PartnerResourceEditError.NoSuchPartnerResource, NetworksEnumMessages.ResourceManager);
                    return result;
                }

                item.Name = request.Name;
                item.Available = request.IsAvailable;
                item = this.Repo.PartnerResources.Update(item);

                this.Services.Wall.PublishPartnerResourceUpdate(item, user.Id, false);
            }
            else
            {
                item = new PartnerResource();

                item.NetworkId = this.Services.NetworkId;
                item.Name = request.Name;
                item.Alias = request.Name.MakeUrlFriendly(true).GetIncrementedString(s => this.Repo.PartnerResources.GetActiveByAlias(s) == null);
                item.Available = request.IsAvailable;
                item.CreatedByUserId = user.Id;
                item.DateCreatedUtc = DateTime.UtcNow;

                if (isApproved)
                {
                    item.IsApproved = true;
                    item = this.Repo.PartnerResources.Insert(item);

                    this.Services.Wall.PublishPartnerResourceUpdate(item, user.Id, true);
                    result.ToApprove = false;
                }
                else
                {
                    item.IsApproved = false;
                    item = this.Repo.PartnerResources.Insert(item);

                    var adminModel = this.Services.Email.GetAdminWorkModel();
                    var adminWork = AdminWorkModel.For(this.Services, item, AdminWorkPriority.Current);
                    adminModel.Items.Add(adminWork);
                    this.Services.Email.SendAdminWorkEmail(adminModel, true, NetworkAccessLevel.ManagePartnerResources, NetworkAccessLevel.NetworkAdmin);
                    result.ToApprove = true;
                }
            }

            this.UpdatePartnerResourceProfileFieldsFromRequest(item.Id, request);

            if (!string.IsNullOrEmpty(request.Category))
            {
                var cat = this.Services.Tags.GetByAlias(request.Category);
                if (cat != null)
                    tags.Add(cat);
            }
            this.UpdatePartnerResourceTags(item.Id, tags);

            result.Alias = item.Alias;
            result.Succeed = true;
            return result;
        }

        private void UpdatePartnerResourceTags(int partnerId, IList<Tag2Model> tags)
        {
            var partnerTags = this.Repo.PartnerResourceTags.GetByPartnerResourceId(partnerId);
            foreach (var item in tags)
            {
                var local = partnerTags.Where(o => o.TagId == item.Id).SingleOrDefault();
                if (local != null)
                {
                    // tag is here, do nothing
                }
                else
                {
                    // insert
                    local = new PartnerResourceTag
                    {
                        PartnerResourceId = partnerId,
                        TagId = item.Id,
                        DateCreatedUtc = DateTime.UtcNow,
                    };
                    this.Repo.PartnerResourceTags.Insert(local);
                }
            }

            foreach (var local in partnerTags)
            {
                if (!tags.Any(o => o.Id == local.TagId))
                {
                    this.Repo.PartnerResourceTags.Delete(local);
                }
            }
        }

        private void UpdatePartnerResourceProfileFieldsFromRequest(int partnerId, PartnerResourceEditRequest request)
        {
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.Site, request.Website);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.Location, request.Address);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.ZipCode, request.ZipCode);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.City, request.City);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.Country, request.Country);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.About, request.About);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.ContactGuideline, request.ContactGuideline);

            var industry = request.Industries.Where(o => o != null && o.SelecterId == request.Industry).SingleOrDefault();
            if (industry != null)
                this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.Industry, industry.Value);

            var contact = new PartnerResourceProfileFieldModel(ProfileFieldType.Contact);
            contact.ContactModel = new ContactProfileFieldModel(request.FirstName, request.LastName, request.Job, request.Phone, request.Email);
            this.SetPartnerResourceProfileField(partnerId, ProfileFieldType.Contact, contact.Value, contact.Data);
        }

        public void SetPartnerResourceProfileField(int partnerId, ProfileFieldType type, string value, string data = null)
        {
            var item = this.Repo.PartnerResourceProfileFields.GetByPartnerIdAndFieldType(partnerId, type);

            if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(data))
            {
                if (item != null)
                {
                    this.Repo.PartnerResourceProfileFields.Delete(item);
                }
            }
            else
            {
                if (item == null)
                {
                    item = new PartnerResourceProfileField
                    {
                        PartnerResourceId = partnerId,
                        ProfileFieldId = (int)type,
                        Value = value,
                        DateCreatedUtc = DateTime.UtcNow,
                        Data = data,
                    };
                    this.Repo.PartnerResourceProfileFields.Insert(item);
                }
                else
                {
                    if (value != item.Value || data != item.Data)
                    {
                        item.Value = value;
                        item.Data = data;
                        item.DateUpdatedUtc = DateTime.UtcNow;
                        item.UpdateCount++;
                        this.Repo.PartnerResourceProfileFields.Update(item);
                    }
                }
            }
        }

        public void FillPartnerResourceEditRequestLists(PartnerResourceEditRequest model)
        {
            // industries
            model.Industries = this.Services.ProfileFields.GetAllAvailiableValuesByType(ProfileFieldType.Industry).Select(o => new IndustryModel(o)).ToList();
            model.Industries.Insert(0, new IndustryModel(0, ""));

            // countries
            model.Countries = this.GetCountriesList(true);

            // categories
            model.Categories = this.Services.Tags.GetTagsByType(TagType.PartnerResource);
            model.Categories.Insert(0, null);
        }

        private IList<RegionInfo> GetCountriesList(bool includeEmptyCountry)
        {
            List<RegionInfo> list;
            var theBests = new System.Collections.ArrayList
            {
                "USA",
                "GBR",
                "FRA",
                "CAN",
            };
            list = SrkToolkit.Globalization.CultureInfoHelper.GetCountries()
                .OrderBy(o => o.NativeName)
                .OrderBy(o =>
                {
                    return -theBests.IndexOf(o.ThreeLetterISORegionName);
                })
                .ToList();

            if (includeEmptyCountry)
            {
                list.Insert(0, null);
            }

            return list;
        }

        public IList<CityPartnershipsModel> GetCitiesPartnerships()
        {
            var model = new List<CityPartnershipsModel>();
            var cities = this.Services.Tags.GetCityTags();
            foreach (var city in cities)
            {
                var partners = this.GetPartnersPerCity(city.Id);
                var partnership = new CityPartnershipsModel();
                partnership.City = city;
                partnership.PartnersCount = partners != null ? partners.Count(o => o.IsAvailable) : 0;
                model.Add(partnership);
            }

            return model;
        }

        public IList<PartnerResourceEditRequest> GetPartnersPerCity(int cityId)
        {
            var model = new List<PartnerResourceEditRequest>();
            var partnerTags = this.Repo.PartnerResourceTags.GetByTagId(cityId);
            var items = this.Repo.PartnerResources.GetActiveByIds(this.Services.NetworkId, partnerTags.Select(o => o.PartnerResourceId).ToArray());
            foreach (var item in items)
            {
                var partnerModel = new PartnerResourceEditRequest();
                partnerModel.UpdateFrom(item, this.Repo.PartnerResourceProfileFields.GetByPartnerResourceId(item.Id));
                partnerModel.TagModels = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, item.Tags.Select(o => o.TagId).ToArray()).Where(o => o.CategoryValue != TagType.City).Select(o => new Tag2Model(o)).ToList();
                model.Add(partnerModel);
            }

            return model;
        }

        public bool ToggleAvailable(string alias, bool enable)
        {
            var partner = this.Repo.PartnerResources.GetActiveByAlias(alias);
            if (partner != null)
            {
                partner.Available = enable;
                this.Repo.PartnerResources.Update(partner);

                return true;
            }

            return false;
        }

        public bool Delete(string alias, int userId, bool sendEmail = false)
        {
            var partner = this.Repo.PartnerResources.GetByAlias(alias);
            if (partner != null)
            {
                partner.DeletedByUserId = userId;
                partner.DateDeletedUtc = DateTime.UtcNow;
                this.Repo.PartnerResources.Update(partner);

                if (sendEmail)
                {
                    var user = this.Services.People.GetActiveById(partner.CreatedByUserId, Data.Options.PersonOptions.None);
                    if (user != null && this.Services.Subscriptions.IsUserSubscribed(user.Id))
                        this.Services.Email.SendPartnerResourceProposalRefused(new PartnerResourceModel(partner), user);
                }
                return true;
            }

            return false;
        }

        public CityPartnershipsModel GetCityPartnerShipsModel(int cityId, bool isAdmin)
        {
            var model = new CityPartnershipsModel();
            var partners = this.GetPartnersPerCity(cityId);
            if (!isAdmin)
                partners = partners.Where(o => o.IsAvailable).ToList();

            var partnerResourceTags = this.Repo.PartnerResourceTags.GetByPartnerResourceIds(partners.Select(o => o.PartnerId.Value).ToArray());
            var usedPartnerCategories = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, partnerResourceTags.Select(o => o.TagId).ToArray());
            model.Tags = usedPartnerCategories.Where(o => o.CategoryValue != TagType.City).Distinct().Select(o => new Tag2Model(o)).ToList();

            return model;
        }

        public IList<PartnerResourceModel> GetAll(ProfileFieldType[] loadFields = null)
        {
            // collect items
            var entities = this.Repo.PartnerResources.GetAllActive(this.Services.NetworkId, PartnerResourceOptions.None);
            var entitiesIds = entities.Select(r => r.Id).ToArray();

            // get all related tags by id
            var entityTagIds = this.Repo.PartnerResourceTags.GetTagIds(entitiesIds);
            var tagIds = entityTagIds.Values.SelectMany(r => r.Select(t => t)).ToArray();
            var tagEntities = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, tagIds);
            var tagDictionary = tagEntities.ToDictionary(t => t.Id, t => new Tag2Model(t));

            // load fields

            IDictionary<int, PartnerResourceProfileField[]> fields = null;
            if (loadFields != null)
            {
                fields = this.Repo.PartnerResourceProfileFields.GetByPartnerIdAndFieldType(entitiesIds, loadFields);

            }

            // create models
            var items = new List<PartnerResourceModel>(entities.Count);
            foreach (var item in entities)
            {
                var model = new PartnerResourceModel(item);
                items.Add(model);

                if (entityTagIds.ContainsKey(item.Id))
                {
                    model.Tags = new List<Tag2Model>(entityTagIds[item.Id].Length);
                    foreach (var tagId in entityTagIds[item.Id])
                    {
                        if (tagDictionary.ContainsKey(tagId))
                        {
                            model.Tags.Add(tagDictionary[tagId]);
                        }
                    }
                }

                if (fields != null && fields.ContainsKey(item.Id))
                {
                    model.Fields = new List<ProfileFieldValueModel>(fields[item.Id].Length);
                    foreach (var field in fields[item.Id])
                    {
                        model.Fields.Add(new ProfileFieldValueModel(field));
                    }
                }
            }

            return items;
        }

        public string GetProfileUrl(PartnerResource partnerResource, UriKind uriKind)
        {
            var path = "PartnerResources/Details/" + partnerResource.Alias;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetProfileUrl(string alias, UriKind uriKind)
        {
            var path = "PartnerResources/Details/" + alias;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public SetProfilePictureResult SetPartnerResourcePicture(string alias, Stream stream, string mime)
        {
            var request = new SetProfilePictureRequest
            {
                PictureStream = stream,
                PictureMime = mime,
            };

            var item = this.Repo.PartnerResources.GetByAlias(alias);
            if (item == null)
            {
                var result = new SetProfilePictureResult(request);
                result.Errors.Add(SetProfilePictureError.NoSuchPartnerResource, NetworksEnumMessages.ResourceManager);
                return result;
            }

            request.UserId = item.Id;
            return this.SetPartnerResourcePicture(request);
        }

        public SetProfilePictureResult SetPartnerResourcePicture(SetProfilePictureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (request.PictureStream == null)
                throw new ArgumentNullException("request.PictureStream");
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory))
                throw new InvalidOperationException("Configuration entry 'Storage.UserContentsDirectory' must have a valid value");

            var result = new SetProfilePictureResult(request);
            var entity = this.Repo.PartnerResources.GetById(request.UserId);
            IFilesystemProvider provider = new IOFilesystemProvider();
            PictureTransformer transformer = new PictureTransformer();

            var basePath = Path.Combine(
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                this.Services.Network.Name,
                "PartnerResources");
            var pictureId = Guid.NewGuid().ToString();

            foreach (var format in pictureFormats)
            {
                MemoryStream resized;
                if (format.Name == "Original")
                {
                    resized = new MemoryStream();
                    request.PictureStream.Seek(0, SeekOrigin.Begin);
                    request.PictureStream.CopyTo(resized);
                }
                else
                {
                    try
                    {
                        resized = transformer.FormatPicture(format, request.PictureStream);
                    }
                    catch (FormatException ex)
                    {
                        this.Services.Logger.Error("PartnerResourceServices.SetPartnerResourcePicture", ErrorLevel.Business, ex.ToString());
                        result.Errors.Add(SetProfilePictureError.FileIsNotPicture, NetworksEnumMessages.ResourceManager);
                        return result;
                    }
                }

                var path = provider.EnsureFilePath(basePath);
                var filePath = string.Join("", format.FilenameMaker(pictureId));
                path = Path.Combine(path, filePath);
                provider.WriteFile(path, resized);
            }

            entity.PictureName = pictureId.ToString();
            entity.DateLastPictureUpdateUtc = DateTime.UtcNow;
            this.Repo.PartnerResources.Update(entity);

            this.Services.Logger.Info("PartnerResourcesService.SetPartnerResourcePicture", ErrorLevel.Success, "Picture for partner resource " + entity.Alias + " has been updated");
            result.Succeed = true;
            return result;
        }

        public PartnerResource GetByAlias(string alias)
        {
            return this.Repo
                .PartnerResources
                .GetByAlias(alias);
        }

        public ProfilePictureModel GetProfilePicture(PartnerResource item, PictureAccessMode mode)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory))
                throw new InvalidOperationException("Configuration entry 'Storage.UserContentsDirectory' must have a valid value");

            var networkBasePath = Path.Combine(
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                this.Services.Network.Name,
                "PartnerResources");
            var commonBasePath = Path.Combine(
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                "Common",
                "PartnerResources");

            IFilesystemProvider provider = new IOFilesystemProvider();
            var model = new ProfilePictureModel();
            var pictures = new Dictionary<string, PictureAccess>(pictureFormats.Length);
            model.Pictures = pictures;
            model.UserId = item.Id;
            model.Username = item.Alias;

            foreach (var format in pictureFormats)
            {
                string remark = "user input";
                var filename = string.Join("", format.FilenameMaker(item.PictureName));
                var path = Path.Combine(provider.EnsureFilePath(networkBasePath), filename);
                if (!File.Exists(path))
                {
                    // default picture for network
                    remark = "network default";
                    path = Path.Combine(networkBasePath, "Default-" + format.Name + ".jpg");
                    if (!File.Exists(path))
                    {
                        // default picture
                        remark = "default";
                        path = Path.Combine(provider.EnsureFilePath(commonBasePath), "Default-" + format.Name + ".jpg");
                        if (!File.Exists(path))
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

        public ProfilePictureModel GetProfilePictureForCreate(MemoryStream stream, PictureAccessMode mode)
        {
            if (stream == null)
                return this.GetProfilePicture(new PartnerResource { Id = 0, Alias = "", PictureName = "", }, mode);

            var model = new ProfilePictureModel();
            PictureTransformer transformer = new PictureTransformer();
            var pictures = new Dictionary<string, PictureAccess>(pictureFormats.Length);
            model.Pictures = pictures;

            foreach (var format in pictureFormats)
            {
                MemoryStream resized;
                if (format.Name == "Original")
                {
                    resized = new MemoryStream();
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(resized);
                }
                else
                {
                    try
                    {
                        resized = transformer.FormatPicture(format, stream);
                    }
                    catch (FormatException ex)
                    {
                        this.Services.Logger.Error("PartnerResourceServices.GetProfilePictureForCreate", ErrorLevel.Business, ex.ToString());
                        return null;
                    }
                }

                byte[] bytes = null;
                string mime = null;
                bytes = mode.HasFlag(PictureAccessMode.Stream) ? resized.ToArray() : null;
                if (bytes != null)
                {
                    var bytes256 = bytes.Length > 600 ? bytes.Take(600).ToArray() : bytes;
                    var mimeType = MimeDetective.MimeTypes.GetFileType(bytes256);
                    if (mimeType != null)
                        mime = mimeType.Mime;
                }

                pictures.Add(format.Name, new PictureAccess
                {
                    Format = format.Clone(),
                    Bytes = mode.HasFlag(PictureAccessMode.Stream) ? bytes : null,
                    MimeType = mime,
                    Remark = "temporary",
                    DateChangedUtc = DateTime.UtcNow,
                });
            }

            return model;
        }

        public string GetPictureUrl(string alias, CompanyPictureSize size, UriKind kind)
        {
            var item = this.Repo.PartnerResources.GetByAlias(alias);
            if (item == null)
                return null;

            return this.GetPictureUrl(item, size, kind);
        }

        public string GetPictureUrl(PartnerResource item, CompanyPictureSize size, UriKind kind)
        {
            var path = string.Format(
                "Data/PartnerResourcePicture/{0}/{1}/{2}",
                item.Alias,
                size.ToString(),
                (item.DateLastPictureUpdateUtc ?? DateTime.MinValue.AsUtc()).ToString(PictureAccess.CacheDateFormat));
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return kind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public IList<PartnerResource> GetPendingRequests()
        {
            return this.Repo
                .PartnerResources
                .GetAllPending(this.Services.NetworkId);
        }

        public void ApprovePending(string alias, int userId)
        {
            var item = this.Repo
                .PartnerResources
                .GetByAlias(alias);

            item.IsApproved = true;
            item.DateApprovedUtc = DateTime.UtcNow;
            item.ApprovedByUserId = userId;
            this.Repo.PartnerResources.Update(item);
            this.Services.Wall.PublishPartnerResourceUpdate(item, item.CreatedByUserId, true);

            var user = this.Services.People.GetActiveById(item.CreatedByUserId, Data.Options.PersonOptions.None);
            if (user != null && this.Services.Subscriptions.IsUserSubscribed(user.Id))
                this.Services.Email.SendPartnerResourceProposalAccepted(new PartnerResourceModel(item), user);
        }
    }
}
