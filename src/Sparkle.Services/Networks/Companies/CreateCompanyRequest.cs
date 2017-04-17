
namespace Sparkle.Services.Networks.Companies
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks.Attributes;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Resources;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class CreateCompanyRequest : BaseRequest, IValidatableObject
    {
        public CreateCompanyRequest()
        {
        }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Name", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(Name = "Baseline", ResourceType = typeof(NetworksLabels))]
        public string Baseline { get; set; }

        public short CategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "About", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string About { get; set; }

        [Display(Name = "Website", ResourceType = typeof(NetworksLabels))]
        public string Website { get; set; }

        [PhoneNumber, PhoneUIHint]
        [Display(Name = "Phone", ResourceType = typeof(NetworksLabels))]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "ContactEmail", ResourceType = typeof(NetworksLabels))]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "CompanyRegistration_AdminEmails", ResourceType = typeof(NetworksLabels))]
        [EmailAddressEx(AllowMultiple = true, MinimumAddresses = 0)]
        public string AdminEmails { get; set; }

        [Display(Name = "CompanyRegistration_OtherEmails", ResourceType = typeof(NetworksLabels))]
        [EmailAddressEx(AllowMultiple = true)]
        public string OtherEmails { get; set; }

        [Display(Name = "CompanyRegistration_IsApproved_Name", Description = "CompanyRegistration_IsApproved_Description", ResourceType = typeof(NetworksLabels))]
        public bool IsApproved { get; set; }

        public bool CanApprove { get; set; }

        [Display(Name = "CompanyRegistration_EmailDomain_Name", Description = "CompanyRegistration_EmailDomain_Description", ResourceType = typeof(NetworksLabels))]
        public string EmailDomain { get; set; }

        [Display(Name = "CompanyRegistration_Alias_Name", Description = "CompanyRegistration_Alias_Description", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Alias { get; set; }

        public string ReturnUrl { get; set; }

        public bool CanSetEmailDomain { get; set; }

        public IList<Models.CompanyCategoryModel> AvailableCategories { get; set; }

        public Guid? CompanyRequestId { get; set; }

        public Guid? CreateRequestUniqueId { get; set; }

        public bool IsAuthor { get; set; }

        public int? UserId { get; set; }

        // Used in apply request <!--
        public bool IsFromApplyRequest { get; set; }

        public Tags.AjaxTagPickerModel AjaxTagPicker { get; set; }
        // -->

        public void UpdateFrom(Sparkle.Entities.Networks.CompanyRequest item)
        {
            this.CreateRequestUniqueId = item.UniqueId != Guid.Empty ? item.UniqueId : default(Guid?);

            this.Name = item.Name;
            this.Alias = item.Alias;
            this.About = item.About;
            this.Email = item.Email;
            this.EmailDomain = item.EmailDomain;
            this.Baseline = item.Baseline;
            this.CategoryId = item.CategoryId;
            this.Phone = item.Phone;
            this.Website = item.Website;

            this.AdminEmails = item.AdminEmails;
            this.OtherEmails = item.OtherEmails;
        }

        protected override void ValidateFields()
        {
            if (!this.IsFromApplyRequest && string.IsNullOrWhiteSpace(this.AdminEmails))
                this.AddValidationError("AdminEmails", "The Admin Emails field is required");
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            this.Validate();

            foreach (var key in this.ValidationErrorList.Keys)
            {
                foreach (var error in this.ValidationErrorList[key])
                {
                    yield return new ValidationResult(error, new string[] { key, }); 
                }
            }
        }
    }

    public class CreateCompanyResult : BaseResult<CreateCompanyRequest, CreateCompanyError>
    {
        public CreateCompanyResult(CreateCompanyRequest request)
            : base(request)
        {
        }

        // TODO: Layer violation: return a CompanyModel instead of a Company
        public Entities.Networks.Company Item { get; set; }
    }

    public enum CreateCompanyError
    {
    }
}
