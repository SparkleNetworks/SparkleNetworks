
namespace Sparkle.ApiClients.Users
{
    using Sparkle.ApiClients.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class UserModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string EmailValue { get; set; }

        [IgnoreDataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public bool? AccountValidated { get; set; }

        [DataMember]
        public object CompanyAccessLevel { get; set; }

        [DataMember]
        public object NetworkAccessLevel { get; set; }

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
        public string Gender { get; set; }

        [DataMember]
        public int CompanyId { get; set; }

        public Sparkle.ApiClients.Companies.CompanyModel Company { get; set; }

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

        ////public IList<Subscriptions.SubscriptionModel> Subscriptions { get; set; }

        [DataMember]
        public string SmallProfilePictureUrl { get; set; }

        [DataMember]
        public string MediumProfilePictureUrl { get; set; }
    }
}
