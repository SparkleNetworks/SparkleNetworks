
namespace Sparkle.Controllers
{
    using Sparkle.Common;
    using Sparkle.Data.Filters;
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Helpers;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.Models;
    using Sparkle.Resources;
    using Sparkle.Services;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.StatusApi;
    using Sparkle.UI;
    using Sparkle.WebBase;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using Twitterizer;
    using Neutral = Sparkle.Entities.Networks.Neutral;

    /// <summary>
    /// The Companies feature.
    /// </summary>
    public class CompaniesController : LocalSparkleController
    {
        private static List<Sparkle.Models.CompanyListModel> companiesCache;
        private static DateTime companiesCacheDate;
        private static TimeSpan companiesCacheDuration = TimeSpan.FromMinutes(10D);
        private static readonly object companiesCacheLock = new object();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ////this.Services.Wall.OptionsList = new List<string> { "PostedBy", "Comments", };
        }

        /// <summary>
        /// Home page.
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <returns></returns>
        public ActionResult Index(short id = 1)
        {
            var allCompanies = GetCompaniesFromCache(this.Services);

            short itemsByPage = 18;
            if (Request.Browser.IsMobileDevice)
                itemsByPage = 100;

            int skip = id * itemsByPage - itemsByPage;
            var companies = allCompanies.Skip(skip).Take(itemsByPage).ToList();

            var model = new CompaniesListModel(companies);
            model.CurrentPage = id;
            model.Pages = (short)(allCompanies.Count / itemsByPage);

            model.Skills = new List<Skill>();

            ////IList<Building> buildings = this.Services.Building.SelectAll();
            ////model.Buildings = buildings.Select(b => b.Name).ToList();

            // get skills
            var allSkills = new List<SkillModel>();
            foreach (var company in allCompanies)
            {
                foreach (var skill in company.Skills)
                {
                    allSkills.InsertOrUpdate(skill, s => s.Value == skill.Value, (l, s) => l.Add(s.Clone().SetWeight(1)), s => s.Weight += 1);
                }
            }

            model.SkillsGroup = allSkills.OrderByDescending(s => s.Weight).ToList();

            // network
            model.Network = this.Services.Networks.GetById(this.Services.NetworkId);

            // filter around a city
            model.UserLocation = this.Services.Places.GetLocationStringFromIp(this.Request.UserHostAddress);
            model.SuggestedPlaces = this.Services.Places.GetPlacesUsedByCompanies().Select(o => o.Key).ToArray();

            model.UsedCompanyTags = new AjaxTagPickerModel(this.Services.Tags.GetUsedEntityTags("Company"), false, true);

            return this.View(model);
        }

        /// <summary>
        /// Page for new arrivals companies.
        /// </summary>
        /// <returns></returns>
        public ActionResult Arrivals()
        {
            var allCompanies = GetCompaniesFromCache(this.Services);
            var companies = allCompanies
                .Where(c => c.ApprovedDateUtc > DateTime.UtcNow.AddDays(-35D))
                .OrderByDescending(c => c.ApprovedDateUtc)
                .ThenBy(c => c.Name)
                .ToList();

            var model = new CompaniesListModel(companies);

            return this.View(model);
        }

        public ActionResult ById(int id)
        {
            var company = GetCompaniesFromCache(this.Services).SingleOrDefault(c => c.Id == id);
            if (company != null)
                return this.RedirectToAction("Company", new { id = company.Alias, });
            return this.ResultService.NotFound();
        }

        /// <summary>
        /// Company profile specified by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Company(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                this.TempData.AddError("Cette entreprise n'existe pas ou a changé de nom");
                return this.RedirectToAction("Index", "Companies");
            }

            var company = this.Services.Company.GetByAlias(id, true);
            if (company == null)
            {
                this.TempData.AddError("Cette entreprise n'existe pas ou a changé de nom");
                return this.RedirectToAction("Index", "Companies");
            }

            if (!company.IsEnabled)
            {
                this.TempData.AddError("Cette entreprise n'est plus active.");
            }

            var companyProfileFields = this.Services.ProfileFields.GetCompanyProfileFieldsByCompanyId(company.ID);

            CompanyProfileModel model = new CompanyProfileModel(company, companyProfileFields);
            //model.CountPublications = this.Services.Company.CountPublications(company.ID);
            model.PictureUrl = this.Services.Company.GetProfilePictureUrl(company, CompanyPictureSize.Large, UriKind.Relative);
            model.Category = this.Services.Company.GetCategoryById(company.CategoryId);

            if (!this.Services.Tags.DisableV1)
            {
                IList<CompanySkill> skills = this.Services.CompanySkills.SelectByCompanyId(company.ID, CompanyTagOptions.Tag);
                model.Skills = skills.Select(o => new SkillModel(this.Services, o)).ToList(); 
            }

            model.Achievements = this.Services.Achievements.SelectAllByCompanyId(company.ID);


            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                model.ExternalTimeline = new Sparkle.Models.TimelineModel()
                {
                    TimelineName = "PublicCompany",
                    TimelineMode = TimelineDisplayContext.ExternalCompany,
                    TimelineId = company.ID,
                    CanPost = false,
                };

                return View("Company_Anonymous", model);
            }

            // *************
            // AUTHENTICATED
            // *************

            #region Companies Visits

            if (this.SessionService.User.Login != "kevin.alexandre")
            {
                var companyVisit = this.Services.CompaniesVisits.GetByCompanyIdAndUserIdAndDay(company.ID, this.UserId.Value, DateTime.Now.Date);
                if (companyVisit != null)
                {
                    companyVisit.ViewCount++;
                    this.Services.CompaniesVisits.Update(companyVisit);
                }
                else
                {
                    companyVisit = new CompaniesVisit
                    {
                        Date = DateTime.Now,
                        CompanyId = company.ID,
                        UserId = this.UserId.Value,
                        ViewCount = 1,
                    };
                    this.Services.CompaniesVisits.Insert(companyVisit);
                }
            }

            #endregion

            if (company.NetworkId != this.Services.NetworkId)
            {
                model.AltNetwork = this.Services.Networks.GetById(company.NetworkId);
            }

            if (company.NetworkId == this.Services.NetworkId && this.Services.AppConfiguration.Tree.Features.EnableCompanies)
            {
                this.Services.People.OptionsList.Add("Job");
                this.Services.People.OptionsList.Add("Company");
                model.People = this.Services.People.GetUsersViewFromCompany(company.ID).Select(o => new PeopleModel(o)).OrderByDescending(p => p.HasPicture).ThenBy(p => p.FirstName).ToList();
                // TODO: avoid using magic numbers
                model.Contacts = model.People.Where(o => o.AccountRight > 1 || o.CompanyAccessLevel > 1).OrderByDescending(o => o.CompanyAccessLevel).ToList();
                model.Visits = this.Services.CompaniesVisits.CountByCompany(company.ID);
            }

            var me = this.SessionService.User;
            model.IsMyCompany = me.CompanyID == company.ID;
            model.IsAdmin = (me.CompanyAccessLevel == (int)CompanyAccessLevel.Administrator && model.IsMyCompany);
            model.IsNetworkAdmin = me.NetworkAccess.HasFlag(NetworkAccessLevel.NetworkAdmin) || me.NetworkAccess.HasFlag(NetworkAccessLevel.SparkleStaff);
            //model.CanPost = model.IsMyCompany && this.SessionService.User.CompanyAccessLevel > (int)CompanyAccessLevel.User;
            model.CanAddPlace = model.IsMyCompany || model.IsNetworkAdmin || me.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.ManageCompany);

            #region Timeline publique de l'entreprise

            model.ExternalTimeline = new Sparkle.Models.TimelineModel()
            {
                TimelineName = "Company",
                TimelineMode = TimelineDisplayContext.Company,
                PolicyMessage = "Vous publiez ici en tant que " + company.Name + ".",
                TimelineId = company.ID,
                CanPost = false,
                CanPostAsCompany = model.IsMyCompany && me.CompanyAccessLevel > (int)CompanyAccessLevel.User,
                CompanyName = company.Name,
                ////CompanyPictureUrl = this.GetSimpleUrl("Data", "CompanyPicture", "DataPicture", new { id = company.Alias, size = "Small", }),
                CompanyPictureUrl = this.Services.Company.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Small, UriKind.Relative),
            };

            #endregion

            if (model.IsMyCompany && this.Services.AppConfiguration.Tree.Features.EnableCompanies)
            {
                model.InternalTimeline = new Sparkle.Models.TimelineModel()
                {
                    TimelineName = "CompanyNetwork",
                    TimelineMode = TimelineDisplayContext.CompanyNetwork,
                    PolicyMessage = "Publication visible pour les personnes de " + me.Company.Name + " uniquement.",
                    TimelineId = me.CompanyID,
                    CanPost = true,
                    ////PersonPictureUrl = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = me.Login, size = "Small", })
                    PersonPictureUrl = this.Services.People.GetProfilePictureUrl(me, UserProfilePictureSize.Small, UriKind.Relative),
                };
            }

            model.CompanyRelationships = this.Services.CompanyRelationships.GetByCompanyIdSortedByType(company.ID);
            model.CompanyPlaces = this.Services.Company.GetPlacesFromCompanyId(company.ID);

            model.Tags = this.Services.Company.GetAjaxTagPickerModel(company.ID, me.Id);

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.ResultService.NotFound();
            }

            bool isAdmin = this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);

            var company = this.Services.Company.GetByAlias(id, isAdmin);
            if (company == null)
            {
                return this.ResultService.NotFound();
            }

            var companyProfileFields = this.Services.ProfileFields.GetCompanyProfileFieldsByCompanyId(company.ID);

            if (!isAdmin)
            {
                isAdmin = this.SessionService.User.CompanyAccessLevel > (int)(CompanyAccessLevel.User) && this.SessionService.User.CompanyID == company.ID;
            }

            if (!isAdmin)
            {
                return this.ResultService.Forbidden();
            }

            var categories = this.Services.Company.GetAllCategories();
            foreach (var item in categories)
            {
                item.Name = this.Services.Lang.T(item.Name);
            }

            CompanyEditModel model = new CompanyEditModel()
            {
                CompanyId = company.ID,
                Name = company.Name,
                Alias = company.Alias,
                Baseline = company.Baseline,
                ////PictureUrl = Urls.GetCompanyPictureUrl(company.Alias, CompanyPictureSize.Medium),
                PictureUrl = this.Services.Company.GetProfilePictureUrl(company, CompanyPictureSize.Medium, UriKind.Relative),
                EmailDomain = company.EmailDomain,
                CategoryId = company.CategoryId,
                Buildings = this.Services.Building.SelectAll().ToList(),
                Categories = categories,
            };
            foreach (var field in companyProfileFields)
            {
                switch (field.Type)
                {
                    case ProfileFieldType.Site:
                        model.Site = field.Value;
                        break;
                    case ProfileFieldType.Phone:
                        model.Phone = field.Value;
                        break;
                    case ProfileFieldType.About:
                        model.About = field.Value;
                        break;
                    case ProfileFieldType.Twitter:
                        model.Twitter = field.Value;
                        break;
                    case ProfileFieldType.Email:
                        model.Email = field.Value;
                        break;
                    case ProfileFieldType.Facebook:
                        model.Facebook = field.Value;
                        break;
                    case ProfileFieldType.AngelList:
                        model.AngelList = field.Value;
                        break;
                    default:
                        break;
                }
            }

            if (!this.Services.Tags.DisableV1)
            {
                var skills = this.Services.CompanySkills.SelectByCompanyId(company.ID, CompanyTagOptions.Tag).Select(o => o.Skill).ToList();
                model.Skills = new Services.Networks.Models.Tags.TagsListEditable();
                model.Skills.CompanyId = company.ID;
                model.Skills.UserId = -1;
                model.Skills.GroupId = -1;
                model.Skills.Items = skills.Select(o => new Sparkle.Services.Networks.Models.Tags.TagModel(o)).ToList();
            }

            return this.View(model);
        }

        /// <summary>
        /// Edits the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [AuthorizeUser]
        [HttpPost]
        public ActionResult Edit(CompanyEditModel model, string returnUrl)
        {
            bool isAdmin = this.SessionService.User.NetworkAccess.HasAnyFlag(
                NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);

            var company = this.Services.Company.GetByAlias(model.Alias, isAdmin);
            if (company == null)
            {
                return this.ResultService.NotFound();
            }

            if (!isAdmin)
            {
                isAdmin = this.SessionService.User.CompanyAccessLevel > (int)(CompanyAccessLevel.User) && this.SessionService.User.CompanyID == company.ID;
            }

            if (!isAdmin)
            {
                return this.ResultService.Forbidden();
            }

            CompanyCategoryModel category = null;
            if (model.CategoryId.HasValue && model.CategoryId.Value > 0)
            {
                if ((category = this.Services.Company.GetCategoryById(model.CategoryId.Value)) == null)
                {
                    this.ModelState.AddModelError("CategoryId", Alerts.NoSuchCompanyCategory);
                }
            }

            if (this.ModelState.IsValid)
            {
                company.Name = model.Name;
                company.Baseline = model.Baseline;

                company.EmailDomain = model.EmailDomain;

                if (category != null)
                    company.CategoryId = category.Id;

                // TODO: there should be a domain method to do this work

                this.Services.Company.Update(company, isAdmin);

                // this is right
                // we allow only one About field value
                var source = ProfileFieldSource.UserInput;
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.About, model.About, source);

                // this is very wrong
                // we might allow multiple values for those field types
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Site, model.Site, source);
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Phone, model.Phone, source);
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Twitter, model.Twitter, source);
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Email, model.Email, source);
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Facebook, model.Facebook, source);
                this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.AngelList, model.AngelList, source);

                #region CompanySkills

                // this is mediocre
                // write a proper domain method

                if (!this.Services.Tags.DisableV1)
                {
                    // Suppression de compétences
                    string RemoveSkillsString = model.RemoveSkillsString;
                    if (!string.IsNullOrEmpty(RemoveSkillsString))
                    {
                        string[] items = RemoveSkillsString.Split(',');
                        foreach (string item in items)
                        {
                            int itemId;
                            if (int.TryParse(item, out itemId))
                            {
                                CompanySkill removeSkill = this.Services.CompanySkills.GetBySkillIdAndCompanyId(itemId, company.ID);
                                if (removeSkill != null)
                                    if (removeSkill.CompanyId == company.ID)
                                        this.Services.CompanySkills.Delete(removeSkill);
                            }
                        }
                    }

                    // Save new skills
                    IList<CompanySkill> currentSkills = this.Services.CompanySkills.SelectByCompanyId(company.ID);

                    if (model.NewsSkills != null)
                    {
                        string[] ns = model.NewsSkills.Split(';');

                        if (ns.Count() > 0)
                        {
                            foreach (string s in ns)
                            {
                                string currentTag = s;

                                int skillId;
                                if (!int.TryParse(currentTag, out skillId)) // Ajout d'un tag
                                {
                                    // currentTag est une chaine de caractere

                                    if (!string.IsNullOrEmpty(currentTag) && !string.IsNullOrWhiteSpace(currentTag))
                                    {
                                        Skill skill = this.Services.Skills.GetByName(currentTag);
                                        if (skill == null)
                                        {
                                            var newSkill = new Skill
                                            {
                                                TagName = currentTag,
                                                ParentId = 1,
                                                CreatedByUserId = this.UserId.Value,
                                                Date = DateTime.Now,
                                            };
                                            currentTag = this.Services.Skills.Insert(newSkill).Id.ToString();
                                        }
                                        else
                                        {
                                            currentTag = skill.Id.ToString();
                                        }
                                    }
                                }

                                if (int.TryParse(currentTag, out skillId))
                                {
                                    bool add = true;
                                    foreach (CompanySkill item in currentSkills.Where(item => item.Skill.Id == skillId))
                                        add = false;
                                    if (add)
                                    {
                                        CompanySkill companySkill = new CompanySkill
                                        {
                                            SkillId = skillId,
                                            CompanyId = company.ID,
                                            Date = DateTime.UtcNow
                                        };
                                        this.Services.CompanySkills.Insert(companySkill);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                // Publication de la mise à jour du profil
                if (this.SessionService.User.CompanyID == company.ID)
                {
                    this.Services.Wall.PublishCompanyProfileUpdate(company, this.UserId.Value, true);
                }

                CompaniesController.ClearCompaniesCache();

                return this.RedirectToAction("Company", "Companies", new { id = company.Alias, });
            }

            model.Categories = this.Services.Company.GetAllCategories();
            foreach (var item in model.Categories)
            {
                item.Name = this.Services.Lang.T(item.Name);
            }

            if (!this.Services.Tags.DisableV1)
            {
                var skills = this.Services.CompanySkills.SelectByCompanyId(this.SessionService.User.CompanyID).Select(o => o.Skill).ToList();
                model.Skills = new Sparkle.Services.Networks.Models.Tags.TagsListEditable();
                model.Skills.CompanyId = company.ID;
                model.Skills.UserId = -1;
                model.Skills.GroupId = -1;
                model.Skills.Items = skills.Select(o => new Sparkle.Services.Networks.Models.Tags.TagModel(o)).ToList();
            }

            model.Buildings = this.Services.Building.SelectAll().ToList();
            {
                model.Name = company.Name;
            }

            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult ChangeName(string id)
        {
            // In the future, this page will allow users to change a company name.
            return this.View();
        }

        [AuthorizeUser]
        public ActionResult Edit2(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.ResultService.NotFound();
            }

            var actingUser = this.SessionService.User;
            bool isNetworkAdmin = actingUser.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff);
            bool canEditProfile = isNetworkAdmin;

            var companyEntity = this.Services.Company.GetByAlias(id, isNetworkAdmin);
            if (companyEntity == null)
            {
                return this.ResultService.NotFound();
            }

            var company = new Sparkle.Services.Networks.Models.CompanyModel(companyEntity);
            this.ViewBag.Company = company;

            bool isCompanyAdmin = actingUser.CompanyID == company.Id && actingUser.CompanyAccess >= CompanyAccessLevel.CommunityManager;
            canEditProfile |= isCompanyAdmin;

            var companyProfileFields = this.Services.ProfileFields.GetCompanyProfileFieldsByCompanyId(company.Id);

            if (!canEditProfile)
            {
                return this.ResultService.Forbidden();
            }

            var categories = this.Services.Company.GetAllCategories();
            foreach (var item in categories)
            {
                item.Name = this.Services.Lang.T(item.Name);
            }

            var request = this.Services.Company.GetEditCompanyFieldsRequest(company.Id, null);
            if (this.Request.IsHttpPostRequest())
            {
                for (int i = 0; ; i++)
                {
                    var fieldIdString = this.Request.Form["Item[" + i + "].FieldId"];
                    var valueIdString = this.Request.Form["Item[" + i + "].ValueId"];
                    var valueValue = this.Request.Form["Item[" + i + "].Value"];
                    var valueDelete = this.Request.Form["Item[" + i + "].Delete"];

                    int fieldId = 0, valueId = 0;
                    if (!int.TryParse(fieldIdString, out fieldId) || !string.IsNullOrEmpty(valueIdString) && !int.TryParse(valueIdString, out valueId))
                        continue;

                    if (fieldId == null)
                        break;

                    ProfileFieldValueModel value = null;
                    ProfileFieldModel field = null;
                    if (valueId > 0)
                    {
                        value = request.Values.Single(x => x.ProfileFieldValueId.Equals(valueId));
                        field = request.Fields.SingleOrDefault(x => x.Id.Equals(value.ProfileFieldId) && x.Id.Equals(fieldId));
                        value.Value = valueValue;
                    }
                    else
                    {
                        field = request.Fields.SingleOrDefault(x => x.Id.Equals(fieldId));
                        value = new ProfileFieldValueModel(field, valueValue);
                        request.Values.Add(value);
                    }

                    if (!string.IsNullOrEmpty(valueDelete))
                    {
                        value.Action = ProfileFieldValueAction.Delete;
                    }
                }

                request.ActingUserId = actingUser.Id;
                request.Source = ProfileFieldSource.UserInput;
                var result = this.Services.Company.EditCompanyFields(request);
                if (this.ValidateResult(result, MessageDisplayMode.TempData))
                {
                    return this.RedirectToAction("Company", new { id = company.Alias, });
                }
            }

            return this.View(request);
        }

        [AuthorizeUser]
        public ActionResult SocialNetworks(string id)
        {
            if (!string.IsNullOrEmpty(id) && this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.ManageCompany)
                || string.IsNullOrEmpty(id) && this.SessionService.User.CompanyAccess.HasAnyFlag(CompanyAccessLevel.CommunityManager | CompanyAccessLevel.Administrator))
            {
                SocialNetworksModel model = new SocialNetworksModel();

                Company company = null;
                if (!string.IsNullOrEmpty(id) && (company = this.Services.Company.GetByAlias(id)) == null)
                    return this.ResultService.NotFound();

                IList<SocialNetworkCompanySubscription> subscriptions = this.Services.SocialNetworkCompanySubscriptions.GetByCompanyId(company != null ? company.ID : this.SessionService.User.CompanyID);

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SocialNetworkConnection.Type)
                    {
                        case (byte)SocialNetworkConnectionType.Twitter:

                            model.Twitter.IsConnected = true;
                            model.Twitter.Username = subscription.SocialNetworkConnection.Username;
                            model.Twitter.Hashtag = subscription.ContentContainsFilter;

                            break;
                        case (byte)SocialNetworkConnectionType.Facebook:

                            model.Facebook.IsConnected = true;

                            break;
                        default:
                            break;
                    }
                }

                model.CompanyAlias = id;
                return View(model);
            }

            return this.ResultService.Forbidden();
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult SocialNetworks(SocialNetworksModel model)
        {
            if (!string.IsNullOrEmpty(model.CompanyAlias) && this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.ManageCompany)
                || string.IsNullOrEmpty(model.CompanyAlias) && this.SessionService.User.CompanyAccess.HasAnyFlag(CompanyAccessLevel.CommunityManager | CompanyAccessLevel.Administrator))
            {
                Company company = null;
                if (!string.IsNullOrEmpty(model.CompanyAlias) && (company = this.Services.Company.GetByAlias(model.CompanyAlias)) == null)
                    return this.ResultService.NotFound();

                if (!string.IsNullOrEmpty(model.Twitter.AdminInput))
                    this.AddCompanyTwitterLink(company.ID, model.Twitter.AdminInput);

                IList<SocialNetworkCompanySubscription> subscriptions = this.Services.SocialNetworkCompanySubscriptions.GetByCompanyId(company != null ? company.ID : this.SessionService.User.CompanyID);

                foreach (var subscription in subscriptions)
                {
                    switch (subscription.SocialNetworkConnection.Type)
                    {
                        case (byte)SocialNetworkConnectionType.Twitter:

                            if (model.Twitter.ToDelete)
                            {
                                this.DeleteCompanyTwitterLink(subscription);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(model.Twitter.Hashtag) && !string.IsNullOrEmpty(subscription.ContentContainsFilter))
                                {
                                    // Remove hashtag
                                    subscription.ContentContainsFilter = null;
                                    this.Services.SocialNetworkCompanySubscriptions.Update(subscription);
                                }
                                else if (!string.IsNullOrEmpty(model.Twitter.Hashtag) && string.IsNullOrEmpty(subscription.ContentContainsFilter))
                                {
                                    // Add hashtag
                                    subscription.ContentContainsFilter = model.Twitter.Hashtag.Replace("#", "");
                                    this.Services.SocialNetworkCompanySubscriptions.Update(subscription);
                                }

                                model.Twitter.Username = subscription.SocialNetworkConnection.Username;
                                model.Twitter.IsConnected = true;
                            }

                            break;
                        case (byte)SocialNetworkConnectionType.Facebook:

                            model.Facebook.IsConnected = true;

                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(model.CompanyAlias))
                    return this.RedirectToAction("SocialNetworks", new { id = model.CompanyAlias, });
                return this.RedirectToAction("SocialNetworks");
            }

            return this.ResultService.Forbidden();
        }

        private void AddCompanyTwitterLink(int companyId, string username)
        {
            // verify configuration
            var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.Twitter);
            if (socialState.Entity == null || !socialState.Entity.IsConfigured)
                return;

            var twitterCredentials = new LinqToTwitter.InMemoryCredentials
            {
                ConsumerKey = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                ConsumerSecret = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                OAuthToken = socialState.Entity.OAuthAccessToken,
                AccessToken = socialState.Entity.OAuthAccessSecret,
            };
            var authorizer = new LinqToTwitter.SingleUserAuthorizer
            {
                Credentials = twitterCredentials,
            };
            var twitterContext = new LinqToTwitter.TwitterContext(authorizer);

            try
            {
                // find list
                var listName = this.Services.SocialNetworkStates.GetTwitterFollowListName(this.Services.Network);
                var lists = twitterContext.List
                    .Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == socialState.Entity.Username)
                    .ToList();
                var list = lists.SingleOrDefault(l => l.Name == listName);

                // create list if it does not exist
                if (list == null)
                {
                    list = LinqToTwitter.ListExtensions.CreateList(twitterContext, listName, "private", "Network:" + this.Services.NetworkId);
                }

                // add user to list
                var result = LinqToTwitter.ListExtensions.AddMemberToList(twitterContext, null, username, null, listName, null, socialState.Entity.Username);
                this.TempData.AddInfo(Lang.T("Le compte Twitter de votre entreprise est maintenant configuré."));
            }
            catch (Exception ex)
            {
                if (ex is LinqToTwitter.TwitterQueryException)
                {
                    var twitterEx = (LinqToTwitter.TwitterQueryException)ex;

                    // can't find user
                    if (twitterEx.ErrorCode == 108)
                        this.TempData.AddError(Lang.T("Le compte twitter que vous avez renseigné est introuvable, veuillez vérifier qu'il s'agit d'un compte valide et public et réessayez."));
                    return;
                }
                this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                this.ReportError(ex, "CompaniesController/SocialNetworks/add-to-twitter-network-list error");
                return;
            }

            this.Services.SocialNetworkCompanySubscriptions.CreateConnection(this.SessionService.User.Id, companyId, SocialNetworkConnectionType.Twitter, username);
        }

        private void DeleteCompanyTwitterLink(SocialNetworkCompanySubscription subscription)
        {
            var stillInUse = this.Services.SocialNetworkConnections.CountByUsernameAndType(subscription.SocialNetworkConnection.Username, SocialNetworkConnectionType.Twitter) > 1;

            // delete from twitter list
            if (!stillInUse)
            {
                // verify configuration
                var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.Twitter);
                if (socialState.Entity == null || !socialState.Entity.IsConfigured)
                    return;

                var twitterCredentials = new LinqToTwitter.InMemoryCredentials
                {
                    ConsumerKey = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                    ConsumerSecret = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                    OAuthToken = socialState.Entity.OAuthAccessToken,
                    AccessToken = socialState.Entity.OAuthAccessSecret,
                };
                var authorizer = new LinqToTwitter.SingleUserAuthorizer
                {
                    Credentials = twitterCredentials,
                };
                var twitterContext = new LinqToTwitter.TwitterContext(authorizer);

                try
                {
                    // find list
                    var listName = this.Services.SocialNetworkStates.GetTwitterFollowListName(this.Services.Network);
                    var lists = twitterContext.List
                        .Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == socialState.Entity.Username)
                        .ToList();
                    var list = lists.SingleOrDefault(l => l.Name == listName);

                    var membersList = twitterContext.List
                        .Where(l => l.Type == LinqToTwitter.ListType.Members && l.Slug == listName && l.OwnerScreenName == socialState.Entity.Username)
                        .ToList();
                    var members = membersList.SingleOrDefault(l => l.Slug == listName);

                    // if list does not exist, abort
                    if (list == null)
                        return;

                    if (members.Users.Any(o => o.Identifier.ScreenName == subscription.SocialNetworkConnection.Username))
                    {
                        // delete user from list
                        var result = LinqToTwitter.ListExtensions.DeleteMemberFromList(twitterContext, null, subscription.SocialNetworkConnection.Username, null, listName, null, socialState.Entity.Username);
                    }
                }
                catch (Exception ex)
                {
                    this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                    this.ReportError(ex, "CompaniesController/SocialNetworks/remove-from-twitter-network-list error");
                    return;
                }
            }

            // delete entities
            var connection = subscription.SocialNetworkConnection;
            this.Services.Logger.Info("CompaniesController.SocialNetworks[POST]", ErrorLevel.Success, "Deleting of SocialNetworkCompanySubscription #" + subscription.Id + " and SocialNetworkConnection #" + connection.Id + " from company #" + subscription.CompanyId);
            this.Services.SocialNetworkCompanySubscriptions.Delete(subscription);
            this.Services.SocialNetworkConnections.Delete(connection);
        }

        [AuthorizeUser]
        public ActionResult ConfigureTwitter(string oauth_token, string oauth_verifier, string denied, string ReturnUrl)
        {
            if (!this.Services.AppConfiguration.Tree.Features.Companies.SocialPull.IsEnabled)
                return this.ResultService.NotFound();

            // verify configuration
            var socialState = this.Services.SocialNetworkStates.GetState(SocialNetworkConnectionType.Twitter);
            if (socialState.Entity == null || !socialState.Entity.IsConfigured)
            {
                // twitter is not configured!
            }

            if (string.IsNullOrEmpty(oauth_token) || string.IsNullOrEmpty(oauth_verifier))
            {
                UriBuilder builder = new UriBuilder(this.Request.Url);
                builder.Query = string.Concat(
                    builder.Query,
                    string.IsNullOrEmpty(builder.Query) ? string.Empty : "&",
                    "ReturnUrl=",
                    ReturnUrl);

                try
                {
                    string token = OAuthUtility.GetRequestToken(
                                this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                                this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                                builder.ToString()).Token;

                    return this.Redirect(OAuthUtility.BuildAuthorizationUri(token, true).ToString());
                }
                catch (Exception ex)
                {
                    this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                    this.ReportError(ex, "AccountController/ConfigureTwitter/no-tokens error");
                    return this.RedirectToAction("SocialNetworks");
                }
            }

            OAuthTokenResponse tokens;
            try
            {
                tokens = OAuthUtility.GetAccessToken(
                    this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                    this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                    oauth_token,
                    oauth_verifier);
            }
            catch (Exception ex)
            {
                this.TempData.AddError("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard. ");
                this.ReportError(ex, "AccountController/ConfigureTwitter/tokens-received error");
                return this.RedirectToAction("SocialNetworks");
            }

            if (tokens != null)
            {
                // Save Social Network Connection
                SocialNetworkConnection connection = new SocialNetworkConnection();
                connection.CreatedByUserId = this.SessionService.User.Id;
                connection.Type = (byte)SocialNetworkConnectionType.Twitter;
                connection.Username = tokens.ScreenName;
                connection.OAuthToken = tokens.Token;
                connection.OAuthVerifier = tokens.TokenSecret;
                connection.IsActive = true;

                int connectionId = this.Services.SocialNetworkConnections.Insert(connection);

                // Save social network subscription
                SocialNetworkCompanySubscription subscription = new SocialNetworkCompanySubscription();
                subscription.CompanyId = this.SessionService.User.CompanyID;
                subscription.AutoPublish = true;
                subscription.SocialNetworkConnectionsId = connectionId;

                this.Services.SocialNetworkCompanySubscriptions.Insert(subscription);

                // add user to network's twitter list
                if (socialState.Entity != null && socialState.Entity.IsConfigured)
                {
                    var twitterCredentials = new LinqToTwitter.InMemoryCredentials
                    {
                        ConsumerKey = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerKey,
                        ConsumerSecret = this.Services.AppConfiguration.Tree.Externals.Twitter.TwitterConsumerSecret,
                        OAuthToken = socialState.Entity.OAuthAccessToken,
                        AccessToken = socialState.Entity.OAuthAccessSecret,
                    };
                    var authorizer = new LinqToTwitter.SingleUserAuthorizer
                    {
                        Credentials = twitterCredentials,
                    };
                    var twitterContext = new LinqToTwitter.TwitterContext(authorizer);
                    try
                    {
                        // find list
                        var listName = this.Services.SocialNetworkStates.GetTwitterFollowListName(this.Services.Network);
                        var lists = twitterContext.List
                            .Where(l => l.Type == LinqToTwitter.ListType.Lists && l.ScreenName == socialState.Entity.Username)
                            .ToList();
                        var list = lists.SingleOrDefault(l => l.Name == listName);

                        // create list if it does not exist
                        if (list == null)
                        {
                            list = LinqToTwitter.ListExtensions.CreateList(twitterContext, listName, "private", "Network:" + this.Services.NetworkId);
                        }

                        // add user to list
                        var result = LinqToTwitter.ListExtensions.AddMemberToList(twitterContext, tokens.UserId.ToString(""), null, null, listName, null, socialState.Entity.Username);
                        this.TempData.AddInfo(Lang.T("Le compte Twitter de votre entreprise est maintenant configuré."));
                    }
                    catch (Exception ex)
                    {
                        this.TempData.AddError(Lang.T("Une erreur de communication avec le service Twitter est survenue. Ce problème est peut-être temporaire, réessayez-donc plus tard."));
                        this.ReportError(ex, "CompaniesController/ConfigureTwitter/add-to-twitter-network-list error");
                        return this.RedirectToAction("SocialNetworks");
                    }
                }
            }

            if (!string.IsNullOrEmpty(denied))
            {
                this.TempData.AddWarning("Vous avez refusé l'autorisation.");
            }

            return RedirectToAction("SocialNetworks");
        }

        public ActionResult Registration(Guid? id, string ReturnUrl)
        {
            var user = this.SessionService.User;
            if (user == null)
            {
                return this.RedirectToAction("Index", "Apply");
            }
            else if (!user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.AddCompany))
            {
                return this.ResultService.Forbidden();
            }

            var request = this.Services.Company.GetCreateRequest(null, this.UserId);

            if (id != null && id != Guid.Empty)
            {
                var createRequest = this.Services.Company.GetRequest(id.Value);
                if (createRequest == null)
                    return this.ResultService.NotFound();
                request.UpdateFrom(createRequest);
            }

            request.ReturnUrl = ReturnUrl;

            var challenge = MultiHash.OomanChallenge();
            this.ViewBag.OomanChallenge = challenge;
            this.ViewBag.OomanClue = MultiHash.OomanValue(challenge);

            return View(request);
        }

        [HttpPost]
        public ActionResult Registration(
            CreateCompanyRequest request,
            string OomanChallenge,
            [Required, Display(Name = "OomanCheck"), OomanCheckAttribute] string OomanCheck)
        {
            var user = this.SessionService.User;
            if (user == null)
            {
                return this.RedirectToAction("Index", "Apply");
            }
            else if (!user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.AddCompany))
            {
                return this.ResultService.Forbidden();
            }

            var challenge = OomanChallenge ?? MultiHash.OomanChallenge();
            this.ViewBag.OomanChallenge = challenge;
            this.ViewBag.OomanClue = MultiHash.OomanValue(challenge);

            request = this.Services.Company.GetCreateRequest(request, this.UserId);
            bool isEditing = false;

            if (this.ModelState.IsValid)
            {
                CreateCompanyResult result = null;
                if (request.CanApprove && request.IsApproved) // create without approbation (staff only)
                {
                    // Enregistrement de l'entreprise
                    result = this.Services.Company.ApplyCreateRequest(request, true);

                    if (!result.Succeed)
                    {
                        this.TempData.AddConfirmation(Alerts.CompanyRegistration_CompanyCreated);
                        return this.RedirectToAction("Company", "Companies", new { id = result.Item.Alias, });
                    }
                }
                else // create approbation request
                {
                    result = this.Services.Company.ApplyCreateRequest(request, false);

                    if (!result.Succeed)
                    {
                        if (request.ReturnUrl.NullIfEmptyOrWhitespace() != null)
                        {
                            this.TempData.AddConfirmation("La demande d'inscription a été modifiée.");
                            return this.Redirect(request.ReturnUrl);
                        }
                        else
                        {
                            return this.RedirectToAction("RegistrationRequest", new { id = request.CreateRequestUniqueId, });
                        }
                    }
                }

                if (result != null && !result.Succeed)
                {
                    var error = result.Errors.First();
                    if (error != null)
                    {
                        this.TempData.AddError(error.DisplayMessage);
                        this.Services.Logger.Error(
                            "CompaniesController.Registration",
                            ErrorLevel.ThirdParty,
                            "CompanyService.ApplyCreateRequest encountered an error ({0}: {1})",
                            error.Code.ToString(),
                            error.DisplayMessage);
                        return View(request);
                    }
                }
            }

            return View(request);
        }

        public ActionResult RegistrationRequest(Guid? id)
        {
            if (id == null)
                return this.ResultService.NotFound();

            var model = new CompanyRequestModel
            {
                RequestEntity = this.Services.Company.GetRequest(id.Value),
            };

            if (model.RequestEntity == null)
                return this.ResultService.NotFound();

            if (model.RequestEntity.CompanyId != null)
            {
                int cid = model.RequestEntity.CompanyId.Value;
                model.CompanyEntity = this.Services.Company.GetById(cid);
                model.PendingMembers = this.Services.Company.CountPendingMembers(cid);
                model.RegisteredMembers = this.Services.Company.CountRegisteredMembers(cid);
            }

            if (model.RequestEntity.Approved == true)
            {
                this.TempData.AddInfo("Votre demande a été acceptée.");
                if (model.CompanyEntity == null)
                {
                    this.TempData.AddError("Erreur : votre demande a été acceptée mais votre entreprise n'est pas inscrite pour autant.");
                }
            }
            else if (model.RequestEntity.Approved == false)
            {
                this.TempData.AddWarning("Votre demande a été refusée.");
            }
            else
            {
                this.TempData.AddWarning("Votre demande est en attente de validation.");
            }

            return this.View(model);
        }

        public ActionResult Registred()
        {
            return View();
        }

        public ActionResult Contact(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.ResultService.NotFound();
            }

            Company company = this.Services.Company.GetByAlias(id, true);
            if (company == null)
            {
                return this.ResultService.NotFound();
            }

            CompanyContactModel model = new CompanyContactModel();
            // Default data
            model.Company = company;
            model.CompanyId = company.ID;
            model.IsAuthenticated = User.Identity.IsAuthenticated;

            if (model.IsAuthenticated)
            {
                model.FirstName = this.SessionService.User.FirstName;
                model.LastName = this.SessionService.User.LastName;
                model.Email = this.SessionService.User.Email;

                // messages history
                model.Messages = this.Services.CompanyContacts.SelectByToCompanyIdAndFromCompanyId(company.ID, this.SessionService.User.CompanyID);
            }

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Contact(CompanyContactModel model)
        {
            int companyId = model.CompanyId;
            var company = this.Services.Company.GetById(companyId);

            if (company == null)
            {
                return this.ResultService.NotFound();
            }

            model.Company = company;

            if (this.ModelState.IsValid)
            {
                CompanyContact contact = new CompanyContact();
                contact.ToCompanyId = companyId;

                if (User.Identity.IsAuthenticated)
                {
                    contact.FromUserId = this.SessionService.User.Id;
                    contact.FromCompanyId = this.SessionService.User.CompanyID;
                }

                contact.FromUserName = model.FirstName + " " + model.LastName;
                contact.FromUserEmail = model.Email;
                contact.FromCompanyName = model.CompanyName;

                contact.Message = model.Message;

                contact.Type = 0;
                contact.Date = DateTime.Now;

                contact = this.Services.CompanyContacts.Insert(contact);
                contact = this.Services.CompanyContacts.GetById(contact.Id);

                this.Services.Email.SendCompanyContact(contact);

                this.TempData.AddInfo("Parfait ! Votre message a bien été envoyé à " + company.Name + ".");
                return RedirectToAction("Company", "Companies", new { id = company.Alias });
            }


            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult CompanyMenu()
        {
            CompanyMenu model = new CompanyMenu();
            model.ManagementCount = this.Services.People.CountMustBeValidateUsersByCompanyId(this.SessionService.User.CompanyID)
                                  + this.Services.RegisterRequests.CountPendingByCompany(this.SessionService.User.CompanyID);

            return this.ResultService.JsonSuccess(model);
            ////return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser]
        public ActionResult LastCompanies()
        {
            List<CompanyQuickResult> lastCompanies = new List<CompanyQuickResult>();

            var list = this.Services.Company
                .GetLastApproved(5)
                .Select(c => new CompanyQuickResult
                {
                    Name = c.Name,
                    Url = "/Company/" + c.Alias,
                    Baseline = c.Baseline ?? string.Empty,
                    ////Picture = this.GetSimpleUrl("Data", "CompanyPicture", "DataPicture", new { id = c.Alias, size = "Small", }),
                    Picture = this.Services.Company.GetProfilePictureUrl(c.Alias, CompanyPictureSize.Small, UriKind.Relative),
                })
                .ToList();
            lastCompanies.AddRange(list);

            if (lastCompanies.Count > 0)
            {
                lastCompanies.Insert(0, new CompanyQuickResult { Name = Lang.T("Nouveaux arrivants"), Url = "" });
            }

            return this.ResultService.JsonSuccess(lastCompanies);
            ////return Json(lastCompanies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendMessage(int id, string message)
        {
            // Gets first CompanyContact - it is an answer, there is necessarily a first element
            CompanyContact cc = this.Services.CompanyContacts.GetById(id);

            if (cc == null)
                return this.ResultService.JsonError("NoSuchFeed", Alerts.SendCompanyMessage_NoSuchFeed);

            //if ((cc.FromCompanyId.HasValue && !cc.ToCompanyId.HasValue && cc.FromCompanyId.Value != this.SessionService.User.CompanyID) ||
            //    (!cc.FromCompanyId.HasValue && cc.ToCompanyId.HasValue && cc.ToCompanyId.Value != this.SessionService.User.CompanyID))
            //{
            //    this.ErrorForbidden();
            //    return null;
            //}

            CompanyContact response = new CompanyContact();
            response.Type = 0;
            response.Date = DateTime.UtcNow;

            response.FromCompanyId = this.SessionService.User.CompanyID;
            response.FromUserId = this.SessionService.User.Id;
            response.ToUserEmail = cc.FromUserEmail;
            response.Message = message;
            response.IsRead = false;

            //if (cc.FromCompanyId.HasValue && cc.FromCompanyId.Value != this.SessionService.User.CompanyID)
            //{
            //    response.ToCompanyId = cc.FromCompanyId.Value;
            //}
            //else if (cc.ToCompanyId.HasValue && cc.ToCompanyId.Value != this.SessionService.User.CompanyID)
            //{
            //    response.ToCompanyId = cc.ToCompanyId.Value;
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(cc.FromUserEmail))
            //    {
            //        response.ToUserEmail = cc.FromUserEmail;
            //    }
            //    else
            //    {
            //        response.ToUserEmail = cc.ToUserEmail;
            //    }
            //}

            this.Services.CompanyContacts.Insert(response);
            response = this.Services.CompanyContacts.GetById(response.Id);

            this.Services.Email.SendCompanyContact(response);

            return this.ResultService.JsonSuccess();
        }

        [AuthorizeUser]
        [Sparkle.Filters.AuthorizeByCompanyAccess(CompanyAccessLevel.Administrator)]
        public ActionResult Messages()
        {
            Company company = this.Services.Company.GetById(this.SessionService.User.CompanyID);

            CompanyMessagesModel model = new CompanyMessagesModel();
            model.Company = company;

            // Gets messages
            model.Messages = this.Services.CompanyContacts.SelectByToCompanyId(this.SessionService.User.CompanyID);
            foreach (var message in model.Messages.Where(m => m.FromCompany != null))
            {
                message.FromCompanyName = message.FromCompany.Name;
            }

            // Detection des différentes conversations
            List<string> Convs = model.Messages.Select(o => o.FromCompanyName).Distinct().ToList();
            foreach (var conv in Convs)
            {
                model.Conversations.Add(model.Messages.Where(m => m.FromCompanyName == conv).FirstOrDefault());
            }

            return View(model);
        }

        [HttpPost]
        [AuthorizeUser]
        [Sparkle.Filters.AuthorizeByCompanyAccess(CompanyAccessLevel.Administrator)]
        public ActionResult Conversation(int id)
        {
            CompanyMessagesModel model = new CompanyMessagesModel();
            if (this.SessionService.User.Company == null)
            {
                this.SessionService.User.Company = this.Services.Company.GetById(this.SessionService.User.CompanyID);
            }

            model.Company = this.SessionService.User.Company;
            model.IdForResponse = id;

            // Gets first CompanyContact
            CompanyContact cc = this.Services.CompanyContacts.GetById(id);
            if (cc != null)
            {
                if (cc.FromCompanyId.HasValue)
                {
                    model.Messages = this.Services.CompanyContacts.SelectByToCompanyIdAndFromCompanyId(cc.ToCompanyId.Value, cc.FromCompanyId.Value).OrderBy(m => m.Id).ToList();
                }
                else
                {
                    model.Messages = this.Services.CompanyContacts.SelectByToCompanyIdAndFromUserEmail(cc.ToCompanyId.Value, cc.FromUserEmail).OrderBy(m => m.Id).ToList();
                }
            }

            return PartialView("Conversation", model);
        }

        [AuthorizeUser]
        [Sparkle.Filters.AuthorizeByCompanyAccess(CompanyAccessLevel.Administrator, CompanyAccessLevel.CommunityManager)]
        public ActionResult Visits()
        {
            VisitsModel model = new VisitsModel();

            var items = this.Services.CompaniesVisits.SelectByCompanyId(this.SessionService.User.CompanyID);

            foreach (var item in items)
            {
                User user = this.Services.People.SelectWithId(item.UserId);

                if (user != null)
                {
                    model.Visits.Add(new VisitModel()
                    {
                        Date = item.Date,
                        Count = item.ViewCount,
                        Name = user.FirstName + " " + user.LastName,
                        Login = user.Login,
                        ////Picture = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = user.Login, size = "Medium", }),
                        Picture = this.Services.People.GetProfilePictureUrl(user, UserProfilePictureSize.Medium, UriKind.Relative),
                        JobName = (user.Job != null ? user.Job.Libelle : null),
                        JobAlias = (user.Job != null ? user.Job.Alias : null),
                        CompanyName = user.Company.Name,
                        CompanyAlias = user.Company.Alias
                    });
                }
            }

            return View(model);
        }

        [AuthorizeUser]
        [Sparkle.Filters.AuthorizeByCompanyAccess(CompanyAccessLevel.Administrator)]
        public ActionResult Resumes()
        {
            // Company skills
            var companySkills = this.Services.CompanySkills.SelectByCompanyId(this.SessionService.User.CompanyID).Select(s => s.SkillId).ToList();
            
            ResumeViewListModel model = new ResumeViewListModel();

            model.Resumes = this.Services.Resumes.SelectApproved().Select(o => new ResumeViewModel(o)).ToList();
            foreach (var resume in model.Resumes)
            {
                resume.ResumeSkills = this.Services.ResumeSkills.SelectByResumeId(resume.Resume.Id);

                foreach (var item in resume.ResumeSkills)
                {
                    if (companySkills.Contains(item.SkillId))
                    {
                        resume.IsForMyCompany = true;
                        break;
                    }
                }
            }

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult Management(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                if (this.SessionService.User != null)
                {
                    var usersCompany = this.SessionService.User.Company ?? this.Services.Company.GetById(this.SessionService.User.CompanyID);
                    return this.RedirectToAction("Management", new { id = usersCompany.Alias, });
                }
                else
                {
                    return this.ResultService.NotFound();
                }
            }

            var user = this.SessionService.User;
            bool isAdmin = user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ManageCompany);

            var company = this.Services.Company.GetByAlias(id, isAdmin);
            if (company == null)
            {
                return this.ResultService.NotFound();
            }

            bool isCommunityMngr = user.CompanyID == company.ID && user.CompanyAccess == CompanyAccessLevel.CommunityManager;

            if (!isAdmin)
            {
                isAdmin = user.CompanyID == company.ID
                    && user.CompanyAccessLevel >= (int)(CompanyAccessLevel.Administrator);
            }

            if (!isAdmin && !isCommunityMngr)
                return this.ResultService.Forbidden();

            var model = new CompanyManagementModel();
            model.CompanyName = company.Name;
            model.CompanyAlias = company.Alias;

            // Personnel
            ////var me = this.SessionService.User;
            var coworkers = this.Services.People.GetAllByCompanyId(company.ID);

            model.MustBeValidate = coworkers
                .MustBeValidatedByCompany()
                .Select(o => new PeopleModel(this.Services, o)).ToList();

            var validated = new List<User>();
            validated = coworkers
                .AlreadyValidatedByCompany()
                .ToList();

            model.DisabledAccounts = validated
                .Where(o => o.CompanyAccess == CompanyAccessLevel.Disabled)
                .Select(o => new PeopleModel(this.Services, o))
                .ToList();

            model.CoWorkers = validated
                .Where(o => o.CompanyAccess == CompanyAccessLevel.User)
                .Select(o => new PeopleModel(this.Services, o))
                .ToList();

            model.CommunityManagers = validated
                .Where(o => o.CompanyAccess == CompanyAccessLevel.CommunityManager)
                .Select(o => new PeopleModel(this.Services, o))
                .ToList();

            model.Administrators = validated
                .Where(o => o.CompanyAccess == CompanyAccessLevel.Administrator)
                .Select(o => new PeopleModel(this.Services, o))
                .ToList();

            model.NetworkDisabled = coworkers.Where(x => x.NetworkAccessLevel == 0).Select(x => new PeopleModel(this.Services, x)).ToList();

            // Retardataires
            IList<Invited> invited = this.Services.Invited.GetPendingByCompany(company.ID);
            model.Invited = invited.Select(o => new CompanyManagementInvitedModel(o)).ToList();

            // register requests
            var requests = this.Services.RegisterRequests.GetAllByCompany(company.ID);
            model.RegisterRequests = requests;

            // apply requests
            var applies = this.Services.Company.GetApplyRequestsWithCompanyId(company.ID);
            model.ApplyRequests = applies;

            return this.View(model);
        }

        [HttpPost]
        [AuthorizeUser]
        public ActionResult UploadPicture(string companyAlias, HttpPostedFileBase image)
        {
            var company = this.Services.Company.GetByAlias(companyAlias);

            var model = new ResultModel();
            if (image != null)
            {
                var stream = image.InputStream;
                var mime = image.ContentType;
                var name = image.FileName;
                SetProfilePictureResult result = this.Services.Company.SetProfilePicture(new SetProfilePictureRequest
                {
                    UserId = company.ID,
                    PictureStream = stream,
                    PictureMime = mime,
                    PictureName = name,
                });
                if (result.Succeed)
                {
                    ////model.Result = Urls.GetCompanyPictureUrl(company.Alias, CompanyPictureSize.Medium);
                    model.Result = this.Services.Company.GetProfilePictureUrl(company, CompanyPictureSize.Medium, UriKind.Relative);

                    this.GetCacheAccessor().CompanyPictures.Invalidate(company.Alias);

                    var actionResult = (JsonNetResult)this.ResultService.JsonSuccess(new
                    {
                        Url = model.Result,
                        RefreshUrl = model.Result + "?refresh=" + DateTime.UtcNow.Ticks,
                    });
                    actionResult.ContentType = "text/html"; // ie does not like json in iframes
                    return actionResult;
                }
                else if (result.Errors.Count > 0)
                {
                    var error = result.Errors.First();
                    return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
                }
                else
                {
                    return this.ResultService.JsonError();
                }
            }
            else
            {
                return this.ResultService.JsonError("ImageNull", Alerts.UploadPictureNullStream);
            }
        }

        [AuthorizeUser]
        public ActionResult History()
        {
            CompanyHistoryModel model = new CompanyHistoryModel();
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult Achievements(string id)
        {
            if (!this.Services.AppConfiguration.Tree.Features.Companies.Achievements.IsEnabled)
                return this.ResultService.NotFound();

            if (string.IsNullOrEmpty(id))
            {
                this.TempData.AddError("Cette entreprise n'existe pas ou a changé de nom");
                return this.RedirectToAction("Index", "Companies");
            }

            var company = this.Services.Company.GetByAlias(id, true);
            if (company == null)
            {
                this.TempData.AddError("Cette entreprise n'existe pas ou a changé de nom");
                return this.RedirectToAction("Index", "Companies");
            }

            CompanyAchievements model = new CompanyAchievements();

            model.EditList.CompanyId = company.ID;
            model.EditList.Items = this.Services.Achievements.SelectAllByCompanyId(company.ID);

            return View(model);
        }

        public ActionResult Search(string query, string location, int[] tagIds, bool accrued)
        {
            IList<Sparkle.Models.CompanyListModel> results = null;
            if (!string.IsNullOrEmpty(query))
            {
                results = new List<Sparkle.Models.CompanyListModel>();
                var list = GetCompaniesFromCache(this.Services);

                foreach (var company in list)
                {
                    if (company.Name.RemoveDiacritics().ToLower().Contains(query.ToLower()))
                    {
                        results.Add(company);
                    }
                    else
                    {
                        foreach (var skill in company.Skills.ToList())
                        {
                            if (skill.Name.RemoveDiacritics().ToLower().Contains(query.ToLower()))
                            {
                                results.Add(company);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                results = GetCompaniesFromCache(this.Services);
            }

            if (!string.IsNullOrEmpty(location))
            {
                var geoloc = this.GetGeolocFromSparkleStatus(location);
                if (geoloc != null)
                {
                    var companiesIds = this.Services.Company.GetCompaniesNearLocation(geoloc.Geocodes).Select(o => o.Id).ToArray();
                    results = results.Where(o => companiesIds.Contains(o.Id)).ToList();
                }
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                var companyIds = this.Services.Tags.GetEntitiesIdsWhoUsesTagsIds("Company", tagIds, accrued);
                results = results.Where(o => companyIds.Contains(o.Id)).ToList();
            }

            var model = new CompaniesListModel(results);

            return PartialView("Lists/CompaniesList", model);
        }

        [HttpGet]
        public ActionResult SearchBySkills()
        {
            return this.ResultService.Gone();
        }

        [HttpPost]
        public ActionResult SearchBySkills(int networkId, string filters, bool accrued, string location, int[] tagIds, bool accruedTags)
        { 
            string[] items = filters != null ? filters.Split(',') : new string[0];
            
            List<int> skills = new List<int>();
            foreach (var item in items)
            {
                int skillId = 0;
                if (int.TryParse(item, out skillId))
                {
                    skills.Add(skillId);
                }
            }

            var allCompanies = GetCompaniesFromCache(this.Services);
            allCompanies = allCompanies.Where(c => c.NetworkId == networkId).ToList();

            var companies = new List<Sparkle.Models.CompanyListModel>();
            foreach (var company in allCompanies)
            {
                bool isValid = false;

                foreach (var skill in skills)
                {
                    if (company.Skills.Select(s => s.Value).Contains(skill))
                    {
                        isValid = true;
                        if (!accrued)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (accrued)
                        {
                            isValid = false;
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    companies.Add(company);
                }
            }

            if (!string.IsNullOrEmpty(location))
            {
                var geoloc = this.GetGeolocFromSparkleStatus(location);
                if (geoloc != null)
                {
                    var localizedCompaniesIds = this.Services.Company.GetCompaniesNearLocation(geoloc.Geocodes).Select(o => o.Id).ToArray();
                    companies = companies.Where(o => localizedCompaniesIds.Contains(o.Id)).ToList();
                }
            }

            if (tagIds != null && tagIds.Length > 0)
            {
                var companyIds = this.Services.Tags.GetEntitiesIdsWhoUsesTagsIds("Company", tagIds, accrued);
                companies = companies.Where(o => companyIds.Contains(o.Id)).ToList();
            }

            var model = new CompaniesListModel(companies);

            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult AjaxSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return this.ResultService.JsonError();
            }

            var companies = GetCompaniesFromCache(this.Services);

            IList<CompanyQuickResult> results = new List<CompanyQuickResult>();
            foreach (var company in companies)
            {
                if (company.Name.ToLower().Contains(query.ToLower()))
                {
                    CompanyQuickResult result = new CompanyQuickResult();
                    result.Name = company.Name;
                    result.Baseline = company.Baseline ?? string.Empty;
                    result.Url = "/Company/" + company.Alias;
                    ////result.Picture = this.GetSimpleUrl("Data", "CompanyPicture", "DataPicture", new { id = company.Alias, size = "Small", });
                    result.Picture = this.Services.Company.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Small, UriKind.Relative);
                    results.Add(result);

                    if (results.Count > 5)
                        break;
                }
                else
                {

                    ////foreach (var skill in company.Skills.ToList())
                    ////{
                    ////    if (skill.Name.ToLower().Contains(query.ToLower()))
                    ////    {
                    ////        CompanyQuickResult result = new CompanyQuickResult();
                    ////        result.Name = company.Name;
                    ////        result.Baseline = company.Baseline ?? string.Empty;
                    ////        result.Url = "/Company/" + company.Alias;
                    ////        result.Picture = this.GetSimpleUrl("Data", "CompanyPicture", "DataPicture", new { id = company.Alias, size = "Small", });
                    ////        result.Picture = this.Services.Company.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Small, UriKind.Relative);
                    ////        results.Add(result);

                    ////        if (results.Count > 9)
                    ////        {
                    ////            break;
                    ////        }
                    ////    }
                    ////}
                }
            }

            return this.ResultService.JsonSuccess(results);
        }

        [HttpPost, AuthorizeUser]
        public ActionResult UpdateRegisterRequests(FormCollection form, string ReturnUrl)
        {
            var requests = new List<UpdateRegisterRequestRequest>();

            foreach (var key in form.AllKeys)
            {
                int id;
                if (int.TryParse(key, out id))
                {
                    var item = new UpdateRegisterRequestRequest();
                    item.Id = id;

                    if (form[key].ToLowerInvariant() == "accept")
                    {
                        item.NewStatus = RegisterRequestStatus.Accepted;
                        requests.Add(item);
                    }

                    if (form[key].ToLowerInvariant() == "deny")
                    {
                        item.NewStatus = RegisterRequestStatus.Refused;
                        requests.Add(item);
                    }
                }
            }

            var currentCompany = this.Services.RegisterRequests.GetCompanyIdFromRequestsIds(requests.Select(o => o.Id).ToArray());
            if (!currentCompany.HasValue)
            {
                this.TempData.AddError("Quelque chose s'est mal passé avec les demandes d'inscriptions.");
                return this.Redirect(this.AnyLocalUrl(ReturnUrl, true, this.Url.Action("Management")));
            }

            if (requests.Count > 0)
            {
                var user = this.SessionService.User;
                var results = this.Services.RegisterRequests.Update(requests, user.Id, currentCompany.Value);

                int accepts = 0, denials = 0;
                foreach (var result in results)
                {
                    if (result.Succeed)
                    {
                        if (result.ItemAfter.StatusCode == RegisterRequestStatus.Refused)
                            denials++;
                        if (result.ItemAfter.StatusCode == RegisterRequestStatus.Accepted)
                            accepts++;
                    }

                    foreach (var error in result.Errors)
                    {
                        this.TempData.AddError(error.DisplayMessage ?? error.Code.ToString()); 
                    }
                }

                if (accepts == 1)
                {
                    this.TempData.AddConfirmation("Une demande d'inscription a été acceptée.");
                }
                else if (accepts > 1)
                {
                    this.TempData.AddConfirmation(accepts + " demandes d'inscription ont été acceptées.");
                }

                if (denials == 1)
                {
                    this.TempData.AddConfirmation("Une demande d'inscription a été refusée.");
                }
                else if (denials > 1)
                {
                    this.TempData.AddConfirmation(denials + " demandes d'inscription ont été refusées.");
                }
            }

            return this.Redirect(this.AnyLocalUrl(ReturnUrl, true, this.Url.Action("Management")));
        }

        [AuthorizeUser]
        public ActionResult GetTagsByCompany(string companyAlias, string categoryAlias)
        {
            if (!this.Request.IsAjaxRequest())
                return this.ResultService.NotFound();
            if (string.IsNullOrEmpty(companyAlias) || string.IsNullOrEmpty(categoryAlias))
                return this.ResultService.JsonError("EmptyArguments", Alerts.EmptyArguments);

            var company = this.Services.Company.GetByAlias(companyAlias);
            if (company == null)
                return this.ResultService.JsonError("NoSuchCompany", Alerts.NoSuchCompany);

            var category = this.Services.Tags.GetCategoryByAlias(categoryAlias);
            if (category == null)
                return this.ResultService.JsonError("NoSuchTagCategory", Alerts.NoSuchTagCategory);

            var tags = this.Services.Tags.GetTagsByCompanyIdAndCategoryId(company.ID, category.Id);

            return this.ResultService.JsonSuccess(tags.OrderBy(o => o.Name).ToList());
        }

        private LocationGeolocData GetGeolocFromSparkleStatus(string location)
        {
            const string logPath = "CompaniesController.GetGeolocFromSparkleStatus";
            var sparkleApi = new SparkleStatusApi(
                this.Services.AppConfiguration.Tree.Externals.SparkleStatus.ApiKey,
                this.Services.AppConfiguration.Tree.Externals.SparkleStatus.Api2Key,
                this.Services.AppConfiguration.Tree.Externals.SparkleStatus.Api2Secret,
                this.Services.AppConfiguration.Tree.Externals.SparkleStatus.BaseUrl);

            LocationGeolocData result = null;
            try
            {
                result = sparkleApi.GetLocationGeolocFromCache(location);
                Trace.TraceInformation(logPath + ": API request for '" + location + "' returned '" + string.Join("', '", result.Geocodes) + "'");
            }
            catch (InvalidOperationException ex)
            {
                this.ReportError(ex, logPath + ": API call to SparkleStatus failed");
                this.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, ex);
                Trace.TraceWarning(logPath + ": API request for '" + location + "' failed '" + ex.Message + "'");
            }

            return result;
        }

        internal static List<Sparkle.Models.CompanyListModel> GetCompaniesFromCache(Sparkle.Services.Networks.IServiceFactory services)
        {
            lock (companiesCacheLock)
            {
                if (companiesCache == null || companiesCacheDate.Add(companiesCacheDuration) < DateTime.UtcNow)
                {
                    var watch = Stopwatch.StartNew();

                    // this is fast but if teches too many items
                    ////var watchFields = Stopwatch.StartNew();
                    ////var companyProfileFields = services.ProfileFields.GetCompanyProfileFieldsByCompany();
                    ////watchFields.Stop();


                    var watchCategories = Stopwatch.StartNew();
                    var categories = services.Company.GetAllCategories();
                    watchCategories.Stop();

                    var watchMain = Stopwatch.StartNew();
                    companiesCache = services.Company.GetWithStatsAndSkillsAndJobs()
                        .Select(c => new Sparkle.Models.CompanyListModel(c, null /*companyProfileFields.ContainsKey(c.ID) ? companyProfileFields[c.ID] : new List<CompanyProfileFieldModel>()*/))
                        .ToList();
                    watchMain.Stop();
                    
                    var companyIds = companiesCache.Select(x => x.Id).ToArray();

                    // this is slow even if it fetches nothing
                    ////var watchFields2 = Stopwatch.StartNew();
                    ////var fields = services.ProfileFields.GetCompanyProfileFieldByCompanyIdAndType(companyIds, new ProfileFieldType[] { ProfileFieldType.Industry, ProfileFieldType.Location, });
                    ////watchFields2.Stop();

                    foreach (var company in companiesCache)
                    {
                        ////company.Picture = Urls.GetCompanyPictureUrl(company.Alias, CompanyPictureSize.Medium);
                        company.Picture = services.Company.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Medium, UriKind.Relative);

                        var category = categories.SingleOrDefault(c => c.Id == company.CategoryId);
                        if (category != null)
                        {
                            company.CategoryName = category.Name;
                        }

                        ////company.ProfileFields = new List<CompanyProfileFieldModel>();
                        ////if (fields.ContainsKey(company.Id))
                        ////{
                        ////    company.ProfileFields.AddRange(fields[company.Id]);
                        ////}
                    }

                    companiesCacheDate = DateTime.UtcNow;
                    watch.Stop();
                    Trace.WriteLine("CompaniesController.GetCompaniesFromCache: took " + watch.Elapsed + " to refresh the cache.");
                }
            }

            return companiesCache;
        }

        internal static void ClearCompaniesCache()
        {
            lock (companiesCacheLock)
            {
                companiesCache = null;
            }
        }
    }

    public class CompanyQuickResult
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Picture { get; set; }

        public string Baseline { get; set; }
    }

    public class CompanyMenu
    {
        public int ManagementCount { get; set; }
    }
}
