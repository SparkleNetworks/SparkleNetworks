
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Resources;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class CreateEmailPassordAccountRequest : BaseRequest
    {
        [Required(ErrorMessageResourceName = "RequiredEmailAddress", ErrorMessageResourceType = typeof(ValidationMessages))]
        [EmailAddressEx]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        [Display(Name = "Email", ResourceType = typeof(NetworksLabels))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "RequiredPassword", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Display(Name = "Password", ResourceType = typeof(NetworksLabels))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Display(Name = "Firstname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Display(Name = "Lastname", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Display(Name = "Gender", ResourceType = typeof(NetworksLabels))]
        public NetworkUserGender Gender { get; set; }

        public Guid? InvitationCode { get; set; }

        public int? JobId { get; set; }

        public IList<JobModel> AvailableJobs { get; set; }

        public RegisterInvitationModel Invitation { get; set; }

        public bool IsEmailAddressReadOnly { get; set; }

        protected override void ValidateFields()
        {
            base.ValidateFields();

            if (this.Password != null && this.Password.Length < 8)
                this.AddValidationError("Password", "Le mot de passe doit contenir au moins 8 caractères.");
        }

        public bool FromApplyRequest { get; set; }

        public int? JoinCompanyId { get; set; }
    }
}
