
namespace Sparkle.Services.Networks.Companies
{
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class ToggleCompanyRequest : BaseRequest
    {
        public ToggleCompanyRequest()
            : base()
        {
        }

        public bool IsEnabled { get; set; }

        [Display(Name = "ToggleCompanyRequest_IsEnabledRemark", ResourceType = typeof(NetworksLabels))]
        [Required(ErrorMessage = "Il vaudrait mieux laisser une remarque pour les autres administrateurs.")]
        public string IsEnabledRemark { get; set; }

        public string CompanyName { get; set; }

        public string CompanyPictureUrl { get; set; }

        public DateTime? LastChangeDateUtc { get; set; }

        public string LastChangeUserFullName { get; set; }

        public string LastChangeUserFirstName { get; set; }

        public string CompanyAlias { get; set; }

        public int CurrentUserId { get; set; }
    }
}
