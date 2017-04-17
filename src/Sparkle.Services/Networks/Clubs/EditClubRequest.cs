
namespace Sparkle.Services.Networks.Clubs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Services.Resources;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Attributes;

    public class EditClubRequest : BaseRequest
    {
        public EditClubRequest()
        {
        }

        public EditClubRequest(Entities.Networks.Club item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.Baseline = item.Baseline;
            this.About = item.About;
            this.Website = item.Website;
            this.Phone = item.Phone;
            this.Email = item.Email;
            this.CreatedByUserId = item.CreatedByUserId;
        }

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Alias { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Baseline { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string About { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [StringLength(120, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Website { get; set; }

        [PhoneNumber, PhoneUIHint]
        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Phone { get; set; }

        [EmailAddressEx]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Email { get; set; }

        public int CreatedByUserId { get; set; }
    }
}
