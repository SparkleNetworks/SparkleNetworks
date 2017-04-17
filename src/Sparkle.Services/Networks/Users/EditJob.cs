
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EditJobRequest : LocalBaseRequest
    {
        public EditJobRequest()
        {
        }

        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(NetworksLabels))]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(80, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(Name = "JobGroupName", ResourceType = typeof(NetworksLabels))]
        [StringLength(80, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string GroupName { get; set; }

        public string[] GroupNames { get; set; }

        public int? UserCount { get; set; }
    }

    public class EditJobResult : BaseResult<EditJobRequest, EditJobError>
    {
        public EditJobResult(EditJobRequest request)
            : base(request)
        {
        }

        public JobModel Item { get; set; }
    }

    public enum EditJobError
    {
        NotAuthorized,
        NoSuchActingUser,
        NoSuchItem,
        NameAlreadyInUse,
    }
}
