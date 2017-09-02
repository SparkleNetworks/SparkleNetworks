
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Entities.Networks.Neutral;
    using SrkToolkit.DataAnnotations;
    using Sparkle.Infrastructure;

    public class ProfileFieldsService : ServiceBase, IProfileFieldsService
    {
        [DebuggerStepThrough]
        internal ProfileFieldsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void Initialize()
        {
            this.InitializeProfileFields();
            this.InitializeProfileFieldAvailableValues();
            this.Services.Cache.InvalidateProfileFields();
        }

        private void InitializeProfileFields()
        {
            var logPath = "ProfileFieldsService.InitializeProfileFields()";

            #region Profile fields dictionary
            var dico = @"
Id | Name               | ApplyToUsers  | ApplyToCompanies  | ApplyToPartners   | Nbr
---------------------------------------------------------------------------------------
1  | Site               | True          | True              | True              | 0..1
2  | Phone              | True          | True              | False             | 0..1
3  | About              | True          | True              | True              | 0..1
4  | City               | True          | True              | True              | 0..1
5  | ZipCode            | True          | True              | True              | 0..1
6  | FavoriteQuotes     | True          | False             | False             | 0..1
7  | CurrentTarget      | True          | False             | False             | 0..1
8  | Contribution       | True          | False             | False             | 0..1
9  | Country            | True          | True              | True              | 0..1
10 | Headline           | True          | False             | False             | 0..1
11 | ContactGuideline   | True          | False             | True              | 0..1
12 | Industry           | True          | True              | True              | 0..1
13 | LinkedInPublicUrl  | True          | True              | False             | 0..1
14 | Language           | True          | False             | False             | 0..∞
15 | Education          | True          | False             | False             | 0..∞
16 | Twitter            | True          | True              | False             | 0..∞
17 | GTalk              | True          | False             | False             | 0..∞
18 | Msn                | True          | False             | False             | 0..∞
19 | Skype              | True          | False             | False             | 0..∞
20 | Yahoo              | True          | False             | False             | 0..∞
21 | Volunteer          | True          | False             | False             | 0..∞
22 | Certification      | True          | False             | False             | 0..∞
23 | Patents            | True          | False             | False             | 0..∞
24 | Location           | True          | True              | True              | 0..∞
25 | Contact            | False         | False             | True              | 0..∞
26 | Recommendation     | True          | False             | False             | 0..∞
27 | Email              | False         | True              | False             | 0..1
28 | Facebook           | False         | True              | False             | 0..1
29 | AngelList          | False         | True              | False             | 0..1
69 | Position           | True          | False             | False             | 0..∞
";
            #endregion

            var lines = dico
                .Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(2)
                .Select(o => o.Split(new string[] { "|" }, StringSplitOptions.None).Select(s => s.Trim()).ToArray())
                .ToArray();

            var all = this.Repo.ProfileFields.GetAll();
            bool missingField = false;

            foreach (var item in lines)
            {
                if (!all.Any(o => o.Id == int.Parse(item[0])
                        && o.Name == item[1]
                        && o.ApplyToUsers == bool.Parse(item[2])
                        && o.ApplyToCompanies == bool.Parse(item[3])))
                {
                    missingField = true;
                    break;
                }
            }

            if (!missingField)
            {
                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "All profile fields OK.");
                return;
            }

            var transaction = this.Services.Repositories.NewTransaction();
            using (transaction.BeginTransaction())
            {
                foreach (var item in lines)
                {
                    var id = int.Parse(item[0]);
                    var name = item[1];
                    var applyToUsers = bool.Parse(item[2]);
                    var applyToCompanies = bool.Parse(item[3]);

                    ProfileField entity = all.SingleOrDefault(o => o.Id == id);
                    if (entity == null)
                    {
                        entity = new ProfileField
                        {
                            Id = id,
                            Name = name,
                            ApplyToUsers = applyToUsers,
                            ApplyToCompanies = applyToCompanies,
                        };

                        transaction.Repositories.ProfileFields.Insert(entity);
                        this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created profile fields " + name);
                    }
                    else if (entity.Name != name || entity.ApplyToUsers != applyToUsers || entity.ApplyToCompanies != applyToCompanies)
                    {
                        entity.Name = name;
                        entity.ApplyToUsers = applyToUsers;
                        entity.ApplyToCompanies = applyToCompanies;

                        transaction.Repositories.ProfileFields.Update(entity);
                        this.Services.Logger.Info(logPath, ErrorLevel.Success, "Update profile fields " + id.ToString());
                    }
                }

                transaction.CompleteTransaction();
            }
        }

        private void InitializeProfileFieldAvailableValues()
        {
            var logPath = "ProfileFieldsService.InitializeProfileFieldsAvailableValues";

            #region Available values dictionary
            var dico = @"
Id  | ProfileFieldId | Value
-----------------------------------
1   | 12 | Accounting
2   | 12 | Airlines/Aviation
3   | 12 | Alternative Dispute Resolution
4   | 12 | Alternative Medicine
5   | 12 | Animation
6   | 12 | Apparel & Fashion
7   | 12 | Architecture & Planning
8   | 12 | Arts and Crafts
9   | 12 | Automotive
10  | 12 | Aviation & Aerospace
11  | 12 | Banking
12  | 12 | Biotechnology
13  | 12 | Broadcast Media
14  | 12 | Building Materials
15  | 12 | Business Supplies and Equipment
16  | 12 | Capital Markets
17  | 12 | Chemicals
18  | 12 | Civic & Social Organization
19  | 12 | Civil Engineering
20  | 12 | Commercial Real Estate
21  | 12 | Computer & Network Security
22  | 12 | Computer Games
23  | 12 | Computer Hardware
24  | 12 | Computer Networking
25  | 12 | Computer Software
26  | 12 | Construction
27  | 12 | Consumer Electronics
28  | 12 | Consumer Goods
29  | 12 | Consumer Services
30  | 12 | Cosmetics
31  | 12 | Dairy
32  | 12 | Defense & Space
33  | 12 | Design
34  | 12 | Education Management
35  | 12 | E-Learning
36  | 12 | Electrical/Electronic Manufacturing
37  | 12 | Entertainment
38  | 12 | Environmental Services
39  | 12 | Events Services
40  | 12 | Executive Office
41  | 12 | Facilities Services
42  | 12 | Farming
43  | 12 | Financial Services
44  | 12 | Fine Art
45  | 12 | Fishery
46  | 12 | Food & Beverages
47  | 12 | Food Production
48  | 12 | Fund-Raising
49  | 12 | Furniture
50  | 12 | Gambling & Casinos
51  | 12 | Glass, Ceramics & Concrete
52  | 12 | Government Administration
53  | 12 | Government Relations
54  | 12 | Graphic Design
55  | 12 | Health, Wellness and Fitness
56  | 12 | Higher Education
57  | 12 | Hospital & Health Care
58  | 12 | Hospitality
59  | 12 | Human Resources
60  | 12 | Import and Export
61  | 12 | Individual & Family Services
62  | 12 | Industrial Automation
63  | 12 | Information Services
64  | 12 | Information Technology and Services
65  | 12 | Insurance
66  | 12 | International Affairs
67  | 12 | International Trade and Development
68  | 12 | Internet
69  | 12 | Investment Banking
70  | 12 | Investment Management
71  | 12 | Judiciary
72  | 12 | Law Enforcement
73  | 12 | Law Practice
74  | 12 | Legal Services
75  | 12 | Legislative Office
76  | 12 | Leisure, Travel & Tourism
77  | 12 | Libraries
78  | 12 | Logistics and Supply Chain
79  | 12 | Luxury Goods & Jewelry
80  | 12 | Machinery
81  | 12 | Management Consulting
82  | 12 | Maritime
83  | 12 | Market Research
84  | 12 | Marketing and Advertising
85  | 12 | Mechanical or Industrial Engineering
86  | 12 | Media Production
87  | 12 | Medical Devices
88  | 12 | Medical Practice
89  | 12 | Mental Health Care
90  | 12 | Military
91  | 12 | Mining & Metals
92  | 12 | Motion Pictures and Film
93  | 12 | Museums and Institutions
94  | 12 | Music
95  | 12 | Nanotechnology
96  | 12 | Newspapers
97  | 12 | Non-Profit Organization Management
98  | 12 | Oil & Energy
99  | 12 | Online Media
100 | 12 | Outsourcing/Offshoring
101 | 12 | Package/Freight Delivery
102 | 12 | Packaging and Containers
103 | 12 | Paper & Forest Products
104 | 12 | Performing Arts
105 | 12 | Pharmaceuticals
106 | 12 | Philanthropy
107 | 12 | Photography
108 | 12 | Plastics
109 | 12 | Political Organization
110 | 12 | Primary/Secondary Education
111 | 12 | Printing
112 | 12 | Professional Training & Coaching
113 | 12 | Program Development
114 | 12 | Public Policy
115 | 12 | Public Relations and Communications
116 | 12 | Public Safety
117 | 12 | Publishing
118 | 12 | Railroad Manufacture
119 | 12 | Ranching
120 | 12 | Real Estate
121 | 12 | Recreational Facilities and Services
122 | 12 | Religious Institutions
123 | 12 | Renewables & Environment
124 | 12 | Research
125 | 12 | Restaurants
126 | 12 | Retail
127 | 12 | Security and Investigations
128 | 12 | Semiconductors
129 | 12 | Shipbuilding
130 | 12 | Sporting Goods
131 | 12 | Sports
132 | 12 | Staffing and Recruiting
133 | 12 | Supermarkets
134 | 12 | Telecommunications
135 | 12 | Textiles
136 | 12 | Think Tanks
137 | 12 | Tobacco
138 | 12 | Translation and Localization
139 | 12 | Transportation/Trucking/Railroad
140 | 12 | Utilities
141 | 12 | Venture Capital & Private Equity
142 | 12 | Veterinary
143 | 12 | Warehousing
144 | 12 | Wholesale
145 | 12 | Wine and Spirits
146 | 12 | Wireless
147 | 12 | Writing and Editing
";
            #endregion

            var lines = dico
                .Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(2)
                .Select(o => o.Split(new string[] { "|" }, StringSplitOptions.None).Select(s => s.Trim()).ToArray())
                .ToArray();

            var all = this.Repo.ProfileFieldsAvailiableValues.GetAll();
            bool missingField = false;

            foreach (var item in lines)
            {
                if (!all.Any(o => o.Id == int.Parse(item[0])
                        && o.ProfileFieldId == int.Parse(item[1])
                        && o.Value == item[2]))
                {
                    missingField = true;
                    break;
                }
            }

            if (!missingField)
            {
                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "All profile fields available values OK.");
                return;
            }

            var transaction = this.Services.Repositories.NewTransaction();
            using (transaction.BeginTransaction())
            {
                foreach (var item in lines)
                {
                    var id = int.Parse(item[0]);
                    var profileFieldId = int.Parse(item[1]);
                    var value = item[2];

                    ProfileFieldsAvailiableValue entity = all.SingleOrDefault(o => o.Id == id);
                    if (entity == null)
                    {
                        entity = new ProfileFieldsAvailiableValue
                        {
                            Id = id,
                            ProfileFieldId = profileFieldId,
                            Value = value,
                        };

                        transaction.Repositories.ProfileFieldsAvailiableValues.Insert(entity);
                        this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created profile fields available value " + value);
                    }
                    else if (entity.Value != value || entity.ProfileFieldId != profileFieldId)
                    {
                        entity.Value = value;
                        entity.ProfileFieldId = profileFieldId;

                        transaction.Repositories.ProfileFieldsAvailiableValues.Update(entity);
                        this.Services.Logger.Info(logPath, ErrorLevel.Success, "Update profile fields available value " + id.ToString());
                    }
                }

                transaction.CompleteTransaction();
            }
        }

        // ProfileFields services

        /// <summary>
        /// Count the ProfileFields in database
        /// </summary>
        /// <returns>The number of ProfileFields</returns>
        public int CountProfileFields()
        {
            return this.Services.Cache.CountProfileFields();
        }

        /// <summary>
        /// Insert an item of type ProfileField in the database
        /// </summary>
        /// <param name="item">The ProfileField to insert</param>
        /// <returns>The id of the newly inserted ProfileField</returns>
        public int Insert(ProfileField item)
        {
            return this.Repo
                .ProfileFields
                .Insert(item)
                .Id;
        }

        /// <summary>
        /// Get a ProfileField by his Id
        /// </summary>
        /// <param name="id">Id of item</param>
        /// <returns>The ProfileField retrieved from database</returns>
        public ProfileField GetProfileFieldById(int id)
        {
            return this.Repo
                .ProfileFields
                .GetById(id);
        }

        // UserProfileFields services

        public UserProfileFieldModel SetUserProfileField(int userId, ProfileFieldType profileFieldType, string value)
        {
            return this.SetUserProfileField(userId, profileFieldType, value, ProfileFieldSource.None);
        }

        public UserProfileFieldModel SetUserProfileField(int userId, ProfileFieldType profileFieldType, string value, ProfileFieldSource source)
        {
            var item = this.Repo.UserProfileFields.GetByUserIdAndFieldType(userId, profileFieldType);

            if (value == null)
            {
                if (item != null)
                {
                    this.Repo.UserProfileFields.Delete(item);
                }
            }
            else
            {
                if (profileFieldType == ProfileFieldType.Twitter)
                {
                    string username = null;
                    if (TwitterUsernameAttribute.GetUsername(value, out username))
                    {
                        value = username;
                    }
                    else
                    {
                        this.Services.Logger.Error(
                            "ProfileFieldsService.SetUserProfileField",
                            ErrorLevel.Data,
                            "Failed to get twitter username.");
                    }
                }

                if (item == null)
                {
                    item = new UserProfileField
                    {
                        UserId = userId,
                        ProfileFieldId = (int)profileFieldType,
                        Value = value,
                        DateCreatedUtc = DateTime.UtcNow,
                        SourceType = source,
                    };
                    this.Repo.UserProfileFields.Insert(item);
                }
                else
                {
                    if (value != item.Value)
                    {
                        item.Value = value;
                        item.DateUpdatedUtc = DateTime.UtcNow;
                        item.UpdateCount++;
                        item.SourceType = source;
                        this.Repo.UserProfileFields.Update(item);
                    }
                }
            }

            return item != null ? new UserProfileFieldModel(item) : null;
        }

        public IList<UserProfileFieldModel> GetUserProfileFieldsByUserId(int userId)
        {
            return this.Repo
                .UserProfileFields
                .GetByUserId(userId)
                .Select(o => new UserProfileFieldModel(o))
                .ToList();
        }

        public IDictionary<int, IList<UserProfileFieldModel>> GetUserProfileFieldsByUserIds(int[] userIds)
        {
            return this.Repo
                .UserProfileFields
                .GetByUserIds(userIds)
                .ToDictionary(o => o.Key, o => (IList<UserProfileFieldModel>)o.Value.Select(p => new UserProfileFieldModel(p)).ToList());
        }

        // ProfileFieldsAvailiableValues services

        /// <summary>
        /// Count the ProfileFieldsAvailiableValues in database
        /// </summary>
        /// <returns>The number of ProfileFieldsAvailiableValues</returns>
        public int CountProfileFieldsAvailiablesValues()
        {
            return this.Repo
                .ProfileFieldsAvailiableValues
                .Count();
        }

        /// <summary>
        /// Insert an item of type ProfileFieldsAvailiableValue in the database
        /// </summary>
        /// <param name="item">The ProfileFieldAvailiableValue to insert</param>
        /// <returns>The id of the newly inserted ProfileFieldsAvailiableValue</returns>
        public int Insert(ProfileFieldsAvailiableValue item)
        {
            return this.Repo
                .ProfileFieldsAvailiableValues
                .Insert(item)
                .Id;
        }

        /// <summary>
        /// Get a ProfileFieldsAvailiableValue by his Id
        /// </summary>
        /// <param name="id">Id of item</param>
        /// <returns>The ProfileFieldsAvailiableValue retrieved from database</returns>
        public ProfileFieldsAvailiableValue GetProfileFieldsAvailableValueById(int id)
        {
            return this.Repo
                .ProfileFieldsAvailiableValues
                .GetById(id);
        }

        /// <summary>
        /// Get a ProfileFieldsAvailiableValue by his Value
        /// </summary>
        /// <param name="value">Value of item</param>
        /// <returns>The ProfileFieldsAvailiableValue retrieved from database</returns>
        public ProfileFieldsAvailiableValue GetProfileFieldsAvailiableValueByValue(ProfileFieldType type, string value)
        {
            return this.Repo
                .ProfileFieldsAvailiableValues
                .GetByValue(type, value);
        }

        public IList<ProfileFieldsAvailiableValue> GetAllAvailiableValuesByType(ProfileFieldType type)
        {
            return this.Repo
                .ProfileFieldsAvailiableValues
                .GetByType(type);
        }

        public ProfileFieldsAvailiableValue GetProfileFieldsAvailiableValueByIdAndType(int id, ProfileFieldType type)
        {
            return this.Repo
                .ProfileFieldsAvailiableValues
                .GetByIdAndType(id, type);
        }

        public int GetAvailableValueIdByName(string name)
        {
            var item =  this.Repo
                .ProfileFieldsAvailiableValues
                .GetByName(name);
            return item != null ? item.Id : 0;
        }

        public IList<ProfileFieldModel> GetUserFields()
        {
            var items = this.Services.Cache.FindProfileFields(x => x.ApplyToUsers);
            return items;
        }

        public IDictionary<string, ProfileFieldModel> GetUserFieldsDictionary()
        {
            var items = this.Services.Cache.FindProfileFields(x => x.ApplyToUsers);
            return items.ToDictionary(f => f.Name, f => f);
        }

        public IList<ProfileFieldModel> GetCompanyFields(bool includeAvailableValues)
        {
            var items = this.Services.Cache.FindProfileFields(x => x.ApplyToCompanies);
            return items;
        }

        public IDictionary<string, ProfileFieldModel> GetCompanyFieldsDictionary(bool includeAvailableValues)
        {
            var items = this.Services.Cache.FindProfileFields(x => x.ApplyToCompanies);
            return items.ToDictionary(f => f.Name, f => f);
        }

        public CompanyProfileFieldModel SetCompanyProfileField(int companyId, ProfileFieldType profileFieldType, string value, ProfileFieldSource source)
        {
            // BE CAREFUL
            // this method does a "replace all" 
            // all fields of the same type will be overwritten by the single parameter value

            var items = this.Repo.CompanyProfileFields.GetByCompanyIdAndFieldType(companyId, profileFieldType);
            CompanyProfileField item;

            if (value == null)
            {
                foreach (var item1 in items)
                {
                    this.Repo.CompanyProfileFields.Delete(item1);
                }

                item = null;
            }
            else
            {
                if (profileFieldType == ProfileFieldType.Twitter)
                {
                    string username;
                    if (TwitterUsernameAttribute.GetUsername(value, out username))
                    {
                        value = username;
                    }
                    else
                    {
                        this.Services.Logger.Warning("ProfileFieldsService.SetCompanyProfileField", ErrorLevel.Input, "Failed to get twitter username from '" + value + "'.");
                    }
                }

                if (items.Count == 0)
                {
                    item = new CompanyProfileField
                    {
                        CompanyId = companyId,
                        ProfileFieldId = (int)profileFieldType,
                        Value = value,
                        DateCreatedUtc = DateTime.UtcNow,
                        SourceType = source,
                    };
                    this.Repo.CompanyProfileFields.Insert(item);
                }
                else
                {
                    item = items.FirstOrDefault(i => i.Value == value) ?? items.First();
                    item.Value = value;
                    item.DateUpdatedUtc=DateTime.UtcNow;
                    item.UpdateCount++;
                    item.SourceType = source;
                    this.Repo.CompanyProfileFields.Update(item);

                    foreach (var item1 in items)
                    {
                        if (item != item1)
                        {
                            this.Repo.CompanyProfileFields.Delete(item1);
                        }
                    }
                }
            }

            return item != null ? new CompanyProfileFieldModel(item) : null;
        }

        public void InsertManyUserProfileFields(IList<UserProfileFieldPoco> items, int? userId)
        {
            this.Repo
                .UserProfileFields
                .InsertMany(items
                    .Where(o => o.Value != null)
                    .Select(o => new UserProfileField
                    {
                        UserId = userId ?? o.UserId,
                        ProfileFieldId = o.ProfileFieldId,
                        Value = o.Value,
                        DateCreatedUtc = DateTime.UtcNow,
                        Source = o.Source,
                        Data = o.Data,
                    })
                    .ToList());
        }

        public void InsertManyCompanyProfileFields(IList<CompanyProfileFieldPoco> items, int? companyId)
        {
            this.Repo
                .CompanyProfileFields
                .InsertMany(items
                    .Where(o => o.Value != null)
                    .Select(o => new CompanyProfileField
                    {
                        CompanyId = companyId ?? o.CompanyId,
                        ProfileFieldId = o.ProfileFieldId,
                        Value = o.Value,
                        DateCreatedUtc = DateTime.UtcNow,
                        Source = o.Source,
                        Data = o.Data,
                    })
                    .ToList());
        }

        public IList<CompanyProfileFieldModel> GetCompanyProfileFieldsByCompanyId(int companyId)
        {
            return this.Repo
                .CompanyProfileFields
                .GetByCompanyId(companyId)
                .Select(o => new CompanyProfileFieldModel(o))
                .ToList();
        }

        public IList<ProfileFieldValueModel> GetCompanyValues(int companyId)
        {
            return this.Repo.CompanyProfileFields.GetByCompanyId(companyId)
                .Select(o => new ProfileFieldValueModel(o))
                .ToList();
        }

        public IDictionary<int, IList<CompanyProfileFieldModel>> GetCompanyProfileFieldsByCompany(int? networkId = null)
        {
            return this.Repo
                .CompanyProfileFields
                .GetAll(networkId ?? this.Services.NetworkId)
                .Select(o => new CompanyProfileFieldModel(o))
                .GroupBy(o => o.CompanyId)
                .ToDictionary(o => o.Key, o => (IList<CompanyProfileFieldModel>)o.ToList());
        }

        public IDictionary<int, IList<CompanyProfileFieldModel>> GetAllCompanyProfileFieldsByCompany()
        {
            return this.Repo
                .CompanyProfileFields
                .GetAll(null)
                .Select(o => new CompanyProfileFieldModel(o))
                .GroupBy(o => o.CompanyId)
                .ToDictionary(o => o.Key, o => (IList<CompanyProfileFieldModel>)o.ToList());
        }

        public IDictionary<int, IList<CompanyProfileFieldModel>> GetCompanyProfileFieldByCompanyIdAndType(int[] companyIds, ProfileFieldType[] profileFieldTypes)
        {
            var items = this.Repo.CompanyProfileFields.GetByCompanyIdAndFieldType(companyIds, profileFieldTypes);
            return items
                .GroupBy(x => x.CompanyId)
                .ToDictionary(
                    g => g.Key,
                    g => (IList<CompanyProfileFieldModel>)g.Select(x => new CompanyProfileFieldModel(x)).ToList());
        }

        public IList<CompanyProfileFieldModel> GetCompanyProfileFieldByCompanyIdAndType(int companyId, ProfileFieldType[] profileFieldTypes)
        {
            return this.Repo.CompanyProfileFields.GetByCompanyIdAndFieldType(companyId, profileFieldTypes)
                .Select(f => new CompanyProfileFieldModel(f))
                .ToList();
        }

        public IList<CompanyProfileFieldModel> GetCompanyProfileFieldByCompanyIdAndType(int companyId, ProfileFieldType profileFieldType)
        {
            return this.Repo.CompanyProfileFields.GetByCompanyIdAndFieldType(companyId, profileFieldType)
                .Select(f => new CompanyProfileFieldModel(f))
                .ToList();
        }

        public IList<UserProfileFieldModel> GetUserProfileFieldsByType(ProfileFieldType type)
        {
            return this.Repo.UserProfileFields.GetByFieldType(type).Select(o => new UserProfileFieldModel(o)).ToList();
        }

        public IDictionary<int, UserProfileFieldModel> GetUniqueUserProfileFieldsByUserIdsAndType(int[] userIds, ProfileFieldType type)
        {
#warning will throw exception if profile field type is not meant to be unique
            return this.Repo
                .UserProfileFields
                .GetByUserIdsAndFieldType(userIds, type)
                .Where(o => o.Value.Count > 0)
                .ToDictionary(o => o.Key, o => (new UserProfileFieldModel(o.Value.SingleOrDefault())));
        }

        public IList<UserProfileFieldModel> GetUserProfileFieldsByTypeAndNetworkId(ProfileFieldType type, int networkId)
        {
            return this.Repo
                .UserProfileFields
                .GetByFieldTypeAndNetworkId(type, networkId)
                .Select(o => new UserProfileFieldModel(o))
                .ToList();
        }

        public IList<ProfileFieldModel> GetTypes()
        {
            return this.GetTypes(false);
        }

        public IList<ProfileFieldModel> GetTypes(bool withCounts)
        {
            var models = this.Services.Cache.AllProfileFields;
            var value = this.Repo.ProfileFieldsAvailiableValues.CountByType();

            foreach (var item in models.Values)
            {
                if (value.ContainsKey(item.Id))
                {
                    item.AvailableValuesCount = value[item.Id];
                }
            }

            if (withCounts)
            {
                var companies = this.Repo.CompanyProfileFields.GetCounts();
                foreach (var numbers in companies)
                {
                    if (models.ContainsKey(numbers[0]))
                    {
                        var model = models[numbers[0]];

                        if (model.Counts == null)
                            model.Counts = new Dictionary<string, object>();
                        model.Counts.Add("Companies", numbers[1]);
                        model.Counts.Add("CompanyValues", numbers[2]);
                    }
                }

                var users = this.Repo.UserProfileFields.GetCounts();
                foreach (var numbers in users)
                {
                    if (models.ContainsKey(numbers[0]))
                    {
                        var model = models[numbers[0]];

                        if (model.Counts == null)
                            model.Counts = new Dictionary<string, object>();
                        model.Counts.Add("Users", numbers[1]);
                        model.Counts.Add("UserValues", numbers[2]);
                    }
                }
            }

            return models.Values.ToList();
        }

        public IDictionary<int, ProfileFieldModel> GetAllForCache()
        {
            var items = this.Repo.ProfileFields.GetAll();
            var value = this.Repo.ProfileFieldsAvailiableValues.CountByType();
            var models = new List<ProfileFieldModel>(items.Count);
            models.AddRange(items.Select(x => new ProfileFieldModel(x)));
            var indexed = models.ToDictionary(x => x.Id, x => x);

            foreach (var item in models)
            {
                if (value.ContainsKey(item.Id))
                {
                    item.AvailableValuesCount = value[item.Id];
                }
            }

            return indexed;
        }

        public IList<ProfileFieldAvailableValueModel> GetAvailiableValuesByType(int[] fieldIds)
        {
            var items = this.Repo.ProfileFieldsAvailiableValues.GetByFieldId(fieldIds);
            var models = items.Select(x => new ProfileFieldAvailableValueModel(x)).ToList();
            return models;
        }

        private void LoadAvailableValues(ICollection<ProfileFieldModel> items)
        {
            var fieldIds = items.Select(x => x.Id).ToArray();
            var values = this.Repo.ProfileFieldsAvailiableValues.GetByFieldId(fieldIds);
            foreach (var item in items)
            {
                if (item.AvailableValues == null)
                    item.AvailableValues = new List<ProfileFieldAvailableValueModel>();
                foreach (var value in values)
                {
                    if (value.ProfileFieldId.Equals(item.Id))
                    {
                        item.AvailableValues.Add(new ProfileFieldAvailableValueModel(value));
                    }
                }
            }
        }
    }
}
