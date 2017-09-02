
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Main.Providers;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Definitions;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Tags.EntityWithTag;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.StatusApi;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Domain;
    using SrkToolkit.Globalization;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Objects.SqlClient;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Permit access to the company
    /// </summary>
    public class CompanyService : ServiceBase, ICompanyService
    {
        #region pictureOriginalFormat pictureFormats

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
                    var company = (Company)o;
                    return new string[]
                    {
                        "small",
                        company.Alias + ".gif",
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
                    var company = (Company)o;
                    return new string[]
                    {
                        company.Alias + ".gif",
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
                    var company = (Company)o;
                    return new string[]
                    {
                        "big",
                        company.Alias + ".jpg",
                    };
                }
            },
        };

        #endregion

        private static readonly string[] forbidenNames = new string[]
        {
            "support", "default",
        };

        private static readonly string[] ispCompanyDomainNames = new string[]
        {
            "aol", "apple", 
            "cogeco", "comcast", "claro", "caramail", 
            "distributel", 
            "eastlink", "email", 
            "gmail", 
            "hotmail", "hushmail", 
            "live", 
            "magacable", "movistar", "mail", "me", 
            "neuf", "numericable", 
            "outlook", "orange", 
            "rogers", 
            "sprinet", "sfr", "shaw", 
            "telecable", "telenet", 
            "verizon", "vmedia", "videotron", "voo", "virginmedia", "virgin",
            "wanadoo", "wow", 
            "yahoo", 
            "zoho", 
        };

        private static readonly string[] ispCompanyFullDomainNames = new string[]
        {
            "9business.fr",
            "altibox.no", "Axion.ca", "akeonet.com", "aliceadsl.fr", "a2cnet.com",
            "bayonette.no", "bnet.hr", "bbox.fr",
            "cablenet.com.cy", "cegetel.net", "club-internet.fr",
            "elisa.fi", 
            "free.fr", 
            "gmx.com", 
            "hs-hkb.ba", "hcn.gr",
            "laposte.net", 
            "me.com",
            "neuf.fr", "nerim.net", "nordnet.com", "netcourrier.fr", 
            "one.com", 
            "sonera.fi", "sfr.fr", 
            "telemach.ba", "telekabel.ba", 
            "voila.fr",
            "wibox.fr",
        };

        private IList<RegionInfo> regionInfos;

        private static bool ValidateEntity(IServiceFactory services, string entityIdentifier, AddOrRemoveTagResult result, out int entityId)
        {
            entityId = 0;
            var company = services.Company.GetByAlias(entityIdentifier);
            if (company == null)
            {
                result.Errors.Add(AddOrRemoveTagError.NoSuchCompany, NetworksEnumMessages.ResourceManager);
                return false;
            }

            entityId = company.ID;
            return true;
        }

        private static bool ValidateTag(IServiceFactory services, string entityIdentifier, string tagId, int? actingUserId, TagCategoryModel tagCategory, AddOrRemoveTagResult result)
        {
            if (!actingUserId.HasValue)
                throw new ArgumentNullException("The value cannot be empty.", "actingUserId");
            if (tagCategory == null || tagCategory.RulesModel == null || !tagCategory.RulesModel.Rules.ContainsKey(RuleType.Company))
                throw new ArgumentNullException("The value cannot be empty.", "tagCategory");

            // Check company exists
            int companyId;
            if (!CompanyService.ValidateEntity(services, entityIdentifier, result, out companyId))
            {
                return false;
            }

            // Check user rights
            var user = services.People.GetActiveById(actingUserId.Value, PersonOptions.None);
            if (user == null || !((user.CompanyId == companyId && user.CompanyAccessLevel.Value > CompanyAccessLevel.User) || user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff)))
            {
                result.Errors.Add(AddOrRemoveTagError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Check TagCategory company rules
            var max = tagCategory.RulesModel.Rules[RuleType.Company].Maximum;
            if (result.Request.AddTag && services.Repositories.CompanyTags.CountByCompanyAndCategory(companyId, tagCategory.Id, false) >= max)
            {
                result.Errors.Add(AddOrRemoveTagError.MaxNumberOfTagForCategory, NetworksEnumMessages.ResourceManager);
                return false;
            }

            // Build EntityWithTag into result
            result.Entity = new SqlEntityWithTag
            {
                EntityId = companyId,
            };

            return true;
        }

        ////static CompanyService()
        ////{
        ////    // see RegisterTags
        ////    TagsService.RegisterEntityValidator("Company", CompanyService.ValidateEntity);
        ////    TagsService.RegisterTagValidator("Company", CompanyService.ValidateTag);
        ////    TagsService.RegisterTagRepository("Company", EntityWithTagRepositoryType.Sql, "CompanyTags", "CompanyId");
        ////}

        [DebuggerStepThrough]
        internal CompanyService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        private IList<RegionInfo> RegionInfos
        {
            get
            {
                if (this.regionInfos == null)
                {
                    this.regionInfos = CultureInfoHelper.GetCountries();
                }

                return this.regionInfos;
            }
        }

        public void Initialize()
        {
            this.InitializeCompanyCategories();
            this.InitializeCompanyPlaces();
        }

        private void InitializeCompanyCategories()
        {
            IList<CompanyCategory> categories = null;
            if ((categories = this.Repo.CompanyCategories.GetByAliasNull()) != null && categories.Count > 0)
            {
                foreach (var item in categories)
                {
                    item.Alias = this.MakeCategoryAlias(item.Name);
                    this.Repo.CompanyCategories.Update(item);
                }
            }

            if (this.Repo.CompanyCategories.Count(this.Services.NetworkId) == 0)
            {
                var items = new List<CompanyCategory>();
                items.Add(new CompanyCategory { Alias = "Incubator", KnownCategoryValue = KnownCompanyCategory.CompanyAccelerator, Name = "Incubator", NetworkId = this.Services.NetworkId, });
                items.Add(new CompanyCategory { Alias = "Startup", KnownCategoryValue = KnownCompanyCategory.Startup, Name = "Startup", NetworkId = this.Services.NetworkId, });
                items.Add(new CompanyCategory { Alias = "Company", KnownCategoryValue = KnownCompanyCategory.Unknown, Name = "Company", NetworkId = this.Services.NetworkId, IsDefault = true, });
                items.Add(new CompanyCategory { Alias = "Association", KnownCategoryValue = KnownCompanyCategory.Unknown, Name = "Association", NetworkId = this.Services.NetworkId, });
                foreach (var item in items)
                {
                    this.Repo.CompanyCategories.Insert(item);
                }
            }
        }

        private void InitializeCompanyPlaces()
        {
            const string logPath = "CompanyService.InitializeCompanyPlaces";

            var allLocationFields = this.Repo.CompanyProfileFields.GetByFieldType(ProfileFieldType.Location).GroupBy(o => o.CompanyId).ToDictionary(o => o.Key, o => o.ToList());
            var allCompanyPlaces = this.Repo.CompanyPlaces.GetAll().GroupBy(o => o.CompanyId).ToDictionary(o => o.Key, o => o.ToList());

            bool missingPlaces = false;
            foreach (var item in allLocationFields)
            {
                if (!allCompanyPlaces.ContainsKey(item.Key) || allCompanyPlaces[item.Key].Count < item.Value.Count)
                {
                    missingPlaces = true;
                    break;
                }
            }

            if (!missingPlaces)
            {
                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "All CompanyPlaces OK.");
                return;
            }

            var now = DateTime.UtcNow;
            var transaction = this.Services.NewTransaction();
            using (transaction.BeginTransaction())
            {
                var fieldsModels = transaction.Repositories.CompanyProfileFields.GetByFieldType(ProfileFieldType.Location).GroupBy(o => o.CompanyId).ToDictionary(o => o.Key, o => o.ToList());
                allCompanyPlaces = transaction.Repositories.CompanyPlaces.GetAll().GroupBy(o => o.CompanyId).ToDictionary(o => o.Key, o => o.ToList());

                foreach (var item in fieldsModels)
                {
                    var company = transaction.Repositories.Companies.GetById(item.Key);
                    if (company != null
                        && (!allCompanyPlaces.ContainsKey(company.ID) || allCompanyPlaces[company.ID].Count < item.Value.Count))
                    {
                        foreach (var location in item.Value)
                        {
                            transaction.Services.Company.AddCompanyPlaceFromCompanyProfileField(logPath, company, location);
                        }
                    }
                }

                transaction.CompleteTransaction();
            }
        }

        /// <summary>
        /// Gets or sets the company repository.
        /// </summary>
        protected ICompanyRepository CompanyRepository
        {
            get { return this.Repo.Companies; }
        }

        /// <summary>
        /// Selects all companies.
        /// </summary>
        /// <returns>List of companies</returns>
        public IList<Company> SelectAll()
        {
            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .OrderBy(o => o.Name)
                .ToList();
        }

        /// <summary>
        /// Selects companie with domain name.
        /// </summary>
        /// <returns>List of companies</returns>
        public Company SelectByDomainName(string domainName)
        {
            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Enabled()
                .HasDomainName(domainName)
                .OrderBy(o => o.Name)
                .FirstOrDefault();
        }

        [Obsolete]
        public IList<Company> SelectBetaAllow()
        {
            return this.CompanyRepository
                .Select()
                .OrderBy(o => o.Name)
                .Where(o => o.ID == 1 || o.ID == 3 || o.ID == 19 || o.ID == 41 || o.ID == 43 || o.ID == 56)
                .ToList();
        }

        public Company GetById(int id)
        {
            return this.CompanyRepository
            .Select()
                //.ByNetwork(this.Services.NetworkId)
            .WithId(id)
            .FirstOrDefault();
        }

        public Company GetById(int id, bool allNetworks)
        {
            if (allNetworks)
            {
                return this.CompanyRepository
                   .Select()
                   .WithId(id)
                   .FirstOrDefault();
            }

            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .WithId(id)
                .FirstOrDefault();
        }

        public Company GetByAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return null;
            }

            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .WithAlias(alias)
                .FirstOrDefault();
        }

        public Company GetByAlias(string alias, bool allNetworks)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return null;
            }

            if (allNetworks)
            {
                return this.CompanyRepository
                    .Select()
                    .WithAlias(alias)
                    .FirstOrDefault();
            }

            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .WithAlias(alias)
                .FirstOrDefault();
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<Company> Search(string request)
        {
            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Contain(request)
                .Take(5)
                .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Update, true)]
        public long Update(Company item, bool allNetworks = false)
        {
            if (!allNetworks)
            {
                this.VerifyNetwork(item);
            }

            item = this.CompanyRepository.Update(item);
            this.Services.Logger.Info("CompanyService.Update", ErrorLevel.Success, "Updated company details {0}", item.ID);
            return item.ID;
        }

        public void DeleteUnapprovedCompany(Company item, bool allNetworks = false)
        {
            if (!allNetworks)
            {
                this.VerifyNetwork(item);
            }

            this.CompanyRepository.Delete(item);
            this.Services.Logger.Info("CompanyService.DeleteUnapprovedCompany", ErrorLevel.Success, "DeleteUnapprovedCompany  {0}", item.ID);
        }

        public int CountCompleteProfiles()
        {
            return this.Repo.CompanyProfileFields.Select()
                .Where(o => o.ProfileFieldId == (int)ProfileFieldType.About && o.Value != null && SqlFunctions.DataLength(o.Value) > 400)
                .Count();
        }

        public IList<Company> GetBySkill(int skillId, bool allNetworks = false)
        {
            var companiesSkills = this.Repo.CompaniesSkills.Select().Where(s => s.SkillId == skillId).ToList();
            List<int> companiesId = new List<int>();
            foreach (var item in companiesSkills)
            {
                companiesId.Add(item.CompanyId);
            }

            if (allNetworks)
            {
                return this.Repo.Companies
                    .Select()
                    .Where(i => companiesId.Contains(i.ID))
                    .ToList();
            }

            return this.Repo.Companies
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(i => companiesId.Contains(i.ID))
                .ToList();
        }

        public IList<Company> GetLastApproved(int max)
        {
            return this.CompanyRepository
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(c => c.ApprovedDateUtc.HasValue)
                .OrderByDescending(c => c.ApprovedDateUtc.Value)
                .Take(max)
                .ToList();
        }

        public IList<Company> SelectByNetworkId(int networkId)
        {
            return this.Repo.Companies.Select()
             .Where(c => c.NetworkId == networkId)
             .ToList();
        }

        public Company Insert(Company item)
        {
            this.SetNetwork(item);

            item = this.Repo.Companies.Insert(item);
            this.Services.Logger.Info("CompanyService.Update", ErrorLevel.Success, "Inserted company details {0}", item.ID);
            return item;
        }

        public CompanyRequest CreateForApproval(CompanyRequest item)
        {
            this.SetNetwork(item);

            if (item.Id != 0) // update
            {
                item = this.Repo.CompanyRequests.Update(item);
                this.Services.Logger.Info("CompanyService.CreateForApproval", ErrorLevel.Success, "CreateForApproval updated CompanyRequest {0}", item.Id);
            }
            else // create
            {
                item.UniqueId = Guid.NewGuid();
                item.Approved = null;
                item.ApprovedReason = null;
                item.CreatedDateUtc = DateTime.UtcNow;
                item.NetworkId = this.Services.NetworkId;

                item = this.Repo.CompanyRequests.Insert(item);
                this.Services.Logger.Info("CompanyService.CreateForApproval", ErrorLevel.Success, "CreateForApproval inserted CompanyRequest {0}", item.Id);

                var recipients = this.Services.People.GetByNetworkAccessLevel(NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.AddCompany);

                this.Services.Email.SendNewCompanyDetailsForApproval(item, recipients);
                this.Services.Email.SendCompanyRequestConfirmation(item, item.Email);
            }

            return item;
        }

        public string MakeAlias(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (this.Repo.Companies.GetIdByAlias(alias) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.Companies.GetIdByAlias(a) == null);
            }
            return alias;
        }

        private string MakeCategoryAlias(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string alias = name.Trim().MakeUrlFriendly(true);
            if (this.Repo.Companies.GetIdByAlias(alias) != null)
            {
                alias = alias.GetIncrementedString(a => this.Repo.CompanyCategories.GetByAlias(a, this.Services.NetworkId) == null);
            }
            return alias;
        }

        public IList<CompanyExtended> GetWithStatsAndSkills()
        {
            return this.CompanyRepository.GetWithStatsAndSkills(this.Services.NetworkId);
        }

        public IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetWithStatsAndSkillsAndJobs()
        {
            return this.CompanyRepository.GetWithStatsAndSkillsAndJobs(this.Services.NetworkId);
        }

        public IList<CompanyExtended> GetWithStats()
        {
            return this.CompanyRepository.GetWithStats(this.Services.NetworkId);
        }

        public IList<CompanyExtended> GetInactiveWithStats()
        {
            return this.CompanyRepository.GetInactiveWithStats(this.Services.NetworkId);
        }

        public IList<CompanyExtended> SelectByApprovedDateUpper(DateTime date)
        {
            return this.CompanyRepository
                .GetWithStatsAndSkills(this.Services.NetworkId)
                .Where(c => c.Company.ApprovedDateUtc > date)
                .ToList();
        }

        public IList<CompanyExtended> SelectByApprovedDateRange(DateTime from, DateTime to)
        {
            return this.CompanyRepository
                .GetWithStatsAndSkills(this.Services.NetworkId)
                .Where(c => from <= c.Company.ApprovedDateUtc && c.Company.ApprovedDateUtc <= to && c.Company.IsEnabled)
                .ToList();
        }

        public int Count()
        {
            return CompanyRepository
               .Select()
               .ByNetwork(this.Services.NetworkId)
               .Count();
        }

        public int CountActive()
        {
            return CompanyRepository
               .Select()
               .ByNetwork(this.Services.NetworkId)
               .Where(x => x.IsEnabled)
               .Count();
        }

        public int CountByNetworkId(int networkId)
        {
            return CompanyRepository
               .Select()
             .ByNetwork(networkId)
               .Count();
        }

        public int CountNonEmpty()
        {
            return CompanyRepository
               .Select()
              .ByNetwork(this.Services.NetworkId)
               .Where(p => p.Users.Count > 0)
               .Count();
        }

        public int CountEmpty()
        {
            return CompanyRepository
               .Select()
               .ByNetwork(this.Services.NetworkId)
               .Where(p => p.Users.Count == 0)
               .Count();
        }

        public int CountPendingMembers(int id)
        {
            return this.Repo.People.CountPendingCompanyMembers(id);
        }

        public int CountRegisteredMembers(int id)
        {
            return this.Repo.People.CountRegisteredCompanyMembers(id);
        }

        public IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetActiveLight()
        {
            return this.CompanyRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .Select(c => new Sparkle.Entities.Networks.Neutral.CompanyPoco
                {
                    ID = c.ID,
                    Alias = c.Alias,
                    Name = c.Name,
                    EmailDomain = c.EmailDomain,
                    ApprovedDateUtc = c.ApprovedDateUtc,
                    IsApproved = c.IsApproved,
                    IsEnabled = c.IsEnabled,
                    NetworkId = c.NetworkId,
                })
                .ToList();
        }

        public IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetInactiveLight()
        {
            return this.CompanyRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Where(c => !c.IsApproved)
                .Select(c => new Sparkle.Entities.Networks.Neutral.CompanyPoco
                {
                    ID = c.ID,
                    Alias = c.Alias,
                    Name = c.Name,
                    EmailDomain = c.EmailDomain,
                    ApprovedDateUtc = c.ApprovedDateUtc,
                    IsApproved = c.IsApproved,
                    IsEnabled = c.IsEnabled,
                    NetworkId = c.NetworkId,
                })
                .ToList();
        }

        /// <summary>
        /// Gets all light.
        /// </summary>
        /// <returns></returns>
        public IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetAllLight()
        {
            return this.CompanyRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Select(c => new Sparkle.Entities.Networks.Neutral.CompanyPoco
                {
                    ID = c.ID,
                    Alias = c.Alias,
                    Name = c.Name,
                    EmailDomain = c.EmailDomain,
                    ApprovedDateUtc = c.ApprovedDateUtc,
                    IsApproved = c.IsApproved,
                    IsEnabled = c.IsEnabled,
                    NetworkId = c.NetworkId,
                })
                .ToList();
        }

        public IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> SearchActiveLight(string nameLike)
        {
            return this.CompanyRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .Where(c => c.Name.Contains(nameLike))
                .Select(c => new Sparkle.Entities.Networks.Neutral.CompanyPoco
                {
                    ID = c.ID,
                    Alias = c.Alias,
                    Name = c.Name,
                    EmailDomain = c.EmailDomain,
                    ApprovedDateUtc = c.ApprovedDateUtc,
                    IsApproved = c.IsApproved,
                    IsEnabled = c.IsEnabled,
                    NetworkId = c.NetworkId,
                })
                .ToList();
        }

        public IList<CompanyExtended> GetWaitingApprobation()
        {
            return this.CompanyRepository.GetWaitingApprobation(this.Services.NetworkId);
        }

        public int CountPublications(int companyId)
        {
            return this.Repo.Wall.CountByCompany(companyId);
        }

        public string FindEmailDomainFromNameAndEmail(string companyName, string emailAddress)
        {
            if (string.IsNullOrEmpty(companyName))
                throw new ArgumentException("The value cannot be empty", "companyName");
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentException("The value cannot be empty", "emailAddress");

            companyName = companyName.Trim().ToLowerInvariant();
            emailAddress = emailAddress.Trim().ToLowerInvariant();
            var email = new EmailAddress(emailAddress);

            var nameSplitRegex = new Regex(@"\W*(\w+)\W*", RegexOptions.IgnoreCase);
            var namesInNameCol = nameSplitRegex.Matches(companyName);
            string[] namesInName = new string[namesInNameCol.Count];
            var namesInDomain = nameSplitRegex.Matches(email.DomainPart).Cast<Match>().Select(x => x.Groups[1].Value).ToArray();

            // ISP full domain names cannot be a company email domain
            if (ispCompanyFullDomainNames.Any(x => email.DomainPart.Equals(x, StringComparison.OrdinalIgnoreCase) || email.DomainPart.EndsWith("." + x, StringComparison.OrdinalIgnoreCase)))
            {
                // altibox.no, elisa.fi, hcn.gr, voila.fr...
                return null;
            }

            // ISP main domain names cannot be a company email domain
            if (namesInDomain.Length >= 2 && namesInDomain[namesInDomain.Length - 2] != null && ispCompanyDomainNames.Any(x => x.Equals(namesInDomain[namesInDomain.Length - 2], StringComparison.OrdinalIgnoreCase)))
            {
                // gmail.xxx, live.xxx, vodafone.xxx, orange.xxx...
                return null;
            }

            for (int i = 0; i < namesInNameCol.Count; i++)
            {
                var match = namesInNameCol[i];
                namesInName[i] = match.Groups[1].Value;
            }

            int namesFound = 0;
            for (int i = 0; i < namesInDomain.Length; i++)
            {
                var name = namesInDomain[i];
                if (namesInName.Contains(name) || namesInName.Any(n => name.StartsWith(n) || name.EndsWith(n)))
                {
                    namesFound += 1;
                }
            }

            if (namesFound > 0)
            {
                return email.DomainPart;
            }
            else
            {
                return null;
            }
        }

        public CompanyRequest GetRequest(Guid id)
        {
            return this.Repo.CompanyRequests.GetByUniqueId(id);
        }

        public CompanyRequest GetRequest(int id)
        {
            return this.Repo.CompanyRequests.GetById(id);
        }

        public IList<CompanyRequest> GetPendingRequests()
        {
            return this.Repo.CompanyRequests.GetPending(this.Services.NetworkId);
        }

        public IList<CompanyRequest> GetAllRequests()
        {
            return this.Repo.CompanyRequests.GetAllTimeDescending(this.Services.NetworkId);
        }

        public Company AcceptRequest(int companyRequestId, int userId)
        {
            var user = this.Repo.People.GetById(userId);
            var request = this.Repo.CompanyRequests.GetById(companyRequestId);

            // create company
            string alias = this.Services.Company.MakeAlias(request.Alias);
            var company = new Company
            {
                Name = request.Name,
                Alias = alias,
                Baseline = request.Baseline,
                EmailDomain = request.EmailDomain,
                CategoryId = (short)request.CategoryId, // bad cast
                NetworkId = request.NetworkId,
                IsApproved = true,
                CreatedDateUtc = request.CreatedDateUtc,
                ApprovedDateUtc = DateTime.UtcNow,
                IsEnabled = true,
            };
            company = this.Repo.Companies.Insert(company);

            // insert company profile fields
            // TODO: prefer AppendCompanyProfileField, we don't need to overwrite non-existent fields!
            var source = ProfileFieldSource.UserInput;
            this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Site, request.Website, source);
            this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Phone, request.Phone, source);
            this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.About, request.About, source);
            this.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Email, request.Email, source);

            // update request
            Debug.Assert(company.ID > 0);
            request.CompanyId = company.ID;
            request.Approved = true;
            request.ClosedDateUtc = DateTime.UtcNow;
            request.ClosedByUserId = user.Id;
            request = this.Repo.CompanyRequests.Update(request);

            // confirm action to requester
            var adminEmails = Validate.ManyEmailAddresses(request.AdminEmails).ToArray();
            var otherEmails = Validate.ManyEmailAddresses(request.OtherEmails).ToArray();
            this.Services.Email.SendCompanyRequestAccepted(company, request.Email, adminEmails, otherEmails);

            // invite people
            foreach (var email in adminEmails)
            {
                var result = this.Services.Invited.Invite(user, email,
                    companyId: company.ID,
                    companyAccess: CompanyAccessLevel.Administrator);
            }

            foreach (var email in otherEmails)
            {
                var result = this.Services.Invited.Invite(user, email,
                    companyId: company.ID,
                    companyAccess: CompanyAccessLevel.User);
            }

            // add timelineitem CompanyJoin
            this.Services.Wall.AddCompanyJoinItem(company, userId);

            return company;
        }

        public CompanyRequest RejectRequest(int companyRequestId, string reason, int userId)
        {
            reason = reason.NullIfEmptyOrWhitespace();
            var user = this.Repo.People.GetById(userId);
            var request = this.Repo.CompanyRequests.GetById(companyRequestId);

            // update request
            request.Approved = false;
            request.ClosedDateUtc = DateTime.UtcNow;
            request.ClosedByUserId = user.Id;
            request.ApprovedReason = reason;
            request.RejectedTimes += 1;
            request = this.Repo.CompanyRequests.Update(request);

            // confirm action to requester
            this.Services.Email.SendCompanyRequestRejected(request, request.Email, reason);

            return request;
        }

        public CompanyRequest BlockRequest(int id, int userId, string reason)
        {
            var item = this.Repo.CompanyRequests.GetById(id);
            var user = this.Repo.People.GetById(userId);
            item.Approved = false;
            if (item.ClosedDateUtc == null)
            {
                item.ClosedDateUtc = DateTime.UtcNow;
                item.RejectedTimes += 1;
            }
            
            if (item.ClosedByUserId == null)
                item.ClosedByUserId = user.Id;
            
            item.BlockedByUserId = user.Id;
            item.BlockedDateUtc = DateTime.UtcNow;
            item.BlockedReason = reason;
            item = this.Repo.CompanyRequests.Update(item);
            return item;
        }

        public int CountPendingRequests()
        {
            return this.Repo.CompanyRequests.CountPending(this.Services.NetworkId);
        }

        public int CountWaitingApprobation()
        {
            return this.CompanyRepository.CountWaitingApprobation(this.Services.NetworkId);
        }

        public IList<Company> GetAllForImportScripts()
        {
            return this.Repo.Companies.GetAllForImportScripts(this.Services.NetworkId);
        }

        public IList<CompanyCategoryModel> GetAllCategories()
        {
            return this.Repo.CompanyCategories.GetAll(this.Services.NetworkId)
                .Select(c => new CompanyCategoryModel(c))
                .ToList();
        }

        public IList<UserModel> GetAdministrators(int companyId)
        {
            return this.CompanyRepository.GetAdministrators(companyId, this.Services.NetworkId)
                .Select(u => new UserModel(u))
                .ToList();
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(int companyId, PictureAccessMode mode)
        {
            var item = this.Services.Company.GetById(companyId);
            if (item == null)
                return null;

            return this.GetProfilePicture(item.ID, item.Alias, mode);
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(string companyAlias, PictureAccessMode mode)
        {
            var item = this.Services.Company.GetByAlias(companyAlias);
            if (item == null)
                return null;

            return this.GetProfilePicture(item.ID, item.Alias, mode);
        }

        public Services.Networks.Models.ProfilePictureModel GetProfilePicture(Company item, PictureAccessMode mode)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return this.GetProfilePicture(item.ID, item.Alias, mode);
        }

        public ProfilePictureModel GetProfilePicture(int id, string alias, PictureAccessMode mode)
        {
            if (string.IsNullOrEmpty(alias))
                throw new ArgumentException("The value cannot be empty", "username");

            var item = new Company
            {
                ID = id,
                Alias = alias,
            };

            string rootPath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
            string[] basePath = new string[]
            {
                rootPath,
                "Networks",
                this.Services.Network.Name,
                "Companies",
            };
            var commonCompaniesPath = Path.Combine(rootPath, "Networks", "Common", "Companies");
            var networkCompaniesPath = Path.Combine(rootPath, "Networks", this.Services.Network.Name, "Companies");

            IFilesystemProvider provider = new IOFilesystemProvider();
            var model = new ProfilePictureModel();
            var pictures = new Dictionary<string, PictureAccess>(4);
            model.Pictures = pictures;
            model.UserId = id;
            model.Username = alias;

            // impossible to get the path to the original picture...
            // the path is not predictable :'(
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

                // search company's picture path
                string remark = "user";
                var filename = format.FilenameMaker(item);
                var pathArray = ArrayCombine(basePath, filename);
                var path = provider.EnsureFilePath(pathArray);
                if (!(found = File.Exists(path)))
                {
                    // defaut picture for network
                    remark = "network default";
                    path = Path.Combine(networkCompaniesPath, "Default-" + format.Name + ".png");
                    if (!(found = File.Exists(path)))
                    {
                        // defaut picture
                        remark = "default";
                        path = Path.Combine(commonCompaniesPath, "Default-" + format.Name + ".png");
                        if (!(found = File.Exists(path)))
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
                        var bytes256 = bytes.Length > 600
                                     ? bytes.Take(600).ToArray()
                                     : bytes;
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

        public SetProfilePictureResult SetProfilePicture(SetProfilePictureRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (request.PictureStream == null)
                throw new ArgumentNullException("request");
            if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory))
                throw new InvalidOperationException("Configuration entry 'Storage.UserContentsDirectory' must have a valid value");

            var result = new SetProfilePictureResult(request);
            var item = this.Repo.Companies.GetById(request.UserId);
            IFilesystemProvider provider = new IOFilesystemProvider();
            PictureTransformer transformer = new PictureTransformer();

            string[] basePath = new string[]
            {
                this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory,
                "Networks",
                this.Services.Network.Name,
                "Companies",
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
                    this.Services.Logger.Error("CompanyServices.SetProfilePicture", ErrorLevel.Business, ex.ToString());
                    result.Errors.Add(SetProfilePictureError.FileIsNotPicture, NetworksEnumMessages.ResourceManager);
                    return result;
                }

                var filename = format.FilenameMaker(item);
                var pathArray = ArrayCombine(basePath, filename);
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
                    "Companies",
                    "Uploads",
                };
                const string uploadExtension = ".jpg";
                var name = item.Alias.GetIncrementedString(s => !provider.FileExists(provider.EnsureFilePath(ArrayCombine(uploadPath, s + uploadExtension))));
                provider.WriteNewFile(provider.EnsureFilePath(ArrayCombine(uploadPath, name + uploadExtension)), request.PictureStream);
            }

            this.Services.Logger.Info("CompanyServices", ErrorLevel.Success, "Picture for company " + item.Alias + " has been updated");
            result.Succeed = true;
            return result;
        }

        private DateTime GetProfilePictureLastChangeDate(string alias)
        {
            if (string.IsNullOrEmpty(alias))
                throw new ArgumentException("The value cannot be empty", "username");

            var item = new Company
            {
                Alias = alias,
            };

            string rootPath = this.Services.AppConfiguration.Tree.Storage.UserContentsDirectory;
            if (string.IsNullOrEmpty(rootPath))
            {
                Trace.TraceError("CompanyService.GetProfilePictureLastChangeDate: AppConfiguration.Tree.Storage.UserContentsDirectory is empty!");
            }

            string[] basePath = new string[]
            {
                rootPath,
                "Networks",
                this.Services.Network.Name,
                "Companies",
            };
            var commonCompaniesPath = Path.Combine(rootPath, "Networks", "Common", "Companies");
            var networkCompaniesPath = Path.Combine(rootPath, "Networks", this.Services.Network.Name, "Companies");

            IFilesystemProvider provider = new IOFilesystemProvider();

            var formats = pictureFormats;
            for (int i = 0; i < formats.Length; i++)
            {
                var format = formats[i];

                // search company's picture path
                var filename = format.FilenameMaker(item);
                var pathArray = ArrayCombine(basePath, filename);
                var path = provider.EnsureFilePath(pathArray);
                if (File.Exists(path))
                {
                    return File.GetLastWriteTimeUtc(path);
                }
            }

            return DateTime.MinValue;
        }

        private T[] ArrayCombine<T>(T[] array1, T array2)
        {
            // this has moved to srktoolkit
            var result = new T[array1.Length + 1];
            Array.Copy(array1, 0, result, 0, array1.Length);
            result[array1.Length] = array2;
            return result;
        }

        private T[] ArrayCombine<T>(T[] array1, T[] array2)
        {
            // this has moved to srktoolkit
            var result = new T[array1.Length + array2.Length];
            Array.Copy(array1, 0, result, 0, array1.Length);
            Array.Copy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }

        public bool IsActive(Company company)
        {
            return company.IsApproved && company.IsEnabled;
        }

        public IDictionary<int, CompanyModel> GetByIdFromAnyNetwork(IList<int> ids, CompanyOptions options)
        {
            var items = this.Repo.Companies.GetById(ids, options)
                .Select(c =>
                {
                    var model = new CompanyModel(c.Value);
                    model.IsActive = this.IsActive(c.Value);
                    return model;
                })
                .ToDictionary(c => c.Id, c => c);

            return items;
        }

        public CompanyModel GetByIdFromAnyNetwork(int ids, CompanyOptions options)
        {
            var item = this.Repo.Companies.GetById(ids, options);
            var model = new CompanyModel(item);
            model.IsActive = this.IsActive(item);
            return model;
        }

        public IList<CompanyModel> GetAllActiveWithModel()
        {
            return this.Repo
                .Companies
                .GetAllActive(this.Services.NetworkId)
                .Select(o => new CompanyModel(o))
                .ToList();
        }

        private object IsActive(KeyValuePair<int, CompanyModel> item)
        {
            throw new NotImplementedException();
        }

        public string GetProfileUrl(Company company, UriKind uriKind)
        {
            var path = "Company/" + Uri.EscapeUriString(company.Alias);
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetProfileUrl(string companyAlias, UriKind uriKind)
        {
            var path = "Company/" + Uri.EscapeUriString(companyAlias);
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public string GetProfilePictureUrl(Company company, Services.Networks.Companies.CompanyPictureSize pictureSize, UriKind uriKind)
        {
            return this.GetProfilePictureUrl(company.Alias, pictureSize, uriKind);
        }

        public string GetProfilePictureUrl(string alias, CompanyPictureSize pictureSize, UriKind uriKind)
        {
            var path = string.Format(
                "Data/CompanyPicture/{0}/{1}/{2}",
                alias,
                pictureSize.ToString(),
                this.GetProfilePictureLastChangeDate(alias).ToString(PictureAccess.CacheDateFormat));
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public IList<CompanyExtended> GetEnabledWithStats()
        {
            return this.Repo.Companies
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .Enabled()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Where(u => u.CompanyAccessLevel > 0 && u.NetworkAccessLevel > 0).Count(),
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();
        }

        public IList<CompanyExtended> GetDisabledWithStats()
        {
            return this.Repo.Companies
                .Select()
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .Disabled()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyExtended
                {
                    Company = c,
                    PeopleCount = c.Users.Where(u => u.CompanyAccessLevel > 0 && u.NetworkAccessLevel > 0).Count(),
                    InvitedCount = c.Inviteds.Where(i => i.User == null).Count(),
                    NetworkName = c.Network.Name,
                })
                .ToList();
        }

        public Services.Networks.Companies.ToggleCompanyRequest GetToggleCompanyRequest(string alias)
        {
            var model = new Sparkle.Services.Networks.Companies.ToggleCompanyRequest();
            var company = this.Services.Company.GetByAlias(alias);

            if (company.IsEnabledLastChangeUserId.HasValue)
            {
                model.LastChangeUserFirstName = company.IsEnabledLastChangeUser.FirstName;
                model.LastChangeUserFullName = company.IsEnabledLastChangeUser.FullName;
                model.LastChangeDateUtc = company.IsEnabledLastChangeDateUtc;
            }
            else if (company.IsEnabledFirstChangeUserId.HasValue)
            {
                model.LastChangeUserFirstName = company.IsEnabledFirstChangeUser.FirstName;
                model.LastChangeUserFullName = company.IsEnabledFirstChangeUser.FullName;
                model.LastChangeDateUtc = company.IsEnabledFirstChangeDateUtc;
            }

            model.CompanyName = company.Name;
            model.CompanyAlias = company.Alias;
            model.CompanyPictureUrl = this.Services.Company.GetProfilePictureUrl(company, Sparkle.Services.Networks.Companies.CompanyPictureSize.Medium, UriKind.Relative);
            model.IsEnabled = company.IsEnabled;
            model.IsEnabledRemark = company.IsEnabledRemark;

            return model;
        }

        public ToggleCompanyResult ToggleCompany(ToggleCompanyRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "CompanyService.ToggleCompany";
            var result = new ToggleCompanyResult(request);

            var company = this.Services.Company.GetByAlias(request.CompanyAlias);
            if (company == null)
            {
                result.Errors.Add(ToggleCompanyError.NoSuchCompany, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var user = this.Services.People.SelectWithId(request.CurrentUserId);
            if (user == null || !user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ManageCompany, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff))
            {
                result.Errors.Add(ToggleCompanyError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            if (request.IsEnabled != company.IsEnabled)
            {
                if (request.IsEnabled)
                    result.Errors.Add(ToggleCompanyError.AlreadyEnabled, NetworksEnumMessages.ResourceManager);
                else
                    result.Errors.Add(ToggleCompanyError.AlreadyDisabled, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            company.IsEnabled = !request.IsEnabled;
            company.IsEnabledRemark = request.IsEnabledRemark;
            if (!company.IsEnabledFirstChangeUserId.HasValue)
            {
                company.IsEnabledFirstChangeDateUtc = DateTime.UtcNow;
                company.IsEnabledFirstChangeUserId = user.Id;
            }
            else
            {
                company.IsEnabledLastChangeDateUtc = DateTime.UtcNow;
                company.IsEnabledLastChangeUserId = user.Id;
            }

            this.Services.Company.Update(company);

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public ApplyCompanyModel VerifyEmailDomainForApply(string email, string search, Guid? key)
        {
            var model = new ApplyCompanyModel();
            EmailAddress address = null;

            IList<CompanyPoco> companies;
            if (string.IsNullOrEmpty(search))
            {
                //companies = this.GetActiveLight();
                companies = new List<CompanyPoco>();
            }
            else
            {
                companies = this.SearchActiveLight(search);
            }

            model.Companies = companies.OrderBy(o => o.Name).Select(c => new CompanyModel(c)).ToList();

            if ((address = EmailAddress.TryCreate(email)) != null)
            {
                var domainMatch = this.SelectByDomainName(address.DomainPart);
                if (domainMatch != null)
                {
                    model.DomainNameMatch = new CompanyModel(domainMatch);
                }
            }

            if (key.HasValue)
            {
                var applyRequest = this.Repo.ApplyRequests.GetByKey(key.Value, this.Services.NetworkId);
                if (applyRequest != null)
                {
                    var applyModel = new ApplyRequestModel(applyRequest);
                    var lInCompanies = applyModel.CompanyDataModel.LinkedInCompanies;
                    if (lInCompanies != null)
                    {
                        model.LinkedInCompanies = lInCompanies.OrderBy(o => o.Company.Name).ToDictionary(o => o.Identifier, o => new CompanyModel(o.Company));
                    }
                }
            }

            return model;
        }

        public CreateCompanyRequest GetCreateRequest(CreateCompanyRequest request, int? userId)
        {
            if (request == null)
            {
                request = new CreateCompanyRequest();
            }

            if (userId.HasValue)
            {
                var user = this.Services.People.SelectWithId(userId.Value);
                if (this.Services.People.IsActive(user))
                {
                    request.CanSetEmailDomain = user.NetworkAccess.HasFlag(NetworkAccessLevel.AddCompany);
                    request.CanApprove = user.NetworkAccess.HasFlag(NetworkAccessLevel.AddCompany) && !request.CompanyRequestId.HasValue;
                    request.IsAuthor = user.NetworkAccess.HasFlag(NetworkAccessLevel.AddCompany);
                }
            }
            else
            {
                request.CanSetEmailDomain = false;
                request.CanApprove = false;
                request.IsAuthor = true;
            }

            request.UserId = userId;
            request.AvailableCategories = this.Services.Company.GetAllCategories();

            return request;
        }

        public CreateCompanyResult ApplyCreateRequest(CreateCompanyRequest request, bool isApproved)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new CreateCompanyResult(request);

            const string logPath = "CompanyService.ApplyCreateRequest";
            var transaction = this.Services.NewTransaction();
            ////var dataTran = transaction.Repositories.BeginTransaction();
            using (transaction.BeginTransaction())
            {
                // alias
                var alias = transaction.Services.Company.MakeAlias(request.Name);

                // email domain
                string emailDomain = null;
                if (!string.IsNullOrWhiteSpace(request.Name) && !string.IsNullOrWhiteSpace(request.Email))
                {
                    emailDomain = transaction.Services.Company.FindEmailDomainFromNameAndEmail(request.Name, request.Email);
                }

                if (!string.IsNullOrWhiteSpace(request.EmailDomain) && request.CanSetEmailDomain)
                {
                    emailDomain = request.EmailDomain;
                }

                if (isApproved)
                {
                    var company = new Company();

                    company.Name = request.Name;
                    company.Baseline = request.Baseline;
                    company.CategoryId = (short)request.CategoryId;
                    company.CreatedDateUtc = DateTime.UtcNow;
                    company.ApprovedDateUtc = DateTime.UtcNow;
                    company.NetworkId = transaction.Services.NetworkId;
                    company.IsApproved = true;
                    company.IsEnabled = true;
                    company.Alias = transaction.Services.Company.MakeAlias(request.Name);
                    company.EmailDomain = emailDomain;

                    if (transaction.Repositories.Companies.GetIdByAlias(company.Alias) != null)
                    {
                        company.Alias = company.Alias.GetIncrementedString(s => transaction.Repositories.Companies.GetIdByAlias(s) == null);
                    }

                    company = transaction.Repositories.Companies.Insert(company);

                    // TODO: prefer a mass-execution method
                    // TODO: prefer AppendCompanyProfileField, we don't need to overwrite non-existent fields!
                    var source = ProfileFieldSource.UserInput;
                    transaction.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Site, request.Website, source);
                    transaction.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Phone, request.Phone, source);
                    transaction.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.About, request.About, source);
                    transaction.Services.ProfileFields.SetCompanyProfileField(company.ID, ProfileFieldType.Email, request.Email, source);

                    transaction.Services.Logger.Info(
                        "CompanyService.ApplyCreateRequestApproved",
                        ErrorLevel.Success,
                        "Create approved {0}",
                        company.ID);

                    // admin emails
                    if (request.UserId.HasValue)
                    {
                        var inviter = transaction.Services.People.SelectWithId(request.UserId.Value);

                        var splitAdmin = string.IsNullOrEmpty(request.AdminEmails) ? new string[0] : Validate.ManyEmailAddresses(request.AdminEmails ?? string.Empty).ToArray();
                        var splitUsers = string.IsNullOrEmpty(request.OtherEmails) ? new string[0] : Validate.ManyEmailAddresses(request.OtherEmails ?? string.Empty).ToArray();
                        if (!request.IsFromApplyRequest)
                            transaction.PostSaveActions.Add(t => t.Services.Email.SendCompanyRegisteredNotification(company, inviter, request.Email, splitAdmin, splitUsers));

                        // invite people
                        foreach (var email in splitAdmin)
                        {
                            transaction.PostSaveActions.Add(t => t.Services.Invited.Invite(inviter, email, company.ID, CompanyAccessLevel.Administrator));
                        }

                        foreach (var email in splitUsers)
                        {
                            transaction.PostSaveActions.Add(t => t.Services.Invited.Invite(inviter, email, company.ID, CompanyAccessLevel.User));
                        }

                        transaction.PostSaveActions.Add(t => t.Services.Wall.AddCompanyJoinItem(company, inviter.Id));
                    }

                    result.Item = company;
                }
                else
                {
                    CompanyRequest item = null;
                    if (request.CreateRequestUniqueId.HasValue)
                        item = transaction.Services.Company.GetRequest(request.CreateRequestUniqueId.Value);
                    if (item == null)
                    {
                        item = new CompanyRequest();
                        item.UniqueId = Guid.NewGuid();
                        item.Approved = null;
                        item.ApprovedReason = null;
                        item.CreatedDateUtc = DateTime.UtcNow;
                        item.NetworkId = transaction.Services.NetworkId;
                    }

                    item.Name = request.Name;
                    item.Alias = alias;
                    item.Baseline = request.Baseline;
                    item.About = request.About;
                    item.CategoryId = request.CategoryId;
                    item.Phone = request.Phone;
                    item.Email = request.Email;
                    item.Website = request.Website;
                    item.EmailDomain = request.EmailDomain;
                    item.IndoorId = 0;
                    item.NetworkId = transaction.Services.NetworkId;
                    // TODO: split-join that
                    item.AdminEmails = request.AdminEmails;
                    item.OtherEmails = request.OtherEmails;

                    if (item.Id != 0)
                    {
                        item = transaction.Repositories.CompanyRequests.Update(item);
                        transaction.Services.Logger.Info("CompanyService.CreateForApproval", ErrorLevel.Success, "CreateForApproval updated CompanyRequest {0}", item.Id);
                    }
                    else
                    {
                        item = transaction.Repositories.CompanyRequests.Insert(item);
                        transaction.Services.Logger.Info("CompanyService.CreateForApproval", ErrorLevel.Success, "CreateForApproval inserted CompanyRequest {0}", item.Id);

                        var recipients = transaction.Services.People.GetByNetworkAccessLevel(NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.AddCompany);

                        transaction.PostSaveActions.Add(t => t.Services.Email.SendNewCompanyDetailsForApproval(item, recipients));
                        transaction.PostSaveActions.Add(t => t.Services.Email.SendCompanyRequestConfirmation(item, item.Email));
                    }
                }

                transaction.CompleteTransaction();
            }

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public void UpdateFieldsFromLinkedInCompany(LinkedInCompanyResult result, LinkedInNET.Companies.Company company, bool saveFields)
        {
            int entityChanges = 0;

            // name
            if (!string.IsNullOrEmpty(company.Name) && (!saveFields || result.CompanyEntity.Name != company.Name))
            {
                result.CompanyEntity.Name = company.Name;
                entityChanges++;
            }

            // about
            if (!string.IsNullOrEmpty(company.Description) && (!saveFields || result.CompanyEntity.ProfileFields.About() != company.Description))
            {
                this.UpdateSingleField(result, result.CompanyEntity, company.Description.TrimTextRight(4000), ProfileFieldType.About, ProfileFieldSource.LinkedIn, saveFields);
            }

            // site
            if (!string.IsNullOrEmpty(company.WebsiteUrl) && (!saveFields || result.CompanyEntity.ProfileFields.Site() != company.WebsiteUrl))
            {
                this.UpdateSingleField(result, result.CompanyEntity, company.WebsiteUrl, ProfileFieldType.Site, ProfileFieldSource.LinkedIn, saveFields);
            }

            // twitter
            if (!string.IsNullOrEmpty(company.TwitterId) && (!saveFields || result.CompanyEntity.ProfileFields.Twitter() != company.TwitterId))
            {
                this.UpdateSingleField(result, result.CompanyEntity, company.TwitterId, ProfileFieldType.Twitter, ProfileFieldSource.LinkedIn, saveFields);
            }

            // industries
            {
                var actualItems = this.Repo.CompanyProfileFields.GetManyByUserIdAndFieldType(result.CompanyEntity.ID, ProfileFieldType.Industry);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => uf);
                var linkedItems = company.Industries != null && company.Industries.Industry != null ? company.Industries.Industry : Enumerable.Empty<LinkedInNET.Companies.CompanyIndustry>();
                foreach (var linkedItem in linkedItems)
                {
                    CompanyProfileField entity;
                    var localItem = actualModels.Values.Where(m => m.Value == linkedItem.Name).SingleOrDefault();
                    if (localItem != null)
                    {
                        // OKAY
                    }
                    else
                    {
                        // insert
                        entity = new CompanyProfileField
                        {
                            DateCreatedUtc = DateTime.UtcNow,
                            ProfileFieldType = ProfileFieldType.Industry,
                            SourceType = ProfileFieldSource.LinkedIn,
                            CompanyId = result.CompanyEntity.ID,
                            Value = linkedItem.Name,
                        };
                        if (saveFields)
                            this.Repo.CompanyProfileFields.Insert(entity);
                        result.Changes.Add(new CompanyFieldUpdate(new CompanyProfileFieldModel(entity), ProfileFieldChange.Create));
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.SourceType == ProfileFieldSource.LinkedIn && localItem.ProfileFieldType == ProfileFieldType.Industry)
                    {
                        if (!linkedItems.Where(i => i.Name == localItem.Value).Any())
                        {
                            if (saveFields)
                                this.Repo.CompanyProfileFields.Delete(localItem);
                            result.Changes.Add(new CompanyFieldUpdate(new CompanyProfileFieldModel(localItem), ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            // locations
            {
                var actualItems = this.Repo.CompanyProfileFields.GetManyByUserIdAndFieldType(result.CompanyEntity.ID, ProfileFieldType.Location);
                var actualModels = actualItems.ToDictionary(
                    uf => uf.Id,
                    uf => new
                    {
                        Entity = uf,
                        Model = new CompanyProfileFieldModel(uf),
                    });
                var linkedItems = company.Locations != null && company.Locations.Location != null ? company.Locations.Location : Enumerable.Empty<LinkedInNET.Companies.CompanyLocation>();
                foreach (var linkedItem in linkedItems)
                {
                    CompanyProfileField entity;
                    LocationProfileFieldModel modelItem;
                    var localItem = actualModels.Values.Where(m => m.Model.LocationModel.Description == linkedItem.Description).SingleOrDefault();
                    if (localItem != null)
                    {
                        // OKAY
                    }
                    else
                    {
                        // insert
                        modelItem = new LocationProfileFieldModel();
                        modelItem.UpdateFrom(linkedItem);
                        var model = new CompanyProfileFieldModel(ProfileFieldType.Location);
                        model.LocationModel = modelItem;
                        if (model.LocationModel != null && model.Value != null)
                        {
                            entity = new CompanyProfileField
                            {
                                Data = model.Data,
                                DateCreatedUtc = DateTime.UtcNow,
                                ProfileFieldType = ProfileFieldType.Location,
                                SourceType = ProfileFieldSource.LinkedIn,
                                CompanyId = result.CompanyEntity.ID,
                                Value = model.Value,
                            };
                            if (saveFields)
                                this.Repo.CompanyProfileFields.Insert(entity);
                            result.Changes.Add(new CompanyFieldUpdate(new CompanyProfileFieldModel(entity), ProfileFieldChange.Create));
                        }
                    }
                }

                // delete items that WERE in LinkedIn
                foreach (var localItem in actualModels.Values)
                {
                    if (localItem.Model.Source == ProfileFieldSource.LinkedIn && localItem.Model.Type == ProfileFieldType.Location)
                    {
                        if (localItem.Model.LocationModel.Description == null || !linkedItems.Any(i => i.Description == localItem.Model.LocationModel.Description))
                        {
                            if (saveFields)
                                this.Repo.CompanyProfileFields.Delete(localItem.Entity);
                            result.Changes.Add(new CompanyFieldUpdate(localItem.Model, ProfileFieldChange.Delete));
                        }
                    }
                }
            }

            result.EntityChanges = entityChanges;
        }

        private void UpdateSingleField(LinkedInCompanyResult result, Company company, string distantValue, ProfileFieldType fieldType, ProfileFieldSource source, bool live)
        {
            var localValue = live ? company.ProfileFields.SingleByType(fieldType) : null;
            if (distantValue != null && (localValue == null || localValue.Value != distantValue))
            {
                CompanyProfileFieldModel model;
                if (live)
                {
                    model = this.Services.ProfileFields.SetCompanyProfileField(company.ID, fieldType, distantValue, source);
                }
                else
                {
                    model = new CompanyProfileFieldModel(company.ID, fieldType, distantValue, source);
                }

                result.Changes.Add(new CompanyFieldUpdate(model, ProfileFieldChange.CreateOrUpdate));
            }
        }

        public IList<UserModel> GetActiveUsersByAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options)
        {
            return this.Repo.People.GetActiveByCompanyAndAccessLevel(companyId, accessLevel, options)
                .Select(u => new UserModel(u))
                .ToList();
        }

        public IList<UserModel> GetAllUsersByAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options)
        {
            return this.Repo.People.GetAllByCompanyAndAccessLevel(companyId, accessLevel, options)
                .Select(u => new UserModel(u))
                .ToList();
        }

        public IList<AdminWorkModel> GetCompanyAccessLevelIssues()
        {
            var items = this.Repo.Companies.GetCompaniesAccessLevelReport(this.Services.NetworkId);
            var models = items
                .Where(r => r.Level3 == 0 && (r.Level1 > 0 || r.Level2 > 0))
                .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Low, AdminWorkType.CompanyHasNoAdministrator))
                .ToList();

            var registerInCompany = this.Services.AppConfiguration.Tree.Features.Users.RegisterInCompany;
            if (registerInCompany != null)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    if (models[i].IntId == registerInCompany)
                    {
                        models.RemoveAt(i--);
                    }
                }
            }

            return models;
        }

        public IList<ApplyRequestModel> GetPendingApplyRequests()
        {
            var items = this.Repo.ApplyRequests.GetPendingWithCompanyCreate(this.Services.NetworkId)
                .Select(r => new ApplyRequestModel(r))
                .ToList();
            return items;
        }

        public IList<ApplyRequestModel> GetApplyRequestsWithCompanyId(int companyId)
        {
            return this.Repo
                .ApplyRequests
                .GetByJoinCompanyId(this.Services.NetworkId, companyId)
                .Select(o => new ApplyRequestModel(o))
                .ToList();
        }

        public CompanyCategoryModel GetCategoryByName(string name)
        {
            var category = this.Repo
                .CompanyCategories
                .GetByName(name);

            return category != null ? new CompanyCategoryModel(category) : null;
        }

        public IList<CompanyCategoryModel> GetCategoriesForDashboard()
        {
            var items = this.Repo.CompanyCategories.GetAll(this.Services.NetworkId);
            var activCompanyUsing = this.Repo.CompanyCategories.GetActiveCompaniesUsingCount();
            var inactivCompanyUsing = this.Repo.CompanyCategories.GetInactiveCompaniesUsingCount();

            return items.Select(o => new CompanyCategoryModel(o)
            {
                UsedByActivCount = activCompanyUsing.ContainsKey(o.Id) ? activCompanyUsing[o.Id] : 0,
                UsedByInactivCount = inactivCompanyUsing.ContainsKey(o.Id) ? inactivCompanyUsing[o.Id] : 0,
            }).ToList();
        }

        public void SetDefaultCategory(short categoryId)
        {
            this.Repo.CompanyCategories.SetDefaultCategory(categoryId, this.Services.NetworkId);
        }

        public EditCompanyCategoryRequest GetEditCompanyCategoryRequest(short? categoryId)
        {
            CompanyCategory item = null;
            if (categoryId.HasValue)
                item = this.Repo.CompanyCategories.GetById(categoryId.Value);

            return new EditCompanyCategoryRequest(item);
        }

        public EditCompanyCategoryResult UpdateCompanyCategory(EditCompanyCategoryRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            const string logPath = "CompanyService.UpdateCompanyCategory";
            var result = new EditCompanyCategoryResult(request);

            CompanyCategory item = null;
            if (request.Id.HasValue)
                item = this.Repo.CompanyCategories.GetById(request.Id.Value);

            if (item == null)
            {
                item = new CompanyCategory();
                item.NetworkId = this.Services.NetworkId;
                item.IsDefault = false;
                item.Name = request.Name;
                item.Alias = this.MakeAlias(item.Name);
                item.KnownCategoryValue = request.KnownCategory;

                this.Repo.CompanyCategories.Insert(item);
            }
            else
            {
                item.Name = request.Name;
                item.KnownCategoryValue = request.KnownCategory;

                this.Repo.CompanyCategories.Update(item);
            }

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public CompanyCategoryModel GetCategoryByAlias(string alias)
        {
            var item = this.Repo.CompanyCategories.GetByAlias(alias, this.Services.NetworkId);
            if (item == null)
                return null;

            return new CompanyCategoryModel(item);
        }

        public CompanyCategoryModel GetDefaultCategory()
        {
            var item = this.Repo.CompanyCategories.GetDefault(this.Services.NetworkId);
            if (item == null)
            {
                item = this.Repo.CompanyCategories.GetAny(this.Services.NetworkId);
            }

            if (item == null)
                return null;

            return new CompanyCategoryModel(item);
        }

        public CompanyCategoryModel GetCategoryById(short id)
        {
            var item = this.Repo.CompanyCategories.GetById(id);
            if (item == null)
                return null;

            return new CompanyCategoryModel(item);
        }

        public IDictionary<short, CompanyCategoryModel> GetCategoryById(short[] ids)
        {
            return this.Repo.CompanyCategories.GetById(ids)
                .ToDictionary(x => x.Key, x => new CompanyCategoryModel(x.Value));
        }

        public IList<CompanyPlaceModel> GetPlacesFromCompanyId(int companyId)
        {
            var items = this.Repo.CompanyPlaces.GetByCompanyId(companyId);
            if (items.Count == 0)
                return new List<CompanyPlaceModel>();

            var placeIds = items.Select(o => o.PlaceId).Distinct().ToArray();
            var places = this.Services.Cache.GetPlaces(placeIds);

            return items.Select(o => new CompanyPlaceModel(o, places.ContainsKey(o.PlaceId) ? places[o.PlaceId] : null, null)).ToList();
        }

        public IList<CompanyPlaceModel> GetCompaniesAtPlace(int placeId)
        {
            var items = this.Repo.CompanyPlaces.GetByPlaceId(placeId);
            ////var places = this.Services.Cache.GetPlaces(items.Select(cp => cp.PlaceId).ToArray());
            var companyIds = items.Select(cp => cp.CompanyId).ToArray();
            var companies = this.Services.Company.GetByIdFromAnyNetwork(companyIds, CompanyOptions.None);

            foreach (var company in companies.Values)
            {
                company.LargeProfilePictureUrl = this.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Large, UriKind.Relative);
                company.MediumProfilePictureUrl = this.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Medium, UriKind.Relative);
                company.SmallProfilePictureUrl = this.GetProfilePictureUrl(company.Alias, CompanyPictureSize.Small, UriKind.Relative);
                company.PictureUrl = company.MediumProfilePictureUrl;
            }

            return items.Select(cp => new CompanyPlaceModel(cp, null, companies[cp.CompanyId])).ToList();
        }

        public IList<CompanyModel> GetCompaniesNearLocation(string[] geocodes)
        {
            var placesNearBy = this.Repo.Places.GetPlacesByRadius(this.Services.NetworkId, geocodes, 30, false);
            var companyPlaces = this.Repo.CompanyPlaces.GetByPlaceId(placesNearBy.Select(o => o.Id).ToArray());

            return this.Repo.Companies.GetById(companyPlaces.Select(o => o.CompanyId).Distinct().ToArray()).Select(o => new CompanyModel(o)).ToList();
        }

        public void AddCompanyPlaceFromCompanyProfileField(string logPath, Company company, ICompanyProfileFieldValue item)
        {
            var model = new CompanyProfileFieldModel(item);

            var name = model.LocationModel.IsHeadquarters ? this.Services.Lang.T("{0} (Siège social)", company.Name) : this.Services.Lang.T("{0} (Bureaux)", company.Name);
            name = name.TrimTextRight(46);

            var address = model.LocationModel.Street1;
            if (!string.IsNullOrEmpty(model.LocationModel.Street2))
                address += ", " + model.LocationModel.Street2;

            string country = null;
            if (!string.IsNullOrEmpty(model.LocationModel.CountryCode))
            {
                RegionInfo region = null;
                try
                {
                    if (model.LocationModel.CountryCode.Length == 2)
                    {
                        region = new RegionInfo(model.LocationModel.CountryCode);
                    }
                    else if (model.LocationModel.CountryCode.Length == 3)
                    {
                        // this region provider fails to give a region in the correct culture
                        // example: France is sometimes given as Frãns
                        region = this.RegionInfos.First(r => r.ThreeLetterISORegionName.Equals(model.LocationModel.CountryCode, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    if (region == null)
                    {
                        // this ctor crashes when it is given a 3 letter country code
                        region = new RegionInfo(model.LocationModel.CountryCode);
                    }

                    country = region.NativeName;
                }
                catch (ArgumentException ex)
                {
                    this.Services.Logger.Warning(logPath, ErrorLevel.Internal, ex);
                }
            }

            var place = new Place
            {
                CategoryId = this.Repo.PlacesCategories.GetDefaultPlaceCategory().Id,
                Name = name,
                Alias = this.Services.Places.MakeAlias(name),
                Description = !string.IsNullOrEmpty(model.LocationModel.Description) ? model.LocationModel.Description.TrimTextRight(4000) : null,
                Address = !string.IsNullOrEmpty(address) ? address.TrimTextRight(50) : null,
                ZipCode = !string.IsNullOrEmpty(model.LocationModel.PostalCode) ? model.LocationModel.PostalCode.TrimTextRight(10) : null,
                City = !string.IsNullOrEmpty(model.LocationModel.City) ? model.LocationModel.City.TrimTextRight(50) : null,
                Country = !string.IsNullOrEmpty(country) ? country : null,
                Phone = !string.IsNullOrEmpty(model.LocationModel.Phone1) ? model.LocationModel.Phone1.TrimTextRight(20) : !string.IsNullOrEmpty(model.LocationModel.Phone2) ? model.LocationModel.Phone2.TrimTextRight(20) : null,
                CreatedByUserId = 1, // kevin.alexandre
                NetworkId = this.Services.NetworkId,
            };

            place = this.Repo.Places.Insert(place);
            this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created " + place.ToString() + ".");

            this.Services.Places.TryGeocodePlace(logPath, new PlaceModel(place));

            var companyPlace = new CompanyPlace
            {
                CompanyId = company.ID,
                PlaceId = place.Id,
                DateCreatedUtc = DateTime.UtcNow,
                CreatedByUserId = 1, // kevin.alexandre
            };

            companyPlace = this.Repo.CompanyPlaces.Insert(companyPlace);
            this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created " + companyPlace.ToString() + ".");
        }

        public AjaxTagPickerModel GetAjaxTagPickerModel(int companyId, int actingUserId)
        {
            var company = this.GetById(companyId);
            var user = this.Services.People.GetActiveById(actingUserId, PersonOptions.Company);
            if (company == null || user == null)
                return null;

            var isAcompanyAdmin = user.CompanyAccessLevel.HasValue && user.CompanyAccessLevel.Value == CompanyAccessLevel.Administrator && user.CompanyId == company.ID;
            var isNetworkAdmin = user.NetworkAccessLevel.HasValue && (user.NetworkAccessLevel.Value.HasFlag(NetworkAccessLevel.NetworkAdmin) || user.NetworkAccessLevel.Value.HasFlag(NetworkAccessLevel.SparkleStaff));

            var categories = this.Services.Tags.GetCategoriesApplyingToCompanies();
            var tags = this.Services.Tags.GetTagsByCompanyIdAndCategories(company.ID, categories);
            return new AjaxTagPickerModel(tags, isAcompanyAdmin || isNetworkAdmin);
        }

        public CompanyListModel Search(string keywords, string location, int[] tagIds, bool combinedTags, int offset, int count, CompanyOptions options)
        {
            var splittedKeywords = keywords != null ? keywords.Split(new char[] { ' ', '\t', '\r', '\n', }).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray() : null;

            string[] geocodes = null;
            if (location != null)
            {
                var geoloc = this.GetGeolocFromSparkleStatus(location);
                if (geoloc != null)
                {
                    geocodes = geoloc.Geocodes;
                    // TODO: put the found locations in the result model
                    // TODO: handle errors
                }
            }

            var results = this.Repo.Companies.Search(new int[] { this.Services.NetworkId, }, splittedKeywords, geocodes, tagIds, combinedTags, offset, count, options);
            var result = new CompanyListModel
            {
                Offset = offset,
                Count = count,
                Total = results.Count,
                LocationGeocodes = geocodes,
            };

            results = results.Skip(offset).Take(count).ToList();
            var entities = this.Repo.Companies.GetById(results.Select(x => x.Id).ToList(), options);
            result.Items = new List<CompanyModel>(results.Count);

            for (int i = 0; i < results.Count; i++)
            {
                var company = entities[results[i].Id];
                var model = new CompanyModel(company);
                result.Items.Add(model);
            }

            var companyIds = results.Select(x => x.Id).ToArray();
            if ((options & CompanyOptions.Places) == CompanyOptions.Places)
            {
                var companyPlaces = this.Repo.CompanyPlaces.GetByCompanyId(companyIds);
                var placeIds = companyPlaces.Select(x => x.PlaceId).ToArray();
                var places = this.Services.Places.GetById(placeIds, PlaceOptions.None);

                foreach (var company in result.Items)
                {
                    foreach (var rel in companyPlaces.Where(x => x.CompanyId == company.Id))
                    {
                        if (places.ContainsKey(rel.PlaceId))
                        {
                            if (company.Places == null)
                                company.Places = new List<PlaceModel>();
                            company.Places.Add(places[rel.PlaceId]);
                        }
                    }
                }
            }

            if ((options & CompanyOptions.Tags) == CompanyOptions.Tags)
            {
                var rels = this.Repo.CompanyTags.GetByCompanyId(companyIds);
                var categories = this.Services.Tags.GetCategories().ToDictionary(x => x.Id, x => x);
                var tags = this.Repo.TagDefinitions.GetByIds(this.Services.NetworkId, rels.Select(x => x.TagId).ToArray())
                    .ToDictionary(x => x.Id, x => new Tag2Model(x, categories[x.CategoryId]));

                foreach (var company in result.Items)
                {
                    foreach (var rel in rels.Where(x => x.CompanyId == company.Id))
                    {
                        if (tags.ContainsKey(rel.TagId))
                        {
                            if (company.Tags == null)
                                company.Tags = new List<Tag2Model>();
                            company.Tags.Add(tags[rel.TagId]);
                        }
                    }
                }
            }

            return result;
        }

        public EditProfileFieldsRequest GetEditCompanyFieldsRequest(int? companyId, EditProfileFieldsRequest request)
        {
            if (request == null)
            {
                request = new EditProfileFieldsRequest();
                request.EntityType = "Company";
            }

            CompanyModel company;
            if (companyId != null)
            {
                company = this.GetByIdFromAnyNetwork(companyId.Value, CompanyOptions.None);
            }
            else
            {
                company = this.GetByIdFromAnyNetwork(request.EntityId, CompanyOptions.None);
            }

            if (company == null)
                return null;

            var fields = this.Services.ProfileFields.GetCompanyFields(true);
            request.Fields = fields;

            if (request.Values == null)
            {
                var values = this.Services.ProfileFields.GetCompanyValues(company.Id);
                request.Values = values;
            }

            return request;
        }

        public EditProfileFieldsResult EditCompanyFields(EditProfileFieldsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditProfileFieldsResult(request);

            using (var tran = this.Services.NewTransaction())
            using (tran.BeginTransaction())
            {
                var user = tran.Services.People.GetByIdFromAnyNetwork(request.ActingUserId, PersonOptions.Company);
                if (user == null)
                {
                    result.Errors.Add(EditProfileFieldsError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
                }

                var company = tran.Services.Company.GetByIdFromAnyNetwork(request.EntityId, CompanyOptions.None);
                if (company == null)
                {
                    result.Errors.Add(EditProfileFieldsError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                }

                var networkFlags = new NetworkAccessLevel[]
                {
                    NetworkAccessLevel.NetworkAdmin,
                    NetworkAccessLevel.ModerateNetwork,
                    NetworkAccessLevel.ManageCompany,
                };
                    var companyFlags = new CompanyAccessLevel[]
                {
                    CompanyAccessLevel.CommunityManager,
                    CompanyAccessLevel.Administrator,
                };
                var canEdit = user != null && company != null &&
                    (
                        (user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.SparkleStaff))
                     || (user.NetworkId.Equals(company.NetworkId) && user.NetworkAccessLevel.Value.HasAnyFlag(networkFlags))
                     || (user.CompanyId.Equals(company.Id) && user.CompanyAccessLevel.Value.HasAnyFlag(companyFlags))
                    );
                if (!canEdit)
                {
                    result.Errors.Add(EditProfileFieldsError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                }

                if (result.Errors.Count > 0)
                    return result;

                var originalRequest = tran.Services.Company.GetEditCompanyFieldsRequest(company.Id, null);

                int i = -1, created = 0, updated = 0, deleted = 0;
                foreach (var item in request.Values)
                {
                    bool save = false;
                    i++;
                    var field = originalRequest.Fields.SingleOrDefault(x => item.ProfileFieldId.Equals(item.ProfileFieldId));
                    if (field == null)
                    {
                        result.Errors.Add(EditProfileFieldsError.ValueHasNoMatchingField, NetworksEnumMessages.ResourceManager, i);
                    }

                    ProfileFieldValueModel value, originalValue = null, newValue = null;
                    if (item.ProfileFieldValueId > 0)
                    {
                        originalValue = originalRequest.Values.SingleOrDefault(x => x.ProfileFieldValueId.Equals(item.ProfileFieldValueId));
                        value = item;
                        save = value.Value != originalValue.Value;
                    }
                    else
                    {
                        value = newValue = new ProfileFieldValueModel(field, item.Value);
                        value.SourceType = request.Source;
                        save = true;
                    }

                    var now = DateTime.UtcNow;
                    if (item.Action == ProfileFieldValueAction.Delete)
                    {
                        if (value.ProfileFieldValueId > 0)
                        {
                            var entity = tran.Repositories.CompanyProfileFields.GetById(item.ProfileFieldValueId);
                            tran.Repositories.CompanyProfileFields.Delete(entity);
                            deleted++; 
                        }
                    }
                    else if (save)
                    {
                        if (newValue != null)
                        {
                            var entity = new CompanyProfileField();
                            entity.CompanyId = company.Id;
                            entity.DateCreatedUtc = now;
                            entity.ProfileFieldId = field.Id;
                            entity.Value = value.Value;
                            entity.SourceType = value.SourceType;
                            entity.DateUpdatedUtc = now;
                            entity.UpdateCount++;
                            tran.Repositories.CompanyProfileFields.Attach(entity);
                            created++;
                        }
                        else
                        {
                            var entity = tran.Repositories.CompanyProfileFields.GetById(item.ProfileFieldValueId);
                            entity.Value = value.Value;
                            entity.SourceType = value.SourceType;
                            entity.DateUpdatedUtc = now;
                            entity.UpdateCount++;
                            updated++;
                        }
                    }
                    else
                    {
                    }
                }

                if (deleted > 0 || updated > 0 || created > 0)
                {
                    tran.CompleteTransaction();
                }

                return result;
            }
        }

        internal static void RegisterTags(TagsService tags)
        {
            tags.RegisterEntityValidator("Company", CompanyService.ValidateEntity);
            tags.RegisterTagValidator("Company", CompanyService.ValidateTag);
            tags.RegisterTagRepository("Company", EntityWithTagRepositoryType.Sql, "CompanyTags", "CompanyId");
        }

        private LocationGeolocData GetGeolocFromSparkleStatus(string location)
        {
            const string logPath = "CompanyService.GetGeolocFromSparkleStatus";
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
                this.Services.Logger.Error(logPath, ErrorLevel.ThirdParty, ex);
                Trace.TraceWarning(logPath + ": API request for '" + location + "' failed '" + ex.Message + "'");
            }

            return result;
        }
    }
}
