
namespace Sparkle.Services.Networks.Users
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure.Crypto;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Tags;
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ApplyRequestModel
    {
        private ApplyRequestDataModel dataModel;
        private ApplyRequestCompanyDataModel companyDataModel;
        private ApplyRequestProcessDataModel processDataModel;
        private string data;
        private string companyData;
        private string processData;
        private static readonly JsonSerializerSettings jsonSerializerSettings;

        static ApplyRequestModel()
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
        }

        public ApplyRequestModel()
        {
        }

        public ApplyRequestModel(ApplyRequest item)
        {
            this.Set(item);
        }

        public DateTime DateCreatedUtc { get; set; }

        public int Id { get; set; }

        public Guid Key { get; set; }

        public DateTime? DateSubmitedUtc { get; set; }

        public DateTime? DateEmailConfirmedUtc { get; set; }

        public DateTime DateLastUpdateUtc
        {
            get { return this.DateAcceptedUtc ?? this.DateRefusedUtc ?? this.DateEmailConfirmedUtc ?? this.DateSubmitedUtc ?? this.DateCreatedUtc; }
        }

        public string Data
        {
            get { return this.dataModel != null ? JsonConvert.SerializeObject(this.dataModel, jsonSerializerSettings) : null; }
        }

        public string CompanyData
        {
            get { return this.companyDataModel != null ? JsonConvert.SerializeObject(this.companyDataModel, jsonSerializerSettings) : null; }
        }

        public string ProcessData
        {
            get { return this.processDataModel != null ? JsonConvert.SerializeObject(this.processDataModel, jsonSerializerSettings) : null; }
        }

        public DateTime? DateAcceptedUtc { get; set; }

        public DateTime? DateRefusedUtc { get; set; }

        public int? AcceptedByUserId { get; set; }

        public int? CreatedCompanyId { get; set; }

        public int? CreatedUserId { get; set; }

        public int? JoinCompanyId { get; set; }

        public CompanyModel JoinCompany { get; set; }

        public int NetworkId { get; set; }

        public int? RefusedByUserId { get; set; }

        public string UserRemoteAddress { get; set; }

        public bool IsNew
        {
            get { return this.DateSubmitedUtc == null; }
        }

        public bool IsPendingEmailConfirmation
        {
            get { return this.DateSubmitedUtc != null && this.DateEmailConfirmedUtc == null && this.DateAcceptedUtc == null && this.DateRefusedUtc == null; }
        }

        public bool IsPendingAccept
        {
            get { return this.DateSubmitedUtc != null && this.DateEmailConfirmedUtc != null && this.DateAcceptedUtc == null && this.DateRefusedUtc == null; }
        }

        public bool IsAccepted
        {
            get { return this.DateAcceptedUtc != null; }
        }

        public bool IsRefused
        {
            get { return this.DateRefusedUtc != null; }
        }

        public bool IsClosed
        {
            get { return this.IsRefused || this.CreatedUserId != null; }
        }

        public ApplyRequestStatus Status
        {
            get
            {
                return this.IsNew ? ApplyRequestStatus.New :
                    this.IsPendingEmailConfirmation ? ApplyRequestStatus.PendingEmailConfirmation :
                    this.IsPendingAccept ? ApplyRequestStatus.PendingAccept :
                    this.IsAccepted ? ApplyRequestStatus.Accepted :
                    this.IsRefused ? ApplyRequestStatus.Refused :
                    ApplyRequestStatus.Unknown;
            }
        }

        public string StatusTitle
        {
            get { return EnumTools.GetDescription(this.Status, NetworksEnumMessages.ResourceManager); }
        }

        public ApplyRequestDataModel UserDataModel
        {
            get
            {
                if (this.dataModel == null && this.data != null)
                {
                    this.dataModel = JsonConvert.DeserializeObject<ApplyRequestDataModel>(this.data, jsonSerializerSettings);
                }
                else if (this.dataModel == null)
                {
                    this.dataModel = new ApplyRequestDataModel();
                }

                if (this.dataModel.UserConnections == null)
                    this.dataModel.UserConnections = new List<SocialNetworkConnectionPoco>();

                if (this.dataModel.UserFields == null)
                    this.dataModel.UserFields = new List<UserProfileFieldPoco>();

                return this.dataModel;
            }
        }

        public ApplyRequestCompanyDataModel CompanyDataModel
        {
            get
            {
                if (this.companyDataModel == null && this.companyData != null)
                {
                    this.companyDataModel = JsonConvert.DeserializeObject<ApplyRequestCompanyDataModel>(this.companyData, jsonSerializerSettings);
                }
                else if (this.companyDataModel == null)
                {
                    this.companyDataModel = new ApplyRequestCompanyDataModel();
                }

                if (this.companyDataModel.CompanyFields == null)
                    this.companyDataModel.CompanyFields = new List<CompanyProfileFieldPoco>();

                return this.companyDataModel;
            }
        }

        public ApplyRequestProcessDataModel ProcessDataModel
        {
            get
            {
                if (this.processDataModel == null && this.processData != null)
                {
                    this.processDataModel = JsonConvert.DeserializeObject<ApplyRequestProcessDataModel>(this.processData, jsonSerializerSettings);
                }
                else if (this.processDataModel == null)
                {
                    this.processDataModel = new ApplyRequestProcessDataModel();
                }

                if (this.processDataModel.CompanyRelationships == null)
                    this.processDataModel.CompanyRelationships = new List<CompanyRelationshipPoco>();

                return this.processDataModel;
            }
        }

        public string RefusedRemark { get; set; }

        [Display(Name = "CreatedUser", ResourceType = typeof(NetworksLabels))]
        public UserModel CreatedUser { get; set; }

        [Display(Name = "CreatedCompany", ResourceType = typeof(NetworksLabels))]
        public CompanyModel CreatedCompany { get; set; }

        public int? InvitedByUserId { get; set; }

        public DateTime? DateInvitedUtc { get; set; }

        public UserModel InvitedByUser { get; set; }

        public UserModel AcceptedByUser { get; set; }
        
        public string GetSecretKey()
        {
            // TODO: put the code back here!
            throw new NotSupportedException();
        }

        public bool CheckSecretKey(string secret)
        {
            // TODO: put the code back here!
            throw new NotSupportedException();
        }
        
        public void UpdateFrom(ApplyRequestRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var data = this.UserDataModel;

            data.User = data.User ?? new UserPoco();
            data.User.FirstName = request.Firstname;
            data.User.LastName = request.Lastname;
            data.User.Email = request.Email;
            data.User.PersonalEmail = request.PersonalEmail;
            data.User.Gender = (int)request.Gender;
            data.User.Culture = request.Culture;
            data.User.Timezone = request.Timezone;

            // lots of comments because of the new this.SetField()
            // remove them when WI 651 is Closed

            ////if (request.Phone != null)
            ////{
            ////    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.Phone);
            ////    if (field != null && field.Value != request.Phone)
            ////        field.Value = request.Phone;
            ////    else if (field == null)
            ////        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.Phone, request.Phone));
            ////}

            string twitterUsername = null;
            if (!string.IsNullOrEmpty(request.Twitter))
                SrkToolkit.DataAnnotations.TwitterUsernameAttribute.GetUsername(request.Twitter, out twitterUsername);

            this.SetField(ProfileFieldType.Phone, request.Phone, false, true, ProfileFieldSource.UserInput);
            this.SetField(ProfileFieldType.About, request.About, true, true, ProfileFieldSource.UserInput);
            this.SetField(ProfileFieldType.City, request.City, true, true, ProfileFieldSource.UserInput);
            this.SetField(ProfileFieldType.Country, request.Country, true, true, ProfileFieldSource.UserInput);
            this.SetField(ProfileFieldType.LinkedInPublicUrl, request.LinkedInPublicUrl, true, true, ProfileFieldSource.UserInput);
            this.SetField(ProfileFieldType.Twitter, twitterUsername, false, true, ProfileFieldSource.UserInput);

            ////if (request.About != null)
            ////{
            ////    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.About);
            ////    if (field != null && field.Value != request.About)
            ////        field.Value = request.About;
            ////    else if (field == null)
            ////        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.About, request.About));
            ////}

            ////if (request.City != null)
            ////{
            ////    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.City);
            ////    if (field != null && field.Value != request.City)
            ////        field.Value = request.City;
            ////    else if (field == null)
            ////        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.City, request.City));
            ////}

            ////if (request.Country != null)
            ////{
            ////    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.Country);
            ////    if (field != null && field.Value != request.Country)
            ////        field.Value = request.Country;
            ////    else if (field == null)
            ////        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.Country, request.Country));
            ////}

            int industryId;
            if (request.Industry != null && int.TryParse(request.Industry, out industryId) && industryId > 0)
            {
                var industry = request.Industries.Where(o => o.SelecterId == industryId).SingleOrDefault();
                if (industry != null)
                {
                    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.Industry);
                    if (field != null && field.Value != industry.Value)
                        field.Value = industry.Value;
                    else if (field == null)
                        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.Industry, industry.Value));
                }
            }

            if (request.JobId > 0)
            {
                data.User.JobId = request.JobId;
            }

            ////if (request.LinkedInPublicUrl != null)
            ////{
            ////    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.LinkedInPublicUrl);
            ////    if (field != null && field.Value != request.LinkedInPublicUrl)
            ////        field.Value = request.LinkedInPublicUrl;
            ////    else if (field == null)
            ////        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.LinkedInPublicUrl, request.LinkedInPublicUrl));
            ////}

            ////if (request.Twitter != null)
            ////{
            ////    var field = data.UserFields.SingleOrDefault(f => f.ProfileFieldType == ProfileFieldType.Twitter && f.Value == request.Twitter);
            ////    if (field == null)
            ////    {
            ////        data.UserFields.Add(new UserProfileFieldPoco(ProfileFieldType.Twitter, request.Twitter));
            ////    }
            ////}
        }

        public override string ToString()
        {
            return "ApplyRequestModel #" + this.Id + " "
                + (this.IsNew ? "IsNew" : this.IsPendingEmailConfirmation ? "IsPendingEmailConfirmation" : this.IsPendingAccept ? "IsPendingAccept" : this.IsPendingAccept ? "IsPendingAccept" : this.IsAccepted ? "IsAccepted" : this.IsRefused ? "IsRefused" : this.IsClosed ? "IsClosed" : "???");
        }

        private void Set(ApplyRequest item)
        {
            this.Id = item.Id;
            this.Key = item.Key;

            this.AcceptedByUserId = item.AcceptedByUserId;
            this.CreatedCompanyId = item.CreatedCompanyId;
            this.CreatedUserId = item.CreatedUserId;
            this.JoinCompanyId = item.JoinCompanyId;
            this.NetworkId = item.NetworkId;
            this.RefusedByUserId = item.RefusedByUserId;
            this.UserRemoteAddress = item.UserRemoteAddress;
            this.RefusedRemark = item.RefusedRemark;
            this.InvitedByUserId = item.InvitedByUserId;

            this.data = item.Data;
            this.companyData = item.CompanyData;
            this.processData = item.ProcessData;

            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateSubmitedUtc = item.DateSubmitedUtc.AsUtc();
            this.DateAcceptedUtc = item.DateAcceptedUtc.AsUtc();
            this.DateRefusedUtc = item.DateRefusedUtc.AsUtc();
            this.DateEmailConfirmedUtc = item.DateEmailConfirmedUtc.AsUtc();
            this.DateInvitedUtc = item.DateInvitedUtc.AsUtc();

            if (item.JoinCompanyId != null)
            {
                this.JoinCompany = new CompanyModel(item.JoinCompanyId.Value, null, null);
            }
        }

        private bool SetField(ProfileFieldType fieldType, string value, bool replaceAll, bool cancelOnEmptyNewValue, ProfileFieldSource? sourceOnValueChange)
        {
            if (cancelOnEmptyNewValue && string.IsNullOrWhiteSpace(value))
                return false;
            
            var data = this.UserDataModel;
            var fields = data.UserFields.Where(f => f.ProfileFieldType == fieldType).ToArray();
            if (fields.Length == 0)
            {
                var item = new UserProfileFieldPoco(fieldType, value);
                data.UserFields.Add(item);

                if (sourceOnValueChange != null)
                    item.SourceType = sourceOnValueChange.Value;
            }
            else if (fields.Length == 1)
            {
                var item = fields[0];
                if ( item.Value != value)
                {
                    item.Value = value;
                    if (sourceOnValueChange != null)
                        item.SourceType = sourceOnValueChange.Value;
                }
            }
            else
            {
                if (replaceAll)
                {
                    var item = fields.FirstOrDefault(f => f.Value == value);
                    if (item != null)
                    {
                        data.UserFields.Clear();
                        data.UserFields.Add(item);
                    }
                    else
                    {
                        item = fields[0];
                        item.Value = value;
                        if (sourceOnValueChange != null)
                            item.SourceType = sourceOnValueChange.Value;

                        for (int i = 1; i < fields.Length; i++)
                        {
                            data.UserFields.Remove(fields[i]);
                        }
                    }
                }
                else
                {
                    var item = fields.FirstOrDefault(f => f.Value == value);
                    if (item != null)
                    {
                        for (int i = 1; i < fields.Length; i++)
                        {
                            data.UserFields.Remove(fields[i]);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot change profile field '" + fieldType + "' value because there are already many values set");
                    }
                }
            }

            return true;
        }
    }

    public enum ApplyRequestStatus
    {
        New,
        PendingEmailConfirmation,
        PendingAccept,
        Accepted,
        Refused,
        Unknown,
    }

    [DataContract]
    public class ApplyRequestDataModel
    {
        private UserPoco user;
        private IList<UserProfileFieldPoco> userFields;
        private IList<SocialNetworkConnectionPoco> userConnections;
        private IList<TagModel> userTags;

        [DataMember]
        public UserPoco User
        {
            get
            {
                if (this.user == null)
                    this.user = new UserPoco();
                return this.user;
            }
            set { this.user = value; }
        }

        [DataMember]
        public IList<UserProfileFieldPoco> UserFields
        {
            get
            {
                if (this.userFields == null)
                    this.userFields = new List<UserProfileFieldPoco>();
                return this.userFields;
            }
            set { this.userFields = value; }
        }

        [DataMember]
        public IList<SocialNetworkConnectionPoco> UserConnections
        {
            get
            {
                if (this.userConnections == null)
                    this.userConnections = new List<SocialNetworkConnectionPoco>();
                return this.userConnections;
            }
            set { this.userConnections = value; }
        }

        [DataMember]
        public IList<TagModel> UserTags
        {
            get
            {
                if (this.userTags == null)
                    this.userTags = new List<TagModel>();
                return this.userTags;
            }
            set { this.userTags = value; }
        }

        [DataMember]
        public string JobTitleToCreate { get; set; }

        public void UpdateFrom(LinkedInPeopleResult linkedInResult)
        {
            var user = linkedInResult.UserEntity;

            this.User.FirstName = user.FirstName;
            this.User.LastName = user.LastName;
            this.User.Birthday = user.Birthday;
            this.User.PersonalEmail = user.PersonalEmail;
            this.User.PersonalDataUpdateDateUtc = DateTime.UtcNow;
            this.User.LinkedInUserId = user.LinkedInUserId;
            if (user.JobId.HasValue)
                this.User.JobId = user.JobId;

            var fields = linkedInResult.Changes;
            foreach (var field in fields)
            {
                var poco = new UserProfileFieldPoco();
                poco.ProfileFieldType = field.Item.Type;
                poco.Value = field.Item.Value;
                poco.DateCreatedUtc = DateTime.UtcNow;
                poco.SourceType = ProfileFieldSource.LinkedIn;
                poco.Data = field.Item.Data;
                this.UserFields.Add(poco);
            }

            this.UserTags = linkedInResult.TagChanges;
            this.JobTitleToCreate = linkedInResult.JobTitleToCreate;
        }
    }

    [DataContract]
    public class ApplyRequestCompanyDataModel
    {
        private CompanyPoco company;
        private string[] adminsEmailAddresses;
        private string[] usersEmailAddresses;
        private IList<CompanyProfileFieldPoco> companyFields;
        private IList<LinkedInCompanyDataModel> linkedInCompanies;
        private IList<Tag2Model> companyTags;

        [DataMember]
        public CompanyPoco Company
        {
            get
            {
                if (this.company == null)
                    this.company = new CompanyPoco();

                return this.company;
            }
            set { this.company = value; }
        }

        [DataMember]
        public string[] AdminsEmailAddresses
        {
            get
            {
                if (this.adminsEmailAddresses == null)
                    this.adminsEmailAddresses = new string[0];

                return this.adminsEmailAddresses;
            }
            set { this.adminsEmailAddresses = value; }
        }

        [DataMember]
        public string[] UsersEmailAddresses
        {
            get
            {
                if (this.usersEmailAddresses == null)
                    this.usersEmailAddresses = new string[0];

                return this.usersEmailAddresses;
            }
            set { this.usersEmailAddresses = value; }
        }

        [DataMember]
        public IList<CompanyProfileFieldPoco> CompanyFields
        {
            get
            {
                if (this.companyFields == null)
                    this.companyFields = new List<CompanyProfileFieldPoco>();

                return this.companyFields;
            }
            set { this.companyFields = value; }
        }

        [DataMember]
        public IList<LinkedInCompanyDataModel> LinkedInCompanies
        {
            get
            {
                if (this.linkedInCompanies == null)
                    this.linkedInCompanies = new List<LinkedInCompanyDataModel>();

                return this.linkedInCompanies;
            }
            set { this.linkedInCompanies = value; }
        }

        [DataMember]
        public IList<Tag2Model> CompanyTags
        {
            get
            {
                if (this.companyTags == null)
                    this.companyTags = new List<Tag2Model>();

                return this.companyTags;
            }
            set { this.companyTags = value; }
        }

        public void UpdateFrom(IList<LinkedInCompanyResult> items)
        {
            foreach (var item in items)
            {
                var company = new LinkedInCompanyDataModel();
                company.CompanyFields = new List<CompanyProfileFieldPoco>();
                company.Identifier = Guid.NewGuid();

                company.Company = new CompanyPoco();
                company.Company.Name = item.CompanyEntity.Name;

                var fields = item.Changes;
                foreach (var field in fields)
                {
                    var fieldPoco = new CompanyProfileFieldPoco();
                    fieldPoco.ProfileFieldId = (int)field.Item.Type;
                    fieldPoco.Value = field.Item.Value;
                    fieldPoco.DateCreatedUtc = DateTime.UtcNow;
                    fieldPoco.Source = (byte)field.Item.Source;
                    fieldPoco.Data = field.Item.Data;
                    company.CompanyFields.Add(fieldPoco);
                }

                company.EmailDomains = item.EmailDomains;

                this.LinkedInCompanies.Add(company);
            }
        }
    }

    [DataContract]
    public class LinkedInCompanyDataModel
    {
        [DataMember]
        public CompanyPoco Company { get; set; }

        [DataMember]
        public IList<CompanyProfileFieldPoco> CompanyFields { get; set; }

        [DataMember]
        public string[] EmailDomains { get; set; }

        [DataMember]
        public Guid Identifier { get; set; }
    }

    [DataContract]
    public class ApplyRequestProcessDataModel
    {
        [DataMember]
        public short? CompanyCategoryId { get; set; }

        [DataMember]
        public IList<CompanyRelationshipPoco> CompanyRelationships { get; set; }

        [DataMember]
        public int? IsApprovedBy { get; set; }
    }

    ////public class JsonCollectionConverter : JsonConverter
    ////{
    ////    public override bool CanConvert(Type objectType)
    ////    {
    ////        return objectType.GetGenericTypeDefinition() == typeof(FixupCollection<>);
    ////    }

    ////    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    ////    {
    ////        throw new NotImplementedException();
    ////    }

    ////    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    ////    {
    ////        if (
    ////        writer.WriteNull();
    ////    }
    ////}
}
