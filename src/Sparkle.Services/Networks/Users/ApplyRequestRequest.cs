
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks;
    using Sparkle.Helpers;
    using Sparkle.Services.Networks.Attributes;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Resources;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    public class ApplyRequestRequest : BaseRequest
    {
        public ApplyRequestRequest()
        {
            this.AvailableTimezones = new Dictionary<string, string> { { "", "" }, };
            TimeZoneInfo.GetSystemTimeZones().ToList().ForEach(o => { this.AvailableTimezones.Add(o.Id, o.DisplayName); });
        }

        public Guid Key { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(NetworksLabels))]
        public NetworkUserGender Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required"), DataType(DataType.Text)]
        [Display(Name = "Firstname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Firstname { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required"), DataType(DataType.Text)]
        [Display(Name = "Lastname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Lastname { get; set; }

        [DataType(DataType.PhoneNumber)]
        [PhoneNumber, PhoneUIHint]
        [Display(Name = "Phone", ResourceType = typeof(NetworksLabels))]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required"), EmailAddressEx, DataType(DataType.EmailAddress)]
        [Display(Name = "ProEmail", ResourceType = typeof(NetworksLabels))]
        public string Email { get; set; }

        [EmailAddressEx, DataType(DataType.EmailAddress)]
        [Display(Name = "PersoEmail", ResourceType = typeof(NetworksLabels))]
        public string PersonalEmail { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(NetworksLabels))]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string City { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Country", ResourceType = typeof(NetworksLabels))]
        public string Country { get; set; }
        public IList<RegionInfo> Countries { get; set; }

        [Display(Name = "JobTitle", ResourceType = typeof(NetworksLabels))]
        public int JobId { get; set; }
        public IList<JobModel> Jobs { get; set; }

        [TwitterUsername]
        [DataType(DataType.Text)]
        [Display(Name = "TwitterAccount", ResourceType = typeof(NetworksLabels))]
        public string Twitter { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [DataType(DataType.Url)]
        [Display(Name = "LinkedInPublicUrl", ResourceType = typeof(NetworksLabels))]
        public string LinkedInPublicUrl { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "About", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string About { get; set; }

        [Display(Name = "Industry", ResourceType = typeof(NetworksLabels))]
        public string Industry { get; set; }
        public IList<IndustryModel> Industries { get; set; }

        public string CompanyId { get; set; }

        public CreateCompanyRequest CreateCompanyRequest { get; set; }

        public string UserRemoteAddress { get; set; }

        public ApplyRequestModel Model { get; set; }

        [Timezone]
        [Display(Name = "Timezone", ResourceType = typeof(NetworksLabels))]
        public string Timezone { get; set; }

        [CultureInfo]
        [Display(Name = "Culture_", ResourceType = typeof(NetworksLabels))]
        public string Culture { get; set; }

        public IDictionary<string, string> AvailableCultures { get; set; }

        public IDictionary<string, string> AvailableTimezones { get; set; }

        public string ApplyProfilePictureUrl { get; set; }

        public string CompanyCategory { get; set; }

        public bool IsLinkedInConfigured { get; set; }

        public string InviterCode { get; set; }
    }


    public class ApplyRequestResult : BaseResult<ApplyRequestRequest, ApplyRequestError>
    {
        public ApplyRequestResult(ApplyRequestRequest request)
            : base(request)
        {
        }

        public bool Submitted { get; set; }

        public ApplyRequestModel Model { get; set; }

        public string EmailErrorMessage { get; set; }
    }

    public enum ApplyRequestError
    {
        ErrorFromLinkedIn,
        EmailAddressAlreadyInUse,
        BadCompanyId,
        CompanyIsDisabled,
        RequestIsNotNew,
        RegisterCompanyMissconfigured,
    }
}
