
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AdminProceduresRequest : BaseRequest
    {
        public AdminProceduresRequest()
            : base()
        {
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [Display(Name = "NewEmail", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]        
        public string Email { get; set; }

        public string Login { get; set; }

        public bool HasAlreadyPending { get; set; }

        public bool OverwritePending { get; set; }

        public string ActualEmail { get; set; }

        [Display(Name = "ForbidNewEmail", ResourceType = typeof(NetworksLabels))]
        public bool ForbidNewEmail { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Remark { get; set; }

        public string PendingEmail { get; set; }

        public string PendingRemark { get; set; }

        public object PasswordRecoveryLink { get; set; }

        public int NewCompanyId { get; set; }

        public string ActualCompany { get; set; }

        public AdminProceduresPostAction PostAction { get; set; }

        public Dictionary<int, string> Companies { get; set; }

        public Entities.Networks.CompanyAccessLevel NewRight { get; set; }

        public Dictionary<Entities.Networks.CompanyAccessLevel, string> CompaniesRights { get; set; }
    }

    public enum AdminProceduresPostAction
    {
        None,
        ChangeEmail,
        ChangeCompany,
    }
}
