
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Attributes;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    
    public class ProfileEditRequest : BaseRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Firstname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Lastname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string LastName { get; set; }

        [Display(Name = "Identifiant")]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Login { get; set; }

        [DateTimeUIHint]
        [Display(Name = "Birthday", ResourceType = typeof(NetworksLabels)), DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Photo")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Picture { get; set; }

        [Display(Name = "PersoEmail", ResourceType = typeof(NetworksLabels))]
        public string PersonalEmail { get; set; }

        [Display(Name = "Zip", ResourceType = typeof(NetworksLabels))]
        [StringLength(5, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string ZipCode { get; set; }
        public ProfileFieldSource ZipCodeSource { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "City", ResourceType = typeof(NetworksLabels))]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string City { get; set; }
        public ProfileFieldSource CitySource { get; set; }

        [SrkToolkit.DataAnnotations.PhoneNumber, PhoneUIHint]
        [Display(Name = "Phone", ResourceType = typeof(NetworksLabels))]
        public string Phone { get; set; }
        public ProfileFieldSource PhoneSource { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "About", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string About { get; set; }
        public ProfileFieldSource AboutSource { get; set; }

        [Display(Name = "CurrentTarget", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string CurrentTarget { get; set; }
        public ProfileFieldSource CurrentTargetSource { get; set; }

        [Display(Name = "FavoriteQuotes", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FavoriteQuotes { get; set; }
        public ProfileFieldSource FavoriteQuotesSource { get; set; }

        [Display(Name = "Contribution", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Contribution { get; set; }
        public ProfileFieldSource ContributionSource { get; set; }

        [Display(Name = "JobTitle", ResourceType = typeof(NetworksLabels))]
        public int JobId { get; set; }
        public List<JobModel> Jobs { get; set; }
        public string JobLibelle { get; set; }

        [Display(Name = "Skills", ResourceType = typeof(NetworksLabels))]
        public TagsListEditable Skills { get; set; }

        [Display(Name = "Interests", ResourceType = typeof(NetworksLabels))]
        public TagsListEditable Interests { get; set; }

        [Display(Name = "Recreations", ResourceType = typeof(NetworksLabels))]
        public TagsListEditable Recreations { get; set; }

        public string RemoveSkillsString { get; set; }

        public string RemoveInterestsString { get; set; }

        public string RemoveRecreationsString { get; set; }

        public List<string> NewRecreations { get; set; }

        public List<string> NewInterests { get; set; }

        public List<string> NewSkills { get; set; }

        [Display(Name = "ProEmail", ResourceType = typeof(NetworksLabels))]
        public string ProEmail { get; set; }

        [Display(Name = "Industry", ResourceType = typeof(NetworksLabels))]
        public int? IndustryId { get; set; }
        public ProfileFieldSource IndustrySource { get; set; }
        public IList<IndustryModel> Industries { get; set; }
        public string IndustryLibelle { get; set; }

        [Display(Name = "Country", ResourceType = typeof(NetworksLabels))]
        public string CountryId { get; set; }
        public ProfileFieldSource CountrySource { get; set; }
        public IList<RegionInfo> Countries { get; set; }

        [Display(Name = "ContactGuideline", ResourceType = typeof(NetworksLabels))]
        public string ContactGuideline { get; set; }

        public ProfileFieldSource ContactGuidelineSource { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [Display(Name = "LinkedInPublicUrl", ResourceType = typeof(NetworksLabels))]
        public string LinkedInPublicUrl { get; set; }

        public ProfileFieldSource LinkedInPublicUrlSource { get; set; }

        public bool IsLinkedInConfigured { get; set; }

        public IList<string> ProfileFields { get; set; }

        public IList<ProfileFieldModel> AvailableProfileFields { get; set; }

        public IList<UserProfileFieldModel> ProfileFieldValues { get; set; }

        public IList<ProfileFieldAvailableValueModel> ProfileFieldsAvailableValues { get; set; }
    }
}
