
namespace Sparkle.Services.Networks.Team
{
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EditNetworkRoleRequest : BaseRequest
    {
        public string Login { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string PictureUrl { get; set; }

        [Display(Name = "Role", ResourceType = typeof(NetworksLabels))]
        [StringLength(120, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string RoleTitle { get; set; }

        [Display(Name = "Description", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string RoleDescription { get; set; }

        public string[] ExistingRoleNames { get; set; }

        [Display(Name = "Group", ResourceType = typeof(NetworksLabels))]
        public int? ActualGroup { get; set; }

        public Dictionary<int, string> Groups { get; set; }

        public string DisplayName
        {
            get { return this.Firstname + " " + this.Lastname; }
        }

        public bool IsNewRole
        {
            get { return !this.ActualGroup.HasValue; }
        }
    }

    public class EditNetworkRoleResult : BaseResult<EditNetworkRoleRequest, EditNetworkRoleError>
    {
        public EditNetworkRoleResult(EditNetworkRoleRequest request)
            : base(request)
        {
        }
    }

    public enum EditNetworkRoleError
    {
        NoSuchUser,
        RoleTitleTooLong,
        RoleDescriptionTooLong,
        MustSpecifyGroup
    }
}
