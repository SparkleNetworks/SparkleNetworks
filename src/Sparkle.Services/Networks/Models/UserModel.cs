
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Data.Filters;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class UserModel : IAspectObject
    {
        private readonly AspectList root;

        public UserModel(Sparkle.Entities.Networks.Neutral.Person user)
            : this()
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.Id = user.Id == 0 ? user.ShortId : user.Id;
            this.Email = string.IsNullOrWhiteSpace(user.Email) ? null : new EmailAddress(user.Email);
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.UserId = user.UserId;
            this.Username = user.Username;
            this.Culture = user.Culture;
            this.Timezone = user.Timezone;
            this.PictureName = user.PictureName;

            this.CompanyId = user.CompanyId;
            this.Company = new CompanyModel(user.CompanyId, user.CompanyName, user.CompanyAlias);

            this.JobId = user.JobId;
            if (user.JobId != null)
            {
                this.Job = new JobModel(user.JobId.Value, user.JobName, user.JobAlias);
            }

            this.CompanyAccessLevel = (CompanyAccessLevel)user.CompanyAccessLevel;
            this.NetworkAccessLevel = (NetworkAccessLevel)user.NetworkAccessLevel;
            this.IsEmailConfirmed = user.IsEmailConfirmed;

            this.IsActive = IsUserActive(null, user.NetworkAccessLevel, user.CompanyAccessLevel, user.CompanyIsEnabled, user.IsEmailConfirmed);
        }

        public UserModel(int id)
            : this()
        {
            this.Id = id;
        }

        public UserModel()
        {
            this.root = new AspectList(this.GetType());
        }

        public UserModel(Sparkle.Entities.Networks.User user, IList<UserProfileFieldModel> profileFields = null)
            : this()
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.Id = user.Id;
            this.Email = string.IsNullOrWhiteSpace(user.Email) ? null : new EmailAddress(user.Email);
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.UserId = user.UserId;
            this.Username = user.Username;
            this.Login = user.Login;
            this.CompanyAccessLevel = (CompanyAccessLevel)user.CompanyAccessLevel;
            this.NetworkAccessLevel = (NetworkAccessLevel)user.NetworkAccessLevel;
            this.AccountClosed = user.AccountClosed;
            this.IsEmailConfirmed = user.IsEmailConfirmed;
            this.NetworkId = user.NetworkId;
            this.Culture = user.Culture;
            this.Timezone = user.Timezone;
            this.PictureName = user.Picture;

            switch (this.CompanyAccessLevel)
            {
                case Entities.Networks.CompanyAccessLevel.Disabled:
                    this.CompanyAccessLabel = "Desactivé";
                    break;
                case Entities.Networks.CompanyAccessLevel.Administrator:
                    this.CompanyAccessLabel = "Administrateur";
                    break;
                case Entities.Networks.CompanyAccessLevel.CommunityManager:
                    this.CompanyAccessLabel = "Community Manager";
                    break;
                default:
                    this.CompanyAccessLabel = "Utilisateur";
                    break;
            }

            switch (this.NetworkAccessLevel)
            {
                case Entities.Networks.NetworkAccessLevel.Disabled:
                    this.NetworkAccessLabel = "Desactivé";
                    break;
                case Entities.Networks.NetworkAccessLevel.User:
                    this.NetworkAccessLabel = "Utilisateur";
                    break;
                default:
                    this.NetworkAccessLabel = "Certains droits d'administrateur";
                    break;
            }

            if (user.Gender.HasValue)
                this.Gender = (NetworkUserGender)user.Gender;
            else
                this.Gender = NetworkUserGender.Unspecified;
            this.GenderName = this.Gender == 0 ? "Homme" : "Femme";
            this.IsFriendWithCurrentId = user.IsFriendWithCurrentId;

            // Profile fields
            if (profileFields != null)
            {
                this.SetProfileFields(profileFields);
            }

            this.Email = user.Email;
            this.PersonalEmail = user.PersonalEmail;
            this.JobId = user.JobId;
            if (user.JobId.HasValue && user.JobReference.IsLoaded)
            {
                this.JobName = user.Job.Libelle;
                this.JobAlias = user.Job.Alias;
            }

            if (this.HasPicture && !string.IsNullOrEmpty(this.About) && this.About.Length > 300)
            {
                this.CompleteProfile = true;
            }

            if (user.CreatedDateUtc.HasValue)
            {
                this.CreateDateUtc = user.CreatedDateUtc.Value.AsUtc();
            }

            this.CompanyId = user.CompanyID;
            if (user.CompanyID != 0 && user.CompanyReference.IsLoaded)
            {
                this.CompanyName = user.Company.Name;
                this.CompanyAlias = user.Company.Alias;

                this.IsActive = IsUserActive(user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, user.Company.IsEnabled, user.IsEmailConfirmed);
            }
        }

        public UserModel(Sparkle.Entities.Networks.GetExportableListOfUsers_Result item)
            : this()
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Id = item.Id;
            this.Email = item.Email;
            this.Username = item.Login;

            this.AccountValidated = item.AccountClosed.HasValue ? !item.AccountClosed.Value : default(bool?);
            this.CompanyAccessLevel = (CompanyAccessLevel)item.CompanyAccessLevel;
            this.NetworkAccessLevel = (NetworkAccessLevel)item.NetworkAccessLevel;
            this.IsEmailConfirmed = item.IsEmailConfirmed;

            this.CreateDateUtc = item.CreateDate.HasValue ? item.CreateDate.Value.ToUniversalTime() : default(DateTime?);
            ////this.Birthday = u.Birthday;
            this.FirstName = item.FirstName;
            this.LastActivityUtc = item.LastActivity.HasValue ? item.LastActivity.Value.ToUniversalTime() : default(DateTime?);
            this.LastLoginDateUtc = item.LastLoginDate.HasValue ? item.LastLoginDate.Value.ToUniversalTime() : default(DateTime?);
            this.LastName = item.LastName;
            this.Phone = item.Phone;
            this.Gender = (NetworkUserGender)item.Gender;

            this.CompanyId = item.CompanyID;
            this.Company = new CompanyModel(item.CompanyID, item.CompanyName, item.CompanyAlias);
            this.Company.IsEnabled = item.CompanyIsEnabled;

            this.JobId = item.JobId;
            this.Job = item.JobId != null ? new JobModel(item.JobId.Value, item.JobName, item.JobAlias) : null;

            this.Data = new Dictionary<string, object>();
            this.Data.Add("CurrentActiveSubscriptions", item.CurrentActiveSubscriptions);

            this.IsActive = IsUserActive(item.AccountClosed, item.NetworkAccessLevel, item.CompanyAccessLevel, item.CompanyIsEnabled, item.IsEmailConfirmed);
            this.PersonalDataUpdateDateUtc = item.PersonalDataUpdateDateUtc.AsUtc();
        }

        public UserModel(UsersView user)
            : this()
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.Id = user.Id;
            this.Email = string.IsNullOrWhiteSpace(user.Email) ? null : new EmailAddress(user.Email);
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.UserId = user.UserId;
            this.Username = user.Login;
            this.Login = user.Login;
            this.CompanyAccessLevel = (CompanyAccessLevel)user.CompanyAccessLevel;
            this.NetworkAccessLevel = (NetworkAccessLevel)user.NetworkAccessLevel;
            this.AccountClosed = user.AccountClosed;
            this.IsEmailConfirmed = user.IsEmailConfirmed;
            this.NetworkId = user.NetworkId;
            this.Culture = user.Culture;
            this.Timezone = user.Timezone;
            this.CreateDateUtc = user.DateCreatedUtc.AsUtc();
            this.PictureName = user.Picture;

            switch (this.CompanyAccessLevel)
            {
                case Entities.Networks.CompanyAccessLevel.Disabled:
                    this.CompanyAccessLabel = "Desactivé";
                    break;
                case Entities.Networks.CompanyAccessLevel.Administrator:
                    this.CompanyAccessLabel = "Administrateur";
                    break;
                case Entities.Networks.CompanyAccessLevel.CommunityManager:
                    this.CompanyAccessLabel = "Community Manager";
                    break;
                default:
                    this.CompanyAccessLabel = "Utilisateur";
                    break;
            }

            switch (this.NetworkAccessLevel)
            {
                case Entities.Networks.NetworkAccessLevel.Disabled:
                    this.NetworkAccessLabel = "Desactivé";
                    break;
                case Entities.Networks.NetworkAccessLevel.User:
                    this.NetworkAccessLabel = "Utilisateur";
                    break;
                default:
                    this.NetworkAccessLabel = "Certains droits d'administrateur";
                    break;
            }

            this.Gender = (NetworkUserGender)user.Gender;
            this.GenderName = this.Gender == 0 ? "Homme" : "Femme";
            this.Email = user.Email;
            this.CompanyId = user.CompanyId;

            this.JobId = user.JobId;
            this.JobName = user.Job_Name;
            this.JobAlias = user.JobAlias;
            this.Job = user.JobId == null ? JobModel.NoJob : new JobModel(user.JobId.Value, user.Job_Name, user.JobAlias);

            this.CompanyId = user.CompanyId;
            this.CompanyAlias = user.Company_Alias;
            this.CompanyName = user.Company_Name;
            this.Company = new CompanyModel(user.CompanyId, user.Company_Name, user.Company_Alias)
            {
                IsActive = user.Company_IsEnabled && user.Company_IsApproved,
                IsApproved = user.Company_IsApproved,
                IsEnabled = user.Company_IsEnabled,
            };

            if (this.HasPicture && !string.IsNullOrEmpty(this.About) && this.About.Length > 300)
                this.CompleteProfile = true;

            this.IsActive = UserModel.IsUserActive(this.AccountClosed, (int)user.NetworkAccessLevel, (int)user.CompanyAccessLevel, user.Company_IsEnabled, user.IsEmailConfirmed);
        }

        public UserModel(Entities.Networks.Neutral.UserPoco user, IList<UserProfileFieldModel> profileFields = null)
            : this()
        {
            if (user == null)
                throw new ArgumentNullException("user");

            this.Id = user.Id;
            this.Email = string.IsNullOrWhiteSpace(user.Email) ? null : new EmailAddress(user.Email);
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.UserId = user.UserId;
            this.Username = user.Login;
            this.Login = user.Login;
            this.CompanyAccessLevel = (CompanyAccessLevel)user.CompanyAccessLevel;
            this.NetworkAccessLevel = (NetworkAccessLevel)user.NetworkAccessLevel;
            this.AccountClosed = user.AccountClosed;
            this.IsEmailConfirmed = user.IsEmailConfirmed;
            this.NetworkId = user.NetworkId;
            this.Culture = user.Culture;
            this.Timezone = user.Timezone;
            this.PictureName = user.Picture;

            switch (this.CompanyAccessLevel)
            {
                case Entities.Networks.CompanyAccessLevel.Disabled:
                    this.CompanyAccessLabel = "Desactivé";
                    break;
                case Entities.Networks.CompanyAccessLevel.Administrator:
                    this.CompanyAccessLabel = "Administrateur";
                    break;
                case Entities.Networks.CompanyAccessLevel.CommunityManager:
                    this.CompanyAccessLabel = "Community Manager";
                    break;
                default:
                    this.CompanyAccessLabel = "Utilisateur";
                    break;
            }

            switch (this.NetworkAccessLevel)
            {
                case Entities.Networks.NetworkAccessLevel.Disabled:
                    this.NetworkAccessLabel = "Desactivé";
                    break;
                case Entities.Networks.NetworkAccessLevel.User:
                    this.NetworkAccessLabel = "Utilisateur";
                    break;
                default:
                    this.NetworkAccessLabel = "Certains droits d'administrateur";
                    break;
            }

            if (user.Gender.HasValue)
                this.Gender = (NetworkUserGender)user.Gender;
            else
                this.Gender = NetworkUserGender.Unspecified;
            this.GenderName = this.Gender == 0 ? "Homme" : "Femme";

            // Profile fields
            if (profileFields != null)
            {
                this.SetProfileFields(profileFields);
            }

            this.Email = user.Email;
            this.CompanyId = user.CompanyID;
            this.JobId = user.JobId;

            if (this.HasPicture && !string.IsNullOrEmpty(this.About) && this.About.Length > 300)
            {
                this.CompleteProfile = true;
            }

            if (user.CreatedDateUtc.HasValue)
            {
                this.CreateDateUtc = user.CreatedDateUtc.Value.AsUtc();
            }

            if (user.Company != null)
            {
                this.CompanyName = user.Company.Name;
                this.CompanyAlias = user.Company.Alias;

                this.IsActive = IsUserActive(user.AccountClosed, user.NetworkAccessLevel, user.CompanyAccessLevel, user.Company.IsEnabled, user.IsEmailConfirmed);
            }
        }

        [DataMember]
        public int Id { get; set; }

        [IgnoreDataMember]
        public EmailAddress Email { get; set; }

        [DataMember]
        public string EmailValue
        {
            get { return this.Email != null ? this.Email.Value : null; }
            set { this.Email = value != null ? new EmailAddress(value) : null; }
        }

        [IgnoreDataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public bool? AccountValidated { get; set; }

        [DataMember]
        public CompanyAccessLevel? CompanyAccessLevel { get; set; }

        [DataMember]
        public NetworkAccessLevel? NetworkAccessLevel { get; set; }

        public bool? IsEmailConfirmed { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string DisplayName
        {
            get { return this.FirstName + " " + this.LastName; }
            set { }
        }

        [DataMember]
        public DateTime? CreateDateUtc { get; set; }

        public DateTime? Birthday { get; set; }

        public DateTime? LastActivityUtc { get; set; }

        public DateTime? LastLoginDateUtc { get; set; }

        public string Phone { get; set; }

        [DataMember]
        public NetworkUserGender Gender { get; set; }

        [DataMember]
        public int CompanyId { get; set; }

        public CompanyModel Company { get; set; }

        public int? JobId { get; set; }

        public JobModel Job { get; set; }

        public bool IsLockedOut { get; set; }

        public string Login { get; set; }

        public bool? AccountClosed { get; set; }

        public string CompanyAccessLabel { get; set; }

        public string NetworkAccessLabel { get; set; }

        public bool HasPicture
        {
            get { return !string.IsNullOrEmpty(this.Picture); }
        }

        public string GenderName { get; set; }

        public bool? IsFriendWithCurrentId { get; set; }

        public string About { get; set; }

        public string FavoriteQuotes { get; set; }

        public string CurrentTarget { get; set; }

        public string Contribution { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public string Site { get; set; }

        public string Twitter { get; set; }

        [DataMember]
        public string JobName { get; set; }

        public bool CompleteProfile { get; set; }

        [DataMember]
        public string JobAlias { get; set; }

        [DataMember]
        public string CompanyAlias { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        public string SkillsString { get; set; }

        public string InterestsString { get; set; }

        public string RecreationsString { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        public string PersonalEmail { get; set; }

        public string Picture { get; set; }

        [DataMember]
        public string Culture { get; set; }

        [DataMember]
        public string Timezone { get; set; }

        public string Country { get; set; }

        public string Headline { get; set; }

        public string ContactGuideline { get; set; }

        public string PictureName { get; set; }

        public IDictionary<string, object> Data { get; set; }

        public IList<Subscriptions.SubscriptionModel> Subscriptions { get; set; }

        [DataMember]
        public string SmallProfilePictureUrl { get; set; }

        [DataMember]
        public string MediumProfilePictureUrl { get; set; }

        public AspectList AspectManager
        {
            get { return this.root; }
        }

        public int? ProfileFieldsCount { get; set; }

        public DateTime? PersonalDataUpdateDateUtc { get; set; }

        public static bool IsUserAuthorized(bool? AccountClosed, int NetworkAccessLevel, int CompanyAccessLevel, bool isCompanyEnabled)
        {
            return AccountClosed != true && NetworkAccessLevel > 0 && CompanyAccessLevel > 0 && isCompanyEnabled;
        }

        public static bool IsUserActive(bool? AccountClosed, int NetworkAccessLevel, int CompanyAccessLevel, bool isCompanyEnabled, bool IsEmailConfirmed)
        {
            return IsUserAuthorized(AccountClosed, NetworkAccessLevel, CompanyAccessLevel, isCompanyEnabled) && IsEmailConfirmed;
        }

        public void SetProfileFields(IList<UserProfileField> fields)
        {
            this.SetProfileFields(fields.Select(f => new UserProfileFieldModel(f)));
        }

        public void SetProfileFields(IEnumerable<UserProfileFieldModel> fields)
        {
            ////var profileFields = fields;
            ////var fields = profileFields.ToDictionary(o => o.Type, o => o.Value);
            ////this.About = fields.ContainsKey(ProfileFieldType.About) ? fields[ProfileFieldType.About] : null;
            ////this.FavoriteQuotes = fields.ContainsKey(ProfileFieldType.FavoriteQuotes) ? fields[ProfileFieldType.FavoriteQuotes] : null;
            ////this.CurrentTarget = fields.ContainsKey(ProfileFieldType.CurrentTarget) ? fields[ProfileFieldType.CurrentTarget] : null;
            ////this.Contribution = fields.ContainsKey(ProfileFieldType.Contribution) ? fields[ProfileFieldType.Contribution] : null;
            ////this.City = fields.ContainsKey(ProfileFieldType.City) ? fields[ProfileFieldType.City] : null;
            ////this.ZipCode = fields.ContainsKey(ProfileFieldType.ZipCode) ? fields[ProfileFieldType.ZipCode] : null;
            ////this.Phone = fields.ContainsKey(ProfileFieldType.Phone) ? fields[ProfileFieldType.Phone] : null;
            ////this.Site = fields.ContainsKey(ProfileFieldType.Site) ? fields[ProfileFieldType.Site] : null;

            foreach (var field in fields)
            {
                switch (field.Type)
                {
                    case ProfileFieldType.Site:
                        this.Site = field.Value;
                        break;

                    case ProfileFieldType.Phone:
                        this.Phone = field.Value;
                        break;

                    case ProfileFieldType.About:
                        this.About = field.Value;
                        break;

                    case ProfileFieldType.City:
                        this.City = field.Value;
                        break;

                    case ProfileFieldType.ZipCode:
                        this.ZipCode = field.Value;
                        break;

                    case ProfileFieldType.FavoriteQuotes:
                        this.FavoriteQuotes = field.Value;
                        break;

                    case ProfileFieldType.CurrentTarget:
                        this.CurrentTarget = field.Value;
                        break;

                    case ProfileFieldType.Contribution:
                        this.Contribution = field.Value;
                        break;

                    case ProfileFieldType.Country:
                        this.Country = field.Value;
                        break;

                    case ProfileFieldType.Headline:
                        this.Headline = field.Value;
                        break;

                    case ProfileFieldType.ContactGuideline:
                        this.ContactGuideline = field.Value;
                        break;

                    // Lang.T is not allowed here :-/
                    ////case ProfileFieldType.Industry:
                    ////    this.Industry = Lang.T("Industry: " + field.Value);
                    ////    break;
                }
            }
        }

        public int CountFields()
        {
            int count = 0;

            if (this.About != null && this.About.Length > 100)
                count += 1;
            if (this.City != null && this.City.Length > 0)
                count += 1;
            if (this.Contribution != null && this.Contribution.Length > 40)
                count += 1;
            if (this.HasPicture)
                count += 1;
            if (this.Headline != null && this.Headline.Length > 8)
                count += 1;
            if (this.PersonalEmail != null && this.PersonalEmail.Length > 8)
                count += 1;
            if (this.Phone != null && this.Phone.Length > 8)
                count += 1;
            if (this.Site != null && this.Site.Length > 8)
                count += 1;
            if (this.Twitter != null && this.Twitter.Length > 2)
                count += 1;
            
            return count;
        }

        public override string ToString()
        {
            return "UserModel Id=" + this.Id + " " + this.DisplayName;
        }
    }

    public class TinyUserModel
    {
        public string PictureUrl { get; set;  }

        public string FullName { get; set; }

        public string Login { get; set; }

        public string ProfileUrl { get; set; }

        public int Id { get; set; }

        public bool IsDisplayed { get; set; }

        public DateTime? DateUtc { get; set; }
    }

    public static class UserModelExtensions
    {
        public static Sparkle.Entities.Networks.Neutral.SimpleContact GetSimpleContact(this UserModel user)
        {
            return new Entities.Networks.Neutral.SimpleContact
            {
                Firstname = user.FirstName,
                Lastname = user.LastName,
                EmailAddress = user.Email.OriginalString,
                Username = user.Username,
            };
        }

        public static Sparkle.Entities.Networks.EmailContact GetEmailContact(this UserModel user)
        {
            return new Entities.Networks.EmailContact(user.Email.OriginalString, user.DisplayName);
        }
    }
}
