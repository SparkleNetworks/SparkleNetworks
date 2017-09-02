
namespace Sparkle.Services.Networks.PartnerResources
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Attributes;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Objects;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    public class PartnerResourceEditRequest : BaseRequest
    {
        public int? PartnerId { get; set; }

        public int UserId { get; set; }

        public IList<Tag2Model> TagModels { get; set; }

        public DateTime? DateDeletedUtc { get; set; }

        public string Alias { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "ConcernedCities", ResourceType = typeof(NetworksLabels))]
        public string Cities { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Category", ResourceType = typeof(NetworksLabels))]
        public string Category { get; set; }
        public string CategoryTitle { get; set; }
        public IList<Tag2Model> Categories { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Title", ResourceType = typeof(NetworksLabels))]
        [StringLength(140, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        ////[Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")] // removed for a customer
        [Sparkle.Common.DataAnnotations.Url]
        [Display(Name = "Website", ResourceType = typeof(NetworksLabels))]
        public string Website { get; set; }

        [Display(Name = "Industry", ResourceType = typeof(NetworksLabels))]
        public int Industry { get; set; }
        public string IndustryTitle { get; set; }
        public IList<IndustryModel> Industries { get; set; }

        [Display(Name = "Address", ResourceType = typeof(NetworksLabels))]
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Address { get; set; }

        [Display(Name = "Zip", ResourceType = typeof(NetworksLabels))]
        [StringLength(5, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string ZipCode { get; set; }

        [Display(Name = "City", ResourceType = typeof(NetworksLabels))]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string City { get; set; }

        [Display(Name = "Country", ResourceType = typeof(NetworksLabels))]
        public string Country { get; set; }

        public IList<RegionInfo> Countries { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Firstname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "Lastname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string LastName { get; set; }

        ////[Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")] // removed for a customer
        [Display(Name = "JobTitle", ResourceType = typeof(NetworksLabels))]
        [StringLength(140, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Job { get; set; }

        [SrkToolkit.DataAnnotations.PhoneNumber, PhoneUIHint]
        ////[Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")] // removed for a customer
        [Display(Name = "Phone", ResourceType = typeof(NetworksLabels))]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(NetworksLabels))]
        public string Email { get; set; }

        [Display(Name = "ContactGuideline", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string ContactGuideline { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(Name = "About", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string About { get; set; }

        [Display(Name = "IsActive", ResourceType = typeof(NetworksLabels))]
        public bool IsAvailable { get; set; }

        public PartnerResourceEditRequest()
        {
            this.IsAvailable = true;
        }

        public void UpdateFrom(PartnerResource item, IList<PartnerResourceProfileField> fields)
        {
            this.PartnerId = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.IsAvailable = item.Available;
            this.IsApproved = item.IsApproved;
            this.DateDeletedUtc = item.DateDeletedUtc;

            foreach (var field in fields)
            {
                switch (field.ProfileFieldType)
                {
                    case ProfileFieldType.Site:
                        this.Website = field.Value;
                        break;
                    case ProfileFieldType.Industry:
                        if (this.Industries != null)
                            this.Industry = this.Industries.Where(o => o != null && o.Value == field.Value).Single().SelecterId;
                        this.IndustryTitle = field.Value;
                        break;
                    case ProfileFieldType.Location:
                        this.Address = field.Value;
                        break;
                    case ProfileFieldType.ZipCode:
                        this.ZipCode = field.Value;
                        break;
                    case ProfileFieldType.City:
                        this.City = field.Value;
                        break;
                    case ProfileFieldType.Country:
                        this.Country = field.Value;
                        break;
                    case ProfileFieldType.Contact:
                        var model = new PartnerResourceProfileFieldModel(field);
                        this.FirstName = model.ContactModel.FirstName;
                        this.LastName = model.ContactModel.LastName;
                        this.Job = model.ContactModel.Job;
                        this.Phone = model.ContactModel.Phone;
                        this.Email = model.ContactModel.Email;
                        break;
                    case ProfileFieldType.About:
                        this.About = field.Value;
                        break;
                    case ProfileFieldType.ContactGuideline:
                        this.ContactGuideline = field.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public StatsCounterHitLink FollowCounter { get; set; }

        public StatsCounterHitLink DisplayCounter { get; set; }

        public string PictureUrl { get; set; }

        public bool IsApproved { get; set; }

        public UserModel CreatedBy { get; set; }

        public UserModel ApprovedBy { get; set; }
    }

    public class PartnerResourceEditResult : BaseResult<PartnerResourceEditRequest, PartnerResourceEditError>
    {
        public PartnerResourceEditResult(PartnerResourceEditRequest request)
            : base(request)
        {
        }

        public string Alias { get; set; }

        public bool ToApprove { get; set; }
    }

    public enum PartnerResourceEditError
    {
        NoSuchUserOrInactiveOrUnauthorized,
        NoSuchPartnerResource,
        CityIsRequired
    }
}
